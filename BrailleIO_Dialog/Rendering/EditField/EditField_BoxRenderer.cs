//Autor:    Stephanie Schöne
// TU Dresden, Germany

using BrailleIO.Interface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace BrailleIO.Dialogs.Rendering
{
    class EditField_BoxRenderer
    {
        #region Members
        private bool[,] cachedEmptyBorderedEdgedMatrix;
        #endregion

        #region Rendering

        /// <summary>
        /// Renders the box matrix.
        /// </summary>
        /// <param name="dimensions">The dimensions.</param>
        /// <param name="needsUpdate">if set to <c>true</c> [needs update].</param>
        /// <param name="IsGraphical">if set to <c>true</c> [is graphical].</param>
        /// <param name="BorderThickness">The border thickness.</param>
        /// <returns></returns>
        public bool[,] RenderBoxMatrix(Point dimensions, Boolean needsUpdate, Boolean IsGraphical, int BorderThickness)
        {
            bool[,] boxMatrix = new bool[0,0];

            if (dimensions != null && dimensions.X > 0 && dimensions.Y > 0)
            {
                if (cachedEmptyBorderedEdgedMatrix != null && !needsUpdate)
                {
                    boxMatrix = cachedEmptyBorderedEdgedMatrix;
                }
                else
                {
                    boxMatrix = new bool[dimensions.Y, dimensions.X];

                    //Renders Edge & Borders around Content
                    if (IsGraphical)
                        boxMatrix = buildBorderAndEdgeMatrix(boxMatrix);
                    //renders indenting
                    else
                    {
                        boxMatrix = new bool[dimensions.Y - (2 * BorderThickness) + 1, dimensions.X];
                    }

                    cachedEmptyBorderedEdgedMatrix = (bool[,])boxMatrix.Clone();
                }
            }

            return boxMatrix;
        }

        /// <summary>
        /// Builds the border and edge matrix new.
        /// </summary>
        /// <param name="borderedEntry">The bordered entry.</param>
        /// <returns></returns>
        private static bool[,] buildBorderAndEdgeMatrix(bool[,] borderedEntry)
        {
            if (borderedEntry != null)
            {
                //Draw the edit field border & edge
                for (int i = 0; i < borderedEntry.GetLength(0); i++)
                {
                    for (int j = 0; j < borderedEntry.GetLength(1); j++)
                    {
                        if (isInUpperLeftCorner(borderedEntry, j, i))
                        {
                            //DRAWING THE UPPER LEFT EDGE
                            if (shouldDrawThisCornerPixel(borderedEntry, j, i))
                            {
                                borderedEntry[i, j] = true;
                            }
                        }
                        else
                        {
                            if (isOnBorder(borderedEntry, j, i))
                                borderedEntry[i, j] = true;
                        }
                    }
                }
            }
            return borderedEntry;
        }

        #endregion

        #region Helper Functions
        /// <summary>
        /// Determines whether coordinate is on border.
        /// </summary>
        /// <param name="borderedEntry">The bordered entry.</param>
        /// <param name="j">The j.</param>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        private static bool isOnBorder(bool[,] borderedEntry, int j, int i)
        {
            if ((i == 1 && j != 0 && j != borderedEntry.GetLength(1) - 1) //upper border
                || (i == borderedEntry.GetLength(0) - 2 && j != 0 && j != borderedEntry.GetLength(1) - 1) //lower Border
                || (j == 1 && i != 0 && i != borderedEntry.GetLength(0) - 1) //left Border
                || (j == borderedEntry.GetLength(1) - 2 && i != 0 && i != borderedEntry.GetLength(0) - 1)) //right border
                return true;
            else return false;
        }

        /// <summary>
        /// Desices weather corner pixel shoulds be drawn.
        /// </summary>
        /// <param name="borderedEntry">The bordered entry.</param>
        /// <param name="j">The j.</param>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        private static bool shouldDrawThisCornerPixel(bool[,] borderedEntry, int j, int i)
        {
            if (j == 1 && i == 4
            || j == 2 && i == 3
            || j == 3 && i == 2
            || j == 4 && i == 1)
                return true;
            else return false;
        }

        /// <summary>
        /// Determines whether coordinate is in upper right corner.
        /// </summary>
        /// <param name="borderedEntry">The bordered entry.</param>
        /// <param name="j">The j.</param>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        private static bool isInUpperLeftCorner(bool[,] borderedEntry, int j, int i)
        {
            if ((j == 1
                             || j == 2
                             || j == 3
                             || j == 4)
                             && (i == 1 || i == 2 || i == 3 || i == 4))
            {

                return true;
            }
            else return false;
        }
        #endregion
    }

}
