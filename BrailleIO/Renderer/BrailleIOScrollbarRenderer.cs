using System;
using BrailleIO.Interface;

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


            // draw the slider track/line
            //      if "paint all arrows" and height < 8 do nothing
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

            // maximum possible offset until the last content line is the last visible line. 
            double maxOffset = view.ContentHeight - view.ContentBox.Height;
            // maximum space for the first pin of the slider to be placed in
            double maxSliderWay = view.ContentBox.Height - 3;
            // ratio between way to go and space available
            double offset2wayRatio = maxSliderWay / maxOffset;

            double sliderOffset = Math.Abs(Math.Round(
                yOffset * offset2wayRatio
                , MidpointRounding.AwayFromZero));

            int ybO = yO + (int)sliderOffset + 1;

            // if paint arrows and the height is smaller than 8 (3 + 3 + 2) -> paint no slider           
            //TODO: paint slider different if arrows are active
            if (paintArrows && view.ContentBox.Height < 8)
            {
                //do nothing
            }
            else if (paintArrows && view.ContentBox.Height < 10)
            {
                if ((viewMatrix.GetLength(0) > ybO) && (viewMatrix.GetLength(1) > (x + 1)))
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
                // paint the three point hight slider
                for (int y = 0; y < 3; y++)
                {
                    if ((viewMatrix.GetLength(0) > (ybO + y - 1)) && (viewMatrix.GetLength(1) > (x + 1)))
                    {
                        try
                        {
                            if ((ybO + y - 1) > 0)
                                viewMatrix[ybO + y - 1, x + 1] = true;
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

            // maximum possible offset until the last content line is the last visible line. 
            double maxOffset = view.ContentWidth - view.ContentBox.Width;
            // maximum space for the first pin of the slider to be placed in
            double maxSliderWay = view.ContentBox.Width - 3;
            // ratio between way to go and space available
            double offset2wayRatio = maxSliderWay / maxOffset;

            double sliderOffset = Math.Abs(Math.Round(
                xOffset * offset2wayRatio
                , MidpointRounding.AwayFromZero));

            int xbO = xO + (int)Math.Abs(sliderOffset) + 1;
            for (int x = 0; x < 3; x++)
            {
                if ((viewMatrix.GetLength(1) > (xbO + x - 1)) && (viewMatrix.GetLength(0) > (y + 1)))
                {
                    try
                    {
                        if ((xbO + x) > 0) viewMatrix[y + 1, x + xbO - 1] = true;
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