using System;
using System.Drawing;
using System.Runtime.ExceptionServices;
using System.Threading;
using BrailleIO.Interface;
using BrailleIO.Structs;

namespace BrailleIO
{
    /// <summary>
    /// Basic structure to hold content that should been displayed on an output device
    /// </summary>
    public class BrailleIOViewRange : AbstractViewBoxModelBase, IContrastThreshold, IZoomable, IBrailleIOContentChangedEventSupplier
    {
        #region Members

        private int threshold = 130;

        // type of?
        private bool is_matrix = false;
        private bool is_image = false;
        private bool is_text = false;
        private bool is_other = false;

        // raw data
        private bool[,] matrix;
        private Bitmap _img;
        private readonly Object _imgLock = new Object();
        private Bitmap image
        {
            get
            {
                lock (_imgLock)
                {
                    return _img;
                }
            }
            set
            {
                lock (_imgLock)
                {
                    _img = value;
                }
            }
        }
        private Size imageSize = new Size();
        private String text;
        private Object otherContent;

        // zoom multiplier
        private double zoom = 1.0;

        /// <summary>
        /// The maximal zoom level that can be applied.
        /// </summary>
        public double MAX_ZOOM_LEVEL = 5;

        private bool _invert_image = false;
        /// <summary>
        /// Gets or sets a value indicating whether the image should be inverted or not.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [invert the image]; otherwise, <c>false</c>.
        /// </value>
        public bool InvertImage
        {
            get { return _invert_image; }
            set
            {
                bool fire = _invert_image != value;
                _invert_image = value;
                if (fire) firePropertyChangedEvent("InvertImage");
            }
        }      

        private BrailleIOScreen _parent = null;
        /// <summary>
        /// Gets or sets the parent <see cref="BrailleIOScreen"/>.
        /// </summary>
        /// <value>
        /// The parent Screen.
        /// </value>
        public BrailleIOScreen Parent
        {
            get { return _parent; }
            protected set
            {
                bool fire = _parent != value;
                _parent = value;
                if (fire) firePropertyChangedEvent("Parent");
            }
        }

        volatile bool _render = true;

        /// <summary>
        /// Gets or sets a flag indicating whether this <see cref="BrailleIOViewRange"/> should be re-rendered because of the content was changed.
        /// </summary>
        /// <value><c>true</c> if the renderer should re-render the content; otherwise, <c>false</c>.</value>
        public bool Render
        {
            get { return _render; }
            set
            {
                _render = true;
            }
        }

        #region for sorting

        //private volatile int tstamp;

        private int zIndex;

        #endregion

        #region Renderer

        private readonly BrailleIO.Renderer.BrailleIOImageToMatrixRenderer _ir = new Renderer.BrailleIOImageToMatrixRenderer();
        private readonly BrailleIO.Renderer.BrailleIOViewMatixRenderer _mr = new BrailleIO.Renderer.BrailleIOViewMatixRenderer();
        private readonly BrailleIO.Renderer.MatrixBrailleRenderer _tr = new BrailleIO.Renderer.MatrixBrailleRenderer();

        private IBrailleIOContentRenderer _cr;
        /// <summary>
        /// Gets the currently used render for the specific content.
        /// </summary>
        /// <value>
        /// The content render.
        /// </value>
        public IBrailleIOContentRenderer ContentRender
        {
            get { return _cr; }
            private set
            {
                bool fire = _cr != value;
                _cr = value;
                if (fire) fireRendererChanged();
            }
        }

        /// <summary>
        /// Updates the size of the content.
        /// </summary>
        public void UpdateContentSize()
        {
            if (Render)
            {
                if (ContentRender != null)
                {
                    Object cnt = GetContent();
                    ContentRender.RenderMatrix(this, cnt);

                    //clean up the cloned elements (e.g. images)
                    if (cnt is IDisposable) { ((IDisposable)cnt).Dispose(); }
                    else { cnt = null; }
                }
            }
        }

        #endregion

        #endregion

        #region Constructors

