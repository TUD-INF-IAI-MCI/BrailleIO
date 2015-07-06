using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrailleIO.Structs;

namespace BrailleIO.Interface
{
    /// <summary>
    /// Abstract Implementation for <see cref="IViewBorder"/> ==> This element has a full box model including margin, border and padding.
    /// Enables the direct usage of the reimplemented interface <seealso cref="IViewBorder"/>
    /// Enables the direct usage of the reimplemented interface <seealso cref="IViewPadding"/>
    /// Enables the direct usage of the reimplemented interface <seealso cref="IViewMargin"/>
    /// </summary>
    public abstract class AbstractViewBorderBase : AbstractViewPaddingBase, IViewBorder
    {
        /// <summary>
        /// Sets the border.
        /// </summary>
        /// <param name="box">The border box model.</param>
        public virtual void SetBorder(BoxModel box)
        {
            this.HasBorder = true;
            this.Border = box;
        }
        /// <summary>
        /// Sets all border.
        /// </summary>
        /// <param name="width">The border in all direction.</param>
        public virtual void SetBorder(uint width) { SetBorder(new BoxModel(width)); }
        /// <summary>
        /// Sets the border.
        /// </summary>
        /// <param name="top">The top border.</param>
        /// <param name="right">The right border.</param>
        /// <param name="bottom">The bottom border.</param>
        /// <param name="left">The left border.</param>
        public virtual void SetBorder(uint top, uint right, uint bottom, uint left) { SetBorder(new BoxModel(top, right, bottom, left)); }
        /// <summary>
        /// Sets the border.
        /// </summary>
        /// <param name="top">The top border.</param>
        /// <param name="horizontal">The horizontal border (left + right).</param>
        /// <param name="bottom">The bottom border.</param>
        public virtual void SetBorder(uint top, uint horizontal, uint bottom) { SetBorder(new BoxModel(top, horizontal, bottom)); }
        /// <summary>
        /// Sets the horizontal and vertical border.
        /// </summary>
        /// <param name="vertical">The vertical border (top + bottom).</param>
        /// <param name="horizontal">The horizontal border (left + right).</param>
        public virtual void SetBorder(uint vertical, uint horizontal) { SetBorder(new BoxModel(vertical, horizontal)); }

        private bool _hasBorder;
        /// <summary>
        /// Gets or sets a value indicating whether this instance has a border.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has a border; otherwise, <c>false</c>.
        /// </value>
        public virtual bool HasBorder { get { return _hasBorder && Border.HasBox(); } set { _hasBorder = value; } }
        /// <summary>
        /// Gets or sets the border.
        /// </summary>
        /// <value>The border.</value>
        public virtual BoxModel Border { get; set; }
    }

}
