using BrailleIO.Dialogs;
using BrailleIO.Interface;
using BrailleIO.Renderer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using tud.mci.LanguageLocalization;

namespace BrailleIO.Dialogs.Rendering
{
    /// <summary>
    /// Renderer for transforming a special <see cref="DialogEntry"/> into a bool matrix. 
    /// </summary>
    /// <seealso cref="BrailleIO.Interface.IBrailleIOContentRenderer" />
    /// <seealso cref="System.IDisposable" />
    public class DialogEntryRenderer : IBrailleIOContentRenderer, IDisposable
    {

        #region Members

        #region public

        static IBrailleIOContentRenderer _brailleRenderer = DialogRenderer.BrailleRenderer;
        /// <summary>
        /// Gets or sets the braille renderer for converting strings to a bool matrix.
        /// </summary>
        /// <value>
        /// The braille renderer.
        /// </value>
        public IBrailleIOContentRenderer BrailleRenderer
        {
            get { return _brailleRenderer; }
            set
            {
                _brailleRenderer = value;
                HasChanged = true;
            }
        }

        IBrailleIOContentRenderer _iconRenderer = TactonRenderer.Instance;
        /// <summary>
        /// Gets or sets the icon renderer for creating an icon.
        /// </summary>
        /// <value>
        /// The icon renderer.
        /// </value>
        public IBrailleIOContentRenderer IconRenderer
        {
            get { return _iconRenderer; }
            set
            {
                _iconRenderer = value;
                HasChanged = true;
            }
        }

        int _iconTextSpace = 2;
        /// <summary>
        /// Gets or sets the space between an icon and the related text.
        /// </summary>
        /// <value>
        /// The icon to text space.
        /// </value>
        public int IconTextSpace
        {
            get { return _iconTextSpace; }
            set
            {
                _iconTextSpace = value;
                HasChanged = true;
            }
        }

        DialogEntry _entry;
        /// <summary>
        /// Gets or sets the entry to render.
        /// </summary>
        /// <value>
        /// The entry to render.
        /// </value>
        public DialogEntry Entry
        {
            get { return _entry; }
            set
            {
                if (value == _entry) return;
                unregisterFromEvents(_entry);
                _entry = value;
                registerToEvents(_entry);
                HasChanged = true;
            }
        }

        bool _hasChanged = true;
        /// <summary>
        /// Flag determining if the content or other properties had changed. 
        /// </summary>
        public bool HasChanged
        {
            get
            {
                lock (_synckLock)
                {
                    return _hasChanged;
                }
            }
            set
            {
                lock (_synckLock)
                {
                    _hasChanged = value;
                }
            }
        }      


        #endregion

        #region private

        readonly Object _synckLock = new Object();
        static readonly LL ll = new LL(Properties.Resources.Language);

        IViewBoxModel lastView;

        #endregion

        #endregion

        #region IBrailleIOContentRenderer
        /// <summary>
        /// The empty matrix placeholder and default initialization. (0 x 0)
        /// </summary>
        protected static bool[,] emptyMatrix = new bool[0, 0];

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
        /// <remarks>Ignores the set content if this render has its own DialogEntry</remarks>
        public virtual bool[,] RenderMatrix(IViewBoxModel view, object content)
        {
            bool[,] m = emptyMatrix;
            if (view != null)
            {
                if (content is DialogEntry)
                {
                    Entry = content as DialogEntry;
                }

                if (lastView == null || lastView.ContentBox.Width != view.ContentBox.Width)
                {
                    HasChanged = true;
                }

                m = renderDialogEntry(view);
                lastView = view;
            }
            return m;
        }

        #endregion

        #region Entry Event Handling

        void unregisterFromEvents(DialogEntry entry)
        {
            if (entry != null) { try { entry.DialogEntryChanged -= entry_DialogEntryChanged; } catch { } }
        }

        private void registerToEvents(DialogEntry entry)
        {
            if (entry != null) { entry.DialogEntryChanged += entry_DialogEntryChanged; }
        }

        private void entry_DialogEntryChanged(object sender, EntryEventArgs e)
        {
            if (e.Entry == this.Entry)
            {
                if (e is DetailedEntryChangedEventArgs)
                {
                    // TODO: check for different types ...
                    HasChanged = true;
                }
                else
                {
                    HasChanged = true;
                }

                if (HasChanged) buildCacheMatrix();
            }
            else // ERROR: ignore 
            {
                unregisterFromEvents(sender as DialogEntry);
            }
        }

