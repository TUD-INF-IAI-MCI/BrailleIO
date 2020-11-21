//Autor: Stephanie Schöne, Jens Bornschein
// Technische Universität Dresden, Germany

using BrailleIO;
using BrailleIO.Interface;
using BrailleIO.Renderer;
using BrailleIO.Renderer.Structs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tud.mci.LanguageLocalization;

namespace BrailleIO.Dialogs.Rendering
{
    /// <summary>
    /// A BarilleIO render for transforming dialog-kind structures into a tactile representation.
    /// </summary>
    /// <seealso cref="BrailleIO.Interface.BrailleIOHookableRendererBase" />
    /// <seealso cref="BrailleIO.Interface.IBrailleIOContentRenderer" />
    /// <seealso cref="BrailleIO.Renderer.ITouchableRenderer" />
    public class DialogRenderer : BrailleIOHookableRendererBase, IBrailleIOContentRenderer, ITouchableRenderer
    {

        #region Members

        #region private

        private static readonly LL ll = new LL(BrailleIO.Dialogs.Properties.Resources.Language);
        /// <summary>
        /// The last view used for rendering.
        /// </summary>
        private IViewBoxModel _lastView = null;

        #endregion

        #region Public

        static int _groupIndentation = MatrixBrailleRenderer.BRAILLE_CHAR_WIDTH * 2 + MatrixBrailleRenderer.INTER_CHAR_WIDTH;
        /// <summary>
        /// Gets or sets the group indentation space.
        /// Its the horizontal displacement for child levels.
        /// </summary>
        /// <value>
        /// The group indentation space.
        /// </value>
        static public int GroupIndentation
        {
            get { return _groupIndentation; }
            set { _groupIndentation = Math.Max(0, value); }
        }

        /// <summary>
        /// The locking object for synchronous usage.
        /// </summary>
        public readonly Object SynchLock = new Object();

        private static MatrixBrailleRenderer _brailleRenderer = new MatrixBrailleRenderer(RenderingProperties.IGNORE_LAST_LINESPACE | RenderingProperties.RETURN_REAL_WIDTH);

        private static EditFieldRenderer _editFieldRenderer = new EditFieldRenderer();
        /// <summary>
        /// Gets or sets the braille renderer.
        /// </summary>
        /// <value>
        /// The braille renderer.
        /// </value>
        public static MatrixBrailleRenderer BrailleRenderer
        {
            get { return DialogRenderer._brailleRenderer; }
            set { DialogRenderer._brailleRenderer = value; }
        }

        IBrailleIOContentRenderer _helpTextRenderer = null;
        /// <summary>
        /// Gets or sets the renderer for the help texts.
        /// </summary>
        /// <value>
        /// The help text renderer.
        /// </value>
        public IBrailleIOContentRenderer HelpTextRenderer
        {
            get
            {
                if (_helpTextRenderer == null) _helpTextRenderer = new HelpTextRenderer(HelpRenderingProperties.ShowHaeder, BrailleRenderer);
                return _helpTextRenderer;
            }
            set { _helpTextRenderer = value; }
        }

        IBrailleIOContentRenderer _titleRenderer = null;
        /// <summary>
        /// Gets or sets the renderer for the help texts.
        /// </summary>
        /// <value>
        /// The help text renderer.
        /// </value>
        public IBrailleIOContentRenderer TitleRenderer
        {
            get
            {
                if (_titleRenderer == null) _titleRenderer = new BreadcrumbRenderer(BrailleRenderer);
                return _titleRenderer;
            }
            set { _titleRenderer = value; }
        }

        private DialogRenderingProperties _properties = DialogRenderingProperties.Normal;
        /// <summary>
        /// Gets or sets the properties controlling the rendering.
        /// </summary>
        /// <value>
        /// The properties.
        /// </value>
        public DialogRenderingProperties Properties
        {
            get { return _properties; }
            set
            {
                if (_properties != value)
                    _properties = value;
            }
        }

