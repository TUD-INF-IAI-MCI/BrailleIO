using System;
using System.Collections.Generic;
using Gestures.Recognition.GestureData;
using Gestures.Geometrie.Vertex;
using Gestures.Recognition.Interfaces;
using Gestures.Geometrie.KdTree;
using System.Linq;

namespace Gestures.Recognition.Preprocessing
{
    /// <summary>
    /// Tracks blobs or touch values on the surface through the time 
    /// and therefore computes trajectories of a single touch say a 
    /// moving finger.
    /// </summary>
    public class BlobTracker : ITrackBlobs
    {
        #region private consts
        private const double MAXMOVEMENTPERFRAME = 20.0; //i.e. speed, max movement of a touch between two frames
        #endregion

        private const int MAXBLOBS = 20;
        static int kdTreeBoundslevel = 3;
        static int kdTreeCutoff = 6;

        #region private fields
        int[] lastToCurrentAssignment = new int[MAXBLOBS];
        private int trajectoryCounter = 0;
        List<Frame> frameList;
        private KdTree kdTreeCurrentTouches;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobTracker"/> class.
        /// </summary>
        public BlobTracker()
        {
            frameList = new List<Frame>();
        }

        #region ITrackBlobs Members

        /// <summary>
        /// Initiates the tracking.
        /// </summary>
        public void InitiateTracking()
        {            
            frameList.Clear();
            trajectoryCounter = 0;
        }
        /// <summary>
        /// Adds a new frame of sensor data.
        /// </summary>
        /// <param name="frame">The frame.</param>
        public void AddFrame(Frame frame)
        {
            TrackFrame(frame);
        }

        /// <summary>
        /// Gets the tracked blobs.
        /// </summary>
        /// <value>The tracked blobs.</value>
        public TrackedGesture TrackedBlobs
        {
            get {
                IDictionary<int, List<Sample>> tokenSamples = new Dictionary<int,List<Sample>>();
                for (int i = 0; i < frameList.Count; i++)
                {
                    foreach (Touch t in frameList[i])
                    {
                        if (!tokenSamples.ContainsKey(t.id))
                        {
                            tokenSamples.Add(t.id, new List<Sample>());
                        }
                        tokenSamples[t.id].Add(GetSampleFromTouch(frameList[i].TimeStamp, t));
                    }
                }
                List<GestureToken> tokenList = new List<GestureToken>();
                foreach (var sampleList in tokenSamples)
                {
                    GestureToken tempToken = new GestureToken(sampleList.Key,
                        sampleList.Value.ToArray());
                    tokenList.Add(tempToken);
                }

                TrackedGesture trackedGesture
                    = new TrackedGesture(tokenList.ToArray(), frameList.ToArray());
                return trackedGesture;            
            }
        }

        /// <summary>
        /// Occurs when a frame is tracked.
        /// </summary>
        public event TrackedTouchesDelegate OnTrackedFrame;

        #endregion

        #region private methods

        #region tracking
        private void TrackFrame(Frame frame)
        {
            Frame trackedFrame = new Frame(frame.TimeStamp);
            int counter = 0;
            foreach(Touch t in frame)//normalize touch ids
            {
                t.id = counter++;
                trackedFrame.AddTouch(t);
            }
            if (/*trajectoryCounter > MAXBLOBS||*/counter>MAXBLOBS) { return; }

            frame = trackedFrame;
            if (frameList.Count == 0 || frameList[frameList.Count - 1].Count == 0)
            {
                trackedFrame = TrackFirstFrame(frame); //handle first frame
            }
            else
            {
                if (frame.Count > 0)
                {
                    trackedFrame = TrackFrames(frameList[frameList.Count - 1],
                        frame);
                }
                else { trackedFrame = frame; }
            }

            frameList.Add(trackedFrame);

            if (OnTrackedFrame != null)
            {
                OnTrackedFrame(frameList.Count > 1 ? frameList[frameList.Count - 2] : null,
                    trackedFrame);
            }
        }
        
        private void InterpolateMissingBlobs(List<Frame> frameList, int startFrame, int endFrame)
        {
            if (startFrame < 0 || endFrame < 0
                || frameList.Count <= startFrame || frameList.Count <= endFrame
                || Math.Abs(startFrame - endFrame) < 2)
            { return; }


            Frame frame1 = frameList[startFrame];
            Frame frame2 = frameList[startFrame + 1];
            Frame frame3 = frameList[endFrame];

            Frame trackedFrame = TrackFrames(frame1, frame3);
            foreach (Touch t in frame1)
            {
                if (frame2.GetTouch(t.id) == null) //trajectory ended in frame1
                {
                    if (trackedFrame.GetTouch(t.id) != null) //possible occlusion of blob in frame2
                    {
                        frame2.AddTouch(t);
                        frame3.AddTouch(t);
                    }
                }
            }
        }

        private Frame TrackFirstFrame(Frame frame)
        {
            Frame trackedFrame = new Frame(frame.TimeStamp);
            foreach (var touch in frame)
            {
                touch.id = trajectoryCounter++;
                trackedFrame.AddTouch(touch);
            }
            return trackedFrame;
        }

