//Autor:    Stephanie Schöne
// TU Dresden, Germany

using BrailleIO.Interface;
using BrailleIO.Renderer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace BrailleIO.Dialogs.Rendering
{
    /// <summary>
    /// Renderer to transform a <see cref="DialogEntry" /> with type EditField into a bool matrix.
    /// </summary>
    /// <seealso cref="BrailleIO.Dialogs.Rendering.DialogEntryRenderer" />
    class EditFieldRenderer : DialogEntryRenderer
    {
        #region Members

        private readonly Object SyncLock = new Object();

        private EditField_DialogEntry editFieldEntry;

        private EditField_BoxRenderer boxRenderer = new EditField_BoxRenderer();
        private EditField_CursorRenderer cursorRenderer = new EditField_CursorRenderer();
        private EditField_TextRenderer textRenderer = new EditField_TextRenderer();
        private EditField_BoxProperties boxProperties = new EditField_BoxProperties();

        #region Caching
        private bool[,] cachedFinalMatrix;

        private bool titleHasChanged = true;
        private bool borderOrEdgeHaveChanged = true;
        private bool cursorHasChanged = true;
        private bool viewHasChanged = true;

        #endregion

        #region LastValues

        private bool lastIsMinimized;
        private int lastCursorPosition = -1;
        private IViewBoxModel lastView;
        private string lastTitle = "";

        #endregion

        #endregion

        #region Rendering

        /// <summary>
        /// Renders a content object into an boolean matrix;
        /// while <c>true</c> values indicating raised pins and <c>false</c> values indicating lowered pins
        /// Special Renderer for the EditField types.
        /// </summary>
        /// <param name="view">The frame to render in. This gives access to the space to render and other parameters. Normally this is a IBrailleIOViewRange.</param>
        /// <param name="content">The content to render.</param>
        /// <returns>
        /// A two dimensional boolean M x N matrix (bool[M,N]) where M is the count of rows (this is height)
        /// and N is the count of columns (which is the width).
        /// Positions in the Matrix are of type [i,j]
        /// while i is the index of the row (is the y position)
        /// and j is the index of the column (is the x position).
        /// In the matrix <c>true</c> values indicating raised pins and <c>false</c> values indicating lowered pins
        /// </returns>
        /// <remarks>
        /// Ignores the set content if this render has its own DialogEntry
        /// </remarks>
        public override bool[,] RenderMatrix(IViewBoxModel view, object content)
        {
            bool[,] m = emptyMatrix;

            if (view != null && content != null && content is EditField_DialogEntry)
            {
                Entry = content as EditField_DialogEntry;
                editFieldEntry = (EditField_DialogEntry)Entry;

                /*EditField items with unknown Box types will be displayed as normal dialog entries.*/
                if (editFieldEntry != null && editFieldEntry.InputBox != null && editFieldEntry.InputBox.BoxHeightType != BoxHeightTypes.Unknown && editFieldEntry.InputBox.MinimizeType != MinimizeTypes.Unknown)
                {
                    HasChanged = changeOccured(view);
                    handleCursorBlinking(editFieldEntry);

                    /*If a cached version exists and nothing has changed, use already cached matrix. Else recalculate.*/
                    if (cachedFinalMatrix == null || HasChanged)
                    {
                        lock (SyncLock)
                        {
                            m = buildCachedFinalMatrix(view);

                            cachedFinalMatrix = (bool[,])m.Clone();

                            titleHasChanged = false;
                            borderOrEdgeHaveChanged = false;
                            cursorHasChanged = false;
                        }
                    }
                    else
                    {
                        m = cachedFinalMatrix;
                    }
                }
                else m = renderDialogEntry(view);

            }
            return m;
        }

        private bool[,] buildCachedFinalMatrix(IViewBoxModel view)
        {
            bool[,] finalMatrix = emptyMatrix;

            if (view != null && editFieldEntry != null)
            {
                calculateBoxProperties(view);

                // Build the content (with or without borders and edges)
                finalMatrix = buildContentMatrix(view);

                //Build a label if editfield has one and combine label and content
                if (editFieldEntry.HasLabel)
                {
                    bool[,] labelMatrix = textRenderer.RenderLabel(editFieldEntry, view);
                    finalMatrix = combineLabelAndBoxMatrix(labelMatrix, finalMatrix);
                }
            }

            return finalMatrix;
        }

        #region Build the content matrix (BOX + text)
        /*Builds the cached matrix consisting out of edges, borders, title and cursor.
         Parts will be rendered new depending on changes on runtime.
         If entry is NOT graphical, boders and edges will not be rendered.*/
        private bool[,] buildContentMatrix(IViewBoxModel view)
        {
            bool[,] boxMatrix = emptyMatrix;

            if (view != null && editFieldEntry != null && editFieldEntry.Title != null)
            {
                //Renders the field text, if needed with cursor
                bool[,] renderedText = buildTextMatrix(view);

                //Renders Box that will contain text. If Graphical, it gets borders and edges. If Not, it will have enough
                // space for an indenting and will get 1 extra pin line to seperate label and content
                boxMatrix = boxRenderer.RenderBoxMatrix(boxProperties.BoxDimensions, borderOrEdgeHaveChanged, 
                    editFieldEntry.InputBox.IsGraphical, EditField_BoxProperties.BORDER_THICKNESS);

                //if editFieldEntry.Title is not empty, fill the box matrix with text matrix
                if (editFieldEntry.Title.Length > 0 || editFieldEntry.Status.HasFlag(DialogEntryStatus.Editing))
                {
                    boxMatrix = fillBoxMatrixWithTextMatrix(boxMatrix, renderedText);
                }

                lastView = view;
                lastTitle = Entry.Title;
                lastCursorPosition = editFieldEntry.GetCursorPosition();
                lastIsMinimized = editFieldEntry.InputBox.IsMinimized;
            }
            return boxMatrix;
        }


        #region Text & Cursor Rendering
        /*Builds the text matrix with cursor if needed.*/
        private bool[,] buildTextMatrix(IViewBoxModel view)
        {
            bool[,] renderedText = emptyMatrix;

            if (view != null)
            {
                //Renders the InputBox content
                renderedText = textRenderer.RenderTitleMatrix(view, editFieldEntry, boxProperties.LineContentWidth, boxProperties.LineCount, boxProperties.CharsPerLine, titleHasChanged);

                //If EditField is activated, the cursor will be rendered into the content matrix
                if (editFieldEntry.Status.HasFlag(DialogEntryStatus.Editing) && cursorRenderer.ShowCursorForBlinking)
                {
                    int position = editFieldEntry.GetCursorPosition();
                    //Adjust CursorPosition for Rendereing
                    if (editFieldEntry.InputBox.BoxHeightType == BoxHeightTypes.SingleLine || editFieldEntry.InputBox.MinimizeType == MinimizeTypes.AlwaysMinimize)
                    {
                        //Position has a shifting due to dots infront/after SingleLine Text
                        position = position - textRenderer.GetLastSegmentIndex() + textRenderer.GetPositionOffset();
                    }
                    //Position has a shifting due to line breaks that might ignore spaces in next lines
                    else if (boxProperties.LineCount != null && boxProperties.LineCount > 1) position = getCharPositionsAfterLineBreaks(renderedText, position, editFieldEntry.Title);

                    renderedText = cursorRenderer.renderCursorIntoTitleMatrix(renderedText, editFieldEntry, position, cursorHasChanged);
                    //renderedText = renderCursorIntoTitleMatrix(renderedText, position);
                }
            }

            return renderedText;
        }

        /// <summary>
        /// Gets the character positions with consideration of line breaks.
        /// Space chars on line beginnings are cut out after rendering. They need to be calculated for correct CursorPos.
        /// </summary>
        /// <param name="renderedText">The rendered text.</param>
        /// <param name="pos">The position.</param>
        /// <param name="Title">The title.</param>
        /// <returns></returns>
        private int getCharPositionsAfterLineBreaks(bool[,] renderedText, int pos, string Title)
        {
            int cursorpos = pos;
            if (cursorpos < 0) cursorpos = 0;
            int height = renderedText.GetLength(0);

            if (boxProperties.LineCount > 1)
            {
                int i = boxProperties.CharsPerLine;
                while (i <= boxProperties.CharsPerLine * boxProperties.LineCount)
                {
                    if (Title.Length > i)
                    {
                        string iChar = Title.ElementAt(i).ToString();
                        if (iChar.Equals(" "))
                            cursorpos--;
                    }
                    i = i + boxProperties.CharsPerLine - 1;
                }
            }

            return cursorpos;
        }
        #endregion

        #region Fill Box Matrix with Text Matrix
        //Fills the text matrix into the box matrix. If editfield is graphical, with borders and edges. Else just indenting and free pin line above
        private bool[,] fillBoxMatrixWithTextMatrix(bool[,] boxMatrix, bool[,] renderedText)
        {
            if (boxMatrix != null && renderedText != null && editFieldEntry != null)
            {
                if (editFieldEntry.InputBox.IsGraphical)
                {
                    for (int i = 0; i < boxMatrix.GetLength(0); i++)
                    {
                        for (int j = 0; j < boxMatrix.GetLength(1); j++)
                        {
                            //EditField_BoxProperties.BORDER_THICKNESS - 1 since i starts with 0, border thickness starts with 1
                            if (i > EditField_BoxProperties.BORDER_THICKNESS - 1 && i < boxMatrix.GetLength(0) - EditField_BoxProperties.BORDER_THICKNESS && j > (EditField_BoxProperties.BORDER_THICKNESS - 1) + EditField_BoxProperties.ADDITIONAL_EDGE_LENGTH && j < boxMatrix.GetLength(1) - EditField_BoxProperties.BORDER_THICKNESS)
                            {
                                if (i - EditField_BoxProperties.BORDER_THICKNESS < renderedText.GetLength(0) && j - EditField_BoxProperties.BORDER_THICKNESS < renderedText.GetLength(1) && renderedText[i - EditField_BoxProperties.BORDER_THICKNESS, j - EditField_BoxProperties.BORDER_THICKNESS - EditField_BoxProperties.ADDITIONAL_EDGE_LENGTH] != null)
                                    boxMatrix[i, j] = renderedText[i - EditField_BoxProperties.BORDER_THICKNESS, j - EditField_BoxProperties.BORDER_THICKNESS - EditField_BoxProperties.ADDITIONAL_EDGE_LENGTH];
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < renderedText.GetLength(0); i++)
                    {
                        for (int j = 0; j < renderedText.GetLength(1); j++)
                        {
                            if (i + 1 < boxMatrix.GetLength(0) && j + (2 * EditField_BoxProperties.BORDER_THICKNESS) < boxMatrix.GetLength(1))
                                boxMatrix[i + 1, j + (2 * EditField_BoxProperties.BORDER_THICKNESS)] = renderedText[i, j];
                        }
                    }
                }
            }

            return boxMatrix;
        }
        #endregion

        #endregion

        #region Combine Label and Content Matrix
        /*Adds the Label first, then the content into one matrix. For non graphical fields, a seperation line will be added in between.*/
        private bool[,] combineLabelAndBoxMatrix(bool[,] labelMatrix, bool[,] contentMatrix)
        {
            bool[,] finalMatrix = emptyMatrix;

            if (labelMatrix != null && contentMatrix != null)
            {
                int fullHeight = labelMatrix.GetLength(0) + contentMatrix.GetLength(0);

                int fullWidth = boxProperties.LineWidth;

                finalMatrix = new bool[fullHeight, fullWidth];

                for (int i = 0; i < finalMatrix.GetLength(0); i++)
                {
                    for (int j = 0; j < finalMatrix.GetLength(1); j++)
                    {
                        if (i < labelMatrix.GetLength(0))
                        {
                            if (j < labelMatrix.GetLength(1) && labelMatrix[i, j] != null)
                            {
                                finalMatrix[i, j] = labelMatrix[i, j];
                            }
                        }
                        else
                        {
                            if (i - labelMatrix.GetLength(0) < contentMatrix.GetLength(0) &&
                                j < contentMatrix.GetLength(1) && contentMatrix[i - labelMatrix.GetLength(0), j] != null)
                            {
                                finalMatrix[i, j] = contentMatrix[i - labelMatrix.GetLength(0), j];
                            }
                        }
                    }
                }
            }

            return finalMatrix;
        }
        #endregion


        #endregion

        #region Helper Functions
        /*Checks if Cursor should be shown/hidden/updated.*/
        private void handleCursorBlinking(EditField_DialogEntry localEntry)
        {
            if (localEntry != null)
            {
                if (localEntry.Status.HasFlag(DialogEntryStatus.Editing))
                {
                    if (cursorRenderer.ShouldCursorToggle(localEntry))
                    {
                        cursorHasChanged = true;
                    }
                }
                else if (!localEntry.Status.HasFlag(DialogEntryStatus.Editing) && cursorRenderer.ShowCursorForBlinking)
                {
                    cursorRenderer.ShowCursorForBlinking = false;
                    cursorHasChanged = true;
                }
            }
        }



        /*Depending on changed last values, box properties are calculated.*/
        private void calculateBoxProperties(IViewBoxModel view)
        {
            if (viewHasChanged)
            {
                boxProperties.CalculateBoxProperties(view, editFieldEntry);
                viewHasChanged = false;
            }
            else
            {
                if (titleHasChanged)
                {
                    boxProperties.CalculateLineCount(editFieldEntry);
                    boxProperties.CalculateLineContentHeight(view);
                }

                if (borderOrEdgeHaveChanged) boxProperties.CalculateBoxDimensions(view, editFieldEntry);
            }
        }

        /*Checks if changes have occured and which parts of the inputbox need to be rendered again*/
        private bool changeOccured(IViewBoxModel view)
        {
            if (view != null && editFieldEntry != null && editFieldEntry.InputBox != null)
            {
                if (lastView != null && lastCursorPosition != null && lastTitle != null && lastIsMinimized != null)
                {
                    if (lastView.ContentBox.Width != view.ContentBox.Width)
                    {
                        cursorHasChanged = true;
                        borderOrEdgeHaveChanged = true;
                        titleHasChanged = true;
                        viewHasChanged = true;
                    }
                    else
                    {
                        if (editFieldEntry.InputBox.IsMinimized != lastIsMinimized || editFieldEntry.Title.Length != lastTitle.Length)
                        {
                            borderOrEdgeHaveChanged = true;
                            cursorHasChanged = true;
                            titleHasChanged = true;
                        }
                        if (!editFieldEntry.Title.Equals(lastTitle))
                        {
                            titleHasChanged = true;
                            cursorHasChanged = true;
                        }
                        if (editFieldEntry.GetCursorPosition() != lastCursorPosition)
                        {
                            cursorHasChanged = true;
                            if (editFieldEntry.InputBox.BoxHeightType != null && (editFieldEntry.InputBox.BoxHeightType == BoxHeightTypes.SingleLine || editFieldEntry.InputBox.MinimizeType == MinimizeTypes.AlwaysMinimize))
                                titleHasChanged = true;
                        }
                    }
                }
                else
                {
                    lastTitle = editFieldEntry.Title;
                    lastView = view;
                    lastIsMinimized = editFieldEntry.InputBox.IsMinimized;

                    viewHasChanged = true;
                    titleHasChanged = true;
                    borderOrEdgeHaveChanged = true;
                    cursorHasChanged = true;
                }
            }
            return (viewHasChanged || titleHasChanged || borderOrEdgeHaveChanged || cursorHasChanged);
        }


        #region Public Getter
        /// <summary>
        /// Gets the last titlesegment.
        /// </summary>
        /// <returns></returns>
        public string GetLastTitleSegment()
        {
            return textRenderer.GetLastTitleSegment();
        }
        #endregion

        #endregion
    }
}
