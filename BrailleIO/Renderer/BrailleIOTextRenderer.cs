using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using BrailleIO.Interface;
using System.Drawing;
using System.Windows.Forms;

namespace BrailleIO.Renderer
{
    //TODO: make this real working
    public class BrailleIOTextRenderer : BrailleIOScrollbarRenderer
    {
        System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
        System.Drawing.Font drawFont = new System.Drawing.Font("TUD Euro-8-Braille equidistant", 22.25977f, FontStyle.Bold);
        
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
                    g.DrawString(drawString, drawFont, drawBrush, x, y);

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
                    //bmp.Save("C:\\Users\\Admin\\Desktop\\test_"+drawString.Substring(0,3)+".bmp"); //FIXME: only for fixing
                }
            }
            return bmp;
        }

        public bool[,] renderMatrix(IViewBoxModel view, String text)
        {
            var ir = new BrailleIOImageToMatrixRenderer();
            var vr = view.ContentBox;
            var m = ir.renderImage(DrawString(text, vr.Width, vr.Height), view, 1, 100f);
            return m;
        }
    }
}
