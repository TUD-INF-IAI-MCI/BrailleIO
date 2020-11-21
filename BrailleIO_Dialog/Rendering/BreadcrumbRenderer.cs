using BrailleIO.Interface;
using BrailleIO.Renderer;
using BrailleIO.Renderer.Structs;
using System;
using System.Collections.Generic;
using System.Drawing;
using tud.mci.LanguageLocalization;

namespace BrailleIO.Dialogs.Rendering
{
    /// <summary>
    /// Renderer for creating a breadcrumb-like menu 
    /// of the current dialog rootline for a header 
    /// region.
    /// </summary>
    /// <seealso cref="BrailleIO.Interface.IBrailleIOContentRenderer" />
    /// <seealso cref="BrailleIO.Renderer.ITouchableRenderer" />
    public class BreadcrumbRenderer : IBrailleIOContentRenderer, ITouchableRenderer
    {

        #region Member

        #region private

        static LL ll = new LL(BrailleIO.Dialogs.Properties.Resources.Language);

        static readonly bool[,] emptyMatrix = new bool[0, 0] { };

        bool[,] _lastRendereinresult = emptyMatrix;
        Size lastViewSize = new Size();

        Dialog oldDialog = null;

        List<RenderElement> _renderedElements = new List<RenderElement>();
        protected List<RenderElement> RenderedElements
        {
            get
            {
                if (_renderedElements == null) _renderedElements = new List<RenderElement>();
                return _renderedElements;
            }
            set { _renderedElements = value; }
        } 

        #endregion

        #region public

        /// <summary>
        /// The locking object for synchronous usage.
        /// </summary>
        public readonly Object SynchLock = new Object();

        /// <summary>
        /// Gets the rendered elements.
        /// </summary>
        /// <value>
        /// The rendered elements.
        /// </value>
        public List<RenderElement> GetAllRenderElements()
        {
             return new List<RenderElement>(RenderedElements); 
        } 

        MatrixBrailleRenderer _brailleRenderer;
        /// <summary>
        /// Gets or sets the braille renderer.
        /// </summary>
        /// <value>
        /// The braille renderer.
        /// </value>
        public MatrixBrailleRenderer BrailleRenderer
        {
            get
            {
                lock (SynchLock)
                {
                    if (_brailleRenderer == null)
                        _brailleRenderer = new MatrixBrailleRenderer(RenderingProperties.IGNORE_LAST_LINESPACE | RenderingProperties.RETURN_REAL_WIDTH);
                    return _brailleRenderer; 
                }
            }
            set
            {
                lock (SynchLock)
                {
                    _brailleRenderer = value; 
                }
            }
        }

        private static String _separator;
        /// <summary>
        /// Gets or sets the separator dividing a parent from its child layer.
        /// </summary>
        /// <value>
        /// The separator.
        /// </value>
        public static String Separator
        {
            get
            {
                if (BreadcrumbRenderer._separator == null)
                    BreadcrumbRenderer._separator = String.Empty;
                return BreadcrumbRenderer._separator;
            }
            set { BreadcrumbRenderer._separator = value; }
        }

        private static String _suppressor;
        /// <summary>
        /// Gets or sets the suppressor, replacing the rest of a string it is to long.
        /// </summary>
        /// <value>
        /// The suppressor.
        /// </value>
        public static String Suppressor
        {
            get
            {
                if (BreadcrumbRenderer._suppressor == null)
                    BreadcrumbRenderer._suppressor = String.Empty;
                return BreadcrumbRenderer._suppressor;
            }
            set { BreadcrumbRenderer._suppressor = value; }
        }


        private static String _omission;
        /// <summary>
        /// Gets or sets the omission string, replacing rootline elements, which will not fit into the space.
        /// </summary>
        /// <value>
        /// The omission.
        /// </value>
        public static String Omission
        {
            get
            {
                if (_omission == null) _omission = String.Empty;
                return BreadcrumbRenderer._omission;
            }
            set { BreadcrumbRenderer._omission = value; }
        }

