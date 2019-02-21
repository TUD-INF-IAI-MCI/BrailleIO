using BrailleIO.Structs;
using Gestures.Geometrie.FibonacciHeap;
using Gestures.Geometrie.KdTree;
using Gestures.Geometrie.Vertex;
using Gestures.Recognition.GestureData;
using Gestures.Recognition.Interfaces;
using System;
using System.Collections.Generic;

namespace Gestures.Recognition.Preprocessing
{
    /// <summary>
    /// 
    /// </summary>
		/// <remarks> </remarks>
    /// <author>Dr. rer. nat. Michael Schmidt - Techniche Universität Dresden 2014.</author>
    /// <seealso cref="Gestures.Recognition.Interfaces.ITrackBlobs" />
    public class BlobTracker : ITrackBlobs
    {
        #region private consts
        private const double MAXMOVEMENTPERFRAME = 20.0; //i.e. speed, max movement of a touch between two frames
        #endregion

        #region private fields
        private const int MAXBLOBS = 20;
        static int kdTreeBoundslevel = 3;
        static int kdTreeCutoff = 6;

        int[] lastToCurrentAssignment = new int[MAXBLOBS];
        private int trajectoryCounter = 0;
        List<Frame> frameList;
        IDictionary<int, IList<Sample>> tokenSamples;
        private KdTree kdTreeCurrentTouches;


        private FibonacciHeap fibHeap;
        #endregion

        public BlobTracker()
        {
            frameList = new List<Frame>();
            fibHeap = new FibonacciHeap();
            tokenSamples = new Dictionary<int, IList<Sample>>();
        }

        #region ITrackBlobs Members

        public virtual void InitiateTracking()
        {
            fibHeap.Initialize(MAXBLOBS);
            frameList.Clear();
            tokenSamples.Clear();
            trajectoryCounter = 0;
        }

        public virtual void AddFrame(Frame frame)
        {
            TrackFrame(frame);
            UpdateTokenCollection(frameList[frameList.Count - 1]);
            if (OnTrajectoryChange != null)
            {
                OnTrajectoryChange(frameList, Trajectories);
            }
        }

        protected virtual void UpdateTokenCollection(Frame frame)
        {
            foreach (Touch t in frame)
            {
                if (!tokenSamples.ContainsKey(t.ID))
                {
                    tokenSamples.Add(t.ID, new List<Sample>());
                }
                tokenSamples[t.ID].Add(GetSampleFromTouch(frame.TimeStamp, t));
            }
        }

        public virtual IList<Frame> FrameList { get { return frameList; } }
        public virtual IDictionary<int, IList<Sample>> Trajectories { get { return tokenSamples; } }

        public delegate void OnTrajectoryChangeDelegate(IList<Frame> frameList, IDictionary<int, IList<Sample>> trajectories);
        public event OnTrajectoryChangeDelegate OnTrajectoryChange;

        public TrackedGesture TrackedBlobs
        {
            get
            {
                List<GestureToken> tokenList = new List<GestureToken>();
                foreach (var sampleList in tokenSamples)
                {
                    GestureToken tempToken = new GestureToken(sampleList.Key,
                        sampleList.Value as List<Sample>);
                    tokenList.Add(tempToken);
                }

                TrackedGesture trackedGesture
                    = new TrackedGesture(tokenList.ToArray(), frameList.ToArray());
                return trackedGesture;
            }
        }

        public event TrackedTouchesDelegate OnTrackedFrame;

        ////TODO: remove
        //public void ArtificialTracking(Frame previousFrame, Frame trackedFrame)
        //{
        //    OnTrackedFrame(previousFrame, trackedFrame);
        //}

        #endregion

        #region private methods

        #region tracking
        protected virtual void TrackFrame(Frame frame)
        {
            Frame trackedFrame = new Frame(frame.TimeStamp);
            int counter = 0;
            foreach (Touch t in frame)//normalize touch ids
            {
                t.ID = counter++;
                trackedFrame.AddTouch(t);
            }
            if (trajectoryCounter > MAXBLOBS || counter > MAXBLOBS) { return; }


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

        protected virtual void InterpolateMissingBlobs(List<Frame> frameList, int startFrame, int endFrame)
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
                if (frame2.GetTouch(t.ID) == null) //trajectory ended in frame1
                {
                    if (trackedFrame.GetTouch(t.ID) != null) //possible occlusion of blob in frame2
                    {
                        frame2.AddTouch(t);
                        frame3.AddTouch(t);
                    }
                }
            }
        }

        protected virtual Frame TrackFirstFrame(Frame frame)
        {
            Frame trackedFrame = new Frame(frame.TimeStamp);
            foreach (var touch in frame)
            {
                touch.ID = trajectoryCounter++;
                trackedFrame.AddTouch(touch);
            }
            return trackedFrame;
        }

