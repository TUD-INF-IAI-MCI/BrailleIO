using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;
using BrailleIO.Interface;
using BrailleIO.Renderer;

namespace BrailleIO
{
    /// <summary>
    /// Central instance for the BrailleIO Framework. 
    /// It connects the hardware abstraction layers and the GUI/TUI components.
    /// It gives you access to the hardware via the <see cref="IBrailleIOAdapterManager"/> AdapterManager.
    /// The GUI/TUI components are available through several methods. 
    /// </summary>
    public class BrailleIOMediator
    {
        #region Members

        #region Private Members

        /// <summary>
        /// The singleton instance
        /// </summary>
        private static BrailleIOMediator instance;
         
        /// <summary>
        /// Timer to enable a continuous refresh rate
        /// </summary>
        private System.Timers.Timer _timer = new System.Timers.Timer();
        /// <summary>
        /// Timer to enable a continuous refresh rate
        /// </summary>
        internal System.Timers.Timer RenderingTimer
        {
            get
            {
                if (_timer == null)
                {
                    _timer = new System.Timers.Timer();
                }
                return _timer;
            }
        }

        /// <summary>
        /// views are either Screens (combined ViewRanges) or simply ViewRanges
        /// Screens should be more comfortable to use for the developer
        /// </summary>
        private ConcurrentDictionary<String, AbstractViewBoxModelBase> views = new ConcurrentDictionary<String, AbstractViewBoxModelBase>();
        private readonly object vvLock = new object();
        private ConcurrentDictionary<String, AbstractViewBoxModelBase> visibleViews
        {
            get
            {
                lock (vvLock)
                {
                    return getVisibleViews();
                }
            }
        }

        private ConcurrentDictionary<string, AbstractViewBoxModelBase> getVisibleViews()
        {
            ConcurrentDictionary<string, AbstractViewBoxModelBase> vvs = new ConcurrentDictionary<string, AbstractViewBoxModelBase>();
            if (views != null && views.Count > 0)
            {
                foreach (var item in views)
                {
                    if (item.Value != null)
                    {
                        if (item.Value is IViewable && ((IViewable)item.Value).IsVisible())
                        {
                            vvs.AddOrUpdate(item.Key, item.Value, (key, oldValue) => item.Value);
                        }
                    }
                    else
                    {
                        //TODO: delete?
                    }
                }
            }
            return vvs;
        }

        /// <summary>
        /// Flag to determine it the resulting matrix is changeable or not
        /// </summary>
        private volatile bool pins_locked = false;

        private bool[,] _matrix;
        /// <summary>
        ///  matrix to be displayed on device
        /// </summary>
        private bool[,] Matrix
        {
            get
            {
                if (_matrix == null || _matrix.GetLength(0) < 1 || _matrix.GetLength(1) < 1)
                {
                    if (AdapterManager != null && AdapterManager.ActiveAdapter != null)
                        _matrix = new bool[AdapterManager.ActiveAdapter.Device.DeviceSizeY, AdapterManager.ActiveAdapter.Device.DeviceSizeX];
                    else
                        _matrix = new bool[0, 0];
                }
                return _matrix;
            }
            set
            {
                //System.Diagnostics.Debug.WriteLine(
                //    "\t\t\t[" + DateTime.UtcNow.ToString("HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture)
                //    + "]\t[MATRIX SET]");
                _matrix = value;
                _newMatrix = true;
            }
        }

        private volatile bool _newMatrix;


        BrailleIOViewMatixRenderer vmr = new BrailleIO.Renderer.BrailleIOViewMatixRenderer();

        #endregion

        #region Public Members

        /// <summary>
        /// The Adapter Manager that knows and handle the connected devices for the output
        /// </summary>
        public IBrailleIOAdapterManager AdapterManager { get; set; }
        
        #endregion

        #endregion

        #region Constructor / Destructor / Singleton