        #region Image Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BrailleIOViewRange"/> class.
        /// </summary>
        /// <param name="left">The left position inside the parent Screen.</param>
        /// <param name="top">The top position inside the parent screen.</param>
        /// <param name="width">The overall width.</param>
        /// <param name="height">The overall height.</param>
        /// <param name="image">The content image.</param>
        public BrailleIOViewRange(int left, int top, int width, int height, Bitmap image)
        {
            this.ViewBox = new Rectangle(left, top, width, height);
            SetBitmap(image);
        }

        #endregion

        #region Text Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BrailleIOViewRange" /> class.
        /// </summary>
        /// <param name="left">The left position inside the parent Screen.</param>
        /// <param name="top">The top position inside the parent screen.</param>
        /// <param name="width">The overall width.</param>
        /// <param name="height">The overall height.</param>
        /// <param name="text">The content text.</param>
        public BrailleIOViewRange(int left, int top, int width, int height, String text)
        {
            this.ViewBox = new Rectangle(left, top, width, height);
            SetText(text);
        }

        #endregion

        #region Matrix Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BrailleIOViewRange" /> class.
        /// </summary>
        /// <param name="left">The left position inside the parent Screen.</param>
        /// <param name="top">The top position inside the parent screen.</param>
        /// <param name="width">The overall width.</param>
        /// <param name="height">The overall height.</param>
        public BrailleIOViewRange(int left, int top, int width, int height)
            : this(left, top, width, height, new bool[0, 0]) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BrailleIOViewRange" /> class.
        /// </summary>
        /// <param name="left">The left position inside the parent Screen.</param>
        /// <param name="top">The top position inside the parent screen.</param>
        /// <param name="width">The overall width.</param>
        /// <param name="height">The overall height.</param>
        /// <param name="matrix">The content matrix.</param>
        public BrailleIOViewRange(int left, int top, int width, int height, bool[,] matrix)
        {
            this.ViewBox = new Rectangle(left, top, width, height);
            SetMatrix(matrix);
        }
        #endregion

        #endregion

        /// <summary>
        /// Sets the parent Screen.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <returns></returns>
        public bool SetParent(BrailleIOScreen parent)
        {
            bool success = false;
            if (parent != null)
            {
                if (parent.GetViewRange(Name) == null || !parent.GetViewRange(Name).Equals(this))
                    parent.AddViewRange(this.Name, this);
                Parent = parent;
                success = true;
                firePropertyChangedEvent("Parent");
            }
            return success;
        }

