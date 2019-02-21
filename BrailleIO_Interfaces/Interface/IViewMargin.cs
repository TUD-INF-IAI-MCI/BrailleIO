using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrailleIO.Structs;

namespace BrailleIO.Interface
{
    /// <summary>
    /// get access to properties for a margin (distance between other surrounding objects and the border)
    /// </summary>
		/// <remarks> </remarks>
    public interface IViewMargin
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance has a margin.
        /// </summary>
		/// <remarks> </remarks>
        /// <value>
        /// 	<c>true</c> if this instance has a margin; otherwise, <c>false</c>.
        /// </value>
        bool HasMargin { get; set; }
        /// <summary>
        /// Gets or sets the margin. The margin is the outer space around an area. Space between the objects and the border.
        /// </summary>
		/// <remarks> </remarks>
        /// <value>The margin.</value>
        BoxModel Margin { get; set; }
    }
}
