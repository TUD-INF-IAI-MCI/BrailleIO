using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrailleIO.Structs;

namespace BrailleIO.Interface
{
    /// <summary>
    /// get access to properties for a padding (distance between the border and the content)
    /// </summary>
    public interface IViewPadding
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance has a padding.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has a padding; otherwise, <c>false</c>.
        /// </value>
        bool HasPadding { get; set; }
        /// <summary>
        /// Gets or sets the padding. The padding is the inner space between the border and the content.
        /// </summary>
        /// <value>The padding.</value>
        BoxModel Padding { get; set; }
    }
}
