using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrailleIO.Interface;
using System.Drawing;

namespace BrailleIO.Renderer
{
    public static class BrailleIOBorderRenderer
    {
        public static bool[,] renderMatrix(AbstractViewBorderBase view, bool[,] contentMatrix)
        {
            if (view != null && view.HasBorder && contentMatrix != null)
            {
                int x = 0, y = 0;
                int w = contentMatrix.GetLength(1);
                int h = contentMatrix.GetLength(0);
                if (view.HasMargin)
                {
                    x += (int)view.Margin.Left;
                    y += (int)view.Margin.Top;
                    w -= ((int)view.Margin.Left + (int)view.Margin.Right);
                    h -= ((int)view.Margin.Top + (int)view.Margin.Bottom);
                }

                //horizontal lines
                for (int i = 0; i < w; i++)
                {
                    if (contentMatrix.GetLength(1) <= i + x) break;

                    //top line
                    drawHorizontalLine(view.Border.Top, ref contentMatrix, i + x, y);
   
                    //bottom line
                    drawHorizontalLine(view.Border.Bottom, ref contentMatrix, i + x, Convert.ToInt32((y + h - view.Border.Bottom)));
                }

                //vertical lines
                for (int i = 0; i < h - (int)view.Border.Bottom; i++)
                {
                    if (contentMatrix.GetLength(1) <= i + y) break;
                    //left line
                    for (int j = 0; j < view.Border.Left; j++)
                    {
                        if (contentMatrix.GetLength(0) <= (i + y + (int)view.Border.Top)) break;
                        contentMatrix[i + y + (int)view.Border.Top, j + x] = true;
                    }

                    //right line
                    for (int j = 0; j < view.Border.Right; j++)
                    {
                        if (contentMatrix.GetLength(0) <= (i + y + (int)view.Border.Top)) break;
                        contentMatrix[i + y + (int)view.Border.Top, x + (w - 1) - j] = true;
                    }
                }
            }
            return contentMatrix;
        }
        

        private static void drawHorizontalLine(uint width, ref bool[,]contentMatrix, int xOffset = 0, int yOffset = 0)
        {
            for (int j = 0; j < width; j++)
            {
                if (contentMatrix.GetLength(0) <= (j + yOffset)) break;
                contentMatrix[j + yOffset, xOffset] = true;
            }
        }


    }
}
