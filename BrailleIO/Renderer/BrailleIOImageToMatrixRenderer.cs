using System;
using System.Drawing;
using System.Drawing.Imaging;
using BrailleIO.Interface;

namespace BrailleIO.Renderer
{
    /// <summary>
    /// A renderer that translates an image into an bool matrix by applying a threshold on the lighness of a color pixel.
    /// </summary>
    /// <seealso cref="BrailleIO.Renderer.AbstractCachingRendererBase" />
    /// <seealso cref="BrailleIO.Interface.IBrailleIOContentRenderer" />
    public class BrailleIOImageToMatrixRenderer : AbstractCachingRendererBase, IBrailleIOContentRenderer
    {
        /// <summary>
        /// If lightness of a color is lower than this threshold, the pin will be lowered. 
        /// A higher threshold leads lighter points to raise pins. 
        /// A low threshold leads darker pins to stay lowered.
        /// Have to be between 0 and 255.
        /// </summary>
        private float Threshold = 210;
        /// <summary>
        /// Resets the threshold.
        /// If lightness of a color is lower than this threshold, the pin will be lowered. 
        /// A higher threshold leads lighter points to raise pins. 
        /// A low threshold leads darker pins to stay lowered.
        /// Have to be between 0 and 255.
        /// </summary>
        /// <returns>the new threshold</returns>
        public float ResetThreshold() { return this.Threshold = 210; }
        /// <summary>
        /// Sets the threshold.
        /// If lightness of a color is lower than this threshold, the pin will be lowered. 
        /// A higher threshold leads lighter points to raise pins. 
        /// A low threshold leads darker pins to stay lowered.
        /// Have to be between 0 and 255.
        /// </summary>
        /// <param name="threshold">The threshold.</param>
        /// <returns></returns>
        public float SetThreshold(float threshold) { return this.Threshold = Math.Min(Math.Max(threshold, 0), 255); }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="BrailleIOImageToMatrixRenderer"/> image should be rendered inverted.
        /// </summary>
        /// <value>
        ///   <c>true</c> if invert the image; otherwise, <c>false</c>.
        /// </value>
        public bool Invert { get; set; }

        /// <summary>
        /// Renders the image.
        /// </summary>
        /// <param name="img">The image.</param>
        /// <param name="view">The view.</param>
        /// <param name="zoom">The zoom factor.</param>
        /// <param name="threshold">The threshold to apply (between 0 ans 255).</param>
        /// <returns>a bool matrix</returns>
        public bool[,] RenderImage(Bitmap img, IViewBoxModel view, double zoom, float threshold) { return RenderImage(img, view, null, zoom, threshold); }
        /// <summary>
        /// Renders the image.
        /// </summary>
        /// <param name="img">The image.</param>
        /// <param name="view">The view.</param>
        /// <param name="offset">The offset for translation.</param>
        /// <param name="zoom">The zoom factor.</param>
        /// <param name="threshold">The threshold to apply (between 0 ans 255).</param>
        /// <returns>
        /// a bool matrix
        /// </returns>
        public bool[,] RenderImage(Bitmap img, IViewBoxModel view, IPannable offset, double zoom, float threshold)
        {
            Threshold = threshold;
            return RenderImage(img, view, offset, zoom);
        }
        /// <summary>
        /// Renders the image.
        /// </summary>
        /// <param name="img">The image.</param>
        /// <param name="view">The view.</param>
        /// <param name="zoom">The zoom factor.</param>
        /// <param name="autoThreshold">if set to <c>true</c> [automatic threshold] is applied.</param>
        /// <returns>
        /// a bool matrix
        /// </returns>
        public bool[,] RenderImage(Bitmap img, IViewBoxModel view, double zoom, bool autoThreshold) { return RenderImage(img, view, null, zoom, autoThreshold); }
        /// <summary>
        /// Renders the image.
        /// </summary>
        /// <param name="img">The image.</param>
        /// <param name="view">The view.</param>
        /// <param name="offset">The offset for translation.</param>
        /// <param name="zoom">The zoom factor.</param>
        /// <param name="autoThreshold">if set to <c>true</c> [automatic threshold] is applied.</param>
        /// <returns>
        /// a bool matrix
        /// </returns>
        public bool[,] RenderImage(Bitmap img, IViewBoxModel view, IPannable offset, double zoom, bool autoThreshold)
        {
            var vr = view.ContentBox;
            return RenderImage(img, view, offset, zoom, GraphicUtils.GetAverageGrayscale(vr.Width, vr.Height, new Bitmap(img, new Size((int)Math.Round(img.Width * zoom), (int)Math.Round(img.Height * zoom)))));
        }
        /// <summary>
        /// Renders the image.
        /// </summary>
        /// <param name="img">The image.</param>
        /// <param name="view">The view.</param>
        /// <param name="zoom">The zoom factor.</param>
        /// <returns>
        /// a bool matrix
        /// </returns>
        public bool[,] RenderImage(Bitmap img, IViewBoxModel view, double zoom) { return RenderImage(img, view, null, zoom); }
        /// <summary>
        /// Renders the image.
        /// </summary>
        /// <param name="img">The image.</param>
        /// <param name="view">The view.</param>
        /// <param name="offset">The offset for translation.</param>
        /// <param name="zoom">The zoom factor.</param>
        /// <returns>
        /// a bool matrix
        /// </returns>
        public bool[,] RenderImage(Bitmap img, IViewBoxModel view, IPannable offset, double zoom) { return RenderImage(img, view, offset, false, zoom); }

