
using System;
using System.Collections.Generic;
using Gestures.Geometrie.Vertex;

namespace Gestures.Recognition.GestureData
{
    /// <summary>
    /// Smallest component of a gesture. Actual consiting of input points with time stamps.
    /// </summary>
    /// <remarks>
    /// Gesture tokens appear as trajectories and are output of the blob tracking procedure.
    /// </remarks>
    [System.Xml.Serialization.XmlRoot("GestureToken",IsNullable = true)]    
    public class GestureToken
    {

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GestureToken"/> class.
        /// </summary>
        public GestureToken()
            :this(0,null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GestureToken"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="points">Set of sample points forming a trajectory.</param>
        public GestureToken(int id,Sample[] points)
        {
            this.Id = id;
            this.Samples = points;
        }

        #endregion

        /// <summary>
        /// Gets or sets the number of sample points.
        /// </summary>
        /// <value>Number of sample points.</value>
        [System.Xml.Serialization.XmlAttribute("Length")]
        public int Count { get { return Samples.Length; } set { } }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        [System.Xml.Serialization.XmlAttribute("Identity")]
        public int Id { get; set; }
                
        //[System.Xml.Serialization.XmlArray("Points")]
        //[System.Xml.Serialization.XmlArrayItem(Type=typeof(Vertex))]
        /// <summary>
        /// Gets or sets the samples.
        /// </summary>
        /// <value>The samples.</value>
        [System.Xml.Serialization.XmlElement]
        public Sample[] Samples { get; set; }

        /// <summary>
        /// Gets the <see cref="Sample"/> at the specified index.
        /// </summary>
        /// <value></value>
        [System.Xml.Serialization.XmlIgnore]
        public Sample this[int index]
        {
            get
            {
                if (index >= 0 && index < Samples.Length)
                    return Samples[index];
                throw new ArgumentOutOfRangeException();
            }
        }
    }

}