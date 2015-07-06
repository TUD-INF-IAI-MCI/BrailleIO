using System;
using System.Drawing;
using BrailleIO.Structs;

namespace BrailleIO.Interface
{
    #region Interfaces

    /// <summary>
    /// get Access to the properties for zooming
    /// </summary>
    public interface IZoomable
    {
        /// <summary>
        /// Gets the actual zoom-level (zoom factor).
        /// </summary>
        /// <returns>Zoom value as ratio</returns>
        double GetZoom();
        /// <summary>
        /// Sets the actual zoom.
        /// </summary>
        /// <param name="zoom">The zoom value as ratio (zoom factor).</param>
        void SetZoom(double zoom);
    }

    /// <summary>
    /// get access to properties for a border 
    /// </summary>
    public interface IViewBorder
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance has a border.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has a border; otherwise, <c>false</c>.
        /// </value>
        bool HasBorder { get; set; }
        /// <summary>
        /// Gets or sets the border.
        /// </summary>
        /// <value>The border.</value>
        BoxModel Border { get; set; }
    }

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

    /// <summary>
    /// get access to properties for a margin (distance between other surrounding objects and the border)
    /// </summary>
    public interface IViewMargin
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance has a margin.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has a margin; otherwise, <c>false</c>.
        /// </value>
        bool HasMargin { get; set; }
        /// <summary>
        /// Gets or sets the margin. The margin is the outer space around an area. Space between the objects and the border.
        /// </summary>
        /// <value>The margin.</value>
        BoxModel Margin { get; set; }
    }

    /// <summary>
    /// Interface for properties for handling a view box setting. 
    /// This means there is a content that can be larger or smaller than the available display space. 
    /// </summary>
    public interface IViewBoxModel
    {
        /// <summary>
        /// Gets or sets the view box. The viewBox defines the viewBox in size and offset to the content
        /// </summary>
        /// <value>The view box.</value>
        Rectangle ViewBox { get; set; }
        /// <summary>
        /// Gets or sets the content box. The real view box. 
        /// The space that can be used to show content. It can maximum be the Size of the ViewBox.
        /// Normally it is less. The Size of the ContentBox depends on the size of the ViewBox with respect of margin, border and padding.
        /// </summary>
        /// <value>The content box.</value>
        Rectangle ContentBox { get; set; }
        /// <summary>
        /// Gets or sets the width of the content. 
        /// This is used to show the Scrollbars and to estimate the ratio between the content box and the hidden content.
        /// </summary>
        /// <value>The width of the whole content.</value>
        int ContentWidth { get; set; }
        /// <summary>
        /// Gets or sets the height of the content. 
        /// This is used to show the Scrollbars and to estimate the ratio between the content box and the hidden content.
        /// </summary>
        /// <value>The height of the whole content.</value>
        int ContentHeight { get; set; }
    }

    /// <summary>
    /// Enables an implementing instance to indicate the possibilities for panning operation 
    /// </summary>
    public interface IPannable
    {
        /// <summary>
        /// Gets or sets a value indicating whether to show some scrollbars.
        /// </summary>
        /// <value><c>true</c> if to show scrollbars; otherwise, <c>false</c>.</value>
        bool ShowScrollbars { get; set; }
        /// <summary>
        /// Gets the offset in horizontal direction.
        /// Should normally be negative. Moves the position of the content by the offset.
        /// E.g. a negative offset should move the content to the left and the viewable 
        /// region to the right.
        /// </summary>
        /// <returns>the horizontal offset</returns>
        int GetXOffset();
        /// <summary>
        /// Sets the offset in horizontal direction.
        /// Should normally be negative. Moves the position of the content by the offset.
        /// E.g. a negative offset should move the content to the left and the viewable 
        /// region to the right.
        /// </summary>
        /// <param name="x">The offset in horizontal direction.</param>
        void SetXOffset(int x);
        /// <summary>
        /// Gets the offset in vertical direction.
        /// Should normally be negative. Moves the position of the content by the offset.
        /// E.g. a negative offset should move the content to up and the viewable 
        /// region down.
        /// </summary>
        /// <returns>the vertical offset</returns>
        int GetYOffset();
        /// <summary>
        /// Sets the offset in vertical direction.
        /// Should normally be negative. Moves the position of the content by the offset.
        /// E.g. a negative offset should move the content to up and the viewable
        /// region down.
        /// </summary>
        /// <param name="x">The offset in vertical direction.</param>
        void SetYOffset(int y);
        /// <summary>
        /// Move the content in both direction simultaneous. 
        /// So the vertical and horizontal offset will been set at the same time.
        /// </summary>
        /// <param name="p">The offsets in vertical and horizontal direction to move.</param>
        /// <returns></returns>
        Point Move(Point p);
    }

    /// <summary>
    /// Enables an implementing instance to enable access to there position properties
    /// </summary>
    public interface IPosition
    {
        /// <summary>
        /// Sets the horizontal, left start position left ==> The X position.
        /// </summary>
        /// <param name="left">The left.</param>
        void SetLeft(int left);
        /// <summary>
        /// Gets the horizontal, left start position ==> the X position.
        /// </summary>
        /// <returns></returns>
        int GetLeft();
        /// <summary>
        /// Sets the vertical, top position 00> the Y position.
        /// </summary>
        /// <param name="top">The top.</param>
        void SetTop(int top);
        /// <summary>
        /// Gets the vertical, top position ==> the Y position./// </summary>
        /// <returns></returns>
        int GetTop();
        /// <summary>
        /// Sets the width.
        /// </summary>
        /// <param name="width">The width.</param>
        void SetWidth(int width);
        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <returns>the width</returns>
        int GetWidth();
        /// <summary>
        /// Sets the height.
        /// </summary>
        /// <param name="height">The height.</param>
        void SetHeight(int height);
        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <returns>the height</returns>
        int GetHeight();
    }

    /// <summary>
    /// Enables an implementing instance to give access to properties of there visibility
    /// </summary>
    public interface IViewable
    {
        /// <summary>
        /// Sets the visibility.
        /// </summary>
        /// <param name="visible">if set to <c>true</c> the instance shoulb bee visible; otherwise the instance is hidden</param>
        void SetVisibility(bool visible);
        /// <summary>
        /// Determines whether this instance is visible.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance is visible; otherwise, <c>false</c> if the instance is hidden.
        /// </returns>
        bool IsVisible();
    }

    /// <summary>
    /// Enables an implementing instance to give access to properties of there used Threshold
    /// </summary>
    public interface IContrastThreshold
    {
        /// <summary>
        /// Sets the contrast threshold.
        /// </summary>
        /// <param name="threshold">The threshold.</param>
        /// <returns>the new set threshold</returns>
        int SetContrastThreshold(int threshold);
        /// <summary>
        /// Gets the contrast threshold.
        /// </summary>
        /// <returns>the threshold</returns>
        int GetContrastThreshold();
    }

    #endregion

    #region Abstract Implementations

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
        public void SetBorder(BoxModel box)
        {
            this.HasBorder = true;
            this.Border = box;
        }
        /// <summary>
        /// Sets all border.
        /// </summary>
        /// <param name="width">The border in all direction.</param>
        public void SetBorder(uint width) { SetBorder(new BoxModel(width)); }
        /// <summary>
        /// Sets the border.
        /// </summary>
        /// <param name="top">The top border.</param>
        /// <param name="right">The right border.</param>
        /// <param name="bottom">The bottom border.</param>
        /// <param name="left">The left border.</param>
        public void SetBorder(uint top, uint right, uint bottom, uint left) { SetBorder(new BoxModel(top, right, bottom, left)); }
        /// <summary>
        /// Sets the border.
        /// </summary>
        /// <param name="top">The top border.</param>
        /// <param name="horizontal">The horizontal border (left + right).</param>
        /// <param name="bottom">The bottom border.</param>
        public void SetBorder(uint top, uint horizontal, uint bottom) { SetBorder(new BoxModel(top, horizontal, bottom)); }
        /// <summary>
        /// Sets the horizontal and vertical border.
        /// </summary>
        /// <param name="vertical">The vertical border (top + bottom).</param>
        /// <param name="horizontal">The horizontal border (left + right).</param>
        public void SetBorder(uint vertical, uint horizontal) { SetBorder(new BoxModel(vertical, horizontal)); }

        private bool _hasBorder;
        /// <summary>
        /// Gets or sets a value indicating whether this instance has a border.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has a border; otherwise, <c>false</c>.
        /// </value>
        public bool HasBorder { get { return _hasBorder && Border.HasBox(); } set { _hasBorder = value; } }
        /// <summary>
        /// Gets or sets the border.
        /// </summary>
        /// <value>The border.</value>
        public BoxModel Border { get; set; }
    }

    /// <summary>
    /// Abstract Implementation for <see cref="IViewPadding"/>
    /// Enables the direct usage of the reimplemented interface <seealso cref="IViewPadding"/>
    /// Enables the direct usage of the reimplemented interface <seealso cref="IViewMargin"/>
    /// </summary>
    public abstract class AbstractViewPaddingBase : AbstractViewMarginBase, IViewPadding
    {
        /// <summary>
        /// Sets the padding.
        /// </summary>
        /// <param name="box">The padding box model.</param>
        public void SetPadding(BoxModel box)
        {
            this.HasPadding = true;
            this.Padding = box;
        }
        /// <summary>
        /// Sets all paddings.
        /// </summary>
        /// <param name="width">The padding in all direction.</param>
        public void SetPadding(uint width) { SetPadding(new BoxModel(width)); }
        /// <summary>
        /// Sets the paddings.
        /// </summary>
        /// <param name="top">The top padding.</param>
        /// <param name="right">The right padding.</param>
        /// <param name="bottom">The bottom padding.</param>
        /// <param name="left">The left padding.</param>
        public void SetPadding(uint top, uint right, uint bottom, uint left) { SetPadding(new BoxModel(top, right, bottom, left)); }
        /// <summary>
        /// Sets the paddings.
        /// </summary>
        /// <param name="top">The top padding.</param>
        /// <param name="horizontal">The horizontal paddings (left + right).</param>
        /// <param name="bottom">The bottom padding.</param>
        public void SetPadding(uint top, uint horizontal, uint bottom) { SetPadding(new BoxModel(top, horizontal, bottom)); }
        /// <summary>
        /// Sets the horizontal and vertical paddings.
        /// </summary>
        /// <param name="vertical">The vertical paddings (top + bottom).</param>
        /// <param name="horizontal">The horizontal paddings (left + right).</param>
        public void SetPadding(uint vertical, uint horizontal) { SetPadding(new BoxModel(vertical, horizontal)); }

        private bool _hasPadding;
        /// <summary>
        /// Gets or sets a value indicating whether this instance has a padding.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has a padding; otherwise, <c>false</c>.
        /// </value>
        public bool HasPadding { get { return _hasPadding && Padding.HasBox(); } set { _hasPadding = value; } }
        /// <summary>
        /// Gets or sets the padding. The padding is the inner space between the border and the content.
        /// </summary>
        /// <value>The padding.</value>
        public BoxModel Padding { get; set; }
    }

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

    /// <summary>
    /// Abstract Implementation for an Element that has a fully functional view box ==> This element has also a full box model including margin, border and padding.
    /// Enables the direct usage of the reimplemented interface <seealso cref="IViewBoxModel"/> for defining a viewable area
    /// Enables the direct usage of the reimplemented interface <seealso cref="IPosition"/> for set and get positions
    /// Enables the direct usage of the reimplemented interface <seealso cref="IPannable"/> for making a oversize content pannable
    /// Enables the direct usage of the reimplemented interface <seealso cref="IViewBorder"/> for defining a border
    /// Enables the direct usage of the reimplemented interface <seealso cref="IViewPadding"/> for defining a padding
    /// Enables the direct usage of the reimplemented interface <seealso cref="IViewMargin"/> for defining a margin
    /// </summary>
    public abstract class AbstractViewBoxModelBase : AbstractViewBorderBase, IViewBoxModel, IPosition, IPannable
    {
        #region IViewBoxModel Member
        private readonly object _viewLock = new object();
        private Rectangle _viewBox = new Rectangle();

        /// <summary>
        /// Rectangle given dimensions and position of the whole view range or screen including the ContentBox, margin, padding and border (see BoxModel).
        /// </summary>
        public Rectangle ViewBox { get { return _viewBox; } set { lock (_viewLock) { _viewBox = value; updateContentBoxFromViewBox(); } } }
        private readonly object _contLock = new object();
        private Rectangle _contBox = new Rectangle();
        /// <summary>
        /// Rectangle given dimensions and position of the view range or screen that can be used for displaying content.
        /// 
        /// 
        /// BrailleIOScreen                                     ViewBox
        /// ┌────────────────────────────────────────────────╱─┐
        /// │              BrailleIOViewRange              ╱   │
        /// │╔═ Margin ════════════════════════════════════════╗│
        /// │║   Border                                        ║│
        /// │║    Padding                                      ║│
        /// │║  ┌╴╴╴╴╴╴╴╴╴╴╴╴╴╴╴╴╴╴╴╴╴╴╴╴┐ ║│
        /// │║  │                                            │ ║│
        /// │║  │                                            │ ║│
        /// │║  │                                            │ ║│
        /// │║  │              ContentBox                    │ ║│
        /// │║  │      = space to present content            │ ║│
        /// │║  │                                            │ ║│
        /// │║  │                                            │ ║│
        /// │║  │                                            │ ║│
        /// │║  └╴╴╴╴╴╴╴╴╴╴╴╴╴╴╴╴╴╴╴╴╴╴╴╴┘ ║│
        /// │║                                                 ║│
        /// │╚═════════════════════════════════════════════════╝│
        /// │╔═════════════════════════════════════════════════╗│
        /// │║           another BrailleIOViewRange            ║│
        /// │╚═════════════════════════════════════════════════╝│
        /// └───────────────────────────────────────────────────┘
        /// 
        /// </summary>
        public Rectangle ContentBox
        {
            get { updateContentBoxFromViewBox(); return _contBox; }
            set
            {
                lock (_contLock)
                {
                    _contBox = value; updateViewBoxFromContentBox();
                }
            }
        }

        private Rectangle updateContentBoxFromViewBox()
        {
            uint tOffset = Margin.Top + Border.Top + Padding.Top;
            uint bOffset = Margin.Bottom + Border.Bottom + Padding.Bottom;
            uint lOffset = Margin.Left + Border.Left + Padding.Left;
            uint rOffset = Margin.Right + Border.Right + Padding.Right;

            int w = (int)(ViewBox.Width - lOffset - rOffset);
            int h = (int)(ViewBox.Height - tOffset - bOffset);

            _contBox = new Rectangle((int)lOffset, (int)tOffset, w < 0 ? 0 : w, h < 0 ? 0 : h);

            //check if show scroll bars
            if (ShowScrollbars)
            {
                //check if content is bigger than available space (contentBox)
                if (ContentHeight > _contBox.Height)
                {
                    //TODO: check if enough space for a scroll bar
                    _contBox.Width = Math.Max(Padding.Right > 0 ? _contBox.Width - 2 : _contBox.Width - 3, 0);
                }

                if (ContentWidth > _contBox.Width)
                {
                    //TODO: check if enough space for a scroll bar
                    _contBox.Height = Math.Max(Padding.Bottom > 0 ? _contBox.Height - 2 : _contBox.Height - 3, 0);
                }
            }

            return _contBox;
        }

        private Rectangle updateViewBoxFromContentBox()
        {
            uint tOffset = Margin.Top + Border.Top + Padding.Top;
            uint bOffset = Margin.Bottom + Border.Bottom + Padding.Bottom;
            uint lOffset = Margin.Left + Border.Left + Padding.Left;
            uint rOffset = Margin.Right + Border.Right + Padding.Right;

            ViewBox = new Rectangle(ViewBox.X, ViewBox.Y, (int)(_contBox.Width + lOffset + rOffset), (int)(_contBox.Height + tOffset + bOffset));

            return ViewBox;
        }

        int _cw = 0;
        /// <summary>
        /// Gets or sets the width of the content.
        /// This is used to show the Scrollbars and to estimate the ratio between the content box and the hidden content.
        /// </summary>
        /// <value>The width of the whole content.</value>
        public virtual int ContentWidth
        {
            get { return _cw; }
            set { _cw = Math.Max(0, value); }
        }

        int _ch = 0;
        /// <summary>
        /// Gets or sets the height of the content.
        /// This is used to show the Scrollbars and to estimate the ratio between the content box and the hidden content.
        /// </summary>
        /// <value>The height of the whole content.</value>
        public virtual int ContentHeight
        {
            get { return _ch; }
            set { _ch = Math.Max(0, value); }
        }

        #endregion

        #region IPosition Member

        /// <summary>
        /// set new X position on display
        /// </summary>
        /// <param name="left">
        /// new x offset
        /// </param>
        public void SetLeft(int left)
        {
            var nVB = ViewBox;
            nVB.X = Math.Abs(left);
            ViewBox = nVB;
        }

        /// <summary>
        /// get X Offset on device
        /// </summary>
        /// <returns>
        /// int x_offset
        /// </returns>
        public int GetLeft()
        {
            return ViewBox.Left;
        }

        /// <summary>
        /// set new Y position on display
        /// </summary>
        /// <param name="top">
        /// new y position
        /// </param>
        public void SetTop(int top)
        {
            var nVB = ViewBox;
            nVB.Y = Math.Abs(top);
            ViewBox = nVB;
        }

        /// <summary>
        /// get Y Offset on device
        /// </summary>
        /// <returns>
        /// int y_offset on device
        /// </returns>
        public int GetTop()
        {
            return ViewBox.Top;
        }

        /// <summary>
        /// set Width on Device
        /// </summary>
        /// <param name="width">width on device</param>
        public void SetWidth(int width)
        {
            var nVB = ViewBox;
            nVB.Width = Math.Abs(width);
            ViewBox = nVB;
        }

        /// <summary>
        /// get Width on device
        /// </summary>
        /// <returns></returns>
        public int GetWidth()
        {
            return ViewBox.Width;
        }

        /// <summary>
        /// set Height on device
        /// </summary>
        /// <param name="height">height on device</param>
        public void SetHeight(int height)
        {
            var nVB = ViewBox;
            nVB.Height = Math.Abs(height);
            ViewBox = nVB;
        }

        /// <summary>
        /// get Height on Device
        /// </summary>
        /// <returns></returns>
        public int GetHeight()
        {
            return ViewBox.Height;
        }
        #endregion

        #region IPannable Member
        public Point OffsetPosition = new Point();
        public bool ShowScrollbars { get; set; }
        public int GetXOffset() { return OffsetPosition.X; }

        public void SetXOffset(int x) { OffsetPosition.X = x; }

        public int GetYOffset() { return OffsetPosition.Y; }

        public void SetYOffset(int y) { OffsetPosition.Y = y; }

        #endregion

        /// <summary>
        /// Moves the viewBox in vertical direction.
        /// </summary>
        /// <param name="steps">The steps (pins) to move.</param>
        /// <returns>The new ViewBox</returns>
        public Point MoveVertical(int steps) { return Move(new Point(0, steps)); }
        /// <summary>
        /// Moves the viewBox in horizontal direction.
        /// </summary>
        /// <param name="steps">The steps (pins) to move.</param>
        /// <returns>The new ViewBox</returns>
        public Point MoveHorizontal(int steps) { return Move(new Point(steps, 0)); }
        /// <summary>
        /// Moves the viewBox in the given directions.
        /// </summary>
        /// <param name="stepsX">The steps (pins) to move in horizontal direction.</param>
        /// <param name="stepsY">The steps (pins) to move in vertical direction.</param>
        /// <returns></returns>
        public Point Move(int stepsX, int stepsY) { return Move(new Point(stepsX, stepsY)); }
        /// <summary>
        /// Moves the viewBox in the given directions.
        /// </summary>
        /// <param name="direktions">The steps (pins) to move.</param>
        /// <returns>The new ViewBox</returns>
        public Point Move(Point direktions)
        {
            int maxXOffset = -(Math.Max(ContentWidth - ContentBox.Width, 0));
            int maxYOffset = -(Math.Max(ContentHeight - ContentBox.Height, 0));

            OffsetPosition.X = Math.Max(Math.Min(OffsetPosition.X + direktions.X, 0), maxXOffset);
            OffsetPosition.Y = Math.Max(Math.Min(OffsetPosition.Y + direktions.Y, 0), maxYOffset);
            return OffsetPosition;
        }
        /// <summary>
        /// Moves the viewBox to the given position.
        /// </summary>
        /// <param name="point">Position to which the viewBox should be moved.</param>
        /// <returns>The new ViewBox</returns>
        public Point MoveTo(Point point)
        {
            OffsetPosition.X = Math.Min(point.X, 0);
            OffsetPosition.Y = Math.Min(point.Y, 0);

            return OffsetPosition;
        }
    }

    #endregion
}