        List<RenderElement> _renderedElements = new List<RenderElement>();
        readonly object _renderedElementLock = new object();
        /// <summary>
        /// The listing of all rendered elements and their position.
        /// For touch interaction and other stuff.
        /// </summary>
        protected List<RenderElement> RenderedElements
        {
            get
            {
                lock (_renderedElementLock)
                {
                    if (_renderedElements == null) _renderedElements = new List<RenderElement>();
                    return _renderedElements;
                }
            }
            set
            {
                lock (_renderedElementLock)
                {
                    _renderedElements = value;
                }
            }
        }

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogRenderer"/> class.
        /// </summary>
        /// <param name="properties">The properties.</param>
        public DialogRenderer(DialogRenderingProperties properties = DialogRenderingProperties.ShowHeader)
        {
            Properties = properties;
        }

        #endregion

        #region ############## IBrailleIOContentRenderer ################

        /// <summary>
        /// The empty matrix constant to have a valid variable initialization instead of <c>null</c>.
        /// </summary>
        protected static readonly bool[,] emptyMatrix = new bool[0, 0];

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
            bool[,] m = emptyMatrix;

            if (content != null && content is Dialog && view != null)
            {
                Dialog activeDialog = ((Dialog)content).GetActiveDialog();
                if (activeDialog != null)
                {
                    List<RenderedDialogEntry> renderedEntries = new List<RenderedDialogEntry>();

                    if (Properties.HasFlag(DialogRenderingProperties.HideEntries))
                    {
                        //////// BREADCRUMB //////
                        if (Properties.HasFlag(DialogRenderingProperties.ShowHeader))
                        {
                            renderBreadcrumbMenu(view, activeDialog, ref renderedEntries);
                        }
                    }
                    else
                    {
                        /////////// HELP ////////////
                        if (activeDialog.HelpIsShown) { m = renderHelp(view, activeDialog); }
                        ////////// NORMAL //////////
                        else
                        {
                            //////// BREADCRUMB //////
                            if (Properties.HasFlag(DialogRenderingProperties.ShowHeader))
                            {
                                renderBreadcrumbMenu(view, activeDialog, ref renderedEntries);
                            }
                            //////// ENTRIES //////
                            renderDialogEntries(view, activeDialog, ref renderedEntries);


                        }
                    }

                    // build them together
                    if (renderedEntries != null && renderedEntries.Count > 0)
                    {
                        m = combineEntries(renderedEntries);
                    }

                    view.ContentHeight = m.GetLength(0);
                    view.ContentWidth = m.GetLength(1);

                    // check if scrollbars are needed
                    if (view.ContentHeight > view.ContentBox.Height)
                        view.ContentWidth -= 3; // cut of the overlapping line elements. Text content is rendered with respect of scrollbar space

                }
            }
            _lastView = view;
            return m;
        }

        #region Subpart Rendering

        /// <summary>
        /// Renders the dialog entries and add the results into the global list of 
        /// rendered entries for a combination afterwards.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="activeDialog">The active dialog to render.</param>
        /// <param name="renderedEntries">The rendered entries for touch interaction where the 
        /// rendering results where added to.</param>
        /// <param name="horizontalOffset">The horizontal offset (spacing/indentation)
        /// from the left for some kind of indentation.</param>
        /// <returns>
        ///   <c>true</c> if the entries were rendered successfully.
        /// </returns>
        protected virtual bool renderDialogEntries(IViewBoxModel view, Dialog activeDialog, ref List<RenderedDialogEntry> renderedEntries, int horizontalOffset = 0)
        {
            // var entries2render = activeDialog.GetEntryList().ToList<IDialogComponent>();

            // get only top-level entries
            List<IDialogComponent> entries2render = activeDialog.GetEntryList().FindAll(
                (entry) => { return entry != null && !entry.IsInGroup(); }).ToList<IDialogComponent>();

            using (DialogEntryRenderer der = new DialogEntryRenderer())
            {
                renderEntries(view, entries2render, ref renderedEntries, horizontalOffset, der);
            }
            return true;
        }

        /// <summary>
        /// Renders the given entry list into the global list of rendered elements to be 
        /// combined afterwards.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="entries2render">The entries to render.</param>
        /// <param name="renderedEntries">The rendered entries to add the rendering results to.</param>
        /// <param name="horizontalOffset">The horizontal offset to allow some indentation.</param>
        /// <param name="der">A renderer for entries, if they do not have their own..</param>
        private void renderEntries(IViewBoxModel view, List<IDialogComponent> entries2render, ref List<RenderedDialogEntry> renderedEntries, int horizontalOffset, DialogEntryRenderer der)
        {
            IViewBoxModel view4rendering = view;
            if (horizontalOffset != 0)
            {
                view4rendering = new DummyViewBox(view);
                // remove 3 pins on the right for scroll bars
                var cBox = view4rendering.ContentBox;
                cBox.Width -= 3;
                view4rendering.ContentBox = cBox;
                view4rendering.ContentBox = new Rectangle(view4rendering.ContentBox.X + horizontalOffset,
                    view4rendering.ContentBox.Y, view4rendering.ContentBox.Width - horizontalOffset, view4rendering.ContentBox.Height);
            }

            foreach (var item in entries2render)
            {
                if (item is DialogEntry)
                {
                    if (item is SelfRenderingDialogEntry)
                    {

                            renderedEntries.Add(
                            new RenderedDialogEntry(
                                item as DialogEntry,
                                ((SelfRenderingDialogEntry)item).Renderer.RenderMatrix(view4rendering, item)) { HorizontalOffset = horizontalOffset });

                        
                    }
                    else
                    {
                        renderedEntries.Add(new RenderedDialogEntry(item as DialogEntry, der.RenderMatrix(view4rendering, item)) { HorizontalOffset = horizontalOffset });
                    }


                    if (((DialogEntry)item).Status.HasFlag(DialogEntryStatus.Selected))   // selection marking
                        renderedEntries.Add(getSolidDividerEntry(view.ContentBox.Width));
                    else renderedEntries.Add(onePinSpacerEntry);                     // spacing

                    // Child handling
                    if (((DialogEntry)item).Type == DialogEntryType.Group)
                    {
                        renderedEntries.Add(onePinSpacerEntry);                        // spacing
                        if (!Properties.HasFlag(DialogRenderingProperties.HideDividers))
                        {
                            renderedEntries.Add(getDividerEntry(view.ContentBox.Width));   // separation
                            renderedEntries.Add(onePinSpacerEntry);                        // spacing
                        }

                        var children = ((DialogEntry)item).GetChildEntryList();
                        if (children != null && children.Count > 0)
                        {
                            // call Dialog entry rendering recursive with increased indentation
                            renderEntries(view, children, ref renderedEntries,
                                horizontalOffset + GroupIndentation, der);
                        }
                    }

                    if (!Properties.HasFlag(DialogRenderingProperties.HideDividers))
                    {
                        renderedEntries.Add(getDividerEntry(view.ContentBox.Width));   // separation
                        renderedEntries.Add(onePinSpacerEntry);                        // spacing 
                    }
                }
            }
        }


        /// <summary>
        /// Renders the breadcrumb menu.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="activeDialog">The active dialog.</param>
        /// <param name="renderedEntries">The rendered entries list for touch interaction.</param>
        protected virtual bool renderBreadcrumbMenu(IViewBoxModel view, Dialog activeDialog, ref List<RenderedDialogEntry> renderedEntries)
        {
            if (TitleRenderer != null)
            {
                var tm = TitleRenderer.RenderMatrix(view, activeDialog);
                if (tm != null && !tm.Equals(emptyMatrix))
                {
                    IList<RenderElement> rE = TitleRenderer is BreadcrumbRenderer ?
                        ((BreadcrumbRenderer)TitleRenderer).GetAllRenderElements() :
                        TitleRenderer is ITouchableRenderer ?
                            ((ITouchableRenderer)TitleRenderer).GetAllContentInArea(0, tm.GetLength(1), 0, tm.GetLength(0)) as IList<RenderElement> :
                            null;

                    renderedEntries.Add(new RenderedDialogEntry(tm, rE));

                    renderedEntries.Add(onePinSpacerEntry);                             // spacing
                    renderedEntries.Add(getSolidDividerEntry(view.ContentBox.Width));   // separation
                    // renderedEntries.Add(getSolidDividerEntry(view.ContentBox.Width));   // separation
                    renderedEntries.Add(onePinSpacerEntry);                             // spacing

                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Renders the help text into the matrix.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="activeDialog">The active dialog.</param>
        /// <returns>A matrix containing the bool values for the rendered help text.</returns>
        protected virtual bool[,] renderHelp(IViewBoxModel view, Dialog activeDialog)
        {
            bool[,] m = emptyMatrix;
            var selected = activeDialog.GetSelectedEntry();
            if (selected != null && HelpTextRenderer != null)
            {
                m = HelpTextRenderer.RenderMatrix(view, selected);
                RenderedElements.Clear();
                RenderedElements.Add(
                     new RenderElement(
                        0, 0,
                        m.GetLength(1), m.GetLength(0),
                        selected.Help));
            }
            return m;
        }

        /// <summary>
        /// Combines the entries into one single matrix.
        /// </summary>
        /// <param name="renderedEntries">The rendered entries.</param>
        /// <returns></returns>
        protected virtual bool[,] combineEntries(List<RenderedDialogEntry> renderedEntries)
        {
            // clear the list of rendered elements;
            RenderedElements.Clear();

            bool[,] m = emptyMatrix;
            if (renderedEntries != null && renderedEntries.Count > 0)
            {
                Size s = calculateMatrixDimension(renderedEntries);
                if (s.Width * s.Height > 0)
                {
                    m = new bool[s.Height, s.Width];
                    int verticalOffset = 0;

                    foreach (var entry in renderedEntries)
                    {
                        if (entry.M != null && entry.M.GetLength(0) * entry.M.GetLength(1) > 0)
                        {
                            m = InsertIntoMatrix(m, entry.M, verticalOffset, entry.HorizontalOffset);

                            if (entry.Entry != null || entry.RenderedElements != null)
                                insertIntoRenderedElements(entry, verticalOffset, entry.HorizontalOffset, s.Width);

                            verticalOffset += entry.M.GetLength(0);
                        }
                    }
                }
            }
            return m;
        }

        /// <summary>
        /// Inserts the rendered dialog elements in the list of rendered elements.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="verticalOffset">The vertical offset where the elements starts.</param>
        /// <param name="width">The minimum width of this element.</param>
        protected virtual void insertIntoRenderedElements(RenderedDialogEntry entry, int verticalOffset, int horizontalOffset = 0, int minWidth = 0)
        {
            if (entry.M != null && (entry.M.GetLength(0) * entry.M.GetLength(1) > 0))
            {
                if (entry.Entry != null)
                {
                    RenderElement renderElement = new RenderElement(
                            horizontalOffset, verticalOffset,
                            Math.Max(minWidth, entry.M.GetLength(1)), entry.M.GetLength(0) + 1, // add one pin space underneath for the selection marking
                            entry.Entry);

                    //DISPLAY NAME FOR SINGLE LINE ITEMS
                    if (entry.Entry.Type == DialogEntryType.EditField)
                    {
                        
                        EditField_DialogEntry editField = (EditField_DialogEntry) entry.Entry;

                        if (editField != null && (editField.InputBox.IsMinimized || editField.InputBox.BoxHeightType == BoxHeightTypes.SingleLine || editField.InputBox.MinimizeType == MinimizeTypes.AlwaysMinimize))
                        {
                            renderElement.DisplayName = ((EditFieldRenderer)editField.Renderer).GetLastTitleSegment();
                        }
                        else
                        {
                            renderElement.DisplayName = editField.Title;
                        }


                        if (editField.HasLabel) renderElement.DisplayName = editField.Label + " " + renderElement.DisplayName;

                    }
                    RenderedElements.Add(renderElement);
                }
                else if (entry.RenderedElements != null && entry.RenderedElements.Count > 0)
                {
                    foreach (var item in entry.RenderedElements)
                    {
                        RenderedElements.Add(item);
                    }
                }
            }
        }

        /// <summary>
        /// Calculates the matrix dimension needed to place all the lines into one matrix.
        /// </summary>
        /// <param name="renderedEntries">The rendered entries.</param>
        /// <returns>The needed size to place all entries underneath each other.</returns>
        protected virtual Size calculateMatrixDimension(List<RenderedDialogEntry> renderedEntries)
        {
            Size s = new Size();

            if (renderedEntries != null && renderedEntries.Count > 0)
            {
                foreach (var r in renderedEntries)
                {
                    if (r.M != null)
                    {
                        if (r.M.GetLength(0) * r.M.GetLength(1) > 0)
                        {
                            s.Height += r.M.GetLength(0);
                            s.Width = Math.Max(s.Width, r.M.GetLength(1));
                        }
                    }
                }
            }
            return s;
        }

        #endregion

        #region Helper Objects

        #region Solid Line

        bool[,] _solidDivider = emptyMatrix;
        /// <summary>
        /// Gets a dividing solid line of raced pins.
        /// </summary>
        /// <param name="width">The width of the divider.</param>
        /// <returns>a 1 x width matrix</returns>
        protected bool[,] getSolidDivider(int width)
        {
            if (_solidDivider.GetLength(1) == width) return _solidDivider;
            if (width > 0)
            {
                _solidDivider = new bool[1, width];
                for (int i = 0; i < width; i++) { _solidDivider[0, i] = true; }
            }
            else return emptyMatrix;
            return _solidDivider;
        }

        RenderedDialogEntry _solidDividerEntry = new RenderedDialogEntry(null, emptyMatrix);
        /// <summary>
        /// Gets the divider entry.
        /// A dummy entry that contains the solid divider matrix.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <returns></returns>
        protected RenderedDialogEntry getSolidDividerEntry(int width)
        {
            if (_solidDividerEntry.M != null && width == _solidDividerEntry.M.GetLength(1)) return _solidDividerEntry;
            else { _solidDividerEntry.M = getSolidDivider(width); }
            return _solidDividerEntry;
        }

        #endregion

        #region Divider Line

        bool[,] _divider = emptyMatrix;
        /// <summary>
        /// Gets a dividing solid line of raced pins.
        /// </summary>
        /// <param name="width">The width of the divider.</param>
        /// <returns>a 1 x width matrix</returns>
        protected virtual bool[,] getDivider(int width)
        {
            if (_divider.GetLength(1) == width) return _divider;
            if (width > 0)
            {
                _divider = new bool[1, width];
                for (int i = 0; i < width; i += 2)
                {
                    if (i < width) _divider[0, i] = true;
                }
            }
            else return emptyMatrix;
            return _divider;
        }
        RenderedDialogEntry _dividEntry = new RenderedDialogEntry(null, emptyMatrix);
        /// <summary>
        /// Gets the divider entry.
        /// A dummy entry that contains the divider matrix.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <returns></returns>
        RenderedDialogEntry getDividerEntry(int width)
        {
            if (_dividEntry.M != null && width == _dividEntry.M.GetLength(1)) return _dividEntry;
            else { _dividEntry.M = getDivider(width); }
            return _dividEntry;
        }

        #endregion

        #region One Pin Spacer

        protected static readonly bool[,] onePinSpacer = new bool[1, 1];
        /// <summary>
        /// Dummy entry containing the one pin spacer matrix
        /// </summary>
        protected static readonly RenderedDialogEntry onePinSpacerEntry = new RenderedDialogEntry(null, onePinSpacer);

        #endregion

        // TODO: unused yet !? Remove?
        #region Height Estimation

        int heightPerEntry = 5; // pins per one-line entry
        int interEntrySpace = 1; // space needed between two entries
        int headerHeight = 7; // height of the dialog header (bread-crumb)

        /// <summary>
        /// Estimates the height of the content based on the given amount of entries.
        /// Here only one line per entry is estimated.
        /// </summary>
        /// <param name="entryCount">The entry count.</param>
        /// <returns>The estimated count of pin rows needed to display the content.</returns>
        /// <remarks>Empty entries, multi-line texts or large icons can increase this.</remarks>
        int estimateContentHeight(int entryCount)
        {
            return headerHeight + (entryCount * heightPerEntry) * (Math.Max(entryCount - 1, 0) * interEntrySpace);
        }

        /// <summary>
        ///  estimate if scrollbars are needed or not
        /// </summary>
        /// <param name="view">The view to show the content in.</param>
        /// <param name="dialog">The dialog to display.</param>
        /// <returns><c>true</c> if scrollbars are needed; otherwise <c>false</c>.</returns>
        bool needScrollbars(IViewBoxModel view, Dialog dialog)
        {
            if (view != null && view.ContentBox.Height > 0 && dialog != null)
            {
                if (view is IPannable && !((IPannable)view).ShowScrollbars) return false;

                return view.ContentBox.Height < estimateContentHeight(dialog.EntryCount);
            }
            return false;
        }


        #endregion

        #endregion

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
        public virtual object GetContentAtPosition(int x, int y)
        {
            if (x > 0 && y > 0)
            {
                foreach (RenderElement item in RenderedElements)
                {
                    if (item.ContainsPoint(x, y))
                    {
                        return item.GetValue();
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
        public virtual System.Collections.IList GetAllContentInArea(int left, int right, int top, int bottom)
        {

            List<RenderElement> elements = new List<RenderElement>();

            if (left >= 0 && right > left && top >= 0 && bottom > top)
            {
                if (RenderedElements != null)
                {
                    foreach (RenderElement item in RenderedElements.ToArray())
                    {
                        if (item.IsInArea(left, right, top, bottom))
                        {
                            elements.Add(item);
                        }
                    }
                }
            }

            return elements;
        }

        #endregion

        #region Further Rendered Elements Handling

        /// <summary>
        /// Gets the position within the rendered content matrix of a certain entry.
        /// </summary>
        /// <param name="entry">The entry to look for its position.</param>
        /// <returns>The bounding box, including position, of the searched entry or an empty bounding box.</returns>
        public virtual System.Drawing.Rectangle GetEntryPositionWithinContent(DialogEntry entry)
        {
            System.Drawing.Rectangle bb = new System.Drawing.Rectangle();
            if (entry != null)
            {
                var rendObj = GetRenderedElementOfEntry(entry);
                if (!rendObj.IsEmpty())
                {
                    bb = new Rectangle(rendObj.X, rendObj.Y, rendObj.Width, rendObj.Height);
                }
            }
            return bb;
        }

        /// <summary>
        /// Gets the rendered element object of a specific entry.
        /// </summary>
        /// <param name="entry">The entry to search for its rendered counterpart.</param>
        /// <returns>The rendered</returns>
        public virtual RenderElement GetRenderedElementOfEntry(DialogEntry entry)
        {
            lock (_renderedElementLock)
            {
                if (entry != null && RenderedElements != null && RenderedElements.Count > 0)
                {
                    foreach (RenderElement item in RenderedElements)
                    {
                        object value = item.GetValue();
                        DialogEntry valueEntry = value as DialogEntry;

                        if (valueEntry != null && valueEntry == entry)
                        {
                            return item;
                        }
                    }
                }
            }
            return new RenderElement();
        }

        /// <summary>
        /// Determines whether the entry is visible or not.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="view">The view to check for (BrailleIoViewRange).</param>
        /// <returns>
        /// -1 if not visible; 0 if party visible, and 1 if fully visible.
        /// </returns>
        public virtual int IsEntryVisible(DialogEntry entry, IViewBoxModel view = null)
        {
            return IsEntryVisible(GetRenderedElementOfEntry(entry), view);
        }

        /// <summary>
        /// Determines whether the entry is visible or not.
        /// </summary>
        /// <param name="renderedEntry">The rendered entry.</param>
        /// <param name="view">The view to check for (BrailleIoViewRange).</param>
        /// <returns>
        /// -1 if not visible; 0 if party visible, and 1 if fully visible.
        /// </returns>
        public virtual int IsEntryVisible(RenderElement renderedEntry, IViewBoxModel view = null)
        {
            if (!renderedEntry.IsEmpty())
            {
                if (view == null) view = _lastView;
                if (view != null && view.ContentBox.Width * view.ContentBox.Height > 0)
                {
                    int left, top, right, bottom;
                    left = top = 0;
                    if (view is IPannable)
                    {
                        left = ((IPannable)view).GetXOffset();
                        top = ((IPannable)view).GetYOffset();
                    }
                    right = view.ContentBox.Width + left;
                    bottom = view.ContentBox.Height + top;

                    if (renderedEntry.IsInArea(left, left + right, top, top + bottom))
                    {
                        if (renderedEntry.IsCompletelyInArea(left, right, top, bottom))
                            return 1;
                        return 0;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// Makes the entry visible in the given view.
        /// If an entry is outside the visible area and the view is <see cref="BrailleIO.Interface.IPannable" />,
        /// the offsets are adapted to make the entry visible.
        /// </summary>
        /// <param name="entry">The entry to make visible.</param>
        /// <param name="view">The view.</param>
        /// <param name="vertical">if set to <c>true</c> adapt view in vertical direction.</param>
        /// <param name="horizontal">if set to <c>true</c> adapt view in horizontal direction.</param>
        /// <returns>
        ///   <c>true</c> if the entry would be visible; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// After a returning <c>true</c> you have to force an rerendering.
        /// </remarks>
        public virtual bool MakeEntryVisible(DialogEntry entry, IViewBoxModel view = null,
            bool vertical = true, bool horizontal = false)
        {
            return MakeEntryVisible(GetRenderedElementOfEntry(entry), view);
        }

        /// <summary>
        /// Makes the entry visible in the given view.
        /// If an entry is outside the visible area and the view is <see cref="BrailleIO.Interface.IPannable" />,
        /// the offsets are adapted to make the entry visible.
        /// </summary>
        /// <param name="renderedEntry">The rendered entry.</param>
        /// <param name="view">The view.</param>
        /// <param name="vertical">if set to <c>true</c> adapt view in vertical direction.</param>
        /// <param name="horizontal">if set to <c>true</c> adapt view in horizontal direction.</param>
        /// <returns>
        ///   <c>true</c> if the entry would be visible; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// After a returning <c>true</c> you have to force an rerendering.
        /// </remarks>
        public virtual bool MakeEntryVisible(RenderElement renderedEntry, IViewBoxModel view = null,
            bool vertical = true, bool horizontal = false)
        {
            bool success = false;
            if (!renderedEntry.IsEmpty() && (vertical || horizontal))
            {
                if (view == null) view = _lastView;
                if (view != null && view.ContentBox.Width * view.ContentBox.Height > 0)
                {
                    int visibility = IsEntryVisible(renderedEntry, view);
                    if (visibility > 0) { return true; }
                    else
                    {
                        if (view is IPannable)
                        {
                            // entry position
                            Rectangle bbox = new Rectangle(
                                renderedEntry.X, renderedEntry.Y,
                                renderedEntry.Width, renderedEntry.Height);

                            // visible view port 
                            Rectangle viewPort = new Rectangle(
                                -((IPannable)view).GetXOffset(),
                                -((IPannable)view).GetYOffset(),
                                view.ContentBox.Width,
                                view.ContentBox.Height);

                            // check if item is selected --> add one more line for marking
                            var entry = renderedEntry.GetValue() as DialogEntry;
                            if (entry != null && entry.Status.HasFlag(DialogEntryStatus.Selected))
                                bbox.Height += 1;

                            // check for scrolling directions

                            /************** vertical ***********/
                            if (vertical)
                            {
                                if (bbox.Top < viewPort.Top) //entry starts over the view port // move upwards
                                {
                                    viewPort.Y = bbox.Y;
                                }
                                else if (bbox.Bottom > viewPort.Bottom) // entry ends after the  view port // move downwards?
                                {
                                    int newY = viewPort.Top + (bbox.Bottom - viewPort.Bottom);
                                    viewPort.Y = Math.Min(bbox.Top, newY);
                                }

                                ((IPannable)view).SetYOffset(-viewPort.Y);
                                success = ((IPannable)view).GetYOffset() == -viewPort.Y;
                            }

                            /************ horizontal ***********/
                            if (horizontal)
                            {
                                if (bbox.Left < viewPort.Left) // move right
                                {
                                    viewPort.X = bbox.X;
                                }
                                else if (bbox.Right > viewPort.Right) // move left?
                                {
                                    int newX = viewPort.Left + (bbox.Right - viewPort.Right);
                                    viewPort.X = Math.Min(bbox.Left, newX);
                                }

                                ((IPannable)view).SetXOffset(-viewPort.X);
                                success = true;
                            }
                        }
                    }
                }
            }
            return success;
        }

        #endregion

        #region Static Utils

        /// <summary>
        /// Inserts one matrix into the other.
        /// Values of the base matrix will be overwritten by the matrix to insert.
        /// </summary>
        /// <param name="m">The base matrix the other is inserted in.</param>
        /// <param name="n">The matrix to insert.</param>
        /// <param name="verticalOffset">The vertical offset (starting row ).</param>
        /// <param name="horizontalOffset">The horizontal offset (starting column).</param>
        /// <returns>The matrix m with the inserted matrix n.</returns>
        public static bool[,] InsertIntoMatrix(bool[,] m, bool[,] n, int verticalOffset, int horizontalOffset)
        {
            if (m != null && m.GetLength(0) > verticalOffset && m.GetLength(1) > horizontalOffset)
            {
                int rows = m.GetLength(0);
                int cols = m.GetLength(1);
                Parallel.For(0, n.GetLength(0), (a) =>
                {     // for each row of the matrix to enter

                    int i = (int)a + verticalOffset;
                    if (i < rows)
                        for (int b = 0; b < n.GetLength(1); b++) // for each column of the selected column
                        {
                            int j = b + horizontalOffset;
                            if (j < cols) m[i, j] = n[a, b]; else break;
                        }
                });
            }
            return m;
        }

        #endregion

        #region Structs

        /// <summary>
        /// Struct for combining a rendering result with its related content element.
        /// </summary>
        protected struct RenderedDialogEntry
        {
            int _horizontalOffset;
            /// <summary>
            /// Gets or sets the vertical offset for the 
            /// matrix to be placed into the final matrix.
            /// </summary>
            /// <value>
            /// The vertical offset.
            /// </value>
            public int HorizontalOffset
            {
                get { return _horizontalOffset; }
                set { _horizontalOffset = value; }
            }

            bool[,] _m;
            /// <summary>
            /// Gets or sets the resulting  bool matrix of the entry.
            /// </summary>
            /// <value>
            /// The m.
            /// </value>
            public bool[,] M
            {
                get { return _m; }
                set { _m = value; }
            }

            private List<RenderElement> _renderedElements;
            /// <summary>
            /// Gets or sets the rendered elements if any prerenderd stuff is added.
            /// </summary>
            /// <value>
            /// The rendered elements.
            /// </value>
            public List<RenderElement> RenderedElements
            {
                get { return _renderedElements; }
                set { _renderedElements = value; }
            }

            DialogEntry _entry;
            /// <summary>
            /// Gets or sets the entry that was rendered.
            /// </summary>
            /// <value>
            /// The entry.
            /// </value>
            public DialogEntry Entry
            {
                get { return _entry; }
                set { _entry = value; }
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="RenderedDialogEntry"/> struct.
            /// </summary>
            /// <param name="entry">The dialog entry that was rendered.</param>
            /// <param name="m">The rendered matrix.</param>
            public RenderedDialogEntry(DialogEntry entry, bool[,] m)
            {
                _entry = entry;
                _m = m;
                _renderedElements = null;
                _horizontalOffset = 0;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="RenderedDialogEntry"/> struct.
            /// </summary>
            /// <param name="m">The rendered matrix.</param>
            /// <param name="renderedElements">The rendered elements this matrix is build of.</param>
            public RenderedDialogEntry(bool[,] m, IList<RenderElement> renderedElements)
            {
                _entry = null;
                _m = m;
                _renderedElements = new List<RenderElement>(renderedElements);
                _horizontalOffset = 0;
            }
        }

        #endregion

        #region Enums

        /// <summary>
        /// Properties defining 
        /// </summary>
        [Flags]
        public enum DialogRenderingProperties : uint
        {
            /// <summary>
            /// Normal rendering: purely display the dialog entries.
            /// </summary>
            Normal = 0,
            /// <summary>
            /// Displays an additional header breadcrumb
            /// </summary>
            ShowHeader = 1,
            /// <summary>
            /// Does not display any dialog entries.
            /// This can be used to render the header only.
            /// </summary>
            HideEntries = 2,
            /// <summary>
            /// If this flag is set no dividers between the 
            /// entries were rendered. 
            /// </summary>
            HideDividers = 4
        }

        #endregion

    }
}