        private BrailleIOMediator()
        {
            AdapterManager = new BasicBrailleIOAdapterManager();

            RenderingTimer.Elapsed += new ElapsedEventHandler(refreshDisplayEvent);
            RenderingTimerInterval = _defaultInterval;
            RenderingTimer.Enabled = true;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="BrailleIOMediator"/> class.
        /// </summary>
        ~BrailleIOMediator() { if (renderingTread != null) renderingTread.Abort(); }

        /// <summary>
        /// lock object so the instance can not been build twice.
        /// </summary>
        private static object syncRoot = new Object();
        /// <summary>
        /// Central instance for the BrailleIO Framework. 
        /// It connects the hardware abstraction layers and the GUI/TUI components.
        /// It gives you access to the hardware via the <see cref="IBrailleIOAdapterManager"/> AdapterManager.
        /// The GUI/TUI components are available through several methods. 
        /// </summary>
        public static BrailleIOMediator Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new BrailleIOMediator();
                        }
                        syncRoot = null;
                    }
                }
                return instance;
            }
        }

        #endregion

        #region Rendering Methods

        #region Public Available Calls

        /// <summary>
        /// Tries to sent the actual build matrix to all devices, that are active.
        /// To enable a sending, the pins have to be unlocked (still rendering or maybe locked by the user)
        /// and at an Adapter has to be active.
        /// </summary>
        /// <param name="rerender">if set to <c>true</c> the mediator will force a rendering of the content.</param>
        public void RefreshDisplay(bool rerender = false)
        {
            if (rerender) RenderDisplay();
            else
            {
                _newMatrix = true;
            }
        }

        /// <summary>
        /// Forces the rendering thread to build the resulting Matrix by
        /// calling all renderer for the visible view ranges.
        /// The matrix will not been sent until the refresh timer is elapsed or the
        /// <see cref="RefreshDisplay" /> Method was called.
        /// </summary>
        public void RenderDisplay()
        {
            //System.Diagnostics.Debug.WriteLine(
            //"[" + DateTime.UtcNow.ToString("HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture) + "]\t[RENDER DISPLAY CALL]");

            stack.Push(true);
            if (stack.Count > 0 && (renderingTread == null || !renderingTread.IsAlive))
            {
                renderingTread = new Thread(delegate() { renderDisplay(); });
                renderingTread.Name = "RenderingThread";
                renderingTread.Priority = ThreadPriority.Highest;
                renderingTread.Start();
            }
            else{}
        }

        #endregion

        #region private Rendering

        #region Synchronization win AdapterManager

        private static int _elapsedTimes = 0;
        private static int _maxTickbeforRefresh = 200; 
        /// <summary>
        /// Event handler for the refresh timer elapsed event.
        /// Refreshes the display.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="e">The <see cref="System.Timers.ElapsedEventArgs"/> instance containing the event data.</param>
        private void refreshDisplayEvent(object source, ElapsedEventArgs e)
        {
            _elapsedTimes++;
            if (AdapterManager != null)
            {
                if (_newMatrix || _elapsedTimes > _maxTickbeforRefresh)
                {
                    //System.Diagnostics.Debug.WriteLine(
                    //    "\t[" + DateTime.UtcNow.ToString("HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture) 
                    //    + "]\t[SYNCH]");

                    _elapsedTimes = 0;
                    _newMatrix = false;
                    AdapterManager.Synchronize(Matrix);
                }
                else
                {
                    //System.Diagnostics.Debug.WriteLine(
                    //    "\t[" + DateTime.UtcNow.ToString("HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture)
                    //    + "]\t[SYNCH ignored]");
                }
            }
        }

        #endregion

        #region THE RENDERING OF THE VIEW RANGES

        /// <summary>
        /// helping stack that helps to determine if a rendering is necessary.
        /// Collects all render calls and the rendering thread can decide if to render or not.
        /// </summary>
        private readonly ConcurrentStack<object> stack = new ConcurrentStack<Object>();
        /// <summary>
        /// separate thread for building the resulting matrix
        /// </summary>
        private Thread renderingTread;
        /// <summary>
        /// Builds the resulting matrix that will be send to the adapters by calling the renderer for each view range.
        /// </summary>
        void renderDisplay()
        {
            while (stack.Count > 0)
            {
                //System.Diagnostics.Debug.WriteLine(
                //"\t\t[" + DateTime.UtcNow.ToString("HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture)
                //+ "]\t[RENDER]");

                ////FIXME: for debugging
                //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                //sw.Start();

                stack.Clear();
                if (AdapterManager == null || AdapterManager.ActiveAdapter == null) return;

                pins_locked = true;
                bool[,] matrix = new bool[Matrix.GetLength(0), Matrix.GetLength(1)];
                foreach (String key in this.visibleViews.Keys)
                {
                    if (this.views[key] is BrailleIOViewRange)
                    {
                        matrix = this.drawViewRange(((BrailleIOViewRange)this.views[key]), matrix);
                    }
                    else if (this.views[key] is BrailleIOScreen)
                    {
                        ////FIXME: for debugging
                        //TimeSpan last = sw.Elapsed;

                        foreach (BrailleIOViewRange vr in ((BrailleIOScreen)this.views[key]).GetViewRanges().Values)
                        {
                            if (vr != null && vr.IsVisible())
                            {
                                matrix = this.drawViewRange(vr, matrix);
                            }

                            ////FIXME: for debugging
                            //System.Diagnostics.Debug.WriteLine(
                            //"\t[" + DateTime.UtcNow.ToString("HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture) + "]\t[VR " + vr.Name + "]\tRendering Time Elapsed=\t{0}", (sw.Elapsed - last));
                            //last = sw.Elapsed;                            
                        }
                    }
                    else { }
                }
                this.Matrix = matrix;

                ////FIXME: for debugging
                //sw.Stop();
                //System.Diagnostics.Debug.WriteLine(
                //    "[" + DateTime.UtcNow.ToString("HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture) + "]\t[SCREEN]\tRendering Time Elapsed=\t{0}", sw.Elapsed);

                pins_locked = false;

                //System.Diagnostics.Debug.WriteLine(
                //     "[" + DateTime.UtcNow.ToString("HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture) + "]_____________________________________FINISHED");
                   
                //GC.Collect(0, GCCollectionMode.Forced);
                //GC.WaitForFullGCComplete(20);
            }
        }

        /// <summary>
        /// draw a ViewRange to this.matrix
        /// </summary>
        /// <param name="vr">ViewRange</param>
        /// <param name="matrix">The matrix to render in.</param>
        /// <returns></returns>
        private bool[,] drawViewRange(BrailleIOViewRange vr, bool[,] matrix)
        {
            try
            {
                if (vr == null || vr.ViewBox == null || !vr.IsVisible()) return matrix;

                ////FIXME: for debugging
                //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                //sw.Start();

                bool[,] viewBoxMatrix = new bool[vr.ViewBox.Height, vr.ViewBox.Width];

                // View Range bounds
                int srcOffsetX = vr.GetLeft();
                int scrOffsetY = vr.GetTop();
                bool handlePanning = true;

                bool[,] contentMatrix = new bool[1, 1];
                // Matrix rendering
                if (vr.IsMatrix())
                {
                    if (vr.GetMatrix() != null)
                    {
                        contentMatrix = vr.GetMatrix();
                        //set content size in vr
                        vr.ContentHeight = contentMatrix.GetLength(0);
                        vr.ContentWidth = contentMatrix.GetLength(1);
                    }
                }
                // Image rendering
                else if (vr.IsImage())
                {
                    int th = (vr is IContrastThreshold) ? ((IContrastThreshold)vr).GetContrastThreshold() : -1;

                    using (System.Drawing.Bitmap img = vr.GetImage())
                    {
                        if (vr.ContentRender is BrailleIOImageToMatrixRenderer)
                        {
                            if (th >= 0)
                            {
                                contentMatrix = ((BrailleIOImageToMatrixRenderer)vr.ContentRender).RenderImage(img, vr, vr as IPannable, vr.InvertImage, vr.GetZoom(), th);
                            }
                            else
                            {
                                contentMatrix = ((BrailleIOImageToMatrixRenderer)vr.ContentRender).RenderImage(img, vr, vr as IPannable, vr.InvertImage, vr.GetZoom(), true);
                            }
                        }
                        else
                        {
                            contentMatrix = vr.ContentRender.RenderMatrix(vr, img);
                        }
                    }
                    handlePanning = false;
                }
                // Text rendering
                else if (vr.IsText())
                {
                    if (!string.IsNullOrEmpty(vr.GetText()))
                    {
                        contentMatrix = vr.ContentRender.RenderMatrix(vr, vr.GetText());
                    }
                }
                // Generic renderer
                else if (vr.IsOther())
                {
                    if (vr.GetOtherContent() != null && vr.ContentRender != null)
                    {
                        contentMatrix = vr.ContentRender.RenderMatrix(vr, vr.GetOtherContent());
                    }
                    else return matrix;
                }
                else return matrix;

                //place the content matrix (contentMatrix) in the view range matrix with aware of the box model 
                viewBoxMatrix = vmr.RenderMatrix(vr, contentMatrix, handlePanning);
                // Border rendering
                viewBoxMatrix = BrailleIO.Renderer.BrailleIOBorderRenderer.renderMatrix(vr, viewBoxMatrix);
                bool pl = pins_locked;
                pins_locked = true;

                // draw content and borders to main matrix
                System.Threading.Tasks.Parallel.For(srcOffsetX, srcOffsetX + viewBoxMatrix.GetLength(1), x =>
                {
                    if (x >= 0 && x < matrix.GetLength(1))
                    {
                        System.Threading.Tasks.Parallel.For(scrOffsetY, scrOffsetY + viewBoxMatrix.GetLength(0), y =>
                        {
                            if (y >= 0 && y < matrix.GetLength(0)) { matrix[y, x] = viewBoxMatrix[y - scrOffsetY, x - srcOffsetX]; }
                        });
                    }
                });

                ////FIXME: for debugging
                //sw.Stop();
                //System.Diagnostics.Debug.WriteLine("[" + DateTime.UtcNow.ToString("HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture) + "] (VR'"+vr.Name+"') *+~ Rendering Time Elapsed={0} ~+*", sw.Elapsed);

                pins_locked = pl;
            }
            catch
            {
            }
            finally
            {
                GC.Collect();
            }

            return matrix;
        }

        #endregion

        #endregion

        #endregion

        #region Getter & Setter

        private double _defaultInterval = 10;
        /// <summary>
        /// Gets or sets the rendering timer interval.
        /// Every tick of the timer the manager checks for
        /// changed content by the renderers and submit it to the 
        /// adapters.
        /// </summary>
        /// <value>
        /// The rendering timer interval.
        /// </value>
        public double RenderingTimerInterval
        {
            get { return RenderingTimer.Interval; }
            set
            {
                RenderingTimer.Interval = value;
                RenderingTimer.Start();
            }
        }

        /// <summary>
        /// Gets or sets the maximum ticks before starting a synchronization.
        /// If the <see cref="RenderingTimerInterval"/> has ticked this count times 
        /// and the content wasn't changed yet, the matrix will be sent to all adapters
        /// anyway for refreshing them.
        /// The minimal refresh frequency is RenderingTimerInterval (ms) * MaxTicksToSynchronize.
        /// </summary>
        /// <value>
        /// The maximum ticks before starting a synchronization anyway.
        /// </value>
        /// <exception cref="ArgumentException">Tick count must be larger then 0</exception>
        public int MaxTicksToSynchronize
        {
            get { return _maxTickbeforRefresh; }
            set
            {
                if (value > 0) { _maxTickbeforRefresh = value; }
                else{throw new ArgumentException("Tick count must be larger then 0");}
            }
        }

        /// <summary>
        /// get current display-matrix.
        /// </summary>
        /// <returns>bool[,] matrix</returns>
        public bool[,] GetMatrix()
        {
            return Matrix;
        }

        /// <summary>
        /// check if pins are locked. This indicates that a rendering is still going on 
        /// or the rendering is disabled by the user by locking the set matrix.
        /// </summary>
        /// <returns>bool pins_locked</returns>
        public bool ArePinsLocked() //TODO: check for this is necessary
        {
            return pins_locked;
        }

        /// <summary>
        /// Locks the pins. Stops renderers to do there work
        /// </summary>
        public void LockPins()
        {
            pins_locked = true;
        }

        /// <summary>
        /// Unlocks the pins. Enables renderers to refresh the matrix that is send to the devices
        /// </summary>
        public void UnlockPins()
        {
            pins_locked = false;
        }

        #endregion

        #region View Handling

        ConcurrentDictionary<String, AbstractViewBoxModelBase> oldVisibleViews = new ConcurrentDictionary<string,AbstractViewBoxModelBase>();

        /// <summary>
        /// show a view.
        /// will be displayed with all other visible views at next display update.
        /// </summary>
        /// <param name="name">
        /// name of view
        /// </param>
        public void ShowView(String name)
        {
            if (views.ContainsKey(name))
            {
                if (!oldVisibleViews.ContainsKey(name)) { oldVisibleViews = getVisibleViews(); }

                if (views[name] is IViewable)
                {
                    ((IViewable)views[name]).SetVisibility(true);
                }
                if (views[name] is BrailleIOScreen)
                {
                    foreach (var item in views.Keys)
                    {
                        if (!item.Equals(name) && views[item] is BrailleIOScreen)
                        {
                            hideView(item.ToString());
                        }
                    }
                }
                //if (!this.VisibleViews.ContainsKey(name))
                //    this.VisibleViews.TryAdd(name, true);

                fire_visibleViewChanged();
            }
            else throw new ArgumentException("View '" + name + "' is unknown", "name");
        }

        /// <summary>
        /// hide a view.
        /// hidden views still exist but will not show on display
        /// </summary>
        /// <param name="name">
        /// name of view
        /// </param>
        public void HideView(String name)
        {
            hideView(name, true);
        }

        /// <summary>
        /// hide a view.
        /// hidden views still exist but will not show on display
        /// </summary>
        /// <param name="name">name of view</param>
        /// <param name="updateOldViewStates">if set to <c>true</c> [update old view states].</param>
        /// <exception cref="ArgumentException">View '" + name + "' is unknown;name</exception>
        private void hideView(String name, bool updateOldViewStates = false)
        {
            if (views.ContainsKey(name))
            {
                if (updateOldViewStates) { oldVisibleViews = getVisibleViews(); }

                if (views[name] is IViewable)
                {
                    ((IViewable)views[name]).SetVisibility(false);
                }

                if (updateOldViewStates) { fire_visibleViewChanged(); }                
            }
            else throw new ArgumentException("View '" + name + "' is unknown", "name");
        }

        /// <summary>
        /// Add a TangramSkApp.ViewRange or a TangramSkApp.Screen to list
        /// </summary>
        /// <param name="name">
        /// name of view
        /// </param>
        /// <param name="view">
        /// TangramSkApp.ViewRange or a TangramSkApp.Screen
        /// </param>
        /// <returns>
        /// bool success
        /// </returns>
        public bool AddView(String name, AbstractViewBoxModelBase view)
        {
            if (view is BrailleIOViewRange || view is BrailleIOScreen)
            {
                bool success = this.views.TryAdd(name, view);

                if (success)
                {
                    view.PropertyChanged += view_PropertyChanged;
                }

                return success;
            }
            return false;
        }

        /// <summary>
        /// remove view
        /// </summary>
        /// <param name="name">
        /// name of view
        /// </param>
        public bool RemoveView(String name)
        {
            AbstractViewBoxModelBase trash;
            bool success = this.views.TryRemove(name, out trash);

            if (success)
            {
                try { trash.PropertyChanged -= view_PropertyChanged; }
                catch { }
            }

            return success;
        }

        /// <summary>
        /// rename a view
        /// </summary>
        /// <param name="from">
        /// old name
        /// </param>
        /// <param name="to">
        /// new name
        /// </param>
        public void RenameView(String from, String to)
        {
            this.views.TryAdd(to, this.views[from]);
            AbstractViewBoxModelBase trash;
            this.views.TryRemove(from, out trash);
        }

        /// <summary>
        /// checks if Instance has a specific view
        /// </summary>
        /// <param name="name">
        /// name of View
        /// </param>
        /// <returns>
        /// bool
        /// </returns>
        public bool ContainsView(String name)
        {
            return this.views != null ? this.views.ContainsKey(name) : false;
        }

        /// <summary>
        /// get View by name
        /// </summary>
        /// <param name="name">Screen or ViewRange or null</param>
        /// <returns></returns>
        public Object GetView(String name)
        {
            if (ContainsView(name))
                if (views[name] is BrailleIOViewRange)
                    return (BrailleIOViewRange)views[name];
                else if (views[name] is BrailleIOScreen)
                    return (BrailleIOScreen)views[name];
                else
                    return null;
            else
                return null;
        }

        /// <summary>
        /// Gets a list of all available top-level views.
        /// </summary>
        /// <returns>list of all available top-level views. Could be <see cref="BrailleIOScreen"/> or <see cref="BrailleIOViewRange"/></returns>
        public List<AbstractViewBoxModelBase> GetViews()
        {
            return views != null ? views.Values.ToList() : new List<AbstractViewBoxModelBase>();
        }

        /// <summary>
        /// Gets the active views.Can be a <see cref="BrailleIOViewRange"/> or <see cref="BrailleIOScreen"/>
        /// </summary>
        /// <returns>List of currently active views</returns>
        public List<AbstractViewBoxModelBase> GetActiveViews()
        {
            var v = getVisibleViews();
            return v != null ? new List<AbstractViewBoxModelBase>(v.Values) : new List<AbstractViewBoxModelBase>();
        }

        /// <summary>
        /// Gets the view at a position.
        /// </summary>
        /// <param name="x">The horizontal position on the device.</param>
        /// <param name="y">The vertical position on the device.</param>
        /// <returns>The topmost view range that containing the point or <c>null</c></returns>
        public BrailleIOViewRange GetViewAtPosition(int x, int y)
        {
            if (x >= 0 || y >= 0)
            {
                var views = getVisibleViews();
                if (views != null && views.Count > 0)
                {
                    var keys = views.Keys;
                    if (keys != null && keys.Count > 0)
                    {
                        var k = keys.ToArray();

                        for (int i = keys.Count - 1; i >= 0; i--)
                        {
                            var view = views[k[i]];
                            if (view != null)
                            {
                                if (view is BrailleIOViewRange)
                                {
                                    // TODO: check if point is inside
                                    if (((BrailleIOViewRange)view).ContainsPoint(x, y))
                                        return ((BrailleIOViewRange)view);
                                }
                                else if (view is BrailleIOScreen)
                                {
                                    BrailleIOScreen s = (BrailleIOScreen)view;

                                    // get all visible view ranges of the screen
                                    var vv = s.GetVisibleViewRangeAtPosition(x, y);
                                    if (vv != null)
                                        return vv;
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// checks if there are any views yet
        /// </summary>
        /// <returns>
        /// bool 
        /// </returns>
        public bool IsEmpty()
        {
            return (this.views != null && this.views.Count > 0) ? false : true;
        }

        /// <summary>
        /// count of available top-level views e.g. screens in a multi screen setting
        /// </summary>
        /// <returns>
        /// int views.count
        /// </returns>
        public int Count()
        {
            return this.views.Count;
        }

        #region Property Events Handling

        void view_PropertyChanged(object sender, BrailleIOPropertyChangedEventArgs e)
        {
            if (sender != null && sender is AbstractViewBoxModelBase && e != null && !String.IsNullOrWhiteSpace(e.PropertyName))
            {
                /// visibility of a view has changed
                if (e.PropertyName.Equals("Visibility"))
                {
                    // check if update is necessary or not
                    if (((AbstractViewBoxModelBase)sender).IsVisible())
                    {
                        if (!oldVisibleViews.ContainsKey(((AbstractViewBoxModelBase)sender).Name))
                        {
                            fire_visibleViewChanged();
                        }
                    }
                    else // hide view 
                    {
                        if (oldVisibleViews.ContainsKey(((AbstractViewBoxModelBase)sender).Name))
                        {
                            fire_visibleViewChanged();
                        }
                    }

                }
                /// Name has changed
                else if (e.PropertyName.Equals("Name"))
                {
                    string newName = ((AbstractViewBoxModelBase)sender).Name;
                    var view = GetView(newName);
                    if (view != sender)
                    {
                        lock (vvLock)
                        {
                            // try to find the old view in the list
                            foreach (string key in views.Keys)
                            {
                                try
                                {
                                    if (views[key] == sender) { RenameView(key, newName); break; }
                                }
                                catch { }
                            } 
                        }
                    }
                }
            }
        }

        #endregion

        #endregion

        #region Adapter/Device Methodes

        /// <summary>
        /// get device Width from active adapter
        /// </summary>
        /// <returns>int Width of device</returns>
        public int GetDeviceSizeX()
        {
            if (AdapterManager != null && AdapterManager.ActiveAdapter != null && AdapterManager.ActiveAdapter.Device != null)
                return AdapterManager.ActiveAdapter.Device.DeviceSizeX;
            else return 0;
        }

        /// <summary>
        /// get device Height from active adapter
        /// </summary>
        /// <returns>int Height of device</returns>
        public int GetDeviceSizeY()
        {
            if (AdapterManager != null && AdapterManager.ActiveAdapter != null && AdapterManager.ActiveAdapter.Device != null)
                return AdapterManager.ActiveAdapter.Device.DeviceSizeY;
            else return 0;
        }

        /// <summary>
        /// Forces the current active adapter devices to recalibrate.
        /// </summary>
        /// <returns><c>true</c> if the adapter is successfully recalibrated</returns>
        public bool Recalibrate()
        {
            return (AdapterManager.ActiveAdapter != null) ? AdapterManager.ActiveAdapter.Recalibrate(0) : false;
        }

        /// <summary>
        /// Forces all connected adapter devices to recalibrate.
        /// </summary>
        /// <returns><c>true</c> if all adapter are successfully recalibrated</returns>
        public bool RecalibrateAll()
        {
            bool result = true;
            if (AdapterManager != null)
            {
                foreach (var adapter in AdapterManager.GetAdapters())
                {
                    result &= adapter.Recalibrate(0);
                }
            }
            return result;
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the visibility of view changed.
        /// </summary>
        public event EventHandler<VisibilityChangedEventArgs> VisibleViewsChanged;

        void fire_visibleViewChanged()
        {
            if (VisibleViewsChanged != null)
            {
                try {
                    VisibleViewsChanged.Invoke(this, 
                        new VisibilityChangedEventArgs(
                            getVisibleViews().Values.ToList<AbstractViewBoxModelBase>(), 
                            oldVisibleViews.Values.ToList<AbstractViewBoxModelBase>()));
                }
                catch { }
            }
        }

        #endregion
    }

    #region Event Args

    /// <summary>
    /// Event arguments for a visibility changed event in the list of visible views.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class VisibilityChangedEventArgs : EventArgs
    {
        /// <summary>
        /// list of all visible views
        /// </summary>
        public readonly List<AbstractViewBoxModelBase> VisibleViews;
        /// <summary>
        /// List of previously visible views
        /// </summary>
        public readonly List<AbstractViewBoxModelBase> PreviouslyVisibleViews;

        public VisibilityChangedEventArgs(List<AbstractViewBoxModelBase> vvs, List<AbstractViewBoxModelBase> pvvs)
        {
            VisibleViews = vvs;
            PreviouslyVisibleViews = pvvs;
        }
    }

    #endregion
}