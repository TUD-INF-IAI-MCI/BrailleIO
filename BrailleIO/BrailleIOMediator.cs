using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Timers;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Drawing;
using System.Text.RegularExpressions;
using BrailleIO.Interface;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;

namespace BrailleIO
{

    public class BrailleIOMediator
    {
        #region Members
        private static BrailleIOMediator instance;
        private static object syncRoot = new Object();
        private System.Timers.Timer device_update_timer = new System.Timers.Timer();

        public AbstractBrailleIOAdapterManagerBase AdapterManager;

        // views are either Screens (combined ViewRanges) or simply ViewRanges
        // Screens should be more comfortable to use for the developer
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

        private volatile bool pins_locked = false;

        private bool[,] resultMatrix;


        // private Object _matrixLock = new Object();
        private bool[,] _matrix;
        // matrix to be displayed on device
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

        private BrailleIOMediator() { }
        ~BrailleIOMediator() { if (renderingTread != null) renderingTread.Abort(); }

        //double checked multi threaded singleton to avoid usage of expensive lock operation
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

        private static void refreshDisplayEvent(object source, ElapsedEventArgs e)
        {
            BrailleIOMediator.Instance.RefreshDisplay();
        }

        public void RefreshDisplay(bool rerender)
        {
            if (rerender)
            {
                if (AdapterManager != null && AdapterManager.ActiveAdapter != null)
                {
                    if (instance.device_update_timer.Interval != AdapterManager.ActiveAdapter.Device.RefreshRate * 10)
                        instance.device_update_timer.Interval = AdapterManager.ActiveAdapter.Device.RefreshRate * 10;
                    SendToDevice();
                }
                else RefreshDisplay();
            }
        }

        int t = 0;

        /// <summary>
        /// transmit matrix to display
        /// </summary>
        public void RefreshDisplay()
        {
            if (ArePinsLocked()) return;
            if (AdapterManager != null && AdapterManager.ActiveAdapter != null && resultMatrix != null)
            {
                if (instance.device_update_timer.Interval != AdapterManager.ActiveAdapter.Device.RefreshRate * 10)
                    instance.device_update_timer.Interval = AdapterManager.ActiveAdapter.Device.RefreshRate * 10;

                AdapterManager.Synchronize(this.resultMatrix.Clone() as bool[,]);

            }
        }


        private readonly ConcurrentStack<object> stack = new ConcurrentStack<Object>();

        private Thread renderingTread;

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

                //BrailleIO.Renderer.GraphicUtils.PaintBoolMatrixToImage(this.resultMatrix, @"C:\Users\Admin\Desktop\tmp\matrixes\sent_" + (t++) + ".bmp");

                this.resultMatrix = Matrix;

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
            else if (vr.IsImage() && vr.GetImage() != null)
            {
                int th = (vr is IContrastThreshold) ? ((IContrastThreshold)vr).GetContrastThreshold() : -1;
                if (th >= 0)
                {
                    contentMatrix = vr.ImageRenderer.RenderImage(vr.GetImage(), vr, vr as IPannable, vr.InvertImage, vr.GetZoom(), th);
                }
                else
                {
                    contentMatrix = vr.ImageRenderer.RenderImage(vr.GetImage(), vr, vr as IPannable, vr.InvertImage, vr.GetZoom(), true);
                }
                handlePanning = false;
            }
            // Text rendering
            else if (vr.IsText())
            {
                if (!string.IsNullOrEmpty(vr.GetText()))
                {
                    contentMatrix = (new BrailleIO.Renderer.BrailleIOTextRenderer()).renderMatrix(vr, vr.GetText());
                }
            }
            // Generic renderer
            else if (vr.IsOther())
            {
                if (vr.GetOtherContent() != null && vr.ContentRender != null)
                {
                    contentMatrix = vr.ContentRender.renderMatrix(vr, vr.GetOtherContent());
                }
                else return false;
            }
            else return false;
            //place the content matrix (contentMatrix) in the view range matrix with aware of the box model 
            viewBoxMatrix = (new BrailleIO.Renderer.BrailleIOViewMatixRenderer()).renderMatrix(vr, contentMatrix, handlePanning);
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


