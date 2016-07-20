using System;
using System.Collections.Generic;
using Gestures.Geometrie.Vertex;
using Gestures.Recognition.GestureData;
using Gestures.Recognition.Interfaces;

namespace Gestures.Recognition
{
    /// <summary>
    /// A classifier to be used by a GestureRecognizer instance.
    /// </summary>
    public class MultitouchClassifier : IClassify
    {
        #region IClassify<TrackedGesture> Members

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MultitouchClassifier"/> class.
        /// </summary>
        public MultitouchClassifier()
            : base()
        {
        }
        #endregion

        /// <summary>
        /// Classifies the specified input data.
        /// </summary>
        /// <param name="inputData">The input data.</param>
        /// <returns>a classification result containing name and parameter of the recognized gesture if
        /// recognition succeeds, null otherwise.</returns>
        public IClassificationResult Classify(TrackedGesture inputData)
        {
            IClassificationResult result = null;
            //RecognizePinchGesture(inputData);
            if (inputData.Count == 0) { return result; }
            //if (result == null)
            //{
            //    result = RecognizeSemiPinchGesture(inputData);
            //}
            if (result == null)
            {
                result = RecognizeBlurryPinchGesture(inputData);
            }
            if (result == null)
            {
                result = RecognizeBlurryLineGesture(inputData);
            }
            if (result == null)
            {
                result = RecognizeBlurryCircleGesture(inputData);
            } 
            if (result == null)
            {
                result = RecognizeXFingerDragging(inputData.FrameList, 3);
            }
            System.Diagnostics.Debug.WriteLine("recognized: " + (result == null ? "nothing" :
                result.ToString()));
            return result;
        }

        #endregion

        #region private members

        #region Parameters
        
        private const int TOKENMINBLOBCOUNT = 4;     
        private double MAXDISTFROMLINE = 4; //max scattering of points to count as line
        private double MINLINELENGTH = 8;
        private int MINEMPTYFRAMES = 2; //at beginning and at the end of a gesture in each case is an empty frame
        private const double MAXANGLEOSCILLATION = 25.0;

        private const double MAXCIRCLEVARIANCE = 0.1;
        private const double LEFTRIGHTDISTVAR = 0.35;

        #endregion

        #region Pinch

