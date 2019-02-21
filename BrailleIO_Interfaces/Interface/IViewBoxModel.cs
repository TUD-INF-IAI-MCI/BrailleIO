using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace BrailleIO.Interface
{
    /// <summary>
    /// Interface for properties for handling a view box setting. 
    /// This means there is a content that can be larger or smaller than the available display space. 
    /// </summary>
		/// <remarks> </remarks>
    public interface IViewBoxModel
    {
        /// <summary>
        /// Gets or sets the view box. The viewBox defines the viewBox in size and offset to the content
        /// </summary>
		/// <remarks> </remarks>
        /// <value>The view box.</value>
        Rectangle ViewBox { get; set; }
        /// <summary>
        /// Gets or sets the content box. The real view box. 
        /// The space that can be used to show content. It can maximum be the Size of the ViewBox.
        /// Normally it is less. The Size of the ContentBox depends on the size of the ViewBox with respect of margin, border and padding.
        /// </summary>
		/// <remarks> </remarks>
        /// <value>The content box.</value>
        Rectangle ContentBox { get; set; }
        /// <summary>
        /// Gets or sets the width of the content. 
        /// This is used to show the Scrollbars and to estimate the ratio between the content box and the hidden content.
        /// </summary>
		/// <remarks> </remarks>
        /// <value>The width of the whole content.</value>
        int ContentWidth { get; set; }
        /// <summary>
        /// Gets or sets the height of the content. 
        /// This is used to show the Scrollbars and to estimate the ratio between the content box and the hidden content.
        /// </summary>
		/// <remarks> </remarks>
        /// <value>The height of the whole content.</value>
        int ContentHeight { get; set; }
    }
}
