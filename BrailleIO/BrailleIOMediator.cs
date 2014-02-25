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

        private Object _matrixLock = new Object();
        private bool[,] _matrix;
        // matrix to be displayed on device
        private bool[,] Matrix
        {
            get
            {
                lock (_matrixLock)
                {
                    if (_matrix == null)
                    {
                        if (AdapterManager != null && AdapterManager.ActiveAdapter != null)
                            _matrix = new bool[AdapterManager.ActiveAdapter.Device.DeviceSizeX, AdapterManager.ActiveAdapter.Device.DeviceSizeX];
                        else
                            _matrix = new bool[0, 0];
                    }
                    return _matrix;
                }
            }
            set
            {
                lock (_matrixLock)
                {
                    _matrix = value;
                }
            }
        }

        #endregion

        private BrailleIOMediator() { }

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
                            instance.device_update_timer.Interval = 200;
                            instance.device_update_timer.Enabled = true;
                            //instance.manager = new BrailleIOAdapterManager(ref instance);
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

        /// <summary>
        /// transmit matrix to display
        /// </summary>
        public void RefreshDisplay()
        {
            if (ArePinsLocked()) return;
            if (AdapterManager != null && AdapterManager.ActiveAdapter != null)
            {
                if (instance.device_update_timer.Interval != AdapterManager.ActiveAdapter.Device.RefreshRate * 10)
                    instance.device_update_timer.Interval = AdapterManager.ActiveAdapter.Device.RefreshRate * 10;
                AdapterManager.Synchronize(this.Matrix);
            }
        }

        /// <summary>
        /// draw a ViewRange to this.matrix
        /// </summary>
        /// <param name="vr">ViewRange</param>
        private void drawViewRange(BrailleIOViewRange vr)
        {
            if (vr == null || vr.ViewBox == null || Matrix == null || !vr.IsVisible()) return;
            bool[,] m = new bool[vr.ViewBox.Height, vr.ViewBox.Width];

            // View Range bounds
            int o_x = vr.GetLeft();
            int o_y = vr.GetTop();

            bool[,] cm = new bool[1, 1];
            if (vr.isMatrix()) // Matrix rendering
            {
                if (vr.getMatrix() != null)
                    cm = vr.getMatrix();
            }
            else if (vr.isImage()) // Image rendering
            {
                if (vr.getImage() != null)
                {
                    int th = (vr is IContrastThreshold) ? ((IContrastThreshold)vr).getContrastThreshold() : -1;
                    if (th > 0)
                    {
                        cm = vr.ImageRenderer.renderImage(vr.getImage(), vr, vr as IPannable, vr.InvertImage, vr.getZoom(), th);
                    }
                    else
                    {
                        cm = vr.ImageRenderer.renderImage(vr.getImage(), vr, vr as IPannable, vr.InvertImage, vr.getZoom(), true);
                    }
                }
            }
            else if (vr.isText())
            {
                if (!string.IsNullOrEmpty(vr.getText()))
                {
                    cm = (new BrailleIO.Renderer.BrailleIOTextRenderer()).renderMatrix(vr, vr.getText());
                }
            }
            else return;
            //place the content matrix (cm) in the view range matrix with aware of the box model 
            m = (new BrailleIO.Renderer.BrailleIOViewMatixRenderer()).renderMatrix(vr, cm);
            // Border rendering
            m = (new BrailleIO.Renderer.BrailleIOBorderRenderer()).renderMatrix(vr, m);


            bool pl = pins_locked;
            pins_locked = true;
            // draw content and borders to main matrix
            for (int i = o_x; i < o_x + m.GetLength(1); i++)
            {
                if (i >= Matrix.GetLength(1)) break;
                for (int j = o_y; j < o_y + m.GetLength(0); j++)
                {
                    if (j >= Matrix.GetLength(0)) break;
                    this.Matrix[j, i] = m[j - o_y, i - o_x];
                }
            }
            pins_locked = pl;
        }

        /// <summary>
        /// parse display-matrix and save for transmission to display.
        /// </summary>
        public void SendToDevice()
        {
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
                    
                    foreach (BrailleIOViewRange vr in ((BrailleIOScreen)this.views[key]).getViewRanges().Values)
                    {
                        this.drawViewRange(vr);
                    }
                }
                else { }
            }
            pins_locked = false;
        }

        /// <summary>
        /// get current display-matrix.
        /// </summary>
        /// <returns>bool[,] matrix</returns>
        public bool[,] GetMatrix()
        {
            SendToDevice();
            return this.Matrix;
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
            if (AdapterManager != null && AdapterManager.ActiveAdapter != null)
                AdapterManager.Synchronize(this.Matrix);
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

        /// <summary>
        /// hide a view.
        /// hidden views still exist but will not show on display
        /// </summary>
        /// <param name="name">
        /// name of view
        /// </param>
        public void HideView(String name)
        {
            if (views[name] is IViewable)
            {
                ((IViewable)views[name]).SetVisibility(false);
            }

            object trash;
            if (this.VisibleViews.ContainsKey(name))
                this.VisibleViews.TryRemove(name, out trash);
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
    }
}
