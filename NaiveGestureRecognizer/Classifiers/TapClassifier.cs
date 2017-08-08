using System;
using System.Collections.Generic;
using Gestures.Geometrie.Vertex;
using Gestures.Recognition.GestureData;
using Gestures.Recognition.Interfaces;
using Gestures.Recognition.Classifier;

namespace Gestures.Recognition.Classifier
{
    public class TapClassifier : IClassify
    {
    
        private static int maxTapDistance = 5; // <-- 5 Every Trajectory up to 5 pins in distance is a tap. Maybe adjust that number.

        #region IClassify Members

        public IClassificationResult Classify(IList<Frame> frames, IDictionary<int, IList<Sample>> trajectories)
        {
            int result;
            IVertex tappedPos;
            if (trajectories.Count == 0) { return null; }
            if (trajectories.Count == 1) { result = CheckForTapGestures(trajectories[0], out tappedPos); }
            else { result = CheckForTapGesturesMT(trajectories, out tappedPos); }
            if (result == -1) { return null; }

            Console.WriteLine("Tap at " + tappedPos[0] + ", " + tappedPos[1]);

            String resultString = "tap";
            return new ClassificationResult(resultString, 1.0, new IVertex[] { tappedPos },
               new Dictionary<String,Object>(){ {"taps", result}});
        }

        #endregion

        /// <summary>
        /// Checks for tap gestures.
        /// </summary>
        /// <param className="inputData">The input data .</param>
        /// <returns>Number of Taps detected, -1 if no tap gesture could be recognized.</returns>F:\Material\Gesture_Source\Gesten\Classifiers\DTWClassifier.cs
        private int CheckForTapGestures2(TrackedGesture inputData, ref Vertex tapedPos)
        {
            tapedPos[0] = -1;
            if (inputData == null || inputData.Count == 0) { return -1; }
            var frameList = inputData.FrameList;
            int framesWithBlobs = 0;
            int taps = 1;
            foreach (var frame in frameList)
            {
                if (frame.Count > 1)
                {
                    return -1;
                }
                if (frame.Count == 1)
                {
                    framesWithBlobs++;
                    if (!(frame[0].DimX < maxTapDistance && frame[0].DimY < maxTapDistance)) //to big blobs
                    {
                        return -1;
                    }
                    if (tapedPos[0] != -1.0 &&
                        MetricDistances.EuclideanDistance(tapedPos,
                        new Vertex(frame[0].X, frame[0].Y)) > maxTapDistance) //to much moving while tapping
                    {
                        return -1;
                    }
                    if (tapedPos[0] < 0.0)
                    {
                        tapedPos = new Vertex(frame[0].X, frame[0].Y); //first tap contact is tap position
                    }
                }
                else //frame without blobs -> touch left surface
                {
                    taps++;
                }
            }
            return taps;
        }

        private int CheckForTapGestures1(GestureToken inputData, out IVertex tapedPos)
        {
            tapedPos = null;
            if (inputData == null || inputData.Count == 0) { return -1; }
            IVertex mean = new Vertex(0, 0);
            //get bounding box of gesture
            double xMax = double.MinValue, xMin = double.MaxValue,
                yMax = double.MinValue, yMin = double.MaxValue;
            foreach (var point in inputData.Samples)
            {
                if (xMax < point[0]) { xMax = point[0]; }
                if (xMin > point[0]) { xMin = point[0]; }
                if (yMax < point[1]) { yMax = point[1]; }
                if (yMin > point[1]) { yMin = point[1]; }
            }

            if (!((xMax - xMin) < maxTapDistance && (yMax - yMin) < maxTapDistance))
            {
                return -1;
            }
            mean[0] /= ((xMax - xMin) / 2);
            mean[1] /= ((yMax - yMin) / 2);
            tapedPos = mean;
            return 1;
        }

        /// <summary>
        /// Checks for tap gestures.
        /// </summary>
        /// <param className="clusteredSamples">The clustered samples.</param>
        /// <returns>Number of Taps detected, -1 if no tap gesture could be recognized.</returns>
        private int CheckForTapGestures(IList<Sample> inputData, out IVertex tappedPos)
        {
            tappedPos = null;
            if (inputData == null || inputData.Count == 0) { return -1; }
            int tapCount = 0;
            bool empty = true;
            IVertex mean = new Vertex(0, 0);
            for (int i = 0; i < inputData.Count; i++)
            {
                if (inputData[i] != null)
                {
                    if (tappedPos != null)
                    {
                        double dist = MetricDistances.EuclideanDistance(inputData[i], tappedPos);
                        if (dist > maxTapDistance)
                        {
                            return -1;
                        }
                    }
                    if (empty)
                    {
                        tapCount++; empty = false;
                        if (tappedPos == null)
                        {
                            tappedPos = inputData[i];
                        }
                        mean[0] += tappedPos[0];
                        mean[1] += tappedPos[1];
                    }
                }
                else
                {
                    empty = true;
                }
            }
            mean[0] /= (double)tapCount;
            mean[1] /= (double)tapCount;
            // tappedPos = mean;

            if (inputData[0] != null)
            {
                tappedPos = inputData[0];
            }
            else
            {//TODO: two samples with zero clusters within...maybe lost signals from brailledis
                return -1;
            }
            return tapCount;
        }

        private int CheckForTapGesturesMT(IDictionary<int, IList<Sample>> gesture, out IVertex tappedPos)
        {
            tappedPos = null;

            int newSampleCount = gesture[0].Count + 1;
            for (int i = 1; i < gesture.Count; i++)
            {
                newSampleCount += gesture[i].Count + 1;
                if (gesture[i - 1][gesture[i - 1].Count - 1].TimeStamp >=
                     gesture[i][0].TimeStamp)
                {
                    return -1;
                }
            }
            List<Sample> samples = new List<Sample>();
            for (int i = 0; i < gesture.Count; i++)
            {
                for (int j = 0; j < gesture[i].Count; j++)
                {
                    samples.Add(gesture[i][j]);
                }
                samples.Add(null);
            }

            return CheckForTapGestures(samples, out tappedPos);
        }

        #region IClassify Members


        public String NotifyOnInput(IDictionary<int, IList<Sample>> trajectories)
        {
            return String.Empty;
        }

        #endregion
    }
}
