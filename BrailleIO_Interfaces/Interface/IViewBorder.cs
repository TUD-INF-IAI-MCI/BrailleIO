using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrailleIO.Structs;

namespace BrailleIO.Interface
{
    /// <summary>
    /// get access to properties for a border 
    /// </summary>
		/// <remarks> </remarks>
    public interface IViewBorder
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance has a border.
        /// </summary>
		/// <remarks> </remarks>
        /// <value>
        /// 	<c>true</c> if this instance has a border; otherwise, <c>false</c>.
        /// </value>
        bool HasBorder { get; set; }
        /// <summary>
        /// Gets or sets the border.
        /// </summary>
		/// <remarks> </remarks>
        /// <value>The border.</value>
        BoxModel Border { get; set; }
    }
}
