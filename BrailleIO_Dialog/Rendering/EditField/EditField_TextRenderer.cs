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
    class EditField_TextRenderer : DialogEntryRenderer
    {
        #region Members
        #region Constants
        private static readonly int INTER_CHAR_WIDTH = Renderer.MatrixBrailleRenderer.INTER_CHAR_WIDTH;
        private static readonly int INTER_LINE_HEIGHT = Renderer.MatrixBrailleRenderer.INTER_LINE_HEIGHT;
        private static readonly int CHAR_WIDTH = Renderer.MatrixBrailleRenderer.BRAILLE_CHAR_WIDTH + INTER_CHAR_WIDTH;
        private static readonly int CHAR_HEIGHT = Renderer.MatrixBrailleRenderer.BRAILLE_CHAR_HEIGHT;

        private const string ALLDOTS = "....";
        private const string HALFDOTS = "..";
        private readonly int DOTAMOUNT = ALLDOTS.Length;
        private readonly int HALFDOTAMOUNT = HALFDOTS.Length;
        #endregion

        private bool[,] cachedTitleMatrix;
        private bool[,] cachedLabelMatrix;

        public string LastTitleSegment = "";
        private string lastTitle = "";
        private string lastLabel = "";
        private int lastTitleSegmentIndex = 0;
        private int lastCursorPosition = -1;

        /// How the position should be adjusted due to used ".." and "...." as symbols for unseen content.
        private int positionDotPhasing = 0;

        /// Prevents jumps between middle segment and beginning/end segment
        /// when scroling reaches their borders. For smooth scrolling in middle part.
        private Boolean isInMiddleSegment = false;

        #endregion

        /// <summary>
        /// Renders the title matrix. Is the content of the edit field box.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns></returns>
        public bool[,] RenderTitleMatrix(IViewBoxModel view, EditField_DialogEntry entry, int LineContentWidth, int LineCount, int CharsPerLine, Boolean needsUpdate)
        {
            bool[,] renderedText = new bool[0, 0];

            if (view != null && entry != null)
            {
                if (!needsUpdate && cachedTitleMatrix != null)
                {
                    renderedText = (bool[,])cachedTitleMatrix.Clone();
                }
                else
                {
                    Entry = entry;
                    /*Content of Box has smaller Space, so temporary view with smaller content box is needed.*/
                    IViewBoxModel smallerView = new DummyViewBox(view);
                    Rectangle contentBox = smallerView.ContentBox;
                    //+1 da sonst statt 105, 104 raus kommt. Benötigt wird %mod3
                    contentBox.Width = LineContentWidth + 1;
                    smallerView.ContentBox = contentBox;

                    if (entry.Title.Length == 0)
                    {
                        //if content is empty
                        renderedText = new bool[CHAR_HEIGHT, LineContentWidth];
                    }
                    else if (entry.Title.Length * CHAR_WIDTH > LineContentWidth && (entry.InputBox.BoxHeightType == BoxHeightTypes.SingleLine || entry.InputBox.MinimizeType == MinimizeTypes.AlwaysMinimize))
                    {

                        //if editfield box is always minimized or single line type and bigger than a simple line
                        renderedText = renderSingleLineTitle(smallerView, entry, LineContentWidth, LineCount, CharsPerLine, needsUpdate);
                    }
                    else
                    {
                        //If through Minimization some text will not be shown until further action. show "TEXTPART...." instead
                        if (entry.InputBox.IsMinimized && (entry.InputBox.MinimizeType != MinimizeTypes.NeverMinimize) && entry.Title.Length > CharsPerLine)
                        {
                            //backup title, since entry is needed for rendering, but title will be displayed shortened
                            string titleBackup = entry.Title;

                            LastTitleSegment = entry.Title.Substring(0, (CharsPerLine - DOTAMOUNT)) + ALLDOTS;
                            entry.Title = LastTitleSegment;
                            renderedText = renderDialogEntry(smallerView);
                            entry.Title = titleBackup;

                        }
                        //if edit field box will be a simple line: default rendering
                        else if (LineCount == 1) renderedText = renderDialogEntry(smallerView);
                        //if edit field box will have more than one line
                        else renderedText = renderMultiLineTitle(smallerView, entry, LineContentWidth, LineCount, CharsPerLine);
                    };

                    if (renderedText.GetLength(0) == 0 && renderedText.GetLength(1) == 0)
                    {
                        renderedText = new bool[4, LineContentWidth];
                    }

                    cachedTitleMatrix = (bool[,])renderedText.Clone();
                }
                lastCursorPosition = entry.GetCursorPosition();

            }

            return renderedText;
        }


        BrailleIO.Renderer.MatrixBrailleRenderer brailleRenderer = null;

        #region render multiple lines
        /// <summary>
        /// Renders the titles that will result in more than one line.
        /// Workaround function. Breaks long strings into parts of length CharsPerLine. 
        /// So the MatrixBrailleRenderer will not break strings unpredictable, resulting in 
        /// spaces after or between broken parts. --> Causes CursorPosition to fail
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns></returns>
        private bool[,] renderMultiLineTitle(IViewBoxModel view, EditField_DialogEntry entry, int LineContentWidth, int LineCount, int CharsPerLine)
        {

            if (brailleRenderer == null) brailleRenderer = new Renderer.MatrixBrailleRenderer(Renderer.RenderingProperties.IGNORE_LAST_LINESPACE | Renderer.RenderingProperties.RETURN_REAL_WIDTH);
            bool[,] completeMatrix = brailleRenderer.RenderMatrix(view.ContentBox.Width, entry.Title);


            //bool[,] completeMatrix = emptyMatrix;

            //if (view != null && entry != null && entry.Title != null)
            //{

            //    List<string> titleParts = new List<string>();

            //    /*Cut string into CharsPerLine-long bits*/
            //    for (int i = 0; i < LineCount; i++)
            //    {
            //        if (entry.Title.Length > i * CharsPerLine + CharsPerLine)
            //            titleParts.Add(entry.Title.Substring(i * CharsPerLine, CharsPerLine));
            //        else
            //        {
            //            if (entry.Title.Length >= i * CharsPerLine)
            //                titleParts.Add(entry.Title.Substring(i * CharsPerLine));
            //            else break;
            //        }
            //    }

            //    List<bool[,]> boolTitleParts = new List<bool[,]>();

            //    int dimensiony = CHAR_HEIGHT;
            //    int dimensionx = LineContentWidth;

            //    int index = 0;

            //    /*Backup because entry's own renderer will be used. Renderer uses title for rendering.*/
            //    string titleBackup = Entry.Title;

            //    /*Render all string parts seperately*/
            //    foreach (var item in titleParts)
            //    {
            //        if (index > 0) dimensiony = INTER_LINE_HEIGHT + CHAR_HEIGHT + dimensiony;

            //        Entry.Title = item;
            //        boolTitleParts.Add(renderDialogEntry(view));

            //        index++;
            //    }

            //    Entry.Title = titleBackup;

            //    completeMatrix = new bool[dimensiony, dimensionx];

            //    index = 0;
            //    /*Join all seperate rendered string parts into one Matrix*/
            //    foreach (var item in boolTitleParts)
            //    {
            //        for (int i = 0; i < item.GetLength(0); i++)
            //        {
            //            if(i < completeMatrix.GetLength(0))
            //            for (int j = 0; j < item.GetLength(1); j++)
            //            {
            //                try
            //                {
            //                    if (j < completeMatrix.GetLength(1))
            //                    {
            //                        if (index == 0)
            //                            completeMatrix[i, j] = item[i, j];
            //                        else
            //                        {
            //                            completeMatrix[i + (index * (INTER_CHAR_WIDTH + CHAR_HEIGHT)), j] = item[i, j];
            //                        }
            //                    }
            //                }
            //                catch (Exception e)
            //                {
            //                    System.Diagnostics.Debug.WriteLine(e.ToString());
            //                }
            //            }
            //        }
            //        index++;
            //    }

            //}
            return completeMatrix;
        }
        #endregion

        #region Render SingleLine Type
        /// <summary>
        /// Renders  a single line type edit field.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns></returns>
        private bool[,] renderSingleLineTitle(IViewBoxModel view, EditField_DialogEntry entry, int LineContentWidth, int LineCount, int CharsPerLine, Boolean needsUpdate)
        {
            bool[,] renderedText = emptyMatrix;

            if (view != null && Entry != null && entry != null && entry.Title != null)
            {


                int width = LineContentWidth - (CHAR_WIDTH * DOTAMOUNT);

                string titleSegment = "";

                string titleBackup = Entry.Title;

                titleSegment = getTitleSegmentOfSingleLine(width, Entry.Title, entry.GetCursorPosition(), CharsPerLine, needsUpdate);

                Entry.Title = titleSegment;
                renderedText = renderDialogEntry(view);
                Entry.Title = titleBackup;

                LastTitleSegment = titleSegment;
            }

            return renderedText;
        }

        /// <summary>
        /// Gets the title segment that should be shown in the single line instead of the whole title.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="title">The title.</param>
        /// <param name="cursorPosition">The cursor position.</param>
        /// <returns></returns>
        private string getTitleSegmentOfSingleLine(int width, string title, int cursorPosition, int CharsPerLine, Boolean needsUpdate)
        {
            string titleSegment = "";

            if (width != null && title != null && cursorPosition != null)
            {
                if (cursorPosition == lastCursorPosition && LastTitleSegment != null && !needsUpdate)
                {
                    titleSegment = LastTitleSegment;
                }
                else
                {
                    Boolean movesRight = true;

                    int charsPerLine = CharsPerLine - DOTAMOUNT;

                    if (lastCursorPosition > cursorPosition) movesRight = false;

                    //Switch from middle handling to beginning/end handling if cursor has already reached start/end position
                    bool MiddleSwitch = false;

                    if (isInMiddleSegment)
                    {
                        if (movesRight && cursorPosition == Entry.Title.Length)
                        {
                            isInMiddleSegment = false;
                            MiddleSwitch = true;
                        }
                        else if (!movesRight && cursorPosition == 0)
                        {
                            isInMiddleSegment = false;
                            MiddleSwitch = true;
                        }
                    }

                    //If Cursor is in first part of the string (0 till charsPerLine)
                    if (cursorPosition <= charsPerLine && !isInMiddleSegment)
                    {
                        if (lastCursorPosition > 0 && lastCursorPosition <= charsPerLine && !MiddleSwitch && !needsUpdate)
                        {
                            titleSegment = LastTitleSegment;
                        }
                        else
                        {
                            titleSegment = title.Substring(0, charsPerLine);
                            titleSegment = titleSegment + ALLDOTS;
                            lastTitleSegmentIndex = 0;
                            positionDotPhasing = 0;
                        }
                    }
                    //If cursor is in the last part of the string (title.length - charsPerLine till titleLength)
                    else if (cursorPosition >= title.Length - charsPerLine && !isInMiddleSegment)
                    {
                        if (lastCursorPosition > 0 && lastCursorPosition >= title.Length - charsPerLine && !MiddleSwitch && !needsUpdate)
                        {
                            titleSegment = LastTitleSegment;
                        }
                        else
                        {
                            titleSegment = title.Substring((title.Length - charsPerLine), charsPerLine);
                            titleSegment = ALLDOTS + titleSegment;
                            lastTitleSegmentIndex = title.Length - charsPerLine;
                            positionDotPhasing = DOTAMOUNT;
                        }
                    }
                    //If the cursor is in the middle
                    else
                    {
                        isInMiddleSegment = true;
                        //cursor is moving right
                        if (movesRight)
                        {
                            if (cursorPosition > lastTitleSegmentIndex + charsPerLine && needsUpdate)
                            {
                                titleSegment = title.Substring(cursorPosition - (charsPerLine), charsPerLine);
                                lastTitleSegmentIndex = cursorPosition - (charsPerLine);
                                titleSegment = HALFDOTS + titleSegment + HALFDOTS;
                            }
                            else if (!title.Equals(lastTitle))
                            {
                                //if text was changed through deletion
                                titleSegment = title.Substring(lastTitleSegmentIndex, charsPerLine);
                                titleSegment = HALFDOTS + titleSegment + HALFDOTS;
                            }
                            else titleSegment = LastTitleSegment;
                        }
                        //cursor is moving left
                        else
                        {
                            if (cursorPosition < lastTitleSegmentIndex && needsUpdate)
                            {
                                titleSegment = title.Substring(cursorPosition, charsPerLine);
                                lastTitleSegmentIndex = cursorPosition;
                                titleSegment = HALFDOTS + titleSegment + HALFDOTS;
                            }
                            else if (!title.Equals(lastTitle))
                            {
                                //if text was changed through deletion
                                titleSegment = title.Substring(lastTitleSegmentIndex, charsPerLine);
                                titleSegment = HALFDOTS + titleSegment + HALFDOTS;
                            }
                            else titleSegment = LastTitleSegment;
                        }
                        positionDotPhasing = HALFDOTAMOUNT;
                    }
                }
            }
            return titleSegment;
        }
        #endregion

        #region Render Label

        /// <summary>
        /// Renders the label.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="view">The view.</param>
        /// <returns></returns>
        public bool[,] RenderLabel(EditField_DialogEntry entry, IViewBoxModel view)
        {
            bool[,] labelMatrix = emptyMatrix;

            if (cachedLabelMatrix == null)
            {
                if (view != null && entry != null && entry.HasLabel)
                {
                    string titleBackup = entry.Title;

                    this.Entry.Title = entry.Label;

                    labelMatrix = renderDialogEntry(view);

                    cachedLabelMatrix = labelMatrix;

                    this.Entry.Title = titleBackup;

                    lastLabel = entry.Label;
                }
            }
            else labelMatrix = (bool[,])cachedLabelMatrix.Clone();

            return labelMatrix;
        }


        #endregion

        #region Public Getter
        /// <summary>
        /// Gets the position offset.
        /// </summary>
        /// <returns></returns>
        public int GetPositionOffset()
        {
            if (positionDotPhasing != null && positionDotPhasing > 0)
                return positionDotPhasing;
            else return 0;
        }

        /// <summary>
        /// Gets the last index of the segment.
        /// </summary>
        /// <returns></returns>
        public int GetLastSegmentIndex()
        {
            if (lastTitleSegmentIndex != null && lastTitleSegmentIndex > 0)
                return lastTitleSegmentIndex;
            else return 0;
        }

        /// <summary>
        /// Gets the last title segment.
        /// </summary>
        /// <returns></returns>
        public string GetLastTitleSegment()
        {
            if (LastTitleSegment != null)
                return LastTitleSegment;
            else return "";
        }
        #endregion
    }
}
