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
        private System.Timers.Timer device_update_timer = new System.Timers.Timer();

        /// <summary>
        /// views are either Screens (combined ViewRanges) or simply ViewRanges
        /// Screens should be more comfortable to use for the developer
        /// /// </summary>
        private ConcurrentDictionary<String, Object> views = new ConcurrentDictionary<String, Object>();
        private readonly object vvLock = new object();
        private ConcurrentDictionary<String, Object> _visibleViews = new ConcurrentDictionary<String, Object>();
        private ConcurrentDictionary<String, Object> VisibleViews
        {
            get
            {
                lock (vvLock)
                {
                    return _visibleViews;
                }
            }
            set
            {
                lock (vvLock)
                {
                    _visibleViews = value;
                }
            }
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
                if (_matrix == null)
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
                _matrix = value;
            }
        }


        #endregion

        #region Public Members

        /// <summary>
        /// The Adapter Manager that knows and handle the connected devices for the output
        /// </summary>
        public IBrailleIOAdapterManager AdapterManager {get; set;}

        #endregion

        #endregion

        #region Constructor / Destructor / Singleton

        private BrailleIOMediator() { }

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
                            instance.device_update_timer.Elapsed += new ElapsedEventHandler(refreshDisplayEvent);
                            instance.device_update_timer.Interval = 100;
                            instance.device_update_timer.Enabled = true;
                            instance.AdapterManager = new BasicBrailleIOAdapterManager();
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
        /// Trys to sent the actual build matrix to all devices, that are active.
        /// To enable a sending, the pins have to be unlocked (still rendering or maybe locked ba user)
        /// and at an Adapter has to be active.
        /// </summary>
        /// <param name="rerender">if set to <c>true</c> it forces the rendering thread to rebuild the matrix by calling all renderers.</param>
        public void RefreshDisplay(bool rerender)
        {
            if (rerender)
            {
                if (AdapterManager != null && AdapterManager.ActiveAdapter != null)
                {
                    //if (instance.device_update_timer.Interval != AdapterManager.ActiveAdapter.Device.RefreshRate * 10)
                    //    instance.device_update_timer.Interval = AdapterManager.ActiveAdapter.Device.RefreshRate * 10;
                    RenderDisplay();
                }
            }
            else RefreshDisplay();
        }

        /// <summary>
        /// Trys to sent the actual build matrix to all devices, that are active.
        /// To enable a sending, the pins have to be unlocked (still rendering or maybe locked by the user)
        /// and at an Adapter has to be active.
        /// </summary>
        public void RefreshDisplay()
        {
            if (ArePinsLocked()) return;
            if (AdapterManager != null && AdapterManager.ActiveAdapter != null && Matrix != null)
            {

                //if (instance.device_update_timer.Interval != AdapterManager.ActiveAdapter.Device.RefreshRate * 10)
                //    instance.device_update_timer.Interval = AdapterManager.ActiveAdapter.Device.RefreshRate * 10;

                AdapterManager.Synchronize(this.Matrix.Clone() as bool[,]);
            }
        }

        /// <summary>
        /// Forces the rendering thread to build the resulting Matrix by 
        /// calling all renderer for the visible view ranges.
        /// The matrix will not been sent until the refresh timer is elapsed or the 
        /// <see cref="RefreshDisplay"/> Method was called.
        /// </summary>
        public void RenderDisplay()
        {
            stack.Push(true);

            if (renderingTread == null || !renderingTread.IsAlive)
            {
                renderingTread = new Thread(delegate() { renderDisplay(); });
                renderingTread.Start();
            }
            else { }
        }

        #endregion

        #region private Rendering

        /// <summary>
        /// Event handler for the refresh timer elapsed event.
        /// Refreshes the display.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="e">The <see cref="System.Timers.ElapsedEventArgs"/> instance containing the event data.</param>
        private static void refreshDisplayEvent(object source, ElapsedEventArgs e)
        {
            BrailleIOMediator.Instance.RefreshDisplay();
        }

        /// <summary>
        /// helping stack that helps to determine if a rendering is necessary.
        /// Collects all render calls and the rendering thread cann decide if to render or not.
        /// </summary>
        private readonly ConcurrentStack<object> stack = new ConcurrentStack<Object>();
        /// <summary>
        /// separate thread for building the resulting matrix
        /// </summary>
        private Thread renderingTread;
        /// <summary>
        /// Builds the resulting matrix that will be send to the adapters by calling the renderers for each view range.
        /// </summary>
        void renderDisplay()
        {
            while (stack.Count > 0)
            {
                stack.Clear();
                if (AdapterManager == null || AdapterManager.ActiveAdapter == null) return;

                pins_locked = true;
                foreach (String key in this.VisibleViews.Keys)
                {
                    if (this.views[key] is BrailleIOViewRange)
                    {
                        this.drawViewRange(((BrailleIOViewRange)this.views[key]));
                    }
                    else if (this.views[key] is BrailleIOScreen)
                    {
                        foreach (BrailleIOViewRange vr in ((BrailleIOScreen)this.views[key]).GetViewRanges().Values)
                        {
                            this.drawViewRange(vr);
                        }
                    }
                    else { }
                }

                BrailleIO.Renderer.GraphicUtils.PaintBoolMatrixToImage(this.Matrix, @"C:\Users\Admin\Desktop\temp\sent_.bmp");

                this.Matrix = Matrix;

                pins_locked = false;
            }
        }

        /// <summary>
        /// draw a ViewRange to this.matrix
        /// </summary>
        /// <param name="vr">ViewRange</param>
        private bool drawViewRange(BrailleIOViewRange vr)
        {
            if (vr == null || vr.ViewBox == null || !vr.IsVisible()) return false;

            //Stopwatch sw = new Stopwatch();
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
                        contentMatrix = vr.ContentRender.RenderMatrix(vr, vr.GetImage());
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
                else return false;
            }
            else return false;
            //place the content matrix (contentMatrix) in the view range matrix with aware of the box model 
            viewBoxMatrix = (new BrailleIO.Renderer.BrailleIOViewMatixRenderer()).RenderMatrix(vr, contentMatrix, handlePanning);
            // Border rendering
            viewBoxMatrix = BrailleIO.Renderer.BrailleIOBorderRenderer.renderMatrix(vr, viewBoxMatrix);
            bool pl = pins_locked;
            pins_locked = true;


            // draw content and borders to main matrix
            System.Threading.Tasks.Parallel.For(srcOffsetX, srcOffsetX + viewBoxMatrix.GetLength(1), x =>
            {
                if (x >= 0 && x < Matrix.GetLength(1))
                {
                    System.Threading.Tasks.Parallel.For(scrOffsetY, scrOffsetY + viewBoxMatrix.GetLength(0), y =>
                    {
                        if (y < Matrix.GetLength(0)) { Matrix[y, x] = viewBoxMatrix[y - scrOffsetY, x - srcOffsetX]; }
                    });
                }
            });

            //sw.Stop();
            //Console.WriteLine("Elapsed={0}", sw.Elapsed);

            pins_locked = pl;

            return true;
        }

        #endregion

        #endregion

        #region Getter & Setter

        /// <summary>
        /// get current display-matrix.
        /// </summary>
        /// <returns>bool[,] matrix</returns>
        public bool[,] GetMatrix()
        {
            return Matrix;
        }

        ///// <summary>
        ///// put all pins down.
        ///// (clear screen)
        ///// </summary>
        //public void AllPinsDown() // TODO: handle that
        //{
        //    this.pins_locked = true;
        //    //foreach (String key in views.Keys)
        //    //{
        //    //    this.HideView(key);
        //    //}
        //    //for (int i = 0; i < AdapterManager.ActiveAdapter.DeviceSizeX; i++)
        //    //    for (int j = 0; j < AdapterManager.ActiveAdapter.DeviceSizeY; j++)
        //    //        this.Matrix[j, i] = false;

        //    Matrix = new bool[1, 1];

        //    if (AdapterManager != null && AdapterManager.ActiveAdapter != null)
        //        AdapterManager.Synchronize(Matrix);
        //}

        ///// <summary>
        ///// put all Pins Up.
        ///// (black screen)
        ///// </summary>
        //public void AllPinsUp()
        //{
        //    this.pins_locked = true;
        //    foreach (String key in views.Keys)
        //    {
        //        this.HideView(key);
        //    }
        //    for (int i = 0; i < AdapterManager.ActiveAdapter.Device.DeviceSizeX; i++)
        //        for (int j = 0; j < AdapterManager.ActiveAdapter.Device.DeviceSizeY; j++)
        //            this.Matrix[j, i] = true;

        //    Matrix = Matrix;

        //    if (AdapterManager != null && AdapterManager.ActiveAdapter != null)
        //        AdapterManager.Synchronize(Matrix.Clone() as bool[,]);
        //}

        ///// <summary>
        ///// release allUp or allDown and show ViewRanges again
        ///// </summary>
        //public void RestoreLastRendering()
        //{
        //    this.pins_locked = false;
        //    this.RenderDisplay();
        //}

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
                            HideView(item.ToString());
                        }
                    }
                }

                if (!this.VisibleViews.ContainsKey(name))
                    this.VisibleViews.TryAdd(name, true);
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
            if (views.ContainsKey(name))
            {
                if (views[name] is IViewable)
                {
                    ((IViewable)views[name]).SetVisibility(false);
                }

                object trash;
                if (this.VisibleViews.ContainsKey(name))
                    this.VisibleViews.TryRemove(name, out trash);
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
        public bool AddView(String name, Object view)
        {
            if (view is BrailleIOViewRange || view is BrailleIOScreen)
            {
                return this.views.TryAdd(name, view);
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
            object trash;
            return this.views.TryRemove(name, out trash);
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
            object trash;
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
        public List<Object> GetViews()
        {
            return views != null ? views.Values.ToList() : new List<Object>();
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

    }
}