        #endregion

        #region ########### RENDERING #############

        protected void buildCacheMatrix()
        {
            if(!_isRendering && lastView != null){
                Task t = new Task(new Action(()=>renderDialogEntry(lastView)));
            }
        }

        bool[,] cachedResult = emptyMatrix;
        bool _isRendering = false;

        protected virtual bool[,] renderDialogEntry(IViewBoxModel view)
        {
            int i = 0;
            while (_isRendering && i++ < 10) { Thread.Sleep(5); }
            if (i >= 10) return cachedResult;

            try
            {
                bool[,] m = cachedResult;
                lock (_synckLock)
                {
                    _isRendering = true;
                    if (HasChanged && Entry != null && view != null && view.ContentBox.Width > 0) // renew rendering result
                    {

                        bool[,] icon = emptyMatrix;
                        // get icon
                        if (IconRenderer != null)
                        {
                            icon = IconRenderer.RenderMatrix(view, Entry);
                        }

                        // calculate the space for the text to start
                        int padding_left = (icon != null && icon.GetLength(1) > 0) ? icon.GetLength(1) + IconTextSpace : 0;
                        // adapt the view 
                        IViewBoxModel view2 = new DummyViewBox(view);
                        if (padding_left > 0)
                        {
                            var contentBox = view2.ContentBox;
                            contentBox.Width = contentBox.Width - padding_left;
                            view2.ContentBox = contentBox;
                        }

                        bool[,] text = emptyMatrix;

                        if (BrailleRenderer != null)
                        {
                            string title = Entry.Title;
                            if (Entry.Type == DialogEntryType.Group) title = ll.GetTrans("grp.title.wrap", title);
                            text = BrailleRenderer.RenderMatrix(view2, title);
                        }

                        m = combineMatrices(icon, text, view);

                        // TODO: set content dimensions?
                    }
                    cachedResult = m;
                    HasChanged = false;
                }
                return m;
            }
            finally
            {
                _isRendering = false;
            }
        }

        /// <summary>
        /// Combines the icon and the text matrix
        /// </summary>
        /// <param name="icon">The icon.</param>
        /// <param name="text">The text.</param>
        /// <param name="view">The view.</param>
        /// <returns>One matrix containing both; icon and text.</returns>
        protected virtual bool[,] combineMatrices(bool[,] icon, bool[,] text, IViewBoxModel view)
        {
            bool[,] m = emptyMatrix;
            if (view != null)
            {
                int width = 0;
                int textStart = 0;
                int height = 0;
                if (icon != null)
                {
                    width += icon.GetLength(1);
                    height += icon.GetLength(0);
                }

                if (width > 0) textStart = width += IconTextSpace;

                if (text != null)
                {
                    width += text.GetLength(1);
                    height = Math.Max(height, text.GetLength(0));
                }

                if (width * height > 0)
                {
                    //bool selected = false;
                    //if (Entry.Status.HasFlag(DialogEntryStatus.Selected))
                    //{
                    //    selected = true;
                    //    height += 1;
                    //}

                    m = new bool[height, width];
                    // insert the icon into the prepared matrix
                    m = DialogRenderer.InsertIntoMatrix(m, icon, 0, 0);
                    // insert the text into the prepared matrix
                    m = DialogRenderer.InsertIntoMatrix(m, text, 0, textStart);
                    //// insert selection
                    //if(selected) m = DialogRenderer.InsertIntoMatrix(m, getSolidDivider(width), m.GetLength(0) - 1, 0);
                }
            }
            return m;
        }

        bool[,] _solidDivider = emptyMatrix;
        /// <summary>
        /// Gets a dividing solid line of raced pins.
        /// </summary>
        /// <param name="width">The width of the divider.</param>
        /// <returns>a 1 x width matrix</returns>
        bool[,] getSolidDivider(int width)
        {
            if (_solidDivider.GetLength(1) == width) return _solidDivider;
            //if (width > devider.GetLength(1))
            if (width > 0)
            {
                _solidDivider = new bool[1, width];
                for (int i = 0; i < width; i++) { _solidDivider[0, i] = true; }
            }
            else return emptyMatrix;
            return _solidDivider;
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            unregisterFromEvents(Entry);
            Entry = null;
        }

        #endregion
    }
}
