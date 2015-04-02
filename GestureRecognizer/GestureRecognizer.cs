using System;
using System.Collections.Generic;
using Gestures.Recognition.GestureData;
using Gestures.Recognition.Interfaces;

namespace Gestures.Recognition
{
    /// <summary>
    /// A gesture recognizer that supports multi touch on a touch display.
    /// </summary>
    public class GestureRecognizer : IRecognizeGestures
    {
        #region private fields
        ITrackBlobs blobTracker;
        private readonly object synchLock = new object();

        private List<IClassify> classifiers = new List<IClassify>();
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GestureRecognizer"/> class.
        /// </summary>
        /// <param name="blobTracker">Module used for blob tracking.</param>
        public GestureRecognizer(ITrackBlobs blobTracker)
        {
            this.blobTracker = blobTracker;
        }

        #endregion

        #region public members
        #region ISynchronizable
        /// <summary>
        /// Gets the synchronization lock object.
        /// </summary>
        /// <value>The synchronization lock object.</value>
        public object SynchLock { get { return synchLock; } }
        #endregion

        #region IRecognizeGestures
        public void AddClassifier(IClassify classifier)
        {
            classifiers.Add(classifier);
        }

        /// <summary>
        /// Starts the evaluation of gesture input.
        /// </summary>               
        public virtual void StartEvaluation()
        {
            RecognitionMode = true;

            blobTracker.InitiateTracking();
            //blobTracker.TrackNextFrame(new Frame(frame));
        }

        public void AddFrame(Frame frame)
        {
            blobTracker.AddFrame(frame);
        }

        public IClassificationResult FinishEvaluation()
        {
            TrackedGesture trackedGesture = blobTracker.TrackedBlobs;
            IClassificationResult classificationResult = null;
            RecognitionMode = false;

            foreach (IClassify classifier in classifiers)
            {

                classificationResult = classifier.Classify(trackedGesture);
                if (classificationResult != null)
                {
                    blobTracker.InitiateTracking();
                    return classificationResult;
                }
            }

            blobTracker.InitiateTracking();
            return classificationResult;
        }
        #endregion

        #endregion

        /// <summary>
        /// Gets a value indicating whether recognizer actually is classifying.
        /// </summary>
        /// <value><c>true</c> if [recognition mode]; otherwise, <c>false</c>.</value>
        protected bool RecognitionMode { get; set; }

        #region private members

        private void ClearTouch()
        {
            //for (int i = 0; i < pinMatrix.Modules; i++)
            //{
            //    pinMatrix.GetModule(i).Touch = 0;
            //}
            //blobTracker.StartTracking();
        }

        #endregion

    }
}