        protected virtual Frame TrackFrames(Frame lastFrame, Frame currentFrame)
        {
            //Frame trackedFrame = new Frame(currentFrame.TimeStamp);
            kdTreeCurrentTouches = new KdTree(GetVertexList(currentFrame), 2,
                kdTreeCutoff, kdTreeBoundslevel);

            Frame trackedFrame
                //= GetNearestNeighbourAssignment(lastFrame, currentFrame, kdTreeCurrentTouches);
                = GetMinSumDistAssignment(lastFrame, currentFrame, kdTreeCurrentTouches);
            for (int i = 0; i < currentFrame.Count; i++)//new touches become new trajectories
            {
                if (kdTreeCurrentTouches.Included(currentFrame[i].ID))
                {
                    Touch t = currentFrame[i];
                    Touch newTouch = new Touch(trajectoryCounter++, t.X, t.Y, t.DimX, t.DimY, t.Intense);
                    trackedFrame.AddTouch(newTouch);
                }
            }
            return trackedFrame;
        }

        protected virtual Frame GetNearestNeighbourAssignment(Frame lastFrame, Frame currentFrame,
            KdTree kdTreeCurrentTouches)
        {
            Frame trackedFrame = new Frame(currentFrame.TimeStamp);
            int fibHeapCount = 0;
            fibHeap.Initialize(MAXBLOBS);

            foreach (var lastTouch in lastFrame)
            {
                IVertex lastVertex = new Vertex(lastTouch.X, lastTouch.Y);
                int nn = kdTreeCurrentTouches.TopDownNearestNeighbour(lastVertex);
                Touch currentTouch = currentFrame.GetTouch(nn);
                IVertex currentVertex = new Vertex(currentTouch.X, currentTouch.Y);

                lastToCurrentAssignment[lastTouch.ID] = nn;
                fibHeap.InsertKey(lastTouch.ID, MetricDistances.EuclideanDistance(
                    lastVertex, currentVertex));
                fibHeapCount++;
            }

            int min;
            int tempCounter = 0;
            while ((min = fibHeap.Minimum) > -1 && (min = fibHeap.DeleteMin()) != -1)
            {
                tempCounter++;

                if (lastFrame.GetTouch(min) == null)
                {
                }
                if (!kdTreeCurrentTouches.Included(lastToCurrentAssignment[min]))
                {
                    Touch lastTouch = lastFrame.GetTouch(min);
                    IVertex lastVertex = new Vertex(lastTouch.X, lastTouch.Y);
                    int nn = kdTreeCurrentTouches.TopDownNearestNeighbour(lastVertex);
                    Touch currentTouch = currentFrame.GetTouch(nn);
                    if (currentTouch == null) // trajectory ended in last frame
                    {
                        return trackedFrame; // if no radii search there is no point left to connect
                    }
                    else
                    {
                        IVertex currentVertex = new Vertex(currentTouch.X, currentTouch.Y);
                        lastToCurrentAssignment[lastTouch.ID] = currentTouch.ID;


                        fibHeap.InsertKey(lastTouch.ID, MetricDistances.EuclideanDistance(
                            lastVertex, currentVertex));
                    }
                    continue;
                }
                Touch t = currentFrame.GetTouch(lastToCurrentAssignment[min]);
                kdTreeCurrentTouches.Delete(lastToCurrentAssignment[min]);
                Touch newTouch = new Touch(min, t.X, t.Y, t.DimX, t.DimY, t.Intense);
                trackedFrame.AddTouch(newTouch);

            }
            return trackedFrame;
        }

