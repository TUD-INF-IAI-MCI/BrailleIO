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
        private bool is_active = false;

        // type of?
        private bool is_matrix = false;
        private bool is_image = false;
        private bool is_text = false;

        // raw data
        private bool[,] matrix;
        private Bitmap image;
        private String text;

        // zoom multiplicator
        private double zoom = 1.0;

        public const double MAX_ZOOM_LEVEL = 5;

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
            setBitmap(image);
        }

        #endregion

        #region Text Constructors

        public BrailleIOViewRange(int left, int top, int width, int height, String text)
        {
            this.ViewBox = new Rectangle(left, top, width, height);
            setText(text);
        }

        #endregion

        #region Matrix Constructors

        public BrailleIOViewRange(int left, int top, int width, int height)
            : this(left, top, width, height, new bool[height, width]) { }

        public BrailleIOViewRange(int left, int top, int width, int height, bool[,] matrix)
        {
            this.ViewBox = new Rectangle(left, top, width, height);
            setMatrix(matrix);
        }
        #endregion

        #endregion

        /// <summary>
        /// set matrix 
        /// </summary>
        /// <param name="matrix">
        /// bool[,] matrix
        /// </param>
        public void setMatrix(bool[,] matrix)
        {
            this.matrix = matrix;
            this.is_matrix = true;
            this.is_text = this.is_image = false;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// bool[,] matrix
        /// </returns>
        public bool[,] getMatrix()
        {
            return this.matrix;
        }

        /// <summary>
        /// Sets the bitmap that should be rendered.
        /// </summary>
        /// <param name="img">The imgage.</param>
        public void setBitmap(Bitmap img)
        {
            this.image = img;
            this.is_image = true;
            this.is_text = this.is_matrix = false;
        }
        /// <summary>
        /// Sets the bitmap that should be rendered.
        /// </summary>
        /// <param name="img">The imgage.</param>
        public void setBitmap(Image img)
        {
            try
            {
                setBitmap(new Bitmap(img));
            }
            catch (ArgumentException) { }
        }

        /// <summary>
        /// Gets the image to render.
        /// </summary>
        /// <returns></returns>
        public Bitmap getImage()
        {
            return this.image;
        }

        /// <summary>
        /// set Visibility of ViewRange
        /// </summary>
        /// <param name="which">
        /// bool desired visibility
        /// </param>
        public void _setState(bool which)
        {
            this.is_active = which;
        }

        /// <summary>
        /// Determines whether this instance should render a matrix.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance renders a matrix; otherwise, <c>false</c>.
        /// </returns>
        internal bool isMatrix()
        {
            return this.is_matrix;
        }

        /// <summary>
        /// Determines whether this instance should render an image.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance renders an image; otherwise, <c>false</c>.
        /// </returns>
        internal bool isImage()
        {
            return this.is_image;
        }

        /// <summary>
        /// Determines whether this instance should render a text.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance renders a text; otherwise, <c>false</c>.
        /// </returns>
        internal bool isText()
        {
            return this.is_text;
        }

        /// <summary>
        /// Gets the text to render.
        /// </summary>
        /// <returns></returns>
        public string getText()
        {
            return this.text;
        }

        /// <summary>
        /// Sets the text that should be rendered.
        /// </summary>
        /// <param name="text">The text.</param>
        public void setText(string text)
        {
            this.text = text;
            this.is_text = true;
            this.is_image = this.is_matrix = false;
        }

        /// <summary>
        /// Gets the actual zoom-level.
        /// </summary>
        /// <returns>Zoom value as ratio</returns>
        public double getZoom()
        {
            if (zoom <= 0 && isImage()) // get zoom to fit in the view range
            {
                var img = getImage();
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
        public void setZoom(double zoom)
        {
            this.zoom = zoom;
            //TODO: zoom in the center
        }

        /// <summary>
        /// Sets the contrast threshold for image contrast rastering.
        /// If lightness of a color is lower than this threshold, the pin will be lowered. 
        /// A higher threshold leads lighter points to raise pins. 
        /// A low threshold leads darker pins to stay lowered.
        /// Have to be between 0 and 255.
        /// </summary>
        /// <param name="threshold">The threshold.</param>
        /// <returns>the new threshold</returns>
        public int SetContrastThreshold(int threshold)
        {
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
        public int getContrastThreshold() { return threshold; }
    }
}