        private IClassificationResult RecognizeBlurryPinchGesture(TrackedGesture inputData)
        {
            double maxTapDistance = 2;
            double startDist = double.MinValue,
                endDist = double.MinValue;
            Sample startNode1 = new Sample(), startNode2 = new Sample();
            bool semi1 = true;
            bool semi2 = true;
            //check for min blob relation
            int equalToX=0,lessThanX=0,moreThanX=0;
            int i = 0;
            while (i < inputData.FrameList.Length)
            {
                if (inputData.FrameList[i].Count == 2)
                {
                    equalToX++;
                    //store the first two blobs occurring together as start blobs for the pinch gesture
                    if (startDist < 0.0)
                    {
                        //if more than 1/3 of the frames belongs to single line movement, than it is no pinch anymore
                        if (i <= inputData.FrameList.Length / 3) 
                        {
                            //first fingers start contact for the pinch
                            startNode1 = new Sample(
                                inputData.FrameList[i].TimeStamp,
                                inputData.FrameList[i][0].x,
                                inputData.FrameList[i][0].y);
                            //second fingers start contact for the pinch
                            startNode2 = new Sample(
                                inputData.FrameList[i].TimeStamp,
                                inputData.FrameList[i][1].x,
                                inputData.FrameList[i][1].y);
                            //pinching fingers distance at the beginning
                            startDist =
                                MetricDistances.EuclideanDistance(startNode1, startNode2);
                        }
                    }
                    //check for a steady first finger (blobs stay close to first contact of one finger)
                    semi1 = semi1 &&
                        (
                        (Math.Abs(startNode1[0] - inputData.FrameList[i][0].x) < maxTapDistance
                        &&
                        Math.Abs(startNode1[1] - inputData.FrameList[i][0].y) < maxTapDistance
                        )
                        ||
                        (Math.Abs(startNode1[0] - inputData.FrameList[i][1].x) < maxTapDistance
                        &&
                        Math.Abs(startNode1[1] - inputData.FrameList[i][1].y) < maxTapDistance
                        )
                        );
                    //check for a steady second finger (blobs stay close to first contact of one finger)
                    semi2 = semi2 &&
                        (
                        (Math.Abs(startNode2[0] - inputData.FrameList[i][0].x) < maxTapDistance
                        &&
                        Math.Abs(startNode2[1] - inputData.FrameList[i][0].y) < maxTapDistance
                        )
                        ||
                        (Math.Abs(startNode2[0] - inputData.FrameList[i][1].x) < maxTapDistance
                        &&
                        Math.Abs(startNode2[1] - inputData.FrameList[i][1].y) < maxTapDistance
                        )
                        );
                }
                else
                {
                    //count cases of more or less than two blobs in one frame
                    //can be caused by performing another gesture, flickering or touch unsensitive modules
                    if (inputData.FrameList[i].Count > 2) { moreThanX++; }
                    else { lessThanX++; }
                }
                i++;
            }

            //if there are to many frames with less or more than two blobs, reject pinch gesture assumption
            if ((double)equalToX / (lessThanX + moreThanX) < MINBLOBRELATION) { return null; }
            

            //get last frame with two blobs for finding end positions of both fingers
            i = inputData.FrameList.Length-1;
            while (endDist < 0.0 && i >= 0)
            {
                //check if last frame with two fingers contact is within last third of frame-range
                //if more than 1/3 of the frames belongs to single line movement, than it is no pinch anymore
                if (i >= inputData.FrameList.Length * 2 /3 && inputData.FrameList[i].Count == 2)
                {
                    IVertex endPoint1 = new Vertex(inputData.FrameList[i][0].x, inputData.FrameList[i][0].y);
                    IVertex endPoint2 = new Vertex(inputData.FrameList[i][1].x, inputData.FrameList[i][1].y);
                    endDist = MetricDistances.EuclideanDistance(endPoint1,endPoint2);

                    if(startDist>endDist)
                    {
                        endPoint1 = startNode1;
                        endPoint2 = startNode2;
                    }

                    //if to much scattering of blobs along a given line between the outmost two fingers contact, reject pinch gesture assumption
                    if(!CheckMaxDistanceFromLineKriterion(inputData.FrameList,endPoint1,endPoint2,MAXDISTFROMLINE))
                    {
                        return null;
                    }
                }
                i--;
            }
            //get direction out of change of start to end distances between the two fingers contacts
            bool direction = startDist > endDist; //pinch or reverse pinch

//            GestureToken token = ConnectTokens(inputData);
  //          if (!CheckMaxDistanceFromLineKriterion(token, 2 * MAXDISTFROMLINE)) { return null; }

            bool pinch = startDist>0.0 && endDist >0.0 &&
                ((direction ? (startDist / endDist) : endDist / startDist) > 0.5);

            return !pinch ? null :
                new ClassificationResult(
                    (semi2 || semi1) ? "one finger pinch" : "pinch",
                    1.0,new Sample[]{
                    semi1 ? startNode1 : (semi2 ? startNode2 : 
                    new Sample(startNode2.TimeStamp,
                        (startNode1[0] + startNode2[0])/2,
                        (startNode1[1] + startNode2[1])/2))},
                    new KeyValuePair<String, double>("expanding",
                                (direction) ? -1.0 : 1.0));
        }

        private int FindFirstFrameIndexWithCorrectCount(Frame[] frameList,int blobCount,bool forward)
        {
              //get last frame with two blobs for finding end positions of both fingers
            int i = forward?0:frameList.Length-1;

            while (forward?i<frameList.Length:(i >= 0))
            {
                if (frameList[i].Count == blobCount)
                {
                    return i;
                }
                i = forward ? (i + 1) : (i - 1);
            }
            return -1;
        }

        #endregion

        #region Blurry Line
        private IClassificationResult RecognizeBlurryLineGesture(TrackedGesture inputData)
        {
            if(!CheckTokenCriterion(inputData.FrameList, 1)){ return null; }
            
            Vertex startPoint, endPoint;
            int startPointIndex = FindFirstFrameIndexWithCorrectCount(inputData.FrameList,1,true);
            int endPointIndex = FindFirstFrameIndexWithCorrectCount(inputData.FrameList,1,false);

            Touch t = inputData.FrameList[startPointIndex][0];
            startPoint = new Vertex(t.x, t.y);

            int maxDistantBlobFrameIndex;
            //search endpoint es point with max distance from start point
            //accounts for misleading data and offers robust identification of lines with disadvantage of
            //more wrong positive classifications
            int maxDistantBlobIndex = GetBlobMaxDistantFromPointIndex(inputData.FrameList, startPoint, out maxDistantBlobFrameIndex);

            if (startPointIndex == -1 || endPointIndex == -1
                || startPointIndex == endPointIndex || maxDistantBlobFrameIndex == -1)
            { return null; }

           
            t = inputData.FrameList[endPointIndex][0];
            endPoint = new Vertex(t.x, t.y);


            IClassificationResult result = null;

            if (CheckMaxDistanceFromLineKriterion(inputData.FrameList, startPoint, endPoint, MAXDISTFROMLINE)
                && MetricDistances.EuclideanDistance(startPoint, endPoint)> MINLINELENGTH)
            {
                //return RecognizeDirectionalLine(startPoint, endPoint);
                result = new ClassificationResult("line", 100.0, new Sample[] { new Sample(DateTime.Now, startPoint), new Sample(DateTime.Now, endPoint) },
                    new KeyValuePair<String, double>("angle", GetAngle(startPoint, endPoint)));
            }
            return result;
        }

