//Autor:    Stephanie Schöne
// TU Dresden, Germany

using BrailleIO.Interface;
using System;
using System.Drawing;

namespace BrailleIO.Dialogs.Rendering
{
    class EditField_BoxProperties
    {
        #region Members

        #region Constants
        public const int BORDER_THICKNESS = 3;
        public const int ADDITIONAL_EDGE_LENGTH = 1;
        private static readonly int INTER_CHAR_WIDTH = Renderer.MatrixBrailleRenderer.INTER_CHAR_WIDTH;
        private static readonly int CHAR_WIDTH = Renderer.MatrixBrailleRenderer.BRAILLE_CHAR_WIDTH + INTER_CHAR_WIDTH;
        private static readonly int CHAR_HEIGHT = Renderer.MatrixBrailleRenderer.BRAILLE_CHAR_HEIGHT; // + Renderer.MatrixBrailleRenderer.INTER_LINE_HEIGHT;
        private const int SCROLLBAR_WIDTH = 3;
        #endregion

        /*Width of the whole line (content + borders)*/
        public int LineWidth;

        /*Width of the line content only*/
        public int LineContentWidth;

        /*Height of content + borders*/
        public int LineContentHeight;

        /*Amount of Lines the content needs*/
        public int LineCount;

        /*Amount of characters that fit into one line*/
        public int CharsPerLine;

        /*Dimensions of the Box (Width, Height)*/
        public Point BoxDimensions;

        #endregion

        #region Constructor
        public EditField_BoxProperties()
        {
            LineWidth = 0;
            LineContentWidth = 0;
            LineContentHeight = 0;
            LineCount = 0;
            BoxDimensions = new Point(0, 0);
            CharsPerLine = 0;
        }
        #endregion

        #region Calculations

        /// <summary>
        /// Calculates the box properties.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="entry">The entry.</param>
        public void CalculateBoxProperties(IViewBoxModel view, EditField_DialogEntry entry)
        {
            if (view != null && entry != null)
            {
                calculateLineWidth(view);
                calculateLineContentWidth();
                calculateCharsPerLine();
                CalculateLineCount(entry);
                CalculateLineContentHeight(view);
                CalculateBoxDimensions(view, entry);
            }

        }

        private void calculateLineWidth(IViewBoxModel view4rendering)
        {
            if (view4rendering != null)
                LineWidth = view4rendering.ContentBox.Width - SCROLLBAR_WIDTH;
        }

        private void calculateLineContentWidth()
        {
            if (LineWidth > 0)
            {
                //Space of linecontentwidth should be a mupltiple of char_width since leftover space smaller than char_width will be empty
                LineContentWidth = LineWidth - (2 * BORDER_THICKNESS + ADDITIONAL_EDGE_LENGTH);

                if (LineContentWidth % (CHAR_WIDTH) != 0)
                    LineContentWidth = LineContentWidth - (LineContentWidth % (CHAR_WIDTH));
            }
        }

        private void calculateCharsPerLine()
        {
            if (LineContentWidth > 0)
            {
                CharsPerLine = (int)Math.Floor(LineContentWidth / (double)CHAR_WIDTH);
            }
        }

        /// <summary>
        /// Calculates the line count.
        /// </summary>
        /// <param name="editFieldEntry">The edit field entry.</param>
        public void CalculateLineCount(EditField_DialogEntry editFieldEntry)
        {
            if (editFieldEntry != null && editFieldEntry.InputBox != null && CharsPerLine > 0)
            {
                switch (editFieldEntry.InputBox.BoxHeightType)
                {
                    case BoxHeightTypes.TextLength:
                        if (editFieldEntry.Title != null)
                            //Only as big as needed to show current content
                            LineCount = (int)Math.Ceiling(editFieldEntry.Title.Length / (double)CharsPerLine);
                        break;
                    case BoxHeightTypes.MaximumTextLength:

                        if (editFieldEntry.Validator != null && editFieldEntry.Validator.MaxTextLength > 0)
                            //maximum allowed space for content box
                            LineCount = (int)Math.Ceiling(editFieldEntry.Validator.MaxTextLength / (double)CharsPerLine);
                        break;
                    case BoxHeightTypes.Unknown:
                    case BoxHeightTypes.SingleLine:
                    default:
                        LineCount = 1;
                        break;
                }
                if (LineCount <= 0) LineCount = 1;
            }
        }

        /// <summary>
        /// Calculates the height of the line content.
        /// </summary>
        /// <param name="view4rendering">The view4rendering.</param>
        public void CalculateLineContentHeight(IViewBoxModel view4rendering)
        {
            if (view4rendering != null && LineCount > 0)
            {
                /*
                 * All char heights view4rendering.ContentHeight * LineCount
                 * one line space between multiple lines (LineCount - 1) 
                 * */
                LineContentHeight = CHAR_HEIGHT * LineCount + (LineCount - 1) + (2 * BORDER_THICKNESS);
            }
        }

        /// <summary>
        /// Calculates the box dimensions.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="editFieldEntry">The edit field entry.</param>
        /// <param name="lineContentWidth">Width of the line content.</param>
        /// <param name="lineWidth">Width of the line.</param>
        /// <param name="lineCount">The line count.</param>
        /// <returns></returns>
        public void CalculateBoxDimensions(IViewBoxModel view, EditField_DialogEntry editFieldEntry)
        {
            Point dimensions = new Point(0, 0);

            if (view != null)
            {
                if (editFieldEntry != null && editFieldEntry.InputBox != null && editFieldEntry.InputBox.IsMinimized && LineWidth != null)
                {
                    dimensions = new Point(LineWidth, CHAR_HEIGHT + (2 * BORDER_THICKNESS));
                }
                else if (LineWidth > 0 && LineContentHeight > 0)
                {
                    dimensions = new Point(LineWidth, LineContentHeight);
                }
            }

            BoxDimensions = dimensions;
        }

        #endregion
    }
}