        private static int _mbccc = 3;
        /// <summary>
        /// Gets or sets the minimal amount of chars a breadcrumb entry is shorten to.
        /// </summary>
        /// <value>
        /// The minimal breadcrumb entry length.
        /// </value>
        public static int MinimalBreadcrumbEntryCharCount
        {
            get
            {
                if (BreadcrumbRenderer._mbccc < 1)
                    BreadcrumbRenderer._mbccc = 1;
                return BreadcrumbRenderer._mbccc;
            }
            set { BreadcrumbRenderer._mbccc = value; }
        }

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BreadcrumbRenderer"/> class.
        /// </summary>
        /// <param name="brailleRenderer">The braille renderer to use.</param>
        public BreadcrumbRenderer(MatrixBrailleRenderer brailleRenderer = null)
        {
            lock (SynchLock)
            {
                _brailleRenderer = brailleRenderer;
                init(); 
            }
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        static void init()
        {
            if (ll != null)
            {
                if (String.IsNullOrEmpty(Separator))
                    Separator = ll.GetTrans("breadcrumb.seperator");
                if (String.IsNullOrEmpty(Suppressor))
                    Suppressor = ll.GetTrans("breadcrumb.suppressor");
                if (String.IsNullOrEmpty(Omission))
                    Omission = ll.GetTrans("breadcrumb.omission");
            }
        }

        #endregion

        #region IBrailleIOContentRenderer

        /// <summary>
        /// Renders a content object into an boolean matrix;
        /// while <c>true</c> values indicating raised pins and <c>false</c> values indicating lowered pins
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
        public bool[,] RenderMatrix(IViewBoxModel view, object content)
        {
            lock (SynchLock)
            {
                if (view != null && content != null && content is Dialog && BrailleRenderer != null)
                {
                    Size currentSize = view.ContentBox.Size;
                    if (((Dialog)content).Equals(oldDialog))
                    {
                        if (currentSize.Equals(lastViewSize))
                        {
                            if (!_lastRendereinresult.Equals(emptyMatrix))
                            {
                                view.ContentHeight = _lastRendereinresult.GetLength(0);
                                view.ContentWidth = _lastRendereinresult.GetLength(1);

                                return _lastRendereinresult;
                            }
                        }
                        else
                        {
                        }
                    }
                    else
                    {
                        oldDialog = content as Dialog;
                    }

                    lastViewSize = currentSize;

                    /************* BUILD LINE ****************/
                    List<RenderElement> renderedElements = new List<RenderElement>();
                    string text = buildBreadcrumbString(oldDialog, lastViewSize, out renderedElements);
                    _lastRendereinresult = BrailleRenderer.RenderMatrix(view, text);
                    // var renderdStuff = BrailleRenderer.GetAllRenderElements();

                    RenderedElements = renderedElements; // combineRenderingResults(renderdStuff, renderedElements);

                }

                view.ContentHeight = _lastRendereinresult.GetLength(0);
                view.ContentWidth = _lastRendereinresult.GetLength(1);
                return _lastRendereinresult; 
            }
        }

        //private List<RenderElement> combineRenderingResults(LinkedList<RenderElement> renderdText, List<RenderElement> renderedElements)
        //{
        //    List<RenderElement> rndEl = new List<RenderElement>();
        //    // FIXME: do this right

        //    if (renderdText != null && renderdText.Count > 0)
        //    {
        //        foreach (var item in renderdText)
        //        {
        //            if (item.HasSubParts())
        //            {
        //                foreach (var subItem in item.GetSubParts())
        //                {
        //                    rndEl.Add(subItem);
        //                }
        //            }
        //        }
        //    }

        //    return rndEl;
        //}


        #endregion

        #region Breadcrumb Building

        private string buildBreadcrumbString(Dialog currentDialog, Size viewSize, out List<RenderElement> renderedElements)
        {
            renderedElements = new List<RenderElement>();
            String breadcrumb = String.Empty;
            var rootline = GetRootLineOfDialog(currentDialog);

            if (rootline != null && rootline.Count > 0)
            {
                // add last entry
                breadcrumb += rootline[0].Title;

                int usedSpace = estimateTextSpaceConsumption(breadcrumb);
                int leftSpace = viewSize.Width - usedSpace;

                if (leftSpace > (Omission.Length + Separator.Length)) // enough space for adding more
                {
                    String rootlineString = getRootlineString(rootline, leftSpace, ref renderedElements);
                    breadcrumb = rootlineString + breadcrumb;

                    renderedElements.Add(
                            new RenderElement(
                                estimateTextSpaceConsumption(rootlineString, true),
                                0,
                                estimateTextSpaceConsumption(rootline[0].Title),
                                MatrixBrailleRenderer.BRAILLE_CHAR_HEIGHT, rootline[0]) { DisplayName = rootline[0].Title });


                }
                else // we have to shorten even the current dialog title?!
                {
                    int chars4PinSpace = estimateCharsForPinSpace(viewSize.Width);
                    String rootlineString = getRootlineString(rootline, leftSpace, ref renderedElements);

                    // does the rootline string fits and left enough space for the dialog title?
                    if (chars4PinSpace - rootlineString.Length > MinimalBreadcrumbEntryCharCount)
                    {
                        int space4dialog = chars4PinSpace - rootlineString.Length;
                        string dlgTitle2Show = rootline[0].Title.Length > space4dialog ?
                            rootline[0].Title.Substring(0, Math.Min(space4dialog - Suppressor.Length, rootline[0].Title.Length)) + Suppressor :
                            rootline[0].Title;
                        breadcrumb = rootlineString + dlgTitle2Show;

                        renderedElements.Add(
                            new RenderElement(
                                estimateTextSpaceConsumption(rootlineString, true),
                                0,
                                estimateTextSpaceConsumption(dlgTitle2Show),
                                MatrixBrailleRenderer.BRAILLE_CHAR_HEIGHT, rootline[0]) { DisplayName = dlgTitle2Show });


                    }
                    else // not enough space to display a rootline and the current dialog title
                    {
                        string minimalRootString = Omission + Separator;
                        if (minimalRootString.Length + MinimalBreadcrumbEntryCharCount + Suppressor.Length > chars4PinSpace)
                        {
                            // not even the slightest hint for the root fits into the space
                            breadcrumb = rootline[0].Title.Substring(0, Math.Min(chars4PinSpace, rootline[0].Title.Length));

                            renderedElements.Clear();
                            renderedElements.Add(
                                new RenderElement(
                                    0,
                                    0,
                                    estimateTextSpaceConsumption(breadcrumb),
                                    MatrixBrailleRenderer.BRAILLE_CHAR_HEIGHT, rootline[0]) { DisplayName = breadcrumb });
                        
                        }
                        else // enough space for something like '.../abc~'
                        {
                            breadcrumb = rootline[0].Title.Substring(0, Math.Min(chars4PinSpace, rootline[0].Title.Length));

                            //TODO: check this and add to rendered elements
                        }
                    }
                }
            }

            return breadcrumb;
        }

        /// <summary>
        /// build the string for the left rootline entries beginning by the root, separated by separator etc. 
        /// </summary>
        /// <param name="rootline">The whole rootline.</param>
        /// <param name="leftSpace">The space left for the parent entries to be displayed.</param>
        /// <returns>A string building an rootline breadcrumb for the parent levels.</returns>
        private String getRootlineString(List<Dialog> rootline, int leftSpace, ref List<RenderElement> renderedElements)
        {
            String rootlineString = String.Empty;
            if (rootline.Count > 1)
            {
                int rootlineEntriesCount = rootline.Count - 1;
                int spaceNeeded = estimateMinimalSpaceForBreadcrumbEntry();
                int amountOfLevelsToFit = leftSpace / spaceNeeded;

                int spacePerLevel = leftSpace / rootlineEntriesCount;
                spacePerLevel = estimateCharsForPinSpace(spacePerLevel);
                spacePerLevel -= calculateInterLevelWordLength();
               

                if (rootlineEntriesCount > amountOfLevelsToFit || spacePerLevel < (MinimalBreadcrumbEntryCharCount + Separator.Length))
                {   // not all levels will fit ... so make special treatment

                    // add root menu
                    string dialogString = String.Empty;
                    var dlg = rootline[rootline.Count - 1];
                    if (dlg != null)
                    {

                        if (dlg.Title == null || dlg.Title.Length <= MinimalBreadcrumbEntryCharCount + Suppressor.Length)
                        {
                            dialogString += dlg.Title;
                        }
                        else
                        {
                            dialogString += dlg.Title.Substring(0, Math.Min(MinimalBreadcrumbEntryCharCount, dlg.Title.Length)) + Suppressor;
                        }

                        // add dialogString and related dialog to the rendered elements
                        renderedElements.Add(
                            new RenderElement(
                                estimateTextSpaceConsumption(rootlineString, true),
                                0,
                                estimateTextSpaceConsumption(dialogString),
                                MatrixBrailleRenderer.BRAILLE_CHAR_HEIGHT, dlg) { DisplayName = dialogString });


                        dialogString += Separator;
                    }
                   

                    // add the hint, that some more elements are in between
                    if (rootlineEntriesCount > 1)
                    {
                        // add Omission string and parent dialog to the rendered elements
                        renderedElements.Add(
                            new RenderElement(
                                estimateTextSpaceConsumption(dialogString, true),
                                0,
                                estimateTextSpaceConsumption(Omission),
                                MatrixBrailleRenderer.BRAILLE_CHAR_HEIGHT, rootline[1]) { DisplayName = Omission });

                        dialogString += Omission + Separator;
                    }

                    rootlineString = dialogString;

                }
                else // all levels of the rootline can be displayed somehow 
                {
                    

                    for (int i = rootlineEntriesCount; i > 0; i--)
                    {
                        if (rootline[i] != null)
                        {
                            string levelStr = rootline[i].Title;
                            if (levelStr.Length > spacePerLevel)
                            {
                                // shorten the text and add suppressor
                                levelStr = levelStr.Substring(0, Math.Min(spacePerLevel, levelStr.Length)) + Suppressor;
                            }

                            // add levelstring and related dialog to the rendered elements
                            renderedElements.Add(
                                new RenderElement(
                                    estimateTextSpaceConsumption(rootlineString, true),
                                    0,
                                    estimateTextSpaceConsumption(levelStr),
                                    MatrixBrailleRenderer.BRAILLE_CHAR_HEIGHT, rootline[i]) { DisplayName = levelStr });
                            rootlineString += levelStr + Separator;
                        }
                    }
                }
            }
            return rootlineString;
        }

        #endregion

        #region ITouchableRenderer

        /// <summary>
        /// Gets the Object at position x,y in the content.
        /// </summary>
        /// <param name="x">The x position in the content matrix.</param>
        /// <param name="y">The y position in the content matrix.</param>
        /// <returns>
        /// An object at the requester position in the content or <c>null</c>
        /// </returns>
        public object GetContentAtPosition(int x, int y)
        {
            List<RenderElement> elementsCopy = GetAllRenderElements();
            if (elementsCopy != null && elementsCopy.Count > 0)
            {
                foreach (RenderElement e in elementsCopy)
                {
                    if (e.ContainsPoint(x, y))
                    {
                        return e;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Get all Objects inside (or at least partial) the given area.
        /// </summary>
        /// <param name="left">Left border of the region to test (X).</param>
        /// <param name="right">Right border of the region to test (X + width).</param>
        /// <param name="top">Top border of the region to test (Y).</param>
        /// <param name="bottom">Bottom border of the region to test (Y + height).</param>
        /// <returns>
        /// A list of elements inside or at least partial inside the requested area.
        /// </returns>
        public System.Collections.IList GetAllContentInArea(int left, int right, int top, int bottom)
        {
            List<RenderElement> eL = new List<RenderElement>();
            List<RenderElement> elementsCopy = GetAllRenderElements();

            if (elementsCopy != null && elementsCopy.Count > 0)
            {
                foreach (RenderElement e in elementsCopy)
                {
                    if (e.IsInArea(left, right, top, bottom))
                    {
                        eL.Add(e);
                    }
                }
            }

            return eL;
        }

        #endregion

        #region Utils

        #region Rootline

        /// <summary>
        /// Gets the root line for the given dialog.
        /// </summary>
        /// <param name="dialog">The dialog.</param>
        /// <returns>The list of parent Dialogs in revers order; the parent is first, the grand parent is second a.s.o..</returns>
        public static List<Dialog> GetRootLineOfDialog(Dialog dialog)
        {
            List<Dialog> rootline = new List<Dialog>();

            if (dialog != null)
            {
                Dialog d = dialog;
                while (d != null)
                {
                    rootline.Add(d);
                    d = d.Parent;
                }
            }
            return rootline;
        }

        #endregion

        #region Space and Length Calculation

        /// <summary>
        /// Estimates the space an text needs when it will be turned into Braille matrix dots.
        /// </summary>
        /// <param name="text">The text.</param>
        /// /// <param name="addLastInterCharSpace">if set to <c>true</c> in the estimation the inter character space after the last space is added.</param>
        /// <returns>the width of the estimated dot matrix.</returns>
        private static int estimateTextSpaceConsumption(String text, bool addLastInterCharSpace = false)
        {
            return estimateTextSpaceConsumption(text.Length, addLastInterCharSpace);
        }

        /// <summary>
        /// Estimates the space an text needs when it will be turned into Braille matrix dots.
        /// </summary>
        /// <param name="textLength">Length of the text.</param>
        /// <param name="addLastInterCharSpace">if set to <c>true</c> in the estimation the inter character space after the last space is added.</param>
        /// <returns>
        /// the width of the estimated dot matrix.
        /// </returns>
        private static int estimateTextSpaceConsumption(int textLength, bool addLastInterCharSpace = false)
        {
            int result = textLength * MatrixBrailleRenderer.BRAILLE_CHAR_WIDTH
                + (textLength > 1 ?
                (textLength - 1) * MatrixBrailleRenderer.INTER_CHAR_WIDTH : 0);
            if (addLastInterCharSpace && result > 0) 
                result += MatrixBrailleRenderer.INTER_CHAR_WIDTH;
            return result;
        }

        /// <summary>
        /// Estimates the minimal space for a breadcrumb entry.
        /// Which is <see cref="MinimalBreadcrumbEntryCharCount"/> (3) chars of the name + the suppressor to shorten the name + the length of the separator.
        /// </summary>
        /// <returns>The minimal width per additional breadcrumb entry.</returns>
        private static int estimateMinimalSpaceForBreadcrumbEntry()
        {
            int wordLength = calculateInterLevelWordLength() + MinimalBreadcrumbEntryCharCount;
            return estimateTextSpaceConsumption(wordLength);
        }

        /// <summary>
        /// Estimates the amount of characters that fits into the given pin space.
        /// </summary>
        /// <param name="space">The space in pins.</param>
        /// <returns>the number of chars fitting into the space.</returns>
        private static int estimateCharsForPinSpace(int space)
        {
            return space / (MatrixBrailleRenderer.BRAILLE_CHAR_WIDTH + MatrixBrailleRenderer.INTER_CHAR_WIDTH);
        }

        /// <summary>
        /// Calculates the length of inter level characters.
        /// </summary>
        /// <returns>The sum of the <see cref="_separator"/> word length plus the <see cref="_suppressor"/> word length.</returns>
        private static int calculateInterLevelWordLength()
        {
            return Separator.Length + Suppressor.Length;
        }

        #endregion

        #endregion

    }
}
