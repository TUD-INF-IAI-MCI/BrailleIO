using BrailleIO.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using tud.mci.LanguageLocalization;

namespace BrailleIO.Dialogs.Rendering
{
    /// <summary>
    /// Renderer for returning an tactile Icon (tacton) related to a specific content element.
    /// </summary>
    /// <seealso cref="BrailleIO.Interface.IBrailleIOContentRenderer" />
    public class TactonRenderer : IBrailleIOContentRenderer
    {
        #region Members

        protected static readonly bool[,] emptyMatrix = new bool[0, 0];

        #region Tacton Storage Paths

        static String _currentPath = String.Empty;
        /// <summary>
        /// Gets the current DLL path.
        /// </summary>
        /// <value>
        /// The current DLL path.
        /// </value>
        static internal String CurrentDllPath
        {
            get {
                if (_currentPath == String.Empty)
                {
                    string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                    UriBuilder uri = new UriBuilder(codeBase);
                    string path = Uri.UnescapeDataString(uri.Path);
                    _currentPath = Path.GetDirectoryName(path);
                }
                return _currentPath; 
            }
        }

        static string _tactonStoragePath = String.Empty;

        /// <summary>
        /// Gets or sets the tacton storage directory path.
        /// </summary>
        /// <value>
        /// The tacton storage path.
        /// </value>
        public static string TactonStoragePath
        {
            get {
                if (String.IsNullOrEmpty(_tactonStoragePath))
                {
                    _tactonStoragePath = CurrentDllPath + @"\Tactons";
                }
                return _tactonStoragePath; }
            set { 
                _tactonStoragePath = value;
                if(Instance != null) Instance.initializeTactons();
            }
        }
        
        #endregion

        #region Tacton Dictionary

        /// <summary>
        /// The tacton set
        /// </summary>
        protected readonly ConcurrentDictionary<String, bool[,]> TactonSet = new ConcurrentDictionary<string, bool[,]>();

        #endregion

        static readonly BrailleIO.Renderer.MatrixBrailleRenderer BrailleRenderer = new BrailleIO.Renderer.MatrixBrailleRenderer(
            Renderer.RenderingProperties.IGNORE_LAST_LINESPACE | Renderer.RenderingProperties.RETURN_REAL_WIDTH);
        static readonly LL ll = new LL(BrailleIO.Dialogs.Properties.Resources.Language);
        
        #region Singleton

        static readonly TactonRenderer _instance = new TactonRenderer();
        /// <summary>
        /// Gets the Singleton instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        static internal TactonRenderer Instance
        {
            get { return _instance; }
        }

        #endregion

        #endregion

        #region Constructor

        protected TactonRenderer()
        {
            initializeTactons();
        }

        #endregion

        #region Initialization

        private bool initializeTactons()
        {
            try
            {
                var tactons = TactonLoader.LoadTactonsFromDirectory(TactonStoragePath);
                AddOrUpdateTacton(tactons);
                return true;
            }
            catch
            {
                return false;
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
        public virtual bool[,] RenderMatrix(IViewBoxModel view, object content)
        {
            bool[,] m = emptyMatrix;
            if (view != null && view.ContentBox.Width > 0 && content != null && content is DialogEntry)
            {


                DialogEntry entry = content as DialogEntry;
                if (entry != null)
                {

                    switch (entry.Type)
                    {
                        case DialogEntryType.Submenu:
                            m = GetTacton("dialogSubmenu"); //dialogSubmenu; 
                            break;
                        case DialogEntryType.Group:
                            break;
                        case DialogEntryType.Checkbox:
                            m = entry.Status.HasFlag(DialogEntryStatus.Checked) ? GetTacton("checkActivated") : GetTacton("check"); //checkActivated : check;
                            break;
                        case DialogEntryType.RadioButton:
                            m = entry.Status.HasFlag(DialogEntryStatus.Checked) ? GetTacton("radioActivated") : GetTacton("radio"); //radioActivated : radio;
                            break;
                        case DialogEntryType.Button:
                            m = GetTacton("button"); //button;
                            break;
                        case DialogEntryType.EditField:
                            break;
                        default:
                            break;
                    }


                    if (entry.Status.HasFlag(DialogEntryStatus.Disabled))
                    {
                        // TODO: how to mark elements as disabled; even if they have already an icon.
                        m = GetTacton("disabled"); //disabled;
                    }
                    //else if (entry.Status.HasFlag(DialogEntryStatus.Checked))
                    //{
                    //   if(entry.Type == DialogEntryType.Checkbox)
                    //     m = checkActivated;
                    //}
                    //else if (entry.Status.HasFlag(DialogEntryStatus.Unchecked))
                    //{
                    //    m = dialogUnchecked;
                    //}
                    else if (entry.Status.HasFlag(DialogEntryStatus.Activated))
                    {
                        m = GetTacton("dialogActive"); //dialogActive;
                    }

                    // TODO: SUBMENU etc.


                }



            }
            return m;
        }

        #endregion

        #region Tacton Set Handling
        
        /// <summary>
        /// Determines whether the specified named tacton is already registered.
        /// </summary>
        /// <param name="name">The name of the tacton.</param>
        /// <returns>
        ///   <c>true</c> if the specified tacton exists; otherwise, <c>false</c>.
        /// </returns>
        public bool HasTacton(String name)
        {
            return TactonSet.ContainsKey(name);
        }

        /// <summary>
        /// Adds a new tacton to the set or update the already registered tacton.
        /// </summary>
        /// <param name="name">The name of the tacton.</param>
        /// <param name="tacton">The tactons tactile matrix.</param>
        /// <returns><c>true</c> if the tacton was added successfully; otherwise, <c>false</c>.</returns>
        public bool AddOrUpdateTacton(String name, bool[,] tacton)
        {
            return tacton == TactonSet.AddOrUpdate(name, tacton, 
                (key, existingVal) => { return existingVal = tacton; });
        }

        /// <summary>
        /// Adds a new tacton set to the set. Already registered tacton will be updated.
        /// </summary>
        /// <param name="tactons">The tactons.</param>
        /// <returns><c>true</c> if all tacton were added successfully; otherwise, <c>false</c>.</returns>
        public bool AddOrUpdateTacton(Dictionary<String, bool[,]> tactons){
            bool success = true;

            if (tactons != null && tactons.Count > 0)
            {
                foreach (var item in tactons)
                {
                    success = success && AddOrUpdateTacton(item.Key, item.Value);
                }
            }

            return success;
        }

        /// <summary>
        /// Removes a tacton from the set.
        /// </summary>
        /// <param name="name">The name of the tacton.</param>
        /// <returns><c>true</c> if the tacton was removed successfully from the set; otherwise, <c>false</c>.</returns>
        public bool RemoveTacton(String name)
        {
            bool[,] trash;
            if (!HasTacton(name)) return true;
            return TactonSet.TryRemove(name, out trash);
        }

        /// <summary>
        /// Trys to gets the tacton from the registerd set.
        /// </summary>
        /// <param name="name">The name of the tacton.</param>
        /// <returns>The registered tacton's tactile matrix or an empty matrix if the tacton is not registered.</returns>
        public bool[,] GetTacton(String name){
            bool[,] t = emptyMatrix;
            if (HasTacton(name))
            {
                int i = 0;
                while (!TactonSet.TryGetValue(name, out t) && i++ < 5) { Thread.Sleep(2); }
            }
            return t;
        }

        #endregion
        
    }
}