//Autor:    Stephanie Schöne
// TU Dresden, Germany

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace BrailleIO.Dialogs.Rendering
{
    class EditField_CursorRenderer
    {
        private static readonly int INTER_CHAR_WIDTH = Renderer.MatrixBrailleRenderer.INTER_CHAR_WIDTH;
        private static readonly int CHAR_WIDTH = Renderer.MatrixBrailleRenderer.BRAILLE_CHAR_WIDTH + INTER_CHAR_WIDTH;
        private static readonly int CHAR_HEIGHT = Renderer.MatrixBrailleRenderer.BRAILLE_CHAR_HEIGHT;

        private const int FRAME_COUNT_LIMIT = 10;

        public Boolean ShowCursorForBlinking = false;

        public int FrameCount = 0;
        private bool[,] cachedCursorMatrix;


        #region Rendering
        /// <summary>
        /// Renders the cursor into the title matrix.
        /// </summary>
        /// <param name="renderedText">The rendered text.</param>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        /// TODO Edit XML Comment Template for renderCursorIntoTitleMatrix
        public bool[,] renderCursorIntoTitleMatrix(bool[,] renderedText, EditField_DialogEntry entry, int position, Boolean needsUpdate)
        {
            if (renderedText != null && position != null && entry != null && ShowCursorForBlinking)
            {
                if (cachedCursorMatrix != null && !needsUpdate)
                {
                    renderedText = cachedCursorMatrix;
                }
                else
                {
                    if (INTER_CHAR_WIDTH > 0)
                    {
                        renderedText = renderCursorAsVerticalLine(renderedText, position, entry.Title);
                    }
                    else renderedText = renderCursorAsDot(renderedText, position, entry.Title);

                    cachedCursorMatrix = (bool[,])renderedText.Clone();
                }
            }
            return renderedText;
        }


        #region Dot Cursor
        /// <summary>
        /// Renders the cursor as dot. Used for Braille Displays with no space between characters.
        /// </summary>
        /// <param name="renderedText">The rendered text.</param>
        /// <param name="p">The p.</param>
        /// <param name="Title">The title.</param>
        /// <returns></returns>
        private bool[,] renderCursorAsDot(bool[,] renderedText, int p, string Title)
        {
            if (renderedText == null || (renderedText.GetLength(0) == 0 && renderedText.GetLength(1) == 0))
            {
                renderedText = new bool[4, 3];
            }

            if (p != null && Title != null)
            {
                int height = renderedText.GetLength(0);

                //Could be renderedText.Width needed!
                Point boolPos = getCursorPositionInBoolMatrix(p, Title.Length, renderedText.GetLength(1));

                if (boolPos != null)
                {
                    if (boolPos.X == 0)
                    {
                        renderedText[boolPos.Y + 3, boolPos.X] = true;
                        renderedText[boolPos.Y + 3, boolPos.X + 1] = true;
                    }
                    else
                    {
                        renderedText[boolPos.Y + 3, boolPos.X - 1] = true;
                        renderedText[boolPos.Y + 3, boolPos.X - 2] = true;
                    }
                }
            }

            return renderedText;
        }
        #endregion

        #region Vertical Cursor

        /// <summary>
        /// Renders the cursor with a vertical line.
        /// </summary>
        /// <param name="renderedText">The rendered text.</param>
        /// <param name="pos">The position.</param>
        /// <param name="Title">The title.</param>
        /// <returns></returns>
        /// TODO Edit XML Comment Template for renderVerticalCursorPosition
        private bool[,] renderCursorAsVerticalLine(bool[,] renderedText, int pos, string Title)
        {

            if (renderedText == null || (renderedText.GetLength(0) == 0 && renderedText.GetLength(1) == 0))
            {
                renderedText = new bool[4, 3];
            }

            if (renderedText != null && Title != null)
            {

                int height = renderedText.GetLength(0);

                Point boolPos = getCursorPositionInBoolMatrix(pos, Title.Length, renderedText.GetLength(1));

                if (boolPos != null && boolPos.X < renderedText.GetLength(1))
                {
                    //if (boolPos.X == 0) boolPos.X++;

                    for (int i = 0; i < CHAR_WIDTH; i++)
                    {
                        renderedText[boolPos.Y + CHAR_HEIGHT - 1, boolPos.X + i] = true;
                    }
                }

            }
            return renderedText;
        }


        #endregion

        /// <summary>
        /// Gets the cursor start position of the character to mark in bool matrix.
        /// </summary>
        /// <param name="pos">The text position (char number in the string).</param>
        /// <param name="titlelength">The length of the text.</param>
        /// <param name="contentWidth">Width of the resulting rendered tactile content.</param>
        /// <returns>The starting point of the character to mark</returns>
        private static Point getCursorPositionInBoolMatrix(int pos, int titlelength, int contentWidth)
        {
            Point p = new Point(0, 0);

            if (pos > -1 && titlelength > 0 && contentWidth > 0)
            {
                p.X = pos * CHAR_WIDTH;
                p.Y = 0;
                // if (pos > titlelength) pos = titlelength;

                ////if (pos <= titlelength)
                //{
                //    while (p.X > contentWidth)
                //    {
                //        p.X = p.X - contentWidth;
                //        p.Y = p.Y + 5;
                //    }
                //}
            }

            return p;
        }

        #endregion

        #region Blinking



        #endregion

        public Boolean ShouldCursorToggle(EditField_DialogEntry localEntry)
        {
            Boolean shouldCursorBeShown = false;

            if (localEntry != null && FrameCount != null)
            {
                FrameCount++;
                if (FrameCount >= FRAME_COUNT_LIMIT && ShowCursorForBlinking != null)
                {
                    if (ShowCursorForBlinking) ShowCursorForBlinking = false;
                    else ShowCursorForBlinking = true;
                    FrameCount = 0;
                    shouldCursorBeShown = true;
                }
            }
            return shouldCursorBeShown;
        }

    }
}
