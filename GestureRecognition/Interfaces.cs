using Gestures.Geometrie.Vertex;
using Gestures.Recognition.GestureData;
using System;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
/// <author>Dr. rer. nat. Michael Schmidt - Techniche Universität Dresden 2014.</author>
namespace Gestures.Recognition.Interfaces
{
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
        String Name { get; }
        double Probability { get; }
        IList<IVertex> NodeParameters { get; }
        IDictionary<String, Object> AdditionalParameters { get; }
    }

    public interface IClassify
    {
        IClassificationResult Classify(IList<Frame> frames, IDictionary<int, IList<Sample>> trajectories);
        String NotifyOnInput(IDictionary<int,IList<Sample>> trajectories);
    }

    //public interface ICompareTrajectories
    //{
    //    double GetNormalizedTrajectoryDistance(IList<Sample> sample1, IList<Sample> sample2);
    //}

    public interface ICompareTokens
    {
        double GetNormalizedTokenSimilarity(IDictionary<int, IList<Sample>> input, int tokenIndex1, GestureTemplate template, int tokenIndex2);
        double GetNormalizedTokenDistance(IDictionary<int, IList<Sample>> input, int tokenIndex1, GestureTemplate template, int tokenIndex2);
        double GetNormalizedTrajectoryDistance(IList<Sample> sample1, IList<Sample> sample2);
    }

    public interface ISortTemplatesTokens
    {
        IDictionary<String, GestureClass> SortTemplatesTokens(IDictionary<String, GestureClass> templates);
        double GetTokenSimilarity(IDictionary<int, IList<Sample>> gesture1, int tokenIndex1, GestureTemplate gesture2, int tokenIndex2);
    }

    

    public interface ILoadGestureTemplates
    {
        void LoadTemplates(String templatePath);
        GestureClass[] GetGestureClasses();
        void AddTemplate(GestureTemplate template);
        void RemoveTemplates(string className);
        void StoreTemplates(string templatePath);
        void StoreTemplate(string templatePath, String templateFileName, GestureTemplate template);
        void StoreTemplate(string templatePath, GestureTemplate template);
    }       

    public interface IRecognizeGestures : ISynchronizable
    {
        /// <summary>
        /// Gets or sets the probability threshold for directly accepting a classification result.
        /// </summary>
        /// <value>
        /// The probability threshold. Must be in range of 0 - 1.0.
        /// </value>
        double ProbabilityThreshold { get; set; }
        void AddClassifier(IClassify classifier);
        void StartEvaluation();
        String AddFrame(Frame frame);
        IClassificationResult FinishEvaluation();
        /// <summary>
        /// Finishes the evaluation and clears the related blob tracker if set.
        /// </summary>
        /// <param name="_clear">if set to <c>true</c> the blob tracker is cleared.</param>
        /// <returns>A recognized gesture or <c>Null</c></returns>
        IClassificationResult FinishEvaluation(bool _clear);
    }
    public delegate void TrackedTouchesDelegate(Frame previousFrame, Frame trackedFrame);
    public interface ITrackBlobs
    {
        void InitiateTracking();
        void AddFrame(Frame frame);
        event TrackedTouchesDelegate OnTrackedFrame;
        TrackedGesture TrackedBlobs { get; }
        IList<Frame> FrameList { get; }
        IDictionary<int, IList<Sample>> Trajectories { get; }
    }

    public interface IRecordTemplates : ISynchronizable
    {
        void StartTemplateRecording();
        string FinishTemplateRecording(String xmlTemplatePath, String gestureClassName);
    }

}