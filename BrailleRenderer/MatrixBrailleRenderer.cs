using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrailleRenderer.BrailleInterpreter;
using System.Text.RegularExpressions;

namespace BrailleRenderer
{
    public class MatrixBrailleRenderer
    {
        #region Members

        /// <summary>
        /// Interprets dots as Characters and vise versa
        /// </summary>
        public readonly IBraileInterpreter BrailleInterpreter;

        #endregion

        #region Constructor

        public MatrixBrailleRenderer(IBraileInterpreter brailleInterpreter)
        {
            BrailleInterpreter = brailleInterpreter;
        }

        #endregion

        #region IBrailleIOContentRenderer

        public bool[,] RenderMatrix(int width, object content, bool scrollbars = false)
        {
            if (content != null && content is String)
            {
                String text = content.ToString();
                if (BrailleInterpreter != null)
                {
                    //reduce the with if scrollbars should been shown
                    width -= scrollbars ? 3 : 0;

                    //check available width
                    if (width > 0)
                    {
                        List<List<List<int>>> lines = new List<List<List<int>>>();

                        // split text into paragraphs/lines
                        string[] paragraphs = getLinesOfString(text);

                        foreach (var p in paragraphs)
                        {
                            var paragraphLines = renderParagraph(p, width);
                            lines.AddRange(paragraphLines);
                        }

                        // build start matrix
                        bool[,] matrix = buildMatrixFromLines(lines, width);

                        return matrix;
                    }
                }
            }
            return null;
        }

        #endregion

        #region String Handling

        private string[] getWordsOfString(string text)
        {
            return Regex.Split(text, @"\s");
        }

        private readonly Regex paragraphSplitter = new Regex("\r\n|\r|\n");

        private string[] getLinesOfString(string text)
        {
            return paragraphSplitter.Split(text);
        }

        List<List<int>> getBrailleFromString(String text)
        {
            return BrailleInterpreter.GetDotsFromString(text);
        }

        #endregion

        #region Rendering

        public const int BRAILLE_CHAR_WIDTH = 2;
        public const int BRAILLE_CHAR_HEIGHT = 4;

        public const int INTER_CHAR_WIDTH = 1;
        public const int INTER_LINE_HEIGHT = 1;

        #region Lenght Calculations

        public bool EstimateNeedOfScrollBar(string content, int width, int height)
        {
            if (!String.IsNullOrEmpty(content) && width > 0 && height > 0)
            {
                int maxCharsToFit = getMaxCountOfChars(width) * getMaxCountOfLines(height);
                if (maxCharsToFit < (content.Length * 0.8)) return true;
            }
            return false;
        }


        /// <summary>
        /// Gets the max width consumed by an rendered string including inter character
        /// space without inter character space at the end of the String.
        /// </summary>
        /// <param name="brailleChars">The braille chars.</param>
        /// <returns>the minimum width consumed by the string</returns>
        int getMinWidthOfString(List<List<int>> brailleChars)
        {
            return getMaxWidthOfString(brailleChars) - INTER_CHAR_WIDTH;
        }

        /// <summary>
        /// Gets the max width consumed by an rendered string including inter character
        /// space and inter character space at the end of the String.
        /// </summary>
        /// <param name="brailleChars">The braille chars.</param>
        /// <returns>the maximum width consumed by the string including separating space at the end of the string</returns>
        int getMaxWidthOfString(List<List<int>> brailleChars)
        {
            return (brailleChars.Count * BRAILLE_CHAR_WIDTH) + (brailleChars.Count * INTER_CHAR_WIDTH);
        }


        /// <summary>
        /// Gets the max count of chars that would fit into the given width.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <returns>the number of chars that would fit</returns>
        int getMaxCountOfChars(int width)
        {
            double ratio = (double)(width + INTER_CHAR_WIDTH) / (double)(BRAILLE_CHAR_WIDTH + INTER_CHAR_WIDTH);
            return (int)Math.Floor(ratio);
        }