        private Frame TrackFrames(Frame lastFrame,Frame currentFrame)
        {
            //Frame trackedFrame = new Frame(currentFrame.TimeStamp);
            kdTreeCurrentTouches = new KdTree(GetVertexList(currentFrame), 2, 
                kdTreeCutoff, kdTreeBoundslevel);

            Frame trackedFrame
                //= GetNearestNeighbourAssignment(lastFrame, currentFrame, kdTreeCurrentTouches);
                = GetMinSumDistAssignment(lastFrame, currentFrame, kdTreeCurrentTouches);
            for (int i = 0; i < currentFrame.Count; i++)//new touches become new trajectories
            {
                if (kdTreeCurrentTouches.Included(currentFrame[i].id))
                { 
                    Touch t = currentFrame[i];
                    Touch newTouch = new Touch(trajectoryCounter++, t.x, t.y, t.cx, t.cy, t.intense);     
                    trackedFrame.AddTouch(newTouch);
                }
            }
            return trackedFrame;
        }

        private Frame GetMinSumDistAssignment(Frame lastFrame, Frame currentFrame,
            KdTree kdTreeCurrentTouches)
        {
            int max = lastFrame.Count > currentFrame.Count ? lastFrame.Count : currentFrame.Count;
            max = max > 7 ? 7 : max; //Maximize to a dimension of 7x7 to maximize calculation time
            double[,] matchingMatrix = new double[max, max];
            for (int i = 0; i < lastFrame.Count && i < max; i++)
            {
                foreach (var current in currentFrame)
                {
                    if (current.id >= max) continue;
                    matchingMatrix[i, current.id] =
                        MetricDistances.EuclideanDistance(new Vertex(current.x, current.y),
                        new Vertex(lastFrame[i].x, lastFrame[i].y));
                }
                for (int j = currentFrame.Count; j < max; j++)
                {
                    matchingMatrix[i, j] = -1000;
                }
            }
            var result = GreedySolver(matchingMatrix);
            Frame trackedFrame = new Frame(currentFrame.TimeStamp);
            for (int i = 0; i < lastFrame.Count; i++)
            {
                if (result.ContainsKey(i))
                {
                    Touch current = currentFrame.GetTouch(result[i]);
                    if (current == null) { continue; }
                    trackedFrame.AddTouch(new Touch(lastFrame[i].id,current.x,current.y,
                        current.cx,current.cy,current.intense));
                    kdTreeCurrentTouches.Delete(current.id);
                }
            }
            return trackedFrame;
        }


        private IDictionary<int, int> GreedySolver(double[,] matchingMatrix)
        {
            var result = new Dictionary<int, int>(matchingMatrix.GetLength(0) + 1);
            var rows = new Stack<KeyValuePair<int,int>>(matchingMatrix.GetLength(0) + 1);
            var cols = new Stack<KeyValuePair<int, int>>(matchingMatrix.GetLength(0) + 1);
            double bestMatch = double.MaxValue;
            RecursiveGreedyMatchingSolver(matchingMatrix, rows, cols, result, ref bestMatch);
            return result;
        }

        private void RecursiveGreedyMatchingSolver(double[,] mm,
            Stack<KeyValuePair<int, int>> rows, Stack<KeyValuePair<int, int>> cols, IDictionary<int, int> assignment, 
            ref double bestMatch)
        {
            //if result is better than previous one, store it
            if (rows.Count == mm.GetLength(0))
            {
                double temp = 0;
                foreach (var row in rows)
                {
                    temp += mm[row.Key, row.Value];
                }
                if (temp < bestMatch)
                {
                    assignment.Clear();
                    foreach (var row in rows)
                    {
                        assignment.Add(row.Key,row.Value);
                    }
                    bestMatch = temp;
                }
                return;
            }

            int i = rows.Count;
            for(int j=0; j< mm.GetLength(1); j++) 
            {
                var value = mm[i, j];

                if (value < 0d ) continue; //Do not use not calculated distances!
                
                if (value > 25d ) continue;  //Ignore to big distances

                if (cols.Any(kvp => kvp.Key == j)) //Do not use already used points...
                    continue;

                rows.Push(new KeyValuePair<int,int>(i, j));
                cols.Push(new KeyValuePair<int, int>(j, i));
                RecursiveGreedyMatchingSolver(mm, rows, cols, assignment, ref bestMatch);
                rows.Pop();
                cols.Pop();
                
            }
                
            
        }



        #endregion

        #region helper methods
        private IList<Vertex> GetVertexList(Frame frame)
        {
            IList<Vertex> vertexList = new List<Vertex>();
            foreach (Touch t in frame)
            {
                Vertex temp = new Vertex(t.x, t.y);
                System.Diagnostics.Debug.Assert(t.id < MAXBLOBS);
                temp.Num = t.id;
                vertexList.Add(temp);
            }
            return vertexList;
        }

        private Sample GetSampleFromTouch(DateTime timeStamp, Touch touch)
        {
            Sample s = new Sample(timeStamp, touch.x, touch.y);
            s.Num = touch.id;
            return s;
        }
        #endregion

        #endregion
    }
}