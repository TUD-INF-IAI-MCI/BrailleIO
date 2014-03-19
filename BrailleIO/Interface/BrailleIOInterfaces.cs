using BrailleIO.Structs;
using System.Drawing;
using System;

namespace BrailleIO.Interface
{
    #region Interfaces

    public interface IViewBorder
    {
        bool HasBorder { get; set; }
        /// <summary>
        /// Gets or sets the border.
        /// </summary>
        /// <value>The border.</value>
        BoxModel Border { get; set; }
    }

    public interface IViewPadding
    {
        bool HasPadding { get; set; }
        /// <summary>
        /// Gets or sets the padding. The padding is the inner space between the border and the content.
        /// </summary>
        /// <value>The padding.</value>
        BoxModel Padding { get; set; }
    }

    public interface IViewMargin
    {
        bool HasMargin { get; set; }
        /// <summary>
        /// Gets or sets the margin. The margin is the outer space around an area. Space between the objects and the border.
        /// </summary>
        /// <value>The margin.</value>
        BoxModel Margin { get; set; }
    }

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

    public interface IPannable
    {
        bool ShowScrollbars { get; set; }
        int GetXOffset();
        void SetXOffset(int x);
        int GetYOffset();
        void SetYOffset(int y);
        Point Move(Point p);
    }

    public interface IPosition
    {
        void SetLeft(int left);
        int GetLeft();
        void SetTop(int top);
        int GetTop();
        void SetWidth(int width);
        int GetWidth();
        void SetHeight(int height);
        int GetHeight();
    }

    public interface IViewable
    {
        void SetVisibility(bool visible);
        bool IsVisible();
    }

    public interface IContrastThreshold
    {
        int SetContrastThreshold(int threshold);
        int getContrastThreshold();
    }

    #endregion

    #region Abstract Implementations

    public abstract class AbstractViewBorderBase : AbstractViewPaddingBase, IViewBorder
    {
        public void SetBorder(BoxModel box)
        {
            this.HasBorder = true;
            this.Border = box;
        }
        public void SetBorder(uint width) { SetBorder(new BoxModel(width)); }
        public void SetBorder(uint top, uint right, uint bottom, uint left) { SetBorder(new BoxModel(top, right, bottom, left)); }
        public void SetBorder(uint top, uint horizontal, uint bottom) { SetBorder(new BoxModel(top, horizontal, bottom)); }
        public void SetBorder(uint vertical, uint horizontal) { SetBorder(new BoxModel(vertical, horizontal)); }

        private bool _hasBorder;
        public bool HasBorder { get { return _hasBorder && Border.HasBox(); } set { _hasBorder = value; } }
        public BoxModel Border { get; set; }
    }

    public abstract class AbstractViewPaddingBase : AbstractViewMarginBase, IViewPadding
    {
        public void SetPadding(BoxModel box)
        {
            this.HasPadding = true;
            this.Padding = box;
        }
        public void SetPadding(uint width) { SetPadding(new BoxModel(width)); }
        public void SetPadding(uint top, uint right, uint bottom, uint left) { SetPadding(new BoxModel(top, right, bottom, left)); }
        public void SetPadding(uint top, uint horizontal, uint bottom) { SetPadding(new BoxModel(top, horizontal, bottom)); }
        public void SetPadding(uint vertical, uint horizontal) { SetPadding(new BoxModel(vertical, horizontal)); }

        private bool _hasPadding;
        public bool HasPadding { get { return _hasPadding && Padding.HasBox(); } set { _hasPadding = value; } }
        public BoxModel Padding { get; set; }
    }

    public abstract class AbstractViewMarginBase : IViewMargin
    {
        public void SetMargin(BoxModel box)
        {
            this.HasMargin = true;
            this.Margin = box;
        }
        public void SetMargin(uint width) { SetMargin(new BoxModel(width)); }
        public void SetMargin(uint top, uint right, uint bottom, uint left) { SetMargin(new BoxModel(top, right, bottom, left)); }
        public void SetMargin(uint top, uint horizontal, uint bottom) { SetMargin(new BoxModel(top, horizontal, bottom)); }
        public void SetMargin(uint vertical, uint horizontal) { SetMargin(new BoxModel(vertical, horizontal)); }

        private bool _hasMargin;
        public bool HasMargin { get { return _hasMargin && Margin.HasBox(); } set { _hasMargin = value; } }
        public BoxModel Margin { get; set; }
    }

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
    }

    #endregion
}