using System;
using BrailleIO.Interface;

namespace BrailleIO.Renderer
{
    /// <summary>
    /// render the border defined for an <see cref="AbstractViewBorderBase"/> view range
    /// </summary>
    public static class BrailleIOBorderRenderer
    {
        /// <summary>
        /// Renders a content object into an boolean matrix;
        /// while <c>true</c> values indicating raised pins and <c>false</c> values indicating lowerd pins
        /// </summary>
        /// <param name="view">The frame to render in. This gives acces to the space to render and other paramenters. Normaly this is a <see cref="BrailleIOViewRange"/>.</param>
        /// <param name="matrix">The content to render.</param>
        /// <returns>
        /// A two dimensional boolean M x N matrix (bool[M,N]) where M is the count of rows (this is height)
        /// and N is the count of columns (which is the width). 
        /// Positions in the Matrix are of type [i,j] 
        /// while i is the index of the row (is the y position) 
        /// and j is the index of the column (is the x position). 
        /// In the matrix <c>true</c> values indicating raised pins and <c>false</c> values indicating lowerd pins</returns>
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
                    if (contentMatrix.GetLength(0) <= i + y)
                        break;
                    //left line
                    for (int j = 0; j < view.Border.Left; j++)
                    {
                        if (contentMatrix.GetLength(0) <= (i + y + (int)view.Border.Top)) 
                            break;
                        contentMatrix[i + y + (int)view.Border.Top, j + x] = true;
                    }

                    //right line
                    for (int j = 0; j < view.Border.Right; j++)
                    {
                        if (contentMatrix.GetLength(0) <= (i + y + (int)view.Border.Top)) 
                            break;
                        contentMatrix[i + y + (int)view.Border.Top, x + (w - 1) - j] = true;
                    }
                }
            }
            return contentMatrix;
        }


        /// <summary>
        /// Draws the horizontal line.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="contentMatrix">The content matrix.</param>
        /// <param name="xOffset">The x offset.</param>
        /// <param name="yOffset">The y offset.</param>
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
