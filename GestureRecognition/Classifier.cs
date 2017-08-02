
using Gestures.Geometrie.Vertex;
using Gestures.Recognition.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gestures.Recognition.Classifier
{
    /// <summary>
    /// 
    /// </summary>
    /// <author>Dr. rer. nat. Michael Schmidt - Techniche Universität Dresden 2014.</author>
    /// <seealso cref="Gestures.Recognition.Interfaces.IClassificationResult" />
    public class ClassificationResult : IClassificationResult
    {
        protected IList<IVertex> nodeParameters;
        protected IList<Object> additionalParameters;
        
        public ClassificationResult(String name, double probability, IVertex[] nodeParameters,
            params Object[] additionalParameters)
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

        public string Name { get; set; }

        public double Probability { get; set; }

        public IList<IVertex> NodeParameters
        {
            get { return nodeParameters; }
        }

        public IList<Object> AdditionalParameters
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
    /// <author>Dr. rer. nat. Michael Schmidt - Techniche Universität Dresden 2014.</author>
    /// <seealso cref="Gestures.Recognition.Classifier.ClassificationResult" />
    public class OnlineClassificationResult : ClassificationResult
    {   
        public OnlineClassificationResult(ClassificationResult originalResult)
            : base(originalResult.Name, originalResult.Probability, originalResult.NodeParameters.ToArray(), originalResult.AdditionalParameters.ToArray())
        { }
    }
}