        //possible invert
        /// <summary>
        /// Renders the image.
        /// </summary>
        /// <param name="img">The image.</param>
        /// <param name="view">The view.</param>
        /// <param name="invert">if set to <c>true</c> the result will be inverted.</param>
        /// <param name="zoom">The zoom factor.</param>
        /// <param name="threshold">The threshold.</param>
        /// <returns>
        /// a bool matrix
        /// </returns>
        public bool[,] RenderImage(Bitmap img, IViewBoxModel view, bool invert, double zoom, float threshold) { return RenderImage(img, view, null, invert, zoom, threshold); }
        /// <summary>
        /// Renders the image.
        /// </summary>
        /// <param name="img">The image.</param>
        /// <param name="view">The view.</param>
        /// <param name="offset">The offset for translation.</param>
        /// <param name="invert">if set to <c>true</c>  the result will be inverted.</param>
        /// <param name="zoom">The zoom factor.</param>
        /// <param name="threshold">The threshold.</param>
        /// <param name="callHooks">if set to <c>true</c> per- and post renderer hooks  are called.</param>
        /// <returns>
        /// a bool matrix
        /// </returns>
        public bool[,] RenderImage(Bitmap img, IViewBoxModel view, IPannable offset, bool invert, double zoom, float threshold, bool callHooks = true)
        {
            Threshold = threshold;
            return RenderImage(img, view, offset, invert, zoom, callHooks);
        }
        /// <summary>
        /// Renders the image.
        /// </summary>
        /// <param name="img">The image.</param>
        /// <param name="view">The view.</param>
        /// <param name="invert">if set to <c>true</c>  the result will be inverted.</param>
        /// <param name="zoom">The zoom factor.</param>
        /// <param name="autoThreshold">if set to <c>true</c> [automatic threshold] is applied.</param>
        /// <param name="callHooks">if set to <c>true</c> per- and post renderer hooks  are called.</param>
        /// <returns>
        /// a bool matrix
        /// </returns>
        public bool[,] RenderImage(Bitmap img, IViewBoxModel view, bool invert, double zoom, bool autoThreshold, bool callHooks = true) { return RenderImage(img, view, null, invert, zoom, autoThreshold, callHooks); }
        /// <summary>
        /// Renders the image.
        /// </summary>
        /// <param name="img">The image.</param>
        /// <param name="view">The view.</param>
        /// <param name="offset">The offset for translation.</param>
        /// <param name="invert">if set to <c>true</c>  the result will be inverted.</param>
        /// <param name="zoom">The zoom factor.</param>
        /// <param name="autoThreshold">if set to <c>true</c> [automatic threshold] is applied.</param>
        /// <param name="callHooks">if set to <c>true</c> per- and post renderer hooks  are called.</param>
        /// <returns>
        /// a bool matrix
        /// </returns>
        public bool[,] RenderImage(Bitmap img, IViewBoxModel view, IPannable offset, bool invert, double zoom, bool autoThreshold, bool callHooks = true)
        {
            var vr = view.ContentBox;
            Bitmap img2 = img.Clone() as Bitmap;
            if (img2 != null)
                return RenderImage(img2, view, offset, invert, zoom,
                   GraphicUtils.GetAverageGrayscale(vr.Width, vr.Height, new Bitmap(img2, new Size((int)Math.Round(img2.Width * zoom), (int)Math.Round(img2.Height * zoom))))
                   , callHooks
                    );
            return null;
        }
        /// <summary>
        /// Renders the image.
        /// </summary>
        /// <param name="img">The image.</param>
        /// <param name="view">The view.</param>
        /// <param name="invert">if set to <c>true</c>  the result will be inverted.</param>
        /// <param name="zoom">The zoom factor.</param>
        /// <returns>
        /// a bool matrix
        /// </returns>
        public bool[,] RenderImage(Bitmap img, IViewBoxModel view, bool invert, double zoom) { return RenderImage(img, view, null, invert, zoom); }

