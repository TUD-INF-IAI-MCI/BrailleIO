using System;
using System.Collections.Generic;
using Gestures.Geometrie.Vertex;
using Gestures.Recognition.GestureData;

namespace Gestures.Recognition.Interfaces
{
    /// <summary>
    /// Holds data of a classified gesture like the name and its parameters.
    /// </summary>
    public class ClassificationResult : IClassificationResult
    {
        IList<Sample> nodeParameters;
        IList<Object> additionalParameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassificationResult"/> class.
        /// </summary>
        /// <param name="name">The name of the classified gesture.</param>
        /// <param name="probability">The probability of correct classification.</param>
        /// <param name="nodeParameters">The node parameters, i.e. gestures execution position on the display.</param>
        /// <param name="additionalParameters">The additional parameters if available, i.e. angle, length of the gesture.</param>
        public ClassificationResult(String name, double probability, Sample[] nodeParameters,
            params Object[] additionalParameters)
        {
            this.Name = name;
            this.Probability = probability;
            this.nodeParameters = new List<Sample>();
            if (nodeParameters != null)
            {
                for (int i = 0; i < nodeParameters.Length; i++)
                {
                    this.nodeParameters.Add(nodeParameters[i]);
                }
            }

            this.additionalParameters = new List<Object>();
            if (additionalParameters != null)
            {
                for (int i = 0; i < additionalParameters.Length; i++)
                {
                    this.additionalParameters.Add(additionalParameters[i]);
                }
            }
        }

        #region IClassificationResult Members

        /// <summary>
        /// Gets or sets the name of the classified gesture.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the probability of correct classification.
        /// </summary>
        /// <value>The probability.</value>
        public double Probability { get; set; }

        /// <summary>
        /// Gets the node parameters.
        /// </summary>
        /// <value>The node parameters.</value>
        public IList<Sample> NodeParameters
        {
            get { return nodeParameters; }
        }

        /// <summary>
        /// Gets the additional parameters.
        /// </summary>
        /// <value>The additional parameters.</value>
        public IList<Object> AdditionalParameters
        {
            get { return additionalParameters; }
        }

        #endregion

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            string result = Name;
            for (int i = 0; i < nodeParameters.Count; i++)
            {
                if (i == 2)
                {
                    result += " ...";
                    break;
                }

                var param = nodeParameters[i];
                result += string.Format(", ({0}/{1})",param[0],param[1] );

            }
            return result;
        }
    }



    /// <summary>
    /// Provides Property to lock on for synchronization purposes.
    /// </summary>
    public interface ISynchronizable
    {
        /// <summary>
        /// Gets the synchronization lock.
        /// </summary>
        /// <value>The synchronization lock.</value>
        Object SynchLock { get; }
    }


    /// <summary>
    /// Holds result of a gestures classification, i.e. name and additional parameters.
    /// </summary>
    public interface IClassificationResult
    {
        /// <summary>
        /// Gets the name of the result (gesture name).
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        String Name { get; }
        /// <summary>
        /// Gets the probability of the result (confidence). 
        /// </summary>
        /// <value>
        /// The probability.
        /// </value>
        double Probability { get; }
        /// <summary>
        /// Gets the node parameters.
        /// </summary>
        /// <value>
        /// The node parameters.
        /// </value>
        IList<Sample> NodeParameters { get; }
        /// <summary>
        /// Gets the additional parameters.
        /// </summary>
        /// <value>
        /// The additional parameters.
        /// </value>
        IList<Object> AdditionalParameters { get; }
    }

    /// <summary>
    /// Interfaces for classify something.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="R"></typeparam>
    public interface IClassify<T, R>
    {
        /// <summary>
        /// Classifies the specified input data.
        /// </summary>
        /// <param name="inputData">The input data.</param>
        /// <returns></returns>
        R Classify(T inputData);
    }

    /// <summary>
    /// Provides interface for classification of some input to some result.
    /// </summary>
    public interface IClassify : IClassify<TrackedGesture, IClassificationResult>
    {
    }

    /// <summary>
    /// Provides methods of a gesture recognizer composed of different classifiers.
    /// </summary>
    public interface IRecognizeGestures : ISynchronizable
    {
        /// <summary>
        /// Adds a specific classifier.
        /// </summary>
        /// <param name="classifier">The classifier.</param>
        void AddClassifier(IClassify classifier);
        /// <summary>
        /// Starts the evaluation of the input data.
        /// </summary>
        void StartEvaluation();
        /// <summary>
        /// Adds a sensory data frame to the frame set.
        /// </summary>
        /// <param name="frame">The frame.</param>
        void AddFrame(Frame frame);
        /// <summary>
        /// Finishes the evaluation.
        /// </summary>
        /// <returns>Classification result or null.</returns>
        IClassificationResult FinishEvaluation();
    }

    /// <summary>
    /// Delegate for handling tracking-events provided with both tracked frames.
    /// </summary>
    public delegate void TrackedTouchesDelegate(Frame previousFrame, Frame trackedFrame);

    /// <summary>
    /// Provides methods for initiate and perform blob tracking of sensor data within a frame.
    /// </summary>
    public interface ITrackBlobs
    {
        /// <summary>
        /// Initiates the tracking.
        /// </summary>
        void InitiateTracking();
        /// <summary>
        /// Adds a sensor data frame to the frame set.
        /// </summary>
        /// <param name="frame">The frame.</param>
        void AddFrame(Frame frame);
        /// <summary>
        /// Occurs when a frame is handled (tracked).
        /// </summary>
        event TrackedTouchesDelegate OnTrackedFrame;
        /// <summary>
        /// Gets the tracked blobs (combined sensory date groups).
        /// </summary>
        /// <value>
        /// The tracked blobs.
        /// </value>
        TrackedGesture TrackedBlobs { get; }
    }     
}