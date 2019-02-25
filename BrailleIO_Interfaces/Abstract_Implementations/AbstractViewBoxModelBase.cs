using System;
using System.Drawing;

namespace BrailleIO.Interface
{
    /// <summary>
    /// Abstract Implementation for an Element that has a fully functional view box ==> This element has also a full box model including margin, border and padding.
    /// Enables the direct usage of the reimplemented interface <seealso cref="IViewBoxModel"/> for defining a viewable area
    /// Enables the direct usage of the reimplemented interface <seealso cref="IPosition"/> for set and get positions
    /// Enables the direct usage of the reimplemented interface <seealso cref="IPannable"/> for making a oversize content pannable
    /// Enables the direct usage of the reimplemented interface <seealso cref="IViewBorder"/> for defining a border
    /// Enables the direct usage of the reimplemented interface <seealso cref="IViewPadding"/> for defining a padding
    /// Enables the direct usage of the reimplemented interface <seealso cref="IViewMargin"/> for defining a margin
    /// </summary>
    /// <remarks> </remarks>
    public abstract class AbstractViewBoxModelBase : AbstractViewBorderBase, IViewBoxModel, IPosition, IPannable, IBrailleIOPropertiesChangedEventSupplier, IViewable
    {
        #region Members

        private string _name = String.Empty;
        /// <summary>
        /// Gets or sets the name of this View - Some kind of UID.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        virtual public String Name
        {
            get { return _name; }
            set
            {
                bool fire = !_name.Equals(value);
                _name = value;
                if (fire) firePropertyChangedEvent("Name");
            }
        }

        #endregion

        #region IViewBoxModel Member
        private readonly object _viewLock = new object();
        private Rectangle _viewBox = new Rectangle();