        /// <summary>
        /// parse display-matrix and save for transmission to display.
        /// </summary>
        public bool SendToDevice()
        {
            //return renderDisplay();

            stack.Push(true);

            if (renderingTread == null || !renderingTread.IsAlive)
            {
                renderingTread = new Thread(delegate() { renderDisplay(); });
                renderingTread.Start();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("------ rendering still running and ignored");
            }

            return true;

        }

        /// <summary>
        /// get current display-matrix.
        /// </summary>
        /// <returns>bool[,] matrix</returns>
        public bool[,] GetMatrix()
        {
            if (SendToDevice()) { return this.resultMatrix; }
            return new bool[0, 0];

        }

        /// <summary>
        /// put all pins down.
        /// (clear screen)
        /// </summary>
        public void AllPinsDown() // TODO: handle that
        {
            this.pins_locked = true;
            //foreach (String key in views.Keys)
            //{
            //    this.HideView(key);
            //}
            //for (int i = 0; i < AdapterManager.ActiveAdapter.DeviceSizeX; i++)
            //    for (int j = 0; j < AdapterManager.ActiveAdapter.DeviceSizeY; j++)
            //        this.Matrix[j, i] = false;

            Matrix = new bool[1, 1];

            if (AdapterManager != null && AdapterManager.ActiveAdapter != null)
                AdapterManager.Synchronize(Matrix);
        }

        /// <summary>
        /// put all Pins Up.
        /// (black screen)
        /// </summary>
        public void AllPinsUp()
        {
            this.pins_locked = true;
            foreach (String key in views.Keys)
            {
                this.HideView(key);
            }
            for (int i = 0; i < AdapterManager.ActiveAdapter.Device.DeviceSizeX; i++)
                for (int j = 0; j < AdapterManager.ActiveAdapter.Device.DeviceSizeY; j++)
                    this.Matrix[j, i] = true;

            resultMatrix = Matrix;

            if (AdapterManager != null && AdapterManager.ActiveAdapter != null)
                AdapterManager.Synchronize(resultMatrix.Clone() as bool[,]);
        }

        /// <summary>
        /// release allUp or allDown and show ViewRanges again
        /// </summary>
        public void RestoreLastRendering()
        {
            this.pins_locked = false;
            this.SendToDevice();
        }

        /// <summary>
        /// check if pins are locked
        /// </summary>
        /// <returns>bool pins_locked</returns>
        public bool ArePinsLocked() //TODO: check for this is necessary
        {
            return pins_locked;
        }

        /// <summary>
        /// get device Width from active adapter
        /// </summary>
        /// <returns>int Width of device</returns>
        public int GetDeviceSizeX()
        {
            return AdapterManager.ActiveAdapter.Device.DeviceSizeX;
        }

        /// <summary>
        /// get device Height from active adapter
        /// </summary>
        /// <returns>int Height of device</returns>
        public int GetDeviceSizeY()
        {
            return AdapterManager.ActiveAdapter.Device.DeviceSizeY;
        }

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
            return this.views.ContainsKey(name);
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

        public List<Object> GetViews()
        {
            return views.Values.ToList();
        }

        /// <summary>
        /// checks if there are any views yet
        /// </summary>
        /// <returns>
        /// bool 
        /// </returns>
        public bool IsEmpty()
        {
            return (this.views.Count > 0) ? false : true;
        }

        /// <summary>
        /// count of views
        /// </summary>
        /// <returns>
        /// int views.count
        /// </returns>
        public int Count()
        {
            return this.views.Count;
        }

        ///<summary>
        /// <para>
        /// subscribe to touchPath Event. Returns BrailleIOTouchPoint
        /// stop manually with touchpoint.stopTouch()
        /// or listen on stopTouch.
        /// </para>
        /// <param name="accuracy">
        /// time in ms to get add TouchPoints to touch path
        /// </param>
        /// <returns>
        /// TangramSkApp.BrailleIOTouchPoint
        /// </returns>
        /// </summary>
        public BrailleIOTouchPath TouchStart(int accuracy) //TODO: implement this
        {
            return new BrailleIOTouchPath();
        }

        public bool Recalibrate()
        {
            return AdapterManager.ActiveAdapter.Recalibrate(0);
        }

        public bool RecalibrateAll()
        {
            bool result = true;
            foreach (var adapter in AdapterManager.GetAdapters())
            {
                result &= adapter.Recalibrate(0);
            }
            return result;
        }

    }
}
