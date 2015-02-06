using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrailleIO.Interface;
using System.Collections.Concurrent;

namespace BrailleIO.Renderer
{
    /// <summary>
    /// renders scroll bars
    /// TODO: unfinished for vertical scroll bars (very small)
    /// </summary>
    public static class BrailleIOScrollbarRenderer
    {
        /// <summary>
        /// Draws scrollbars in the viewMatrix.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="viewMatrix">The view matrix.</param>
        /// <param name="xOffset">The x offset.</param>
        /// <param name="yOffset">The y offset.</param>
        /// <param name="paintArrows">if set to <c>true</c> [paint arrows].</param>
        /// <returns></returns>
        public static bool DrawScrollbars(IViewBoxModel view, ref bool[,] viewMatrix, int xOffset, int yOffset, bool paintArrows = false)
        {
            AbstractViewBoxModelBase vb = view as AbstractViewBoxModelBase;
            if (vb != null)
            {
                if (vb.GetXOffset() != 0)
                {
                    xOffset = vb.GetXOffset();
                }

                if (vb.GetYOffset() != 0)
                {
                    yOffset = vb.GetYOffset();
                }
            }

            bool vertical = false;
            if (view.ContentBox.Height < view.ContentHeight)
            {
                vertical = true;
                drawVerticalScrollBar(view, ref viewMatrix, yOffset, (view.ContentBox.Height < 8 ? true : paintArrows));
            }

            if (view.ContentBox.Width < view.ContentWidth)
            {
                drawHorizontalScrollBar(view, ref viewMatrix, xOffset, vertical, (view.ContentBox.Width < 8 ? true : paintArrows));
            }
            return true;
        }

        private static void drawVerticalScrollBar(IViewBoxModel view, ref bool[,] viewMatrix, int yOffset, bool paintArrows)
        {
            int x = view.ContentBox.X + view.ContentBox.Width + 1;
            int yO = view.ContentBox.Y;


            // if paint all arrows and height < 8 do nothing
            if (!(paintArrows && view.ContentBox.Height < 8))
            {
                for (int y = 0; y < view.ContentBox.Height; y++)
                {
                    if (viewMatrix.GetLength(0) > y + yO)
                    {
                        if (viewMatrix.GetLength(1) > x)
                        {
                            try
                            {
                                viewMatrix[y + yO, x] = true;
                            }
                            catch { }
                        }
                    }
                }
            }


            //paint slider
            double offsetRatio = Math.Abs((double)yOffset / (double)view.ContentHeight);
            double viewRatio = (double)view.ContentBox.Height / (double)view.ContentHeight;
            double posSliderRatio = offsetRatio + (viewRatio * offsetRatio);
            double restOffsetRatio = 1 - Math.Min((viewRatio + offsetRatio), 1);
            double negSliderRatio = restOffsetRatio + (viewRatio * restOffsetRatio);
            double sliderRatio = posSliderRatio < negSliderRatio ? posSliderRatio : 1 - negSliderRatio;
            double sliderOffset = Math.Abs(Math.Round((view.ContentBox.Height - 2) * Math.Min(sliderRatio, 1.0), MidpointRounding.AwayFromZero));

            int ybO = yO + (int)Math.Abs(sliderOffset) + 1;

            // if paint arrows and the size is smaller than 8 (3 + 3 + 2) - paint no slider           
            //TODO: paint slider different if arrows are active


            if (paintArrows && view.ContentBox.Height < 8)
            {
                //do nothing
            }
            else if (paintArrows && view.ContentBox.Height < 10)
            {
                if ((viewMatrix.GetLength(0) > (ybO + 1 - 1)) && (viewMatrix.GetLength(1) > (x + 1)))
                {
                    try
                    {
                        if (ybO > 0) viewMatrix[ybO, x + 1] = true;
                    }
                    catch { }
                }
            }
            else // normal handling
            {
                for (int y = 0; y < 3; y++)
                {
                    if ((viewMatrix.GetLength(0) > (ybO + y - 1)) && (viewMatrix.GetLength(1) > (x + 1)))
                    {
                        try
                        {
                            if ((ybO + y - 1) > 0) viewMatrix[ybO + y - 1, x + 1] = true;
                        }
                        catch { }
                    }
                }
            }



            if (paintArrows)
            {
                //top arrow
                if (viewMatrix.GetLength(0) > yO + 1)
                {
                    if (viewMatrix.GetLength(1) > (x + 1))
                    {
                        try
                        {
                            viewMatrix[yO, x] = true;
                            viewMatrix[yO + 1, x - 1] = true;
                            if (yOffset < 0) 
                                viewMatrix[yO + 1, x] = true;
                            viewMatrix[yO + 1, x + 1] = true;
                        }
                        catch { }
                    }
                }

                //bottom arrow
                if (viewMatrix.GetLength(0) > yO + view.ContentBox.Height - 1)
                {
                    if (viewMatrix.GetLength(1) > (x + 1))
                    {
                        try
                        {
                            viewMatrix[yO + view.ContentBox.Height - 1, x] = true;
                            viewMatrix[yO + view.ContentBox.Height - 2, x - 1] = true;
                            if (Math.Abs(yOffset - view.ContentBox.Height) < view.ContentHeight) 
                                viewMatrix[yO + view.ContentBox.Height - 2, x] = true;
                            viewMatrix[yO + view.ContentBox.Height - 2, x + 1] = true;
                        }
                        catch { }
                    }
                }
            }

        }