        private String RecognizeDirectionalLine<Vert1,Vert2>(Vert1 startPoint, Vert2 endPoint)
            where Vert1 : struct, IVertex
            where Vert2 : struct, IVertex
        {
            double angle = GetAngle(startPoint, endPoint);
            switch (((int)angle) / 45)
            {
                case 0: return "LineRight";
                case 1: return "LineTop";
                case 2: return "LineTop";
                case 3: return "LineLeft";
                case 4: return "LineLeft";
                case 5: return "LineBottom";
                case 6: return "LineBottom";
                case 7: return "LineRight";
                default: return "LineRight";
            }
        }

        #endregion

        #region Blurry Semi Circle

        private IClassificationResult RecognizeBlurryCircleGesture(TrackedGesture inputData)
        {
            String resultString = null;

            if(!CheckTokenCriterion(inputData.FrameList,1)){return null;}
            int startPointIndex = FindFirstFrameIndexWithCorrectCount(inputData.FrameList,1,true);
            int endPointIndex = FindFirstFrameIndexWithCorrectCount(inputData.FrameList,1,false);
            if(startPointIndex==-1 || endPointIndex == -1){return null;}

            Vertex startPoint = new Vertex(inputData.FrameList[startPointIndex][0].x,inputData.FrameList[startPointIndex][0].y);
            Vertex endPoint = new Vertex(inputData.FrameList[endPointIndex][0].x,inputData.FrameList[endPointIndex][0].y);
                        
            //search endpoint es point with max distance from start point
            var centre = GetCentre(inputData.FrameList); //centre of all blobs

            if (MetricDistances.EuclideanDistance(centre, startPoint)
                > MetricDistances.EuclideanDistance(startPoint, endPoint))
            {
                int endPointBlobIndex = GetBlobMaxDistantFromPointIndex(inputData.FrameList, startPoint, out endPointIndex);
            }

            if (CheckCircleFormKriterion(inputData.FrameList))
            {
                if (CheckForFullCircle(inputData.FrameList,startPoint,endPoint)) { resultString = "circle"; }
                else { resultString = "semi circle"; }
            }
            else { return null; }
          
            return new ClassificationResult(resultString, 100.0, new Sample[] { new Sample(DateTime.Now,startPoint)},
                new KeyValuePair<String, double>("direction", GetCircleDirection(inputData.FrameList) ? 1.0 : -1.0));
        }

        private bool CheckCircleFormKriterion(Frame[] frameList)
        {
            //all points must lay on a circular shape around the center 
            //so check variance of distance from center
            var midPoint = GetCentre(frameList);
            int blobCount = 0;
            double averageDist = 0.0;
            for (int i = 0; i < frameList.Length; i++)
            {
                for (int j = 0; j < frameList[i].Count; j++)
                {
                    averageDist += MetricDistances.EuclideanDistance(new Vertex(frameList[i][j].x, frameList[i][j].y), midPoint);
                    blobCount++;
                }
            }
            averageDist /= blobCount;

            double diff = 0.0;
            for (int i = 0; i < frameList.Length; i++)
            {
                for (int j = 0; j < frameList[i].Count; j++)
                {
                    double dist = MetricDistances.EuclideanDistance(new Vertex(frameList[i][j].x, frameList[i][j].y), midPoint) - averageDist;
                    diff += dist * dist;
                }
            }
            diff = Math.Sqrt(diff);
            diff /= blobCount;

            if (diff > MAXCIRCLEVARIANCE * averageDist) { return false; }

            return true;
        }
                
