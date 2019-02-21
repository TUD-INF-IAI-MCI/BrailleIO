using BrailleIO.Structs;

namespace BrailleIO.Interface
{
    /// <summary>
    /// Abstract Implementation for <see cref="IViewPadding"/>
    /// Enables the direct usage of the reimplemented interface <seealso cref="IViewPadding"/>
    /// Enables the direct usage of the reimplemented interface <seealso cref="IViewMargin"/>
    /// </summary>
    /// <remarks> </remarks>
    public abstract class AbstractViewPaddingBase : AbstractViewMarginBase, IViewPadding
    {
        /// <summary>
        /// Sets the padding.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="box">The padding box model.</param>
        public virtual void SetPadding(BoxModel box)
        {
            this.HasPadding = true;
            this.Padding = box;
        }
        /// <summary>
        /// Sets all paddings.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="width">The padding in all direction.</param>
        public virtual void SetPadding(uint width) { SetPadding(new BoxModel(width)); }
        /// <summary>
        /// Sets the paddings.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="top">The top padding.</param>
        /// <param name="right">The right padding.</param>
        /// <param name="bottom">The bottom padding.</param>
        /// <param name="left">The left padding.</param>
        public virtual void SetPadding(uint top, uint right, uint bottom, uint left) { SetPadding(new BoxModel(top, right, bottom, left)); }
        /// <summary>
        /// Sets the paddings.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="top">The top padding.</param>
        /// <param name="horizontal">The horizontal paddings (left + right).</param>
        /// <param name="bottom">The bottom padding.</param>
        public virtual void SetPadding(uint top, uint horizontal, uint bottom) { SetPadding(new BoxModel(top, horizontal, bottom)); }
        /// <summary>
        /// Sets the horizontal and vertical paddings.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="vertical">The vertical paddings (top + bottom).</param>
        /// <param name="horizontal">The horizontal paddings (left + right).</param>
        public virtual void SetPadding(uint vertical, uint horizontal) { SetPadding(new BoxModel(vertical, horizontal)); }

        private bool _hasPadding;
        /// <summary>
        /// Gets or sets a value indicating whether this instance has a padding.
        /// </summary>
		/// <remarks> </remarks>
        /// <value>
        /// 	<c>true</c> if this instance has a padding; otherwise, <c>false</c>.
        /// </value>
        public virtual bool HasPadding { get { return _hasPadding && Padding.HasBox(); } set { _hasPadding = value; } }
        /// <summary>
        /// Gets or sets the padding. The padding is the inner space between the border and the content.
        /// </summary>
		/// <remarks> </remarks>
        /// <value>The padding.</value>
        public virtual BoxModel Padding { get; set; }
    }

}
