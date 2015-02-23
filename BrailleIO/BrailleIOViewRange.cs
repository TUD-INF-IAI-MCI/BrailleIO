using System;
using System.Drawing;
using BrailleIO.Interface;

namespace BrailleIO
{
    /// <summary>
    /// Basic structure to hold content that should been displayed on an output device
    /// </summary>
    public class BrailleIOViewRange : AbstractViewBoxModelBase, IViewable, IContrastThreshold, IZoomable
    {
        #region Members

        private int threshold = 130;

        // is visible?
        private bool is_visible = true;

        // type of?
        private bool is_matrix = false;
        private bool is_image = false;
        private bool is_text = false;
        private bool is_other = false;

        // raw data
        private bool[,] matrix;
        private Bitmap image;
        private Size imageSize = new Size();
        private String text;
        private Object otherContent;


        // zoom multiplicator
        private double zoom = 1.0;

        public const double MAX_ZOOM_LEVEL = 2;

        public bool InvertImage { get; set; }

        public String Name { get; set; }

        public BrailleIOScreen Parent { get; private set; }

        #region for sorting

        private volatile int tstamp;

        private int zIndex;

        #endregion

        #region Renderers
        private readonly BrailleIO.Renderer.BrailleIOImageToMatrixRenderer _ir = new Renderer.BrailleIOImageToMatrixRenderer();
        private readonly BrailleIO.Renderer.BrailleIOViewMatixRenderer _mr = new BrailleIO.Renderer.BrailleIOViewMatixRenderer();
        private readonly BrailleIO.Renderer.NewBrailleRenderer _tr = new BrailleIO.Renderer.NewBrailleRenderer();

        private IBrailleIOContentRenderer _cr;
        public IBrailleIOContentRenderer ContentRender
        {
            get { return _cr; }
            private set
            {
                bool fire = _cr != value;
                _cr = value;
                if(fire) fireRendererChanged();
            }
        }

        #endregion

        #endregion

        #region Constructors

        #region Image Constructors
        public BrailleIOViewRange(int left, int top, int width, int height, Bitmap image)
        {
            this.ViewBox = new Rectangle(left, top, width, height);
            SetBitmap(image);
        }

        #endregion

        #region Text Constructors

        public BrailleIOViewRange(int left, int top, int width, int height, String text)
        {
            this.ViewBox = new Rectangle(left, top, width, height);
            SetText(text);
        }

        #endregion

        #region Matrix Constructors

        public BrailleIOViewRange(int left, int top, int width, int height)
            : this(left, top, width, height, new bool[0, 0]) { }

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
        /// <param name="_img">The imgage.</param>
        public void SetBitmap(Bitmap img)
        {
            try
            {
                //clean up the memory
                if (this.image != null) this.image.Dispose();
                this.image = img.Clone() as Bitmap;
                imageSize = new Size(img.Width, img.Height);
                this.is_image = true;
                this.is_text = this.is_matrix = this.is_other = false;
                this.ContentRender = _ir;
            }
            catch { }
        }
        /// <summary>
        /// Sets the bitmap that should be rendered.
        /// </summary>
        /// <param name="_img">The imgage.</param>
        public void SetBitmap(Image img)
        {
            try
            {
                if (img != null) SetBitmap(new Bitmap(img));
                else SetBitmap(null);
            }
            catch (ArgumentException) { }
        }

        /// <summary>
        /// Gets the image to render.
        /// </summary>
        /// <returns></returns>
        public Bitmap GetImage()
        {
            try
            {
                return this.image.Clone() as Bitmap;
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// set Visibility of ViewRange
        /// </summary>
        /// <param name="visible">
        /// bool desired visibility
        /// </param>
        public void SetVisibility(bool visible)
        {
            this.is_visible = visible;
        }

        public bool IsVisible() { return this.is_visible; }

        /// <summary>
        /// Determines whether this instance should render a matrix.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance renders a matrix; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsMatrix()
        {
            return this.is_matrix;
        }

        /// <summary>
        /// Determines whether this instance should render an image.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance renders an image; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsImage()
        {
            return this.is_image;
        }

        /// <summary>
        /// Determines whether this instance should render a text.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance renders a text; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsText()
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
        }

        /// <summary>
        /// Sets an generic content and a related renderer for this type.
        /// </summary>
        /// <param name="contet">The contet.</param>
        /// <param name="renderer">The renderer - can not be null.</param>
        public void SetOtherContent(Object contet, IBrailleIOContentRenderer renderer)
        {
            if (renderer == null)
            {
                throw new ArgumentException("No content render set! The content renderer can not be null.", "renderer");
            }

            this.otherContent = contet;
            if (renderer != null) this.ContentRender = renderer;
            this.is_other = true;
            this.is_image = this.is_matrix = this.is_text = false;
        }

        /// <summary>
        /// Gets the content that is not of the standart types.
        /// </summary>
        /// <returns></returns>
        public object GetOtherContent()
        {
            return this.otherContent;
        }

        /// <summary>
        /// Determines whether this instance has a special type of content taht can not been rendered with one of the standard renderer.
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
            this.zoom = zoom;
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
            if (threshold <= 0) threshold = 1;
            if (threshold > 255) threshold = 255;
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
        public void SetZIndex(int zIndex) { this.zIndex = zIndex; }
        /// <summary>
        /// Gets the z-index of the view range. A lager z-index overlays a smaller.
        /// </summary>
        /// <returns>the z-index of the viewRange.</returns>
        public int GetZIndex() { return this.zIndex; }


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

    }
}