        //TODO: do the same scrolling and arrow stuff as in vertical
        private static void drawHorizontalScrollBar(IViewBoxModel view, ref bool[,] viewMatrix, int xOffset, bool vertical, bool paintArrows)
        {
            int y = view.ContentBox.Y + view.ContentBox.Height + 1;
            int xO = view.ContentBox.X;
            for (int x = 0; x < view.ContentBox.Width; x++)
            {
                if (viewMatrix.GetLength(0) > y)
                {
                    if (viewMatrix.GetLength(1) > (x + xO))
                    {
                        try
                        {
                            viewMatrix[y, x + xO] = true;
                        }
                        catch { }
                    }
                }
            }

            //paint slider
            double offsetRatio = Math.Abs((double)xOffset / (double)view.ContentWidth);
            double viewRatio = (double)view.ContentBox.Width / (double)view.ContentWidth;
            double posSliderRatio = offsetRatio + (viewRatio * offsetRatio);
            double restOffsetRatio = 1 - Math.Min((viewRatio + offsetRatio), 1);
            double negSliderRatio = restOffsetRatio + (viewRatio * restOffsetRatio);
            double sliderRatio = posSliderRatio < negSliderRatio ? posSliderRatio : 1 - negSliderRatio;
            double sliderOffset = Math.Abs(Math.Round((view.ContentBox.Width - 2) * Math.Min(sliderRatio, 1.0), MidpointRounding.AwayFromZero));

            int xbO = xO + (int)Math.Abs(sliderOffset) + 1;
            for (int x = 0; x < 3; x++)
            {
                if ((viewMatrix.GetLength(1) > (xbO + x - 1)) && (viewMatrix.GetLength(0) > (y + 1)))
                {
                    try
                    {
                        if ((xbO + x - 1) > 0) viewMatrix[y + 1, x + xbO - 1] = true;
                    }
                    catch { }
                }
            }

            if (vertical) // fill the last gap between the bars
            {
                if (viewMatrix.GetLength(0) > y)
                {
                    if (viewMatrix.GetLength(1) > (xO + view.ContentBox.Width + 1))
                    {
                        try
                        {
                            viewMatrix[y, xO + view.ContentBox.Width] = true;
                            viewMatrix[y, xO + view.ContentBox.Width + 1] = true;
                            viewMatrix[y - 1, xO + view.ContentBox.Width + 1] = true;
                        }
                        catch { }
                    }
                }
            }

            if (paintArrows)
            {
                //left arrow
                if (viewMatrix.GetLength(0) > (y + 1))
                {
                    if (viewMatrix.GetLength(1) > (xO + 1))
                    {
                        try
                        {
                            viewMatrix[y, xO] = true;
                            viewMatrix[y, xO + 1] = true;
                            viewMatrix[y - 1, xO + 1] = true;
                            viewMatrix[y + 1, xO + 1] = true;
                        }
                        catch { }
                    }
                }

                //right arrow
                if (viewMatrix.GetLength(0) > (y + 1))
                {
                    if (viewMatrix.GetLength(1) > (xO + +view.ContentBox.Width))
                    {
                        try
                        {
                            viewMatrix[y, xO + view.ContentBox.Width] = true;
                            viewMatrix[y, xO + view.ContentBox.Width - 1] = true;
                            viewMatrix[y - 1, xO + view.ContentBox.Width - 1] = true;
                            viewMatrix[y + 1, xO + view.ContentBox.Width - 1] = true;
                        }
                        catch { }
                    }
                }
            }
        }
    }
}