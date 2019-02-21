
using Gestures.Geometrie.Vertex;
using Gestures.Recognition.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Namespace for Gesture recognition classifiers
/// </summary>
namespace Gestures.Recognition.Classifier
{
    /// <summary>
    /// 
    /// </summary>
		/// <remarks> </remarks>
    /// <author>Dr. rer. nat. Michael Schmidt - Techniche Universität Dresden 2014.</author>
    /// <seealso cref="Gestures.Recognition.Interfaces.IClassificationResult" />
    public class ClassificationResult : IClassificationResult
    {
        protected IList<IVertex> nodeParameters;
        protected IDictionary<String, Object> additionalParameters;

        public ClassificationResult(String name, double probability, IVertex[] nodeParameters,
            IDictionary<String, Object> additionalParameters = null)
        {
            this.Name = name;
            this.Probability = probability;
            this.nodeParameters = new List<IVertex>();
            if (nodeParameters != null)
            {
                for (int i = 0; i < nodeParameters.Length; i++)
                {
                    this.nodeParameters.Add(nodeParameters[i]);
                }
            }

            this.additionalParameters = new Dictionary<String, Object>();
            if (additionalParameters != null && additionalParameters.Count > 0)
            {
                //for (int i = 0; i < additionalParameters.Length; i++)
                foreach (var item in additionalParameters)
                {
                    if (this.additionalParameters.ContainsKey(item.Key))
                        this.additionalParameters[item.Key] = item.Value;
                    this.additionalParameters.Add(item.Key, item.Value);
                }
            }
        }

        #region IClassificationResult Members

        public string Name { get; set; }

        public double Probability { get; set; }

        public IList<IVertex> NodeParameters
        {
            get { return nodeParameters; }
        }

        public IDictionary<String,Object> AdditionalParameters
        {
            get { return additionalParameters; }
        }

        #endregion

        public override string ToString()
        {
            return Name;
        }
    }

    /// <summary>
    /// 
    /// </summary>
		/// <remarks> </remarks>
    /// <author>Dr. rer. nat. Michael Schmidt - Techniche Universität Dresden 2014.</author>
    /// <seealso cref="Gestures.Recognition.Classifier.ClassificationResult" />
    public class OnlineClassificationResult : ClassificationResult
    {
        public OnlineClassificationResult(ClassificationResult originalResult)
            : base(
                originalResult.Name, 
                originalResult.Probability, 
                originalResult.NodeParameters.ToArray(), 
                originalResult.AdditionalParameters)
        { }
    }
}