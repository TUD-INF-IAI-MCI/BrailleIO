using System;
using System.Collections.Generic;
using Gestures.Geometrie.Vertex;
using Gestures.Recognition.GestureData;
using Gestures.Recognition.Interfaces;

namespace Gestures.Recognition
{
    public class TapClassifier : IClassify
    {
        private static double MAXTAPDISTANCE = 4;

        #region IClassify Members

        public IClassificationResult Classify(TrackedGesture gesture)
        {
            int result;
            Vertex tappedPos = new Vertex();
            if (gesture.Count == 0) { return null; }
            //if (gesture.Count == 1) 
            { result = CheckForTapGestures(gesture, ref tappedPos); }

            //else { result = CheckForTapGesturesMT(gesture, ref tapedPos); }
            if (result == -1) { return null; }

            String resultString = "tap";
            return new ClassificationResult(resultString, 100.0, new Sample[] { new Sample(DateTime.Now, tappedPos) },
                new KeyValuePair<String, double>("taps", result));
        }

        #endregion
        /// <summary>
        /// Checks for tap gestures.
        /// </summary>
        /// <param className="clusteredSamples">The clustered samples.</param>
        /// <returns>Number of Taps detected, -1 if no tap gesture could be recognized.</returns>
        private int CheckForTapGestures(TrackedGesture inputData, ref Vertex tapedPos)
        {
            tapedPos[0] = -1;
            if (inputData == null || inputData.Count == 0) { return -1; }
            var frameList = inputData.FrameList;
            int framesWithBlobs = 0;
            int taps = 1;
            int frameCounter = 0;
            foreach (var frame in frameList)
            {
                if (frame.Count > 1)
                {
                    return -1;
                }
                if (frame.Count == 1)
                {
                    framesWithBlobs++;
                    if (!(frame[0].cx < MAXTAPDISTANCE && frame[0].cy < MAXTAPDISTANCE)) //to big blobs
                    {
                        return -1;
                    }
                    if (tapedPos[0] != -1.0 &&
                        MetricDistances.EuclideanDistance(tapedPos,
                        new Vertex(frame[0].x, frame[0].y)) > MAXTAPDISTANCE) //to much moving while tapping
                    {
                        return -1;
                    }
                    if (tapedPos[0] < 0.0)
                    {
                        tapedPos = new Vertex(frame[0].x, frame[0].y); //first tap contact is tap position
                    }
                }
                else //frame without blobs -> touch left surface
                {
                    if (frameCounter != 0 && frameCounter != frameList.Length - 1) { taps++; }
                }
                ++frameCounter;
            }
            return taps;
        }
    }


}