using System;
using System.Drawing;
using BrailleIO.Interface;

namespace BrailleIO
{
    public class BrailleIOViewRange : AbstractViewBoxModelBase, IViewable, IContrastThreshold
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
        private String text;
        private Object otherContent;
        private IBrailleIOContentRenderer otherContentRender;

        // zoom multiplicator
        private double zoom = 1.0;

        public const double MAX_ZOOM_LEVEL = 2;

        public bool InvertImage { get; set; }

        public String Name { get; set; }

        #region Renderers
        private BrailleIO.Renderer.BrailleIOImageToMatrixRenderer _ir;
        public BrailleIO.Renderer.BrailleIOImageToMatrixRenderer ImageRenderer { get { if (_ir == null) _ir = new Renderer.BrailleIOImageToMatrixRenderer(); return _ir; } }
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
            this.image = img;
            this.is_image = true;
            this.is_text = this.is_matrix = this.is_other = false;
        }
        /// <summary>
        /// Sets the bitmap that should be rendered.
        /// </summary>
        /// <param name="_img">The imgage.</param>
        public void SetBitmap(Image img)
        {
            try
            {
                SetBitmap(new Bitmap(img));
            }
            catch (ArgumentException) { }
        }

        /// <summary>
        /// Gets the image to render.
        /// </summary>
        /// <returns></returns>
        public Bitmap GetImage()
        {
            return this.image;
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
        }

        public void SetOtherContent(Object contet, IBrailleIOMatrixRenderer renderer)
        {
            this.otherContent = contet;
            if (renderer != null) this.otherContentRender = renderer;
            this.is_image = this.is_matrix = this.is_text = false;
        }

        public object GetOtherContent()
        {
            return this.otherContent;
        }

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
                var img = GetImage();
                if (ContentBox != null && img != null)
                {
                    double wr = (double)ContentBox.Width / (double)img.Width;
                    double hr = (double)ContentBox.Height / (double)img.Height;
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
            System.Diagnostics.Debug.WriteLine("Threshold ist: " + threshold.ToString());
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
    }
}