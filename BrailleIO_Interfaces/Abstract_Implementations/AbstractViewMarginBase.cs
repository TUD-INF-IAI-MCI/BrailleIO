using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrailleIO.Structs;

namespace BrailleIO.Interface
{
    /// <summary>
    /// Abstract Implementation for <see cref="IViewMargin"/>
    /// Enables the direct usage of the reimplemented interface <seealso cref="IViewMargin"/>
    /// </summary>
    public abstract class AbstractViewMarginBase : IViewMargin
    {
        /// <summary>
        /// Sets the margin.
        /// </summary>
        /// <param name="box">The margin box model.</param>
        public void SetMargin(BoxModel box)
        {
            this.HasMargin = true;
            this.Margin = box;
        }

        /// <summary>
        /// Sets all margins.
        /// </summary>
        /// <param name="width">The margin in all direction.</param>
        public void SetMargin(uint width) { SetMargin(new BoxModel(width)); }
        /// <summary>
        /// Sets the margins.
        /// </summary>
        /// <param name="top">The top margin.</param>
        /// <param name="right">The right margin.</param>
        /// <param name="bottom">The bottom margin.</param>
        /// <param name="left">The left margin.</param>
        public void SetMargin(uint top, uint right, uint bottom, uint left) { SetMargin(new BoxModel(top, right, bottom, left)); }
        /// <summary>
        /// Sets the margins.
        /// </summary>
        /// <param name="top">The top margin.</param>
        /// <param name="horizontal">The horizontal margins (left + right).</param>
        /// <param name="bottom">The bottom margin.</param>
        public void SetMargin(uint top, uint horizontal, uint bottom) { SetMargin(new BoxModel(top, horizontal, bottom)); }
        /// <summary>
        /// Sets the horizontal and vertical margins.
        /// </summary>
        /// <param name="vertical">The vertical margins (top + bottom).</param>
        /// <param name="horizontal">The horizontal margins (left + right).</param>
        public void SetMargin(uint vertical, uint horizontal) { SetMargin(new BoxModel(vertical, horizontal)); }

        private bool _hasMargin;
        /// <summary>
        /// Gets or sets a value indicating whether this instance has a margin.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has a margin; otherwise, <c>false</c>.
        /// </value>
        public bool HasMargin { get { return _hasMargin && Margin.HasBox(); } set { _hasMargin = value; } }
        /// <summary>
        /// Gets or sets the margin. The margin is the outer space around an area. Space between the objects and the border.
        /// </summary>
        /// <value>The margin.</value>
        public BoxModel Margin { get; set; }
    }

}