        /// <summary>
        /// Renders the image.
        /// </summary>
        /// <param name="img">The image.</param>
        /// <param name="view">The view.</param>
        /// <param name="offset">The offset for translation.</param>
        /// <param name="invert">if set to <c>true</c>  the result will be inverted.</param>
        /// <param name="zoom">The zoom factor.</param>
        /// <param name="callHooks">if set to <c>true</c> per- and post renderer hooks  are called.</param>
        /// <returns>
        /// a bool matrix
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// The zoom level is with a value of  + zoom + to high. The zoom level should not be more than 3.;zoom
        /// or
        /// The zoom level is with a value of  + zoom + to low. The zoom level should be between 0 and 3.;zoom
        /// </exception>
        public bool[,] RenderImage(Bitmap img, IViewBoxModel view, IPannable offset, bool invert, double zoom, bool callHooks = true)
        {
            try
            {

                //call pre hooks
                object cImg = img as object;
                if (callHooks) callAllPreHooks(ref view, ref cImg, offset, invert, zoom);
                img = cImg as Bitmap;

                //if (zoom > 3) throw new ArgumentException("The zoom level is with a value of " + zoom + "to high. The zoom level should not be more than 3.", "zoom");
                if (zoom < 0) throw new ArgumentException("The zoom level is with a value of " + zoom + "to low. The zoom level should be between 0 and 3.", "zoom");

                if (view == null) return new bool[0, 0];
                //TODO: bring in threshold here
                //TODO: check how to get the threshold 
                Rectangle viewRange = view.ContentBox;
                int matrixWidth = viewRange.Width;
                int matrixHeight = viewRange.Height;

                bool[,] m = new bool[matrixHeight, matrixWidth];

                int offsetX = 0;
                int offsetY = 0;
                if (offset != null && zoom > 0)
                {
                    offsetX = offset.GetXOffset();
                    offsetY = offset.GetYOffset();

                    offsetX = (Int32)Math.Round(offsetX / zoom);
                    offsetY = (Int32)Math.Round(offsetY / zoom);
                }
                if (img != null && img.PixelFormat != PixelFormat.Undefined)
                {
                    try
                    {
                        Bitmap _img = null;
                        int trys = 0;
                        while (_img == null && trys++ < 10)
                        {
                            try {
                                _img = img.Clone() as Bitmap;
                            }
                            catch { System.Threading.Thread.Sleep(2); }
                        }

                        if(_img != null)
                        //using (Bitmap _img = img.Clone() as Bitmap)
                        {
                            Int32 contentWidth = (Int32)Math.Max(Math.Round(_img.Width * zoom), 1);
                            Int32 contentHeight = (Int32)Math.Max(Math.Round(_img.Height * zoom), 1);
                            //set the content size fields in the view.
                            view.ContentHeight = contentHeight;
                            view.ContentWidth = contentWidth;

                            double vrZoom = zoom > 0 ? (double)1 / zoom : 0;

                            Int32 zoomedVrWidth = (Int32)Math.Max(Math.Round(matrixWidth * vrZoom), 1);
                            Int32 zoomedVrHeight = (Int32)Math.Max(Math.Round(matrixHeight * vrZoom), 1);

                            int imgWidth = Math.Min(zoomedVrWidth, _img.Width);
                            int imgHeiht = Math.Min(zoomedVrHeight, _img.Height);

                            using (Bitmap viewRangeImage = new Bitmap(matrixWidth, matrixHeight))
                            {
                                try
                                {
                                    using (Graphics grMatrix = Graphics.FromImage(viewRangeImage))
                                    {
                                        // good results with a Threshold of 210 but smooths the edges
                                        grMatrix.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
                                        grMatrix.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighSpeed;

                                        grMatrix.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                                        grMatrix.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;

                                        Rectangle sourceRectangle = new Rectangle(offsetX * -1, offsetY * -1, zoomedVrWidth, zoomedVrHeight);
                                        Rectangle destRectangle1 = new Rectangle(0, 0, matrixWidth, matrixHeight);

                                        grMatrix.FillRectangle(Brushes.White, destRectangle1);
                                        grMatrix.DrawImage(_img, destRectangle1, sourceRectangle, GraphicsUnit.Pixel);
                                    }
                                }
                                catch { }

                                if (viewRangeImage != null)
                                {
                                    using (LockBitmap lockBitmap = new LockBitmap(viewRangeImage))
                                    {
                                        lockBitmap.LockBits();

                                        int rw = lockBitmap.Width;
                                        int rh = lockBitmap.Height;

                                        //for (int y = 0; y < matrixHeight; y++)
                                        System.Threading.Tasks.Parallel.For(0, matrixHeight, y =>
                                        {
                                            //System.Threading.Tasks.Parallel.For(0, matrixWidth, x =>
                                            for (int x = 0; x < matrixWidth; x++)
                                            {
                                                //int cX = x;
                                                if (x >= 0)
                                                {
                                                    //int cY = y;
                                                    if (y >= 0)
                                                    {
                                                        if (x < rw && y < rh)
                                                        {
                                                            Color c = lockBitmap.GetPixel(x, y);
                                                            var l = GraphicUtils.GetLightness(c);
                                                            m[y, x] = (l >= Threshold) ?
                                                                (invert ? true : false)
                                                                :
                                                                (invert ? false : true);
                                                        }
                                                    }
                                                }
                                            }//);
                                        });
                                    }
                                }
                            }

                            _img.Dispose();
                        }
                        else
                        {

                        }
                    }
                    catch (ArgumentException) { }
                    catch (InvalidOperationException) { }
                }
                //call post hooks
                if (callHooks) callAllPostHooks(view, img, ref m, offset, invert, zoom);

                return m;
            }
            finally
            {
                if (img != null) img.Dispose();
            }
        }

        #region IBrailleIOContentRenderer

        /// <summary>
        /// Renders a content object into an boolean matrix;
        /// while <c>true</c> values indicating raised pins and <c>false</c> values indicating lowered pins
        /// ATTENTION: have to be implemented. check for the
        /// </summary>
        /// <param name="view">The frame to render in. This gives access to the space to render and other parameters. Normally this is a <see cref="BrailleIOViewRange" />.</param>
        /// <param name="content">The content to render.</param>
        /// <param name="callHooks">if set to <c>true</c> [call the pre- and post-rendering hooks].</param>
        /// <returns>
        /// A two dimensional boolean M x N matrix (bool[M,N]) where M is the count of rows (this is height)
        /// and N is the count of columns (which is the width).
        /// Positions in the Matrix are of type [i,j]
        /// while i is the index of the row (is the y position)
        /// and j is the index of the column (is the x position).
        /// In the matrix <c>true</c> values indicating raised pins and <c>false</c> values indicating lowered pins
        /// </returns>
        public override bool[,] RenderMatrix(IViewBoxModel view, object content, bool callHooks = true)
        {
            return RenderImage(content as Bitmap, view, Invert, view is IZoomable ? ((IZoomable)view).GetZoom() : 1);
        }

        #endregion

        #region Override AbstractCachingRendererBase

        /// <summary>
        /// Informs the renderer that the content the or view has changed.
        /// You have to call the PrerenderMatrix function manually if you want to have a cached result.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="content">The content.</param>
        override public void ContentOrViewHasChanged(IViewBoxModel view, object content)
        {
            base.ContentOrViewHasChanged(view, content);
            PrerenderMatrix(view, content);
        }

        #endregion
    }

