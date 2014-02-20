using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using BrailleIO.Interface;
using System.Drawing.Imaging;

namespace BrailleIO.Renderer
{
    public class BrailleIOImageToMatrixRenderer
    {
        /// <summary>
        /// If lightness of a color is lower than this threshold, the pin will be lowered. 
        /// A higher threshold leads lighter points to raise pins. 
        /// A low threshold leads darker pins to stay lowered.
        /// Have to be between 0 and 255.
        /// </summary>
        private float Threshold = 130;
        /// <summary>
        /// Resets the threshold.
        /// </summary>
        /// <returns>the new threshold</returns>
        public float ResetThreshold() { return this.Threshold = 130; }

        public bool[,] renderImage(Bitmap img, IViewBoxModel view, double zoom, float threshold) { return renderImage(img, view, null, zoom, threshold); }
        public bool[,] renderImage(Bitmap img, IViewBoxModel view, IPannable offset, double zoom, float threshold)
        {
            Threshold = threshold;
            return renderImage(img, view, offset, zoom);
        }
        public bool[,] renderImage(Bitmap img, IViewBoxModel view, double zoom, bool autoThreshold) { return renderImage(img, view, null, zoom, autoThreshold); }
        public bool[,] renderImage(Bitmap img, IViewBoxModel view, IPannable offset, double zoom, bool autoThreshold)
        {
            var vr = view.ContentBox;
            return renderImage(img, view, offset, zoom, GraphicUtils.getAverageGrayscale(vr.Width, vr.Height, new Bitmap(img, new Size((int)Math.Round(img.Width * zoom), (int)Math.Round(img.Height * zoom)))));
        }
        public bool[,] renderImage(Bitmap img, IViewBoxModel view, double zoom) { return renderImage(img, view, null, zoom); }
        public bool[,] renderImage(Bitmap img, IViewBoxModel view, IPannable offset, double zoom) { return renderImage(img, view, offset, false, zoom); }

        //possible invert
        public bool[,] renderImage(Bitmap img, IViewBoxModel view, bool invert, double zoom, float threshold) { return renderImage(img, view, null, invert, zoom, threshold); }
        public bool[,] renderImage(Bitmap img, IViewBoxModel view, IPannable offset, bool invert, double zoom, float threshold)
        {
            Threshold = threshold;
            return renderImage(img, view, offset, invert, zoom);
        }
        public bool[,] renderImage(Bitmap img, IViewBoxModel view, bool invert, double zoom, bool autoThreshold) { return renderImage(img, view, null, invert, zoom, autoThreshold); }
        public bool[,] renderImage(Bitmap img, IViewBoxModel view, IPannable offset, bool invert, double zoom, bool autoThreshold)
        {
            var vr = view.ContentBox;
            return renderImage(img, view, offset, invert, zoom, GraphicUtils.getAverageGrayscale(vr.Width, vr.Height, new Bitmap(img, new Size((int)Math.Round(img.Width * zoom), (int)Math.Round(img.Height * zoom)))));
        }
        public bool[,] renderImage(Bitmap img, IViewBoxModel view, bool invert, double zoom) { return renderImage(img, view, null, invert, zoom); }
        public bool[,] renderImage(Bitmap img, IViewBoxModel view, IPannable offset, bool invert, double zoom)
        {
            //TODO: bring in threshold here
            //TODO: check how to get the threshold 
            var vr = view.ContentBox;
            bool[,] m = new bool[vr.Height, vr.Width];
            int m_w = m.GetLength(1);
            int m_h = m.GetLength(0);

            int oX = 0;
            int oY = 0;
            if (offset != null && zoom > 0)
            {
                oX = offset.GetXOffset();
                oY = offset.GetYOffset();
            }

            using (Bitmap _img = img.Clone() as Bitmap)
            {
                using (Bitmap rescaled = new Bitmap((Int32)Math.Max(Math.Round(_img.Width * zoom), 1),
                                (Int32)Math.Max(Math.Round(_img.Height * zoom), 1)))
                {
                    try
                    {
                        using (Graphics g2 = Graphics.FromImage(rescaled))
                        {
                            g2.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
                            g2.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                            g2.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.None;
                            g2.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;

                            g2.DrawImage(_img, new Rectangle(0, 0, rescaled.Width, rescaled.Height), new Rectangle(0, 0, _img.Width, _img.Height), GraphicsUnit.Pixel);
                            g2.Flush();
                            try
                            {
                                rescaled.Save("C:\\Users\\Admin\\Desktop\\tmp\\test_" + vr.GetHashCode() + ".bmp"); //FIXME: only for fixing
                            }
                            catch (System.Exception) { }
                        }


                    }
                    catch (ArgumentException) { }
                    catch (InvalidOperationException) { if (rescaled != null) rescaled.Dispose(); return renderImage(_img, view, offset, invert, zoom); }

                    if (rescaled != null)
                    {
                        for (int x = 0; x + oX < m_w; x++)
                        {
                            int cX = x + oX;
                            if (cX < 0) continue;
                            for (int y = 0; oY + y < m_h; y++)
                            {
                                int cY = oY + y;
                                if (cY < 0) continue;
                                if (x < rescaled.Width && y < rescaled.Height)
                                {
                                    Color c = rescaled.GetPixel(x, y);
                                    var l = GraphicUtils.getLightness(c);
                                    m[cY, cX] = (l > Threshold) ? invert ? true : false : invert ? false : true;
                                }
                            }
                        }
                    }

                }
            }
            return m;
        }

    }

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
            var ba = getLightness(a);
            var bb = getLightness(b);
            return ba - bb > 0;
        }

        /// <summary>
        /// Gets the lightness of a color. Keeping respect to the alpha value of a background of white.
        /// R * 0.3 + B * 0.11 + G * 0.59
        /// </summary>
        /// <param name="c">The color.</param>
        /// <returns>a float value between 0 and 255</returns>
        public static float getLightness(Color c)
        {
            float aw, ar, ab, ag;
            aw = (255F * (1F - ((float)c.A / 255F)));
            ar = ((c.R * (c.A / 255F)) + aw) * 0.3F;
            ab = ((c.B * (c.A / 255F)) + aw) * 0.11F;
            ag = ((c.G * (c.A / 255F)) + aw) * 0.59F;
            float b = ar + ab + ag;
            return b;
        }

        /// <summary>
        /// Gets the average gray scale of an Image.
        /// </summary>
        /// <param name="m_w">The M_W.</param>
        /// <param name="m_h">The M_H.</param>
        /// <param name="rescaled">The rescaled.</param>
        /// <returns></returns>
        public static float getAverageGrayscale(int m_w, int m_h, Bitmap rescaled)
        {
            // get average
            double r = 0;
            double gr = 0;
            double b = 0;

            for (int x = 0; x < m_w; x++)
                for (int y = 0; y < m_h; y++)
                {
                    if (x < rescaled.Width && y < rescaled.Height)
                    {
                        Color c = rescaled.GetPixel(x, y);
                        r += c.R;
                        gr += c.G;
                        b += c.B;
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
