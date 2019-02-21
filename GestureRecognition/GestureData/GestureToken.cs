
using System;
using System.Collections.Generic;
using Gestures.Geometrie.Vertex;

namespace Gestures.Recognition.GestureData
{
    /// <summary>
    /// Smallest component of a gesture. Actual consisting of input points with time stamps.
    /// </summary>
    /// <remarks>
    /// Gesture tokens appear as trajectories and are output of the blob tracking procedure.
    /// </remarks>
    /// <author>Dr. rer. nat. Michael Schmidt - Techniche Universität Dresden 2014.</author>
    [System.Xml.Serialization.XmlRoot("GestureToken",IsNullable = true)]    
    public class GestureToken
    {

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GestureToken"/> class.
        /// </summary>
		/// <remarks> </remarks>
        public GestureToken()
            :this(0,null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GestureToken"/> class.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="id">The id.</param>
        /// <param name="points">Set of sample points forming a trajectory.</param>
        public GestureToken(int id, List<Sample> points)
        {
            this.Id = id;
            this.Samples = points;
        }

        #endregion

        /// <summary>
        /// Gets or sets the number of sample points.
        /// </summary>
		/// <remarks> </remarks>
        /// <value>Number of sample points.</value>
        [System.Xml.Serialization.XmlAttribute("Length")]
        public int Count { get { return Samples.Count; } set { } }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
		/// <remarks> </remarks>
        /// <value>The id.</value>
        [System.Xml.Serialization.XmlAttribute("Identity")]
        public int Id { get; set; }
                
        //[System.Xml.Serialization.XmlArray("Points")]
        //[System.Xml.Serialization.XmlArrayItem(Type=typeof(Vertex))]
        /// <summary>
        /// Gets or sets the samples.
        /// </summary>
		/// <remarks> </remarks>
        /// <value>The samples.</value>
        [System.Xml.Serialization.XmlElement]
        public List<Sample> Samples { get; set; }

        /// <summary>
        /// Gets the <see cref="IVertex"/> at the specified index.
        /// </summary>
		/// <remarks> </remarks>
        /// <value></value>
        [System.Xml.Serialization.XmlIgnore]
        public IVertex this[int index] { 
            get { return index >= 0 && index < Samples.Count ? Samples[index] : null; }
        }
    }

}