        /// <summary>
        /// set matrix 
        /// </summary>
        /// <param name="matrix">
        /// bool[,] matrix
        /// </param>
        public void SetMatrix(bool[,] matrix)
        {
            this.matrix = matrix;
            this.is_matrix = true;
            this.is_text = this.is_image = this.is_other = false;
            this.ContentRender = _mr;
            Render = true;
            fireContentChangedEvent();
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// bool[,] matrix
        /// </returns>
        public bool[,] GetMatrix()
        {
            return this.matrix;
        }

        /// <summary>
        /// Sets the bitmap that should be rendered.
        /// </summary>
        /// <param name="img">The image.</param>
        [HandleProcessCorruptedStateExceptions]
        public void SetBitmap(Bitmap img)
        {
            try
            {
                //clean up the memory
                if (this.image != null) this.image.Dispose();
                if (img == null) img = new Bitmap(1, 1);

                int tries = 0;
                while (tries++ < 5)
                {
                    try
                    {
                        this.image = img.Clone() as Bitmap;
                    }
                    catch (InvalidOperationException) { Thread.Sleep(5); }
                    catch (AccessViolationException) { Thread.Sleep(5); }
                    catch (Exception)
                    {
                        break;
                    }
                }
                imageSize = new Size(img.Width, img.Height);
                this.is_image = true;
                this.is_text = this.is_matrix = this.is_other = false;
                this.ContentRender = _ir;
                img.Dispose();
                Render = true;
                fireContentChangedEvent();
            }
            catch { }
        }
        /// <summary>
        /// Sets the bitmap that should be rendered.
        /// </summary>
        /// <param name="img">The content image.</param>
        public void SetBitmap(Image img)
        {
            try
            {
                if (img != null) SetBitmap(new Bitmap(img));
                else
                {
                    SetBitmap(null);
                    Render = true;
                    fireContentChangedEvent();
                }
            }
            catch (ArgumentException) { }
        }

        /// <summary>
        /// Gets the image to render.
        /// </summary>
        /// <returns></returns>
        [HandleProcessCorruptedStateExceptions]
        public Bitmap GetImage()
        {
            int i = 0;
            while (i++ < 5)
            {
                try
                {
                    return this.image.Clone() as Bitmap;
                }
                catch (InvalidOperationException)
                {
                    Thread.Sleep(5);
                }
                catch (AccessViolationException)
                {
                    Thread.Sleep(5);
                }
                catch (System.Exception) { break; }
            }
            return null;
        }

        /// <summary>
        /// Determines whether this instance should render a matrix.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance renders a matrix; otherwise, <c>false</c>.
        /// </returns>
        public bool IsMatrix()
        {
            return this.is_matrix;
        }

        /// <summary>
        /// Determines whether this instance should render an image.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance renders an image; otherwise, <c>false</c>.
        /// </returns>
        public bool IsImage()
        {
            return this.is_image;
        }

        /// <summary>
        /// Determines whether this instance should render a text.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance renders a text; otherwise, <c>false</c>.
        /// </returns>
        public bool IsText()
        {
            return this.is_text;
        }

        /// <summary>
        /// Gets the text to render.
        /// </summary>
        /// <returns></returns>
        public string GetText()
        {
            return this.text;
        }

        /// <summary>
        /// Sets the text that should be rendered.
        /// </summary>
        /// <param name="text">The text.</param>
        public void SetText(string text)
        {
            this.text = text;
            this.is_text = true;
            this.is_image = this.is_matrix = this.is_other = false;
            this.ContentRender = _tr;
            Render = true;
            fireContentChangedEvent();
        }

        /// <summary>
        /// Sets an generic content and a related renderer for this type.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="renderer">The renderer - can not be null.</param>
        public void SetOtherContent(Object content, IBrailleIOContentRenderer renderer)
        {
            if (renderer == null)
            {
                throw new ArgumentException("No content render set! The content renderer can not be null.", "renderer");
            }
            //bool update = !this.is_other || !this.otherContent.Equals(content);
            this.otherContent = content;
            if (renderer != null) this.ContentRender = renderer;
            this.is_other = true;
            this.is_image = this.is_matrix = this.is_text = false;
            Render = true;
            fireContentChangedEvent();
            //if (update) UpdateContentSize();
        }

        /// <summary>
        /// Gets the content that is not of the standard types.
        /// </summary>
        /// <returns></returns>
        public object GetOtherContent()
        {
            return this.otherContent;
        }

        /// <summary>
        /// Determines whether this instance has a special type of content that can not been rendered with one of the standard renderer.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance has an specialized content type other; otherwise, <c>false</c>.
        /// </returns>
        public bool IsOther()
        {
            return this.is_other;
        }

        /// <summary>
        /// Gets the actual zoom-level.
        /// </summary>
        /// <returns>Zoom value as ratio</returns>
        public double GetZoom()
        {
            if (zoom <= 0 && IsImage()) // get zoom to fit in the view range
            {
                if (ContentBox != null && imageSize != null)
                {
                    double wr = (double)ContentBox.Width / (double)imageSize.Width;
                    double hr = (double)ContentBox.Height / (double)imageSize.Height;
                    zoom = Math.Min(wr, hr);
                }
            }
            else if (zoom > MAX_ZOOM_LEVEL) { zoom = MAX_ZOOM_LEVEL; }
            return this.zoom;
        }

        /// <summary>
        /// Sets the actual zoom.
        /// </summary>
        /// <param name="zoom">The zoom value as ratio.</param>
        public void SetZoom(double zoom)
        {
            if (zoom > MAX_ZOOM_LEVEL) throw new ArgumentException("The zoom level is with a value of " + zoom + "to high. The zoom level should not be more than " + MAX_ZOOM_LEVEL + ".", "zoom");
            //TODO: update the content size
            this.zoom = zoom;
            Render = true;
            UpdateContentSize();
            firePropertyChangedEvent("Zoom");
        }

        /// <summary>
        /// Sets the contrast threshold for image contrast rastering.
        /// If lightness of a color is lower than this threshold, the pin will be lowered. 
        /// A higher threshold leads lighter points to raise pins. 
        /// A low threshold leads darker pins to stay lowered.
        /// Threshold has to be between 0 and 255.
        /// </summary>
        /// <param name="threshold">The threshold.</param>
        /// <returns>the new threshold</returns>
        public int SetContrastThreshold(int threshold)
        {
            Render = true;
            return this.threshold = Math.Max(Math.Min(threshold, 255), 0);
        }

        /// <summary>
        /// Gets the contrast for image contrast rastering.
        /// If lightness of a color is lower than this threshold, the pin will be lowered. 
        /// A higher threshold leads lighter points to raise pins. 
        /// A low threshold leads darker pins to stay lowered.
        /// Have to be between 0 and 255.
        /// </summary>
        /// <returns></returns>
        public int GetContrastThreshold() { return threshold; }

        /// <summary>
        /// Sets the z-index of the view range. A lager z-index overlays a smaller.
        /// </summary>
        /// <param name="zIndex">the z-index of the viewRange.</param>
        public void SetZIndex(int zIndex)
        {
            this.zIndex = zIndex;
            firePropertyChangedEvent("ZIndex");
        }
        /// <summary>
        /// Gets the z-index of the view range. A lager z-index overlays a smaller.
        /// </summary>
        /// <returns>the z-index of the viewRange.</returns>
        public int GetZIndex() { return this.zIndex; }

        /// <summary>
        /// Get the Generic content
        /// </summary>
        /// <returns>the untyped content of this view range</returns>
        public Object GetContent()
        {
            object cnt = null;
            if (is_image)
            {
                cnt = GetImage();
            }
            else if (is_matrix)
            {
                cnt = GetMatrix();
            }
            else if (is_text)
            {
                cnt = GetText();
            }
            else
            {
                cnt = GetOtherContent();
            }
            return cnt;
        }

        /// <summary>
        /// Occurs when the renderer was changed.
        /// </summary>
        public event EventHandler<EventArgs> RendererChanged;

        private void fireRendererChanged()
        {
            if (RendererChanged != null)
            {
                RendererChanged.DynamicInvoke(this, null);
            }
        }


        #region IBrailleIOContentChangedEventSupplier

        /// <summary>
        /// Occurs when the content has been changed.
        /// </summary>
        public event EventHandler<EventArgs> ContentChanged;

        private void fireContentChangedEvent()
        {
            if (ContentRender != null && ContentRender is BrailleIO.Renderer.ICacheingRenderer)
            {
                ((BrailleIO.Renderer.ICacheingRenderer)ContentRender).ContentOrViewHasChanged(this, GetContent());
            }
            if (ContentChanged != null)
            {
                ContentChanged.DynamicInvoke(this, null);
            }
        }

        #endregion

        #region Override
        /*
         * Overrides setters for firing property changed events 
         */

        #region AbstractViewBoxModelBase

        /// <summary>
        /// Rectangle given dimensions and position of the whole view range or screen 
        /// including the ContentBox, margin, padding and border (see BoxModel).
        /// </summary>
        /// <value>ViewBox of this view (position and size)</value>
        public override Rectangle ViewBox
        {
            set
            {
                bool fire = !ViewBox.Equals(value);
                base.ViewBox = value;
                firePropertyChangedEvent("ViewBox");
            }
        }

        /// <summary>
        /// Rectangle given dimensions and position of the view range or screen that can be used for displaying content.
        /// The position is the top-left position of ContentBox inside the viewRange.
        /// The size is the visible size inside the view range --> ViewBox size without margin, border and padding.
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
        /// </summary>
        /// <value></value>
        public override Rectangle ContentBox
        {
            set
            {
                bool fire = !ContentBox.Equals(value);
                base.ContentBox = value;
                if (fire) firePropertyChangedEvent("ContentBox");
            }
        }

        /// <summary>
        /// Sets the offset position. This is the relation between the 
        /// content's position to the visible view. This value is added to the 
        /// contents positions to compute the points rendered to the output.
        /// Standard is 0,0 which is means, the content is started rendered in the 
        /// top-left corner of the viewBox. To move the content to the top - which is 
        /// something like a pan down - you have to set the y-position to a negative 
        /// value. To move the content down, you have to add a positive offset. The 
        /// same happens for horizontal movement. A negative value will move the 
        /// content to the left - which is a pan to the right - and a positive value 
        /// will move the content to the right.
        /// </summary>
        /// <value>
        /// The offset position.
        /// </value>
        public override Point OffsetPosition
        {
            set
            {
                bool fire = !OffsetPosition.Equals(value);
                base.OffsetPosition = value;
                if (fire) firePropertyChangedEvent("OffsetPosition");
            }
        }

        #endregion

        #region Collision Tests

        /// <summary>
        /// Determines whether this view contains a specific point on the display.
        /// Attention: a <c>true</c> does not mean that the point is in the content. 
        /// It can be on a spacing or the border.
        /// </summary>
        /// <param name="x">The horizontal position on the display.</param>
        /// <param name="y">The vertical position on the display.</param>
        /// <returns><c>true</c> if this view contain the point; otherwise <c>false</c>.</returns>
        public bool ContainsPoint(int x, int y)
        {
            if (x >= ViewBox.X && x < ViewBox.X + ViewBox.Width
                && y >= ViewBox.Y && y < ViewBox.Height + ViewBox.Y
                ) 
                return true;

            return false;
        }

        /// <summary>
        /// Contents the contains point.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        public bool ContentContainsPoint(int x, int y)
        {
            if (ContainsPoint(x,y))
            {
                int _x = x-ViewBox.X;
                int _y = y - ViewBox.Y;

                if(_x >= ContentBox.X && _x < ContentBox.Right
                    && _y >= ContentBox.Y && _y < ContentBox.Bottom
                    )
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Translates the device position into a position inside the content with respect to 
        /// ViewRange-, ContenBox- and Offset-Position.
        /// </summary>
        /// <param name="x">The horizontal position on the device.</param>
        /// <param name="y">The vertical position on the device.</param>
        /// <returns>A point with the position inside the content if possible; otherwise a point with the coordinates -1,-1.</returns>
        public Point TranslateDevicePositionToContentPosition(int x, int y)
        {
            Point p = new Point(-1, -1);

            if (ContentContainsPoint(x, y))
            {
                int _x = x - ViewBox.X - ContentBox.X;
                int _y = y - ViewBox.Y - ContentBox.Y;

                p.X = _x + OffsetPosition.X;
                p.Y = _y + OffsetPosition.Y;
            }
            return p;
        } 

        #endregion

        #region AbstractViewBorderBase

        /// <summary>
        /// Gets or sets the border.
        /// </summary>
        /// <value>
        /// The border.
        /// </value>
        public override BoxModel Border
        {
            set
            {
                bool fire = !Border.Equals(value);
                base.Border = value;
                if (fire) firePropertyChangedEvent("Border");
            }
        }

        #endregion

        #region AbstractViewPaddingBase

        /// <summary>
        /// Gets or sets the padding. The padding is the inner space between the border and the content.
        /// </summary>
        /// <value>
        /// The padding.
        /// </value>
        public override BoxModel Padding
        {
            set
            {
                bool fire = !Padding.Equals(value);
                base.Padding = value;
                if (fire) firePropertyChangedEvent("Padding");
            }
        }

        #endregion

        #region AbstractViewMarginBase

        /// <summary>
        /// Gets or sets the margin. The margin is the outer space around an area. Space between the objects and the border.
        /// </summary>
        /// <value>
        /// The margin.
        /// </value>
        public override BoxModel Margin
        {
            set
            {
                bool fire = !Margin.Equals(value);
                base.Margin = value;
                if (fire) firePropertyChangedEvent("Margin");
            }
        }

        #endregion

        #endregion

    }
}