        /// <summary>
        /// Gets the max count of chars that would fit into the given width.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <returns>the number of chars that would fit</returns>
        int getMaxCountOfLines(int height)
        {
            double ratio = (double)(height + INTER_LINE_HEIGHT) / (double)(BRAILLE_CHAR_HEIGHT + INTER_LINE_HEIGHT);
            return (int)Math.Floor(ratio);
        }

        /// <summary>
        /// Checks if a word fits into an available width.
        /// </summary>
        /// <param name="brailleChars">The braille chars.</param>
        /// <param name="availableWidth">Available width.</param>
        /// <returns><c>true</c> if the word fits with his minimal lenght into the available space. Otherwise <c>false</c>.</returns>
        bool fitsWordInRestOfLine(List<List<int>> brailleChars, int availableWidth)
        {
            if (getMinWidthOfString(brailleChars) <= availableWidth) return true;
            return false;
        }

        #endregion

        #region Text to Braille Dot-Pattern Lines

        /// <summary>
        /// Renders a paragraph.
        /// </summary>
        /// <param name="text">The text to render.</param>
        /// //TODO: make the rendering Positions available
        private List<List<List<int>>> renderParagraph(string text, int width)
        {
            List<List<List<int>>> lines = new List<List<List<int>>>();
            if (width > 0)
            {
                string[] words = getWordsOfString(text);

                List<List<int>> currentLine = new List<List<int>>();
                int availableWidth = width;

                foreach (var word in words)
                {
                    // get the dot patterns for that word
                    List<List<int>> dots = getBrailleFromString(word);

                    // check if the line has still some entries
                    // if so, add a space char or open a new line
                    if (currentLine.Count > 0)
                    {
                        // check if the line is full
                        if (getMaxWidthOfString(currentLine) >= width)
                        {
                            makeNewLine(ref lines, ref currentLine, ref availableWidth, width);
                        }
                        else
                        {
                            // add a space
                            currentLine.Add(new List<int>());
                            availableWidth -= INTER_CHAR_WIDTH + BRAILLE_CHAR_WIDTH;
                        }
                    }

                    //check if it fits into the available space
                    if (!fitsWordInRestOfLine(dots, availableWidth)) //no
                    {
                        makeNewLine(ref lines, ref currentLine, ref availableWidth, width);
                    }

                    int minWidth = getMinWidthOfString(dots);
                    if (minWidth > width)
                    {
                        //split the word into several lines
                        List<List<List<int>>> wordLines = splitWordOverLines(dots, width);

                        if (wordLines != null && wordLines.Count > 0)
                        {
                            // add them to the lines
                            lines.AddRange(wordLines);
                            //set the current line
                            currentLine = lines[lines.Count - 1];

                            //remove from list so it will not added twice
                            lines.Remove(currentLine);

                            //update the available size
                            availableWidth = width - getMinWidthOfString(currentLine);
                        }
                    }
                    else
                    {
                        //fill the word into the line
                        currentLine.AddRange(dots);

                        //update the available width
                        availableWidth -= getMinWidthOfString(dots) + INTER_CHAR_WIDTH;
                    }
                }
                lines.Add(currentLine);
            }
            return lines;
        }

        private List<List<List<int>>> splitWordOverLines(List<List<int>> dots, int width)
        {
            List<List<List<int>>> lines = new List<List<List<int>>>();

            if (dots != null && width > BRAILLE_CHAR_WIDTH)
            {
                //Split the word char list in peaces width max length of 'width'
                int count = getMaxCountOfChars(width);
                if (count > 0)
                {
                    int i = 0;

                    while (i < dots.Count)
                    {
                        int l = (i + count) < dots.Count ? count : (dots.Count - i);
                        List<List<int>> subList = dots.GetRange(i, l);
                        lines.Add(subList);
                        i += count;
                    }
                }
            }
            return lines;
        }

        /// <summary>
        /// Saves the current line to the lines list and opens a new line
        /// </summary>
        /// <param name="lines">The list of filled lines.</param>
        /// <param name="currentLine">The current line to fill with chars.</param>
        /// <param name="availableWidth">Current available space for chars on the current line.</param>
        /// <param name="width">The max width of a line.</param>
        private void makeNewLine(ref List<List<List<int>>> lines, ref List<List<int>> currentLine, ref int availableWidth, int width)
        {
            //save the line and open a new one
            lines.Add(currentLine);
            currentLine = new List<List<int>>();

            //reset available with
            availableWidth = width;
        }

        #endregion

        #region Build Bool/Dot Matrix

        private bool[,] buildMatrixFromLines(List<List<List<int>>> lines, int width)
        {
            int height = 0;
            if (lines != null)
            {
                height = lines.Count * (BRAILLE_CHAR_HEIGHT + INTER_LINE_HEIGHT);
            }

            bool[,] m = new bool[height, width];

            if (lines != null)
            {
                //do each line
                for (int lineNumber = 0; lineNumber < lines.Count; lineNumber++) // do this as a parallel for?!
                {
                    List<List<int>> line = lines[lineNumber];
                    int yoffset = lineNumber * (BRAILLE_CHAR_HEIGHT + INTER_LINE_HEIGHT);

                    // do every char
                    for (int charNumber = 0; charNumber < line.Count; charNumber++)
                    {
                        List<int> brailleChar = line[charNumber];
                        int xOffset = charNumber * (BRAILLE_CHAR_WIDTH + INTER_CHAR_WIDTH);

                        addDotPatternToMatrix(ref m, brailleChar, xOffset, yoffset);
                    }
                }
            }
            return m;
        }

        /// <summary>
        /// Adds the dots of a char dot pattern to a matrix.
        /// </summary>
        /// <param name="m">The matrix to fill.</param>
        /// <param name="brailleChar">The braille char to add.</param>
        /// <param name="xOffset">The x offset for the chr to place.</param>
        /// <param name="yoffset">The yoffset for the char to place.</param>
        private void addDotPatternToMatrix(ref bool[,] m, List<int> brailleChar, int xOffset, int yoffset)
        {
            if (brailleChar != null
                && m != null && m.GetLength(0) > 0 && m.GetLength(1) > 0)
            {
                for (int dotNumber = 0; dotNumber < brailleChar.Count; dotNumber++)
                {
                    int x, y = -1;
                    bool success = tryGetPinPositionOfbRailleDot(brailleChar[dotNumber], out x, out y);
                    if (success && x >= 0 && y >= 0)
                    {
                        x += xOffset;
                        y += yoffset;

                        if (y >= 0 && y < m.GetLength(0)
                            && x >= 0 && x < m.GetLength(1))
                        {
                            m[y, x] = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Tries the get pin position of a Braille dot e.g. the dot 5 has the position x=1 y=1
        /// </summary>
        /// <param name="dot">The dot.</param>
        /// <param name="x">The returning x position of this dot.</param>
        /// <param name="y">The returning y position of this dot.</param>
        /// <returns><c>true</c> if it was posible to mathch the dot to a position. Otherwise <c>false</c></returns>
        static bool tryGetPinPositionOfbRailleDot(int dot, out int x, out int y)
        {
            if (dot < 9 && dot > 0)
            {
                x = y = -1;
                switch (dot)
                {
                    case 1:
                        x = 0; y = 0;
                        break;
                    case 2:
                        x = 0; y = 1;
                        break;
                    case 3:
                        x = 0; y = 2;
                        break;
                    case 4:
                        x = 1; y = 0;
                        break;
                    case 5:
                        x = 1; y = 1;
                        break;
                    case 6:
                        x = 1; y = 2;
                        break;
                    case 7:
                        x = 0; y = 3;
                        break;
                    case 8:
                        x = 1; y = 3;
                        break;
                    default:
                        return false;
                }
                return true;
            }
            x = y = -1;
            return false;
        }

        #endregion

        #endregion

    }
}