        protected virtual Frame GetMinSumDistAssignment(Frame lastFrame, Frame currentFrame,
            KdTree kdTreeCurrentTouches)
        {
            int max = lastFrame.Count > currentFrame.Count ? lastFrame.Count : currentFrame.Count;
            double[,] matchingMatrix = new double[max, max];
            for (int i = 0; i < lastFrame.Count; i++)
            {
                foreach (var current in currentFrame)
                {
                    matchingMatrix[i, current.ID] =
                        MetricDistances.EuclideanDistance(new Vertex(current.X, current.Y),
                        new Vertex(lastFrame[i].X, lastFrame[i].Y));
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
                Touch current = currentFrame.GetTouch(result[i]);
                if (current == null) { continue; }
                trackedFrame.AddTouch(new Touch(lastFrame[i].ID, current.X, current.Y,
                    current.DimX, current.DimY, current.Intense));
                kdTreeCurrentTouches.Delete(current.ID);
            }
            return trackedFrame;
        }


        protected virtual IDictionary<int, int> GreedySolver(double[,] matchingMatrix)
        {
            var result = new Dictionary<int, int>();
            var rows = new Dictionary<int, int>();
            var cols = new Dictionary<int, int>();
            double bestMatch = double.MaxValue;
            RecursiveGreedyMatchingSolver(matchingMatrix, rows, cols, result, ref bestMatch);
            return result;
        }

        protected virtual void RecursiveGreedyMatchingSolver(double[,] mm, IDictionary<int, int> rows
            , IDictionary<int, int> cols, IDictionary<int, int> assignment, ref double bestMatch)
        {
            //if result is better than previous one, store it
            if (rows.Count == mm.GetLength(0))
            {
                double temp = 0;
                foreach (int row in rows.Keys)
                {
                    temp += mm[row, rows[row]];
                }
                if (temp < bestMatch)
                {
                    assignment.Clear();
                    foreach (int row in rows.Keys)
                    {
                        assignment.Add(row, rows[row]);
                    }
                    bestMatch = temp;
                }
                return;
            }

            for (int i = 0; i < mm.GetLength(0); i++)
            {
                if (!rows.ContainsKey(i))
                {
                    for (int j = 0; j < mm.GetLength(1); j++)
                    {
                        if (!cols.ContainsKey(j))
                        {
                            rows.Add(i, j);
                            cols.Add(j, i);
                            RecursiveGreedyMatchingSolver(mm, rows, cols, assignment, ref bestMatch);
                            rows.Remove(i);
                            cols.Remove(j);
                        }
                    }
                }
            }
        }



        #endregion

        #region helper methods
        protected virtual IList<IVertex> GetVertexList(Frame frame)
        {
            IList<IVertex> vertexList = new List<IVertex>();
            foreach (Touch t in frame)
            {
                Vertex temp = new Vertex(t.X, t.Y);
                System.Diagnostics.Debug.Assert(t.ID < MAXBLOBS);
                temp.Num = t.ID;
                vertexList.Add(temp);
            }
            return vertexList;
        }

        protected virtual Sample GetSampleFromTouch(DateTime timeStamp, Touch touch)
        {
            Sample s = new Sample(timeStamp, touch.X, touch.Y);
            s.Num = touch.ID;
            return s;
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
		/// <remarks> </remarks>
    /// <author>Dr. rer. nat. Michael Schmidt - Techniche Universität Dresden 2014.</author>
    /// <seealso cref="Gestures.Recognition.Interfaces.ITrackBlobs" />
    public class BlobTrackerWin7 : ITrackBlobs
    {
        private List<Frame> frameList = new List<Frame>();
        IDictionary<int, IList<Sample>> tokenSamples;

        #region ITrackBlobs Members

        public BlobTrackerWin7()
        {
            tokenSamples = new Dictionary<int, IList<Sample>>();
        }

        public virtual void InitiateTracking()
        {
            frameList.Clear();
            tokenSamples.Clear();
        }

        public virtual void AddFrame(Frame frame)
        {
            frameList.Add(frame);
            if (OnTrackedFrame != null)
            {
                OnTrackedFrame(frameList.Count > 1 ? frameList[frameList.Count - 2] : null,
                    frame);
            }
        }

        public event TrackedTouchesDelegate OnTrackedFrame;

        public TrackedGesture TrackedBlobs
        {
            get
            {
                List<GestureToken> tokenList = new List<GestureToken>();
                foreach (var sampleList in tokenSamples)
                {
                    GestureToken tempToken = new GestureToken(sampleList.Key,
                        sampleList.Value as List<Sample>);
                    tokenList.Add(tempToken);
                }

                TrackedGesture trackedGesture
                    = new TrackedGesture(tokenList.ToArray(), frameList.ToArray());
                return trackedGesture;
            }

        }

        public IList<Frame> FrameList { get { return frameList; } }
        public IDictionary<int, IList<Sample>> Trajectories { get { return tokenSamples; } }

        #endregion

        ////TODO: remove
        //public void ArtificialTracking(Frame previousFrame, Frame trackedFrame)
        //{
        //    OnTrackedFrame(previousFrame, trackedFrame);
        //}

        protected virtual Sample GetSampleFromTouch(DateTime timeStamp, Touch touch)
        {
            Sample s = new Sample(timeStamp, touch.X, touch.Y);
            s.Num = touch.ID;
            return s;
        }

        protected virtual void UpdateTokenCollection(Frame frame)
        {
            foreach (Touch t in frame)
            {
                if (!tokenSamples.ContainsKey(t.ID))
                {
                    tokenSamples.Add(t.ID, new List<Sample>());
                }
                tokenSamples[t.ID].Add(GetSampleFromTouch(frame.TimeStamp, t));
            }
        }

    }
}