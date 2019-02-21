using System;
using System.Collections.Generic;


namespace Gestures.Recognition.GestureData
{

    /// <summary>
    /// Represents a class of gesture templates.
    /// </summary>
		/// <remarks> </remarks>
    /// <author>Dr. rer. nat. Michael Schmidt - Techniche Universität Dresden 2014.</author>
    public class GestureClass
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GestureClass"/> class.
        /// </summary>
		/// <remarks> </remarks>
        public GestureClass()
        {
            GestureList = new List<GestureTemplate>();
        }

        private List<GestureTemplate> GestureList { get; set; }

        /// <summary>
        /// Gets the <see cref="GestureTemplate"/> at the specified index.
        /// </summary>
		/// <remarks> </remarks>
        /// <value></value>
        public virtual GestureTemplate this[int index]
        {
            get
            {
                return index < Count && index >= 0 ? GestureList[index] : null;
            }
        }
        /// <summary>
        /// Adds the specified gesture template to the class.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="gestureTemplate">The gesture template.</param>
        public virtual void Add(GestureTemplate gestureTemplate)
        {
            GestureList.Add(gestureTemplate);
        }

        /// <summary>
        /// Gets the number of currently held gesture templates.
        /// </summary>
		/// <remarks> </remarks>
        /// <value>The count.</value>
        public int Count { get { return GestureList.Count; } }

        /// <summary>
        /// Gets or sets the name of the class.
        /// </summary>
		/// <remarks> </remarks>
        /// <value>The name of the class.</value>
        public String ClassName { get; set; }
    
    }
}