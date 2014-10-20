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
    public class BrailleIOTextRenderer : BrailleIOHookableRendererBase, IBrailleIOContentRenderer, IBrailleIOHookableRenderer
    {
        const String fName = "TUD Euro-8-Braille equidistant";
        System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
        System.Drawing.Font __drawFont;
        System.Drawing.Font drawFont
        {
            get
            {
                if (__drawFont == null )
                {
                    if (IsFontInstalled(fName))
                    {
                        __drawFont = new System.Drawing.Font(fName, 22.25977f, FontStyle.Bold);
                    }
                    else
                    {
                        __drawFont = loadBrailleFont();
                    }
                }else if(!__drawFont.IsSystemFont){
                    __drawFont = loadBrailleFont();
                }

                return __drawFont;
            }
        }

        int fac = 6;
        private Bitmap DrawString(String drawString, int width, int height)
        {
            if (width <= 0 || height <= 0 || drawString == null || drawString.Equals(String.Empty))
                return null;
            Bitmap bmp = new Bitmap(width, height);
            using (Bitmap objBmpImage = new Bitmap(width * fac, height * fac))
            {
                using (Graphics g = Graphics.FromImage(objBmpImage))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

                    float x = -7.0f;
                    float y = 0.01f;

                    if (drawFont != null)
                    {
                        if (drawFont.IsSystemFont)
                        {
                            g.DrawString(drawString, drawFont, drawBrush, x, y);
                        }
                        else
                        {
                            using (drawFont)
                            {
                                g.DrawString(drawString, drawFont, drawBrush, x, y);
                            }
                        }
                    }
                    g.Flush();
                }

                using (Graphics g2 = Graphics.FromImage(bmp))
                {
                    g2.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    g2.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                    g2.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighSpeed;
                    g2.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;

                    g2.DrawImage(objBmpImage, new Rectangle(0, 0, width, height), new Rectangle(0, 0, objBmpImage.Width, objBmpImage.Height), GraphicsUnit.Pixel);
                    g2.Flush();
                }
            }
            return bmp;
        }




        #region IBrailleIOContentRenderer

        public bool[,] RenderMatrix(IViewBoxModel view, String text)
        {
            //call pre hooks
            object cT = text as object;
            callAllPreHooks(ref view, ref cT);
            text = cT as String;

            var ir = new BrailleIOImageToMatrixRenderer();
            var vr = view.ContentBox;
            var m = ir.RenderImage(DrawString(text, vr.Width, vr.Height), view, 1, 100f);

            //call post hooks
            callAllPostHooks(view, text, ref m);

            return m;
        }

        public bool[,] RenderMatrix(IViewBoxModel view, object content)
        {
            return RenderMatrix(view, content.ToString());
        }

        #endregion


        private static bool IsFontInstalled(string fontName)
        {
            using (var testFont = new Font(fontName, 8))
            {
                return 0 == string.Compare(
                  fontName,
                  testFont.Name,
                  StringComparison.InvariantCultureIgnoreCase);
            }
        }

        private static Font loadBrailleFont()
        {
            try
            {
                PrivateFontCollection _fonts = new PrivateFontCollection();
                IntPtr fontBuffer;
                if (_fonts != null)
                {
                    _fonts = new PrivateFontCollection();
                    byte[] font = Properties.Resources.TUD_EuroBraille_equidistant_10;
                    fontBuffer = Marshal.AllocCoTaskMem(font.Length);
                    Marshal.Copy(font, 0, fontBuffer, font.Length);
                    _fonts.AddMemoryFont(fontBuffer, font.Length);
                    Marshal.FreeCoTaskMem(fontBuffer);

                    Font customFont = new Font(_fonts.Families[0], 22.25977f, FontStyle.Bold);
                    return customFont;
                }
            }
            catch (System.Exception ex)
            {

            }
            return null;
        }



    }



}