    /// <summary>
    /// Class for useful Graphic utilities that are used by the BrailleIo framework.
    /// </summary>
    public static class GraphicUtils
    {
        /// <summary>
        /// Returns if color A the is lighter than color B.
        /// </summary>
        /// <param name="a">the first color.</param>
        /// <param name="b">the second color.</param>
        /// <returns></returns>
        public static bool ColorIsLighterThan(Color a, Color b)
        {
            var ba = GetLightness(a);
            var bb = GetLightness(b);
            return ba - bb > 0;
        }

        /// <summary>
        /// Gets the lightness of a color. Keeping respect to the alpha value of a background of white.
        /// R * 0.3 + B * 0.11 + G * 0.59
        /// </summary>
        /// <param name="c">The color.</param>
        /// <returns>a float value between 0 and 255</returns>
        public static float GetLightness(Color c)
        {
            float aw, ar, ab, ag;
            float correction = (float)c.A / 255F;
            aw = (255F * (1F - correction));

            ar = (((float)c.R * correction) + aw) * 0.3F;
            ab = (((float)c.B * correction) + aw) * 0.11F;
            ag = (((float)c.G * correction) + aw) * 0.59F;
            float b = (ar + ab + ag);

            return b;
        }

        /// <summary>
        /// Gets the average gray scale of an Image.
        /// </summary>
        /// <param name="m_w">The M_W.</param>
        /// <param name="m_h">The M_H.</param>
        /// <param name="rescaled">The rescaled.</param>
        /// <returns></returns>
        public static float GetAverageGrayscale(int m_w, int m_h, Bitmap rescaled)
        {
            // get average
            double r = 0;
            double gr = 0;
            double b = 0;

            if (rescaled != null)
            {
                using (LockBitmap lockBitmap = new LockBitmap(rescaled))
                {
                    lockBitmap.LockBits();

                    int rw = lockBitmap.Width;
                    int rh = lockBitmap.Height;

                    for (int x = 0; x < m_w; x++)
                        for (int y = 0; y < m_h; y++)
                        {
                            if (x < rw && y < rh)
                            {
                                Color c = lockBitmap.GetPixel(x, y);
                                r += c.R;
                                gr += c.G;
                                b += c.B;
                            }
                        }
                }
            }

            r /= m_w * m_h;
            gr /= m_w * m_h;
            b /= m_w * m_h;

            return (float)(r + gr + b) / 3;
        }

        private static Object gLock = new Object();
        private static Pen _p = new Pen(Brushes.LightGray, 0.4F);
        private static Object pLock = new Object();
        private static Pen Stroke
        {
            get
            {
                lock (gLock)
                {
                    return _p;
                }
            }
        }
        private static Object graphicsLock = new Object();
        const int pixel = 5;

        //paints display!
        /// <summary>
        /// Paints the bool matrix into an BMP image.
        /// </summary>
        /// <param name="m">The matrix.</param>
        /// <param name="filePath">The file path.</param>
        public static void PaintBoolMatrixToImage(bool[,] m, string filePath)
        {
            if (m == null || m.GetLength(0) < 1 || m.GetLength(1) < 1) return;

            Bitmap bmp = new Bitmap(m.GetLength(1) * (pixel + 1), m.GetLength(0) * (pixel + 1));
            lock (graphicsLock)
            {
                using (Graphics PinGraphic = Graphics.FromImage(bmp))
                {
                    PinGraphic.FillRectangle(Brushes.White, 0, 0, bmp.Width, bmp.Height);

                    for (int i = 0; i < m.GetLength(0); i++)
                        for (int j = 0; j < m.GetLength(1); j++)
                        {
                            lock (graphicsLock)
                            {
                                if (m[i, j])
                                {
                                    PinGraphic.FillRectangle(Brushes.Black, j * (pixel + 1), i * (pixel + 1), pixel, pixel);
                                }
                                else
                                {
                                    PinGraphic.DrawEllipse(Stroke, j * (pixel + 1), i * (pixel + 1), pixel - 1, pixel - 1);
                                }
                            }
                        }
                    try
                    {
                        PinGraphic.Flush();
                        if (string.IsNullOrEmpty(filePath) || string.IsNullOrWhiteSpace(filePath)) return;
                        bmp.Save(filePath, ImageFormat.Bmp);
                    }
                    catch { }
                }
            }
        }
    }
}