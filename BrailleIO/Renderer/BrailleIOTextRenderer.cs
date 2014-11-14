using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using BrailleIO.Interface;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Text;
using BrailleIO.Properties;
using System.Runtime.InteropServices;

namespace BrailleIO.Renderer
{
    //TODO: make this real working
    //TODO: error with long texts -- hangs on
    //TODO: does not fill the whole line. The last char don't fit in
    public class BrailleIOTextRenderer : BrailleIOHookableRendererBase, IBrailleIOContentRenderer
    {
        float fontSize = 22.5f;
        const String fName = "TUD Euro-8-Braille equidistant";
        System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
        System.Drawing.Font __drawFont;
        System.Drawing.Font drawFont
        {
            get
            {
                if (__drawFont == null)
                {
                    if (IsFontInstalled(fName))
                    {
                        __drawFont = new System.Drawing.Font(fName, fontSize, FontStyle.Regular);
                    }
                    else
                    {
                        __drawFont = loadBrailleFont();
                    }
                }
                else if (!__drawFont.IsSystemFont)
                {
                    __drawFont = loadBrailleFont();
                }

                return __drawFont.Clone() as Font;
            }
        }

        /// <summary>
        /// increasing and decreasing factor for the Braille-font rendering
        /// </summary>
        const int fac = 6;
        /// <summary>
        /// x-offset for Braille-font rendering
        /// was -7.0f
        /// </summary>
        float fontRenderingX = -1.5f;
        /// <summary>
        /// y-offset for Braille-font rendering
        /// was 0.01f
        /// </summary>
        float fontRenderingY = -3.5f;

        readonly BrailleIOImageToMatrixRenderer ir = new BrailleIOImageToMatrixRenderer();
        const float threshold = 250;


        public BrailleIOTextRenderer() : base()
        {
            ir.SetThreshold(threshold);
        }

        #region String rendering

        private Bitmap drawWholeString(String drawString, int width, int height)
        {
            Bitmap bmp = null;
            int c = 0;
            // measure how big the bitmap should be
            SizeF size = measureString(drawString, (width) * fac, height * fac);
            size = roundSize(size);

            // making a temp image with needed size ...
            using (Bitmap tmp = new Bitmap((int)size.Width, (int)size.Height))
            {
                // ...to get a Graphics object
                using (Graphics g = Graphics.FromImage(tmp))
                {
                    g.Clear(Color.White);
                    // prepare font rendering
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

                    drawStringWithLocalFont(g, drawString, (int)size.Width, (int)size.Height);

                    g.Flush();
                }

                //tmp.Save(@"upscalled" + drawString.GetHashCode() + ".png", System.Drawing.Imaging.ImageFormat.Png);

                // downsize it to the real dimensions
                SizeF realSize = decreseSize(size);

                bmp = new Bitmap((int)realSize.Width, (int)realSize.Height);

                if (tmp != null && bmp != null)
                {
                    using (Graphics g2 = Graphics.FromImage(bmp))
                    {
                        g2.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                        g2.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                        g2.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighSpeed;
                        g2.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;

                        g2.DrawImage(tmp, new Rectangle(0, 0, (int)realSize.Width, (int)realSize.Height), new Rectangle(0, 0, (int)size.Width, tmp.Height), GraphicsUnit.Pixel);
                        g2.Flush();
                    }
                }

                //bmp.Save(@"output_" + drawString.GetHashCode() + "_" + fontSize + ".png", System.Drawing.Imaging.ImageFormat.Png);

                //Pen a = new Pen(Color.FromArgb(100, Color.Red));
                //Pen b = new Pen(Color.FromArgb(100, Color.Blue));
                ////FIXME: paint in the raster
                //using (Graphics g = Graphics.FromImage(tmp))
                //{
                //    for (int x = 0; x < tmp.Width; x += 6)
                //    {
                //        c++;
                //        g.DrawLine(a, new Point(x, 0), new Point(x, tmp.Height));
                //    }
                //    for (int y = 0; y < tmp.Height; y += 6)
                //    {
                //        g.DrawLine(b, new Point(0, y), new Point(tmp.Width, y));
                //    }
                //    g.Flush();
                //}
                //tmp.Save(@".upscalled_MARKS_" + drawString.GetHashCode() + ".png", System.Drawing.Imaging.ImageFormat.Png);
            
            }
            return bmp;
        }

        /// <summary>
        /// Draws the string in the given Graphics object and measure its recommended size.
        /// </summary>
        /// <param name="g">The Graphics object to draw the String in.</param>
        /// <param name="drawFont">The draw font to use.</param>
        /// <param name="drawString">The string to draw.</param>
        /// <param name="width">The available width.</param>
        /// <param name="height">The recommanded height.</param>
        /// <param name="x">The start x-position.</param>
        /// <param name="y">The start y-position.</param>
        /// <param name="contentSize">Size of the content.</param>
        private void drawStringAndMeasure(Graphics g, Font drawFont, String drawString, int width, int height, float x, float y, out SizeF contentSize)
        {
            contentSize = measureString(g, drawFont, drawString, width);
            g.DrawString(drawString, drawFont, drawBrush,
                       new RectangleF(x, y, width, height),
                       StringFormat.GenericTypographic
                       );
        }

        /// <summary>
        /// Measures the recommended space for the string.
        /// </summary>
        /// <param name="g">The Graphics object to draw the String in.</param>
        /// <param name="drawFont">The draw font to use.</param>
        /// <param name="drawString">The string to draw.</param>
        /// <param name="width">The available width.</param>
        /// <param name="height">The recommanded height.</param>
        /// <returns></returns>
        private SizeF measureString(Graphics g, Font drawFont, String drawString, int width)
        {
            SizeF contentSize = new SizeF();
            contentSize = g.MeasureString(drawString, drawFont, width);
            return contentSize;
        }

        /// <summary>
        /// Measures the string.
        /// </summary>
        /// <param name="drawString">The draw string.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>BEWARE: the size is increased by the factor that is set in the renderer method as constant variable (fac)</returns>
        private SizeF measureString(String drawString, int width, int height)
        {
            SizeF contentSize = new SizeF();
            using (Bitmap tmp = new Bitmap(width, height))
            {
                using (Graphics g = Graphics.FromImage(tmp))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

                    contentSize = measureStringWithLocalFont(g, drawString, width, height);

                    g.Flush();
                }
            }
            return contentSize;
        }


        /// <summary>
        /// Draws the string with the local font member-variable.
        /// Regarding it is a system font or a temporary font the usage differs.
        /// </summary>
        /// <param name="g">The Graphics object to draw the String in.</param>
        /// <param name="drawString">The string to draw.</param>
        /// <param name="width">The available width.</param>
        /// <param name="height">The recommanded height.</param>
        private void drawStringWithLocalFont(Graphics g, String drawString, int width, int height)
        {
            if (drawFont != null)
            {
                if (drawFont.IsSystemFont)
                {
                    g.DrawString(drawString, drawFont, drawBrush,
                        new RectangleF(fontRenderingX, fontRenderingY, width, height),
                        StringFormat.GenericTypographic
                        );
                }
                else
                {
                    using (drawFont)
                    {
                        g.DrawString(drawString, drawFont, drawBrush,
                        new RectangleF(fontRenderingX, fontRenderingY, width, height),
                        StringFormat.GenericTypographic
                        );
                    }
                }
            }
        }

        /// <summary>
        /// Draws the string with the local font member-variable.
        /// Regarding it is a system font or a temporary font the usage differs.
        /// </summary>
        /// <param name="g">The Graphics object to draw the String in.</param>
        /// <param name="drawString">The string to draw.</param>
        /// <param name="width">The available width.</param>
        /// <param name="height">The recommanded height.</param>
        /// <param name="contentSize">Size of the content.</param>
        private void drawStringWithLocalFontAndMeasureIt(Graphics g, String drawString, int width, int height, out SizeF contentSize)
        {
            contentSize = new SizeF();
            if (drawFont != null)
            {
                if (drawFont.IsSystemFont)
                {
                    drawStringAndMeasure(g, drawFont, drawString, width, height, fontRenderingX, fontRenderingY, out contentSize);
                }
                else
                {
                    using (drawFont)
                    {
                        drawStringAndMeasure(g, drawFont, drawString, width, height, fontRenderingX, fontRenderingY, out contentSize);
                    }
                }
            }
        }

        /// <summary>
        /// Draws the string with the local font member-variable.
        /// Regarding it is a system font or a temporary font the usage differs.
        /// </summary>
        /// <param name="g">The Graphics object to draw the String in.</param>
        /// <param name="drawString">The string to draw.</param>
        /// <param name="width">The available width.</param>
        /// <param name="height">The recommanded height.</param>
        /// <param name="contentSize">Size of the content.</param>
        private SizeF measureStringWithLocalFont(Graphics g, String drawString, int width, int height)
        {
            SizeF contentSize = new SizeF();
            if (drawFont != null)
            {
                if (drawFont.IsSystemFont)
                {
                    return measureString(g, drawFont, drawString, width);
                }
                else
                {
                    using (drawFont)
                    {
                        contentSize = measureString(g, drawFont, drawString, width);
                    }
                }
            }
            return contentSize;
        }

        #endregion

        #region IBrailleIOContentRenderer
        
        public bool[,] RenderMatrix(IViewBoxModel view, String text)
        {
            
            //call pre hooks
            object cT = text as object;
            callAllPreHooks(ref view, ref cT);
            text = cT as String;

            var vr = view.ContentBox;

            Bitmap bImage = drawWholeString(text, vr.Width, vr.Height);
            // generate content matrix
            var m = ir.RenderImage(bImage, new FakeView(bImage.Width, bImage.Height), 1, 250);

            view.ContentWidth = bImage.Width;
            view.ContentHeight = bImage.Height;     
 
            //call post hooks
            callAllPostHooks(view, text, ref m);

            return m;
        }

        public bool[,] RenderMatrix(IViewBoxModel view, object content)
        {
            return RenderMatrix(view, content.ToString());
        }

        #endregion

        /// <summary>
        /// Determines whether [is font installed (system font)] [the specified font name].
        /// </summary>
        /// <param name="fontName">Name of the font.</param>
        /// <returns>
        /// 	<c>true</c> if [is font installed]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsFontInstalled(string fontName)
        {
            using (var testFont = new Font(fontName, 8))
            {
                return 0 == string.Compare(
                  fontName,
                  testFont.Name,
                  StringComparison.InvariantCultureIgnoreCase);
            }
        }

        /// <summary>
        /// Loads the braille font from the properties.
        /// </summary>
        /// <returns></returns>
        private Font loadBrailleFont()
        {
            try
            {
                PrivateFontCollection _fonts = new PrivateFontCollection();
                IntPtr fontBuffer;
                if (_fonts != null)
                {
                    _fonts = new PrivateFontCollection();

                    // Load font from Properties
                    byte[] font = Properties.Resources.TUD_EuroBraille_equidistant_10;
                    fontBuffer = Marshal.AllocCoTaskMem(font.Length);
                    Marshal.Copy(font, 0, fontBuffer, font.Length);
                    _fonts.AddMemoryFont(fontBuffer, font.Length);
                    Marshal.FreeCoTaskMem(fontBuffer);
                    
                    //load from file
                    //_fonts.AddFontFile(@".\TUDEuro-8-Brailleequidistant.ttf");

                    Font customFont = new Font(_fonts.Families[0], fontSize, FontStyle.Regular);
                    return customFont;
                }
            }
            catch{}
            return null;
        }

        #region Tools

        SizeF roundSize(SizeF size)
        {
            SizeF s = size != null ? size : new SizeF();

            try
            {
                s.Height = (float)Math.Round(s.Height, 0, MidpointRounding.AwayFromZero);
                s.Width = (float)Math.Round(s.Width, 0, MidpointRounding.AwayFromZero);
            }
            catch { }

            return s;
        }


        SizeF decreseSize(SizeF size)
        {
            size.Width /= fac;
            size.Height /= fac;
            return roundSize(size);
        }


        #endregion


    }


    class FakeView : IViewBoxModel
    {
        public FakeView(int width, int height)
        {
            this.ContentBox = new Rectangle(0, 0, width, height);
            this.ContentHeight = height;
            this.ContentWidth = width;
            this.ViewBox = this.ContentBox;
        }

        public Rectangle ViewBox { get; set; }
        public Rectangle ContentBox { get; set; }
        public int ContentWidth { get; set; }
        public int ContentHeight { get; set; }
    }

}