        /// <summary>Rectangle given dimensions and position of the whole view range or screen including the ContentBox, margin, padding and border (see BoxModel).</summary>
        /// <value>The available space on the display.</value>
        public virtual Rectangle ViewBox { get { return _viewBox; } set { lock (_viewLock) { _viewBox = value; updateContentBoxFromViewBox(); firePropertyChangedEvent("ViewBox"); } } }
        private readonly object _contLock = new object();
        private Rectangle _contBox = new Rectangle();
        /// <summary>
        /// Rectangle given dimensions and position of the view range or screen that can be used for displaying content.
        /// 
        /// 
        /// BrailleIOScreen                                   ViewBox
        /// ┌───────────────────────────────────────────────╱───┐
        /// │              BrailleIOViewRange              ╱    │
        /// │╔═ Margin ════════════════════════════════════════╗│
        /// │║   Border                                        ║│
        /// │║    Padding                                      ║│
        /// │║  ┌┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┐ ║│
        /// │║  ┆                                            ┆ ║│
        /// │║  ┆                                            ┆ ║│
        /// │║  ┆                                            ┆ ║│
        /// │║  ┆              ContentBox                    ┆ ║│
        /// │║  ┆      = space to present content            ┆ ║│
        /// │║  ┆                                            ┆ ║│
        /// │║  ┆                                            ┆ ║│
        /// │║  ┆                                            ┆ ║│
        /// │║  └┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┄┘ ║│
        /// │║                                                 ║│
        /// │╚═════════════════════════════════════════════════╝│
        /// │╔═════════════════════════════════════════════════╗│
        /// │║           another BrailleIOViewRange            ║│
        /// │╚═════════════════════════════════════════════════╝│
        /// └───────────────────────────────────────────────────┘
        /// 
        /// </summary>
        /// <value>the available space for the content</value>
        public virtual Rectangle ContentBox
        {
            get { updateContentBoxFromViewBox(); return _contBox; }
            set
            {
                lock (_contLock)
                {
                    _contBox = value; updateViewBoxFromContentBox();
                    firePropertyChangedEvent("ContentBox");
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
            set { _cw = Math.Max(0, value); firePropertyChangedEvent("ContentWidth"); }
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
            set { _ch = Math.Max(0, value); firePropertyChangedEvent("ContentHeight"); }
        }

        #endregion

        #region IPosition Member

        /// <summary>
        /// set new X position on display
        /// </summary>
        /// <param name="left">
        /// new x offset
        /// </param>
        public virtual void SetLeft(int left)
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
        public virtual int GetLeft()
        {
            return ViewBox.Left;
        }

        /// <summary>
        /// set new Y position on display
        /// </summary>
        /// <param name="top">
        /// new y position
        /// </param>
        public virtual void SetTop(int top)
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
        public virtual int GetTop()
        {
            return ViewBox.Top;
        }

        /// <summary>
        /// set Width on Device
        /// </summary>
        /// <param name="width">width on device</param>
        public virtual void SetWidth(int width)
        {
            var nVB = ViewBox;
            nVB.Width = Math.Abs(width);
            ViewBox = nVB;
        }

        /// <summary>
        /// get Width on device
        /// </summary>
        /// <returns>The current width in pins</returns>
        public virtual int GetWidth()
        {
            return ViewBox.Width;
        }

        /// <summary>
        /// set Height on device
        /// </summary>
        /// <param name="height">height on device</param>
        public virtual void SetHeight(int height)
        {
            var nVB = ViewBox;
            nVB.Height = Math.Abs(height);
            ViewBox = nVB;
        }

        /// <summary>
        /// get Height on Device
        /// </summary>
        /// <returns>The current height in pins</returns>
        public virtual int GetHeight()
        {
            return ViewBox.Height;
        }
        #endregion

        #region IPannable Member

        private Point _offsetPosition = new Point();
        /// <summary>
        /// Gets or sets the offset position.
        /// The offset position is the position of the content related to the view port - 
        /// the visible part.
        /// For handling panning the content is moved underneath view port - like a sheet
        /// of paper under a fixed microscope. This means the offsets normally are negative values! 
        /// </summary>
        /// <value>
        /// The offset position of the content related to the view port.
        /// </value>
        public virtual Point OffsetPosition { get { return _offsetPosition; } set { _offsetPosition = value; firePropertyChangedEvent("OffsetPosition"); } }

        private bool _show_scrollbars = false;
        /// <summary>
        /// Gets or sets a value indicating whether to show some scrollbars.
        /// </summary>
        /// <value>
        ///   <c>true</c> if to show scrollbars; otherwise, <c>false</c>.
        /// </value>
        public bool ShowScrollbars { get { return _show_scrollbars; } set { _show_scrollbars = value; firePropertyChangedEvent("ShowScrollbars"); } }

        /// <summary>
        /// Gets the offset in horizontal direction.
        /// Should normally be negative. Moves the position of the content by the offset.
        /// E.g. a negative offset should move the content to the left and the viewable
        /// region to the right.
        /// </summary>
        /// <returns>
        /// the horizontal offset
        /// </returns>
        public virtual int GetXOffset() { return OffsetPosition.X; }

        /// <summary>
        /// Sets the offset in horizontal direction.
        /// Should normally be negative. Moves the position of the content by the offset.
        /// E.g. a negative offset should move the content to the left and the viewable
        /// region to the right.
        /// </summary>
        /// <param name="x">The offset in horizontal direction.</param>
        public virtual void SetXOffset(int x) { OffsetPosition = new Point(x, GetYOffset()); }

        /// <summary>
        /// Gets the offset in vertical direction.
        /// Should normally be negative. Moves the position of the content by the offset.
        /// E.g. a negative offset should move the content to up and the viewable
        /// region down.
        /// </summary>
        /// <returns>
        /// the vertical offset
        /// </returns>
        public virtual int GetYOffset() { return OffsetPosition.Y; }

        /// <summary>
        /// Sets the offset in vertical direction.
        /// Should normally be negative. Moves the position of the content by the offset.
        /// E.g. a negative offset should move the content to up and the viewable
        /// region down.
        /// </summary>
        /// <param name="y">The offset in vertical direction.</param>
        public virtual void SetYOffset(int y) { OffsetPosition = new Point(GetXOffset(), y); }

        #endregion

        /// <summary>
        /// Moves the viewBox in vertical direction.
        /// </summary>
        /// <param name="steps">The steps (pins) to move.</param>
        /// <returns>the new <see cref="OffsetPosition"/></returns>
        public virtual Point MoveVertical(int steps) { return Move(new Point(0, steps)); }
        /// <summary>
        /// Moves the viewBox in horizontal direction.
        /// </summary>
        /// <param name="steps">The steps (pins) to move.</param>
        /// <returns>the new <see cref="OffsetPosition"/></returns>
        public virtual Point MoveHorizontal(int steps) { return Move(new Point(steps, 0)); }
        /// <summary>
        /// Moves the viewBox in the given directions.
        /// </summary>
        /// <param name="stepsX">The steps (pins) to move in horizontal direction.</param>
        /// <param name="stepsY">The steps (pins) to move in vertical direction.</param>
        /// <returns>the new <see cref="OffsetPosition"/></returns>
        public virtual Point Move(int stepsX, int stepsY) { return Move(new Point(stepsX, stepsY)); }
        /// <summary>
        /// Moves the viewBox in the given directions.
        /// </summary>
        /// <param name="direktions">The steps (pins) to move.</param>
        /// <returns>the new <see cref="OffsetPosition"/></returns>
        public virtual Point Move(Point direktions)
        {
            int maxXOffset = -(Math.Max(ContentWidth - ContentBox.Width, 0));
            int maxYOffset = -(Math.Max(ContentHeight - ContentBox.Height, 0));

            OffsetPosition = new Point(
                Math.Max(Math.Min(OffsetPosition.X + direktions.X, 0), maxXOffset),
                Math.Max(Math.Min(OffsetPosition.Y + direktions.Y, 0), maxYOffset));

            return OffsetPosition;
        }
        /// <summary>
        /// Moves the viewBox to the given position.
        /// </summary>
        /// <param name="point">Position to which the viewBox should be moved.</param>
        /// <returns>the new <see cref="OffsetPosition"/></returns>
        public virtual Point MoveTo(Point point)
        {
            SetXOffset(Math.Min(point.X, 0));
            SetYOffset(Math.Min(point.Y, 0));

            return OffsetPosition;
        }

        #region IBrailleIOPropertiesChangedEventSupplier

        /// <summary>
        /// Occurs when a property has changed.
        /// </summary>
        public event EventHandler<BrailleIOPropertyChangedEventArgs> PropertyChanged;

        /// <summary>Fires the property changed event.</summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void firePropertyChangedEvent(string propertyName)
        {
            //System.Diagnostics.Debug.WriteLine("Property changed : " + propertyName);
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new BrailleIOPropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region IViewable

        private bool is_visible = true;

        /// <summary>
        /// set Visibility of ViewRange
        /// </summary>
        /// <remarks> </remarks>
        /// <param name="visible">
        /// bool desired visibility
        /// </param>
        public virtual void SetVisibility(bool visible)
        {
            bool fire = this.is_visible != visible;
            this.is_visible = visible;
            if (fire) firePropertyChangedEvent("Visibility");
        }

        /// <summary>
        /// Determines whether this instance is visible.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is visible; otherwise, <c>false</c> if the instance is hidden.
        /// </returns>
        public virtual bool IsVisible() { return this.is_visible; }

        #endregion

    }

}