        private bool CheckForFullCircle(Frame[] frameList, IVertex lineThroughCentreStart, IVertex lineThroughCentreEnd)
        {
            int leftSideCount = 0;
            int rightSideCount = 0;
            int onLineCount = 0;
            //check for every point its position relative to a line through the circles centre

            //compute area of formed triangle
            //A = 0.5 ( bx*cy + cx*ay + ax*by - cx*by - ax*cy - bx*ay)
            //positive -> left turn; negative -> right turn of points
            //therefore positive-> point is on right side

            //precompute redundant part of formula
            //m = bx*cy - cx*by
            //n = by - cy
            //o = cx - bx
            //A = m + ax*n + ay*o

            var b = lineThroughCentreStart;
            var c = lineThroughCentreEnd;
            double m = b[0] * c[1] - c[0] * b[1];
            double n = b[1] - c[1];
            double o = c[0] - b[0];
            double r;

            for (int i = 0; i < frameList.Length; i++)
                for (int j = 0; j < frameList[i].Count; j++)
                {
                    {
                        var a = new Vertex(frameList[i][j].x, frameList[i][j].y);
                        r = m + a[0] * n + a[1] * o;
                        if (r > 0)
                        {
                            rightSideCount++;
                        }
                        else
                        {
                            if (r < 0)
                            {
                                leftSideCount++;
                            }
                            else { onLineCount++; }
                        }
                    }
                }
            if (leftSideCount == 0 || rightSideCount == 0) { return false; }
            double rel = leftSideCount < rightSideCount ? (double)leftSideCount / rightSideCount :
                (double)rightSideCount / leftSideCount;

            if (rel > LEFTRIGHTDISTVAR)
            {
                return true;
            }
            return false;
        }

        private bool GetCircleDirection(Frame[] frameList)
        {
            //return true for clockwise, false otherwise
            var centre = GetCentre(frameList);
            int clockwise = 0, cclockwise = 0;
            var c = centre;
            for (int i = 0; i < frameList.Length-1; i++)
            {
                //check only on two blobs in consecutive frames
                if (frameList[i].Count > 0 && frameList[i + 1].Count > 0)
                {
                    var b = new Vertex(frameList[i][0].x, frameList[i][0].y);
                    var a = new Vertex(frameList[i+1][0].x, frameList[i+1][0].y);
                    double r = (b[0] * c[1] - c[0] * b[1]) + a[0] * (b[1] - c[1]) + a[1] * (c[0] - b[0]);
                    if (r > 0) { clockwise++; }
                    else { if (r < 0) { cclockwise++; } }
                }
            }
            if (clockwise > cclockwise) { return true; } else { return false; }
        }

        #endregion

        #region Three Finger Dragging
               
        private static double MINBLOBRELATION = 0.5;

        private IClassificationResult RecognizeXFingerDragging(Frame[] input, int finger)
        {
            int equalToX=0,lessThanX=0,moreThanX=0;
            int firstIndex = -1, lastIndex = 0;
            Vertex start, end;
            //find first and last index of frame with exactly x blobs

            for (int i = 0; i < input.Length; i++)
            {
                //check for three blobs most of time
                if (input[i].Count == finger) 
                {
                    equalToX++;
                    if (firstIndex == -1) { firstIndex = i; }
                    lastIndex = i;
                }
                else
                {
                    if (input[i].Count < finger) { lessThanX++; }
                    else
                    {
                        //TODO: maybe reduce possible noice blobs 
                        moreThanX++;
                    }
                }            
            }
            //System.Diagnostics.Debug.WriteLine("expected continuous touch of " + finger.ToString() + " blobs and got:");
            //System.Diagnostics.Debug.WriteLine("==: " + equalToX.ToString() + "\t<: " + lessThanX + "\t>: " + moreThanX.ToString());
                         
            //check for min blob relation
            if ((double)equalToX / (lessThanX + moreThanX) < MINBLOBRELATION) { return null; }

            //TODO: check for reliable start and end point with sufficient number of blobs
            //at time to restrictive because of faulty sensor data
            //if (lastIndex < input.Count - MAXINDEXDIFF) { return null; }
            //if (firstIndex > MAXINDEXDIFF) { return null; }
           

            //get parameters, i.e. start and end point of gesture
            Vertex[] temp = new Vertex[input[firstIndex].Count];
            for (int i = 0; i < input[firstIndex].Count; i++)
            {
                temp[i] =
                    new Vertex(input[firstIndex][i].x, input[firstIndex][i].y); 
            }
            start = GetCentre(temp);

            temp = new Vertex[input[lastIndex].Count];
            for (int i = 0; i < input[lastIndex].Count; i++)
            {
                temp[i] =
                    new Vertex(input[lastIndex][i].x, input[lastIndex][i].y);
            }
            end = GetCentre(temp);

            return new ClassificationResult("drag", 100.0,new Sample[] { (Sample)start, (Sample)end },
                new KeyValuePair<String, double>("contacts", finger));
        }

        #endregion

        #region Helper Methods
       
        private T GetCentre<T>(params T[] points)
            where T : struct, IVertex
        {
            var result = points[0];
            for (int i = 1; i < points.Length; i++)
            {
                result = result.Add(points[i]);
            }
            result[0] /= points.Length;
            result[1] /= points.Length;
            return result;
        }

        private IVertex GetCentre(Frame[] frameList)
        {
            IVertex centre = new Vertex(0.0, 0.0);
            int blobCount = 0;
            for (int i = 0; i < frameList.Length; i++)
            {
                for (int j = 0; j < frameList[i].Count; j++)
                {
                    centre[0]+= frameList[i][j].x;
                    centre[1]+=  frameList[i][j].y;
                    blobCount++;
                }
            }
            centre[0] /= blobCount;
            centre[1] /= blobCount;
            return centre;
        }

        private double GetAngle<Vert1,Vert2>(Vert1 startPoint, Vert2 endPoint)
            where Vert1 : struct, IVertex
            where Vert2 : struct, IVertex
        {
            double x = 0, y = 0;
            double angle =
                (y = (endPoint[1] - startPoint[1])) /
                (x = (endPoint[0] - startPoint[0]));
            if (x == 0)
            {
                if (y < 0) { angle = 270.0; }
                else { angle = 90; }
            }
            else
            {
                angle = (x < 0 ? 180 : 0) + 180 / Math.PI * Math.Atan(angle); //deg = 180/"Pi" * rad
                angle = angle < 0 ? 360 + angle : angle;
            }
            return angle;
        }

        #region Criterions

        private int GetBlobMaxDistantFromPointIndex(Frame[] frameList, IVertex point, out int frameIndex)
        {
            double maxDist = 0;
            frameIndex = -1;
            int blobIndex = 0;
            for(int f = 0; f<frameList.Length;f++)
            {
                for(int blob = 0;blob < frameList[f].Count; blob++)
                {
                    double temp = MetricDistances.EuclideanDistance(point, 
                        new Vertex(frameList[f][blob].x, frameList[f][blob].y));
                    if (temp > maxDist)
                    {
                        maxDist = temp;
                        frameIndex = f;
                        blobIndex = blob;
                    }
                }
            }
            return frameIndex != -1 ? blobIndex : -1;
        }
        
        private bool CheckMaxDistanceFromLineKriterion(Frame[] frameList, IVertex linePointA, IVertex linePointB, double maxDistanceFromLine)
        {
            var pA = linePointA;
            var pB = linePointB;
            double lengthC = MetricDistances.EuclideanDistance(pA, pB);

            foreach (var frame in frameList)
            {
                foreach (var touch in frame)
                {
                    var pC = new Vertex(touch.x, touch.y);
                    double lengthA = MetricDistances.EuclideanDistance(pB, pC);
                    double lengthB = MetricDistances.EuclideanDistance(pA, pC);
                    if (lengthA == 0 || lengthB == 0) { continue; }

                    double cosA = (lengthB * lengthB + lengthC * lengthC - lengthA * lengthA) /
                        (2 * lengthB * lengthC);

                    double sinA = Math.Sin(Math.Acos(cosA));

                    double sinB = lengthB * sinA / lengthA;

                    double distC = lengthA * sinB;

                    if (distC > maxDistanceFromLine)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        //checks if throughout the frame sequence consistently a fixed number of blobs is present
        private bool CheckTokenCriterion(Frame[] frameList, int blobsPerFrame)
        {
            int correctBlobsCount = 0;
            int lessBlobsCount = 0;
            int moreBlobsCount = 0;
            foreach(var frame in frameList)
                if (frame.Count == blobsPerFrame)
                {
                    correctBlobsCount++;
                }
                else
                {
                    if (frame.Count < blobsPerFrame) { lessBlobsCount++; }
                    else { moreBlobsCount++; }
                }
            lessBlobsCount -= MINEMPTYFRAMES;
            return ((lessBlobsCount + moreBlobsCount)>correctBlobsCount * MINBLOBRELATION || correctBlobsCount == 0) ? false : true;
        }

        #endregion

        #endregion

        #endregion
    }
}