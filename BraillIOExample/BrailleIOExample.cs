using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrailleIO;
using System.Drawing;
using BrailleIO.Interface;

namespace BraillIOExample
{
    class BrailleIOExample
    {
        #region Members
        AbstractBrailleIOAdapterBase showOff;
        BrailleIOMediator io;

        ShowOff monitor;

        #endregion

        public BrailleIOExample()
        {
            io = BrailleIOMediator.Instance;
            io.AdapterManager = new ShowOffBrailleIOAdapterManager();
            showOff = createShowOff();
            createBrailleDis();
            showExample();
        }

        #region Adapters

        #region ShowOff

        private AbstractBrailleIOAdapterBase createShowOff()
        {
            monitor = new ShowOff();
            if (io != null && io.AdapterManager != null)
            {
                var soa = monitor.GetAdapter(ref io.AdapterManager);
                soa.Synch = true;
                io.AdapterManager.AddAdapter(soa);

                #region events
                soa.touchValuesChanged += new AbstractBrailleIOAdapterBase.BrailleIO_TouchValuesChanged_EventHandler(_bda_touchValuesChanged);
                soa.keyStateChanged += new AbstractBrailleIOAdapterBase.BrailleIO_KeyStateChanged_EventHandler(_bda_keyStateChanged);
                #endregion

                return soa;
            }
            return null;
        }

        #endregion

        #region BrailleDis

        private AbstractBrailleIOAdapterBase createBrailleDis()
        {
            if (io != null && io.AdapterManager != null)
            {
                AbstractBrailleIOAdapterBase _bda = new BrailleIOBraillDisAdapter.BrailleIOAdapter_BrailleDisNet(ref io.AdapterManager);
                io.AdapterManager.ActiveAdapter = _bda;

                #region BrailleDis events
                _bda.touchValuesChanged += new AbstractBrailleIOAdapterBase.BrailleIO_TouchValuesChanged_EventHandler(_bda_touchValuesChanged);
                _bda.keyStateChanged += new AbstractBrailleIOAdapterBase.BrailleIO_KeyStateChanged_EventHandler(_bda_keyStateChanged);
                #endregion

                return _bda;
            }
            return null;
        }

        #region Events

        void _bda_keyStateChanged(object sender, BrailleIO.Interface.BrailleIO_KeyStateChanged_EventArgs e)
        {
            //throw new NotImplementedException();
            interpretGeneralButtons(e.keyCode);
            if ((e.keyCode & BrailleIO_DeviceButtonStates.Unknown) == BrailleIO_DeviceButtonStates.Unknown
                || ((e.keyCode & BrailleIO_DeviceButtonStates.None) == BrailleIO_DeviceButtonStates.None && e.raw != null)
                ) { interpretGenericButtons(sender, e.raw); }
        }

        private void interpretGenericButtons(object sender, System.Collections.Specialized.OrderedDictionary orderedDictionary)
        {
            //var adapter = sender as BrailleIOAdapter_BrailleDisNet;
            if (orderedDictionary.Contains("allPressedKeys"))
            {
                var keys = orderedDictionary["allPressedKeys"] as List<String>;
                if (keys != null)
                {
                    if (keys.Contains("nsrr")) { moveHorizontal("foo", "center", -25); }
                    if (keys.Contains("nsr")) { moveHorizontal("foo", "center", -5); }
                    if (keys.Contains("nsll")) { moveHorizontal("foo", "center", 25); }
                    if (keys.Contains("nsl")) { moveHorizontal("foo", "center", 5); }
                    if (keys.Contains("nsuu")) { moveVertical("foo", "center", 25); }
                    if (keys.Contains("nsu")) { moveVertical("foo", "center", 5); }
                    if (keys.Contains("nsdd")) { moveVertical("foo", "center", -25); }
                    if (keys.Contains("nsd")) { moveVertical("foo", "center", -5); }

                    if (keys.Contains("crc")) { zoomToRealSize(sender); }
                    if (keys.Contains("rsru")) { updateContrast("foo", "center", 10); }
                    if (keys.Contains("rsrd")) { updateContrast("foo", "center", -10); }
                }
            }

        }

        private void updateContrast(string viewName, string viewRangeName, int factor)
        {
            if (io == null && io.GetView(viewName) as BrailleIOScreen != null) return;
            // zoom in
            BrailleIOViewRange vr = ((BrailleIOScreen)io.GetView(viewName)).getViewRange(viewRangeName);
            if (vr != null)
            {
                vr.SetContrastThreshold(vr.getContrastThreshold() + factor);
            }
            io.SendToDevice();
        }

        private void zoomToRealSize(object sender)
        {
            IBrailleIOAdapter adapter = sender as IBrailleIOAdapter;
            if (adapter != null)
            {
                float adapterRes = Utils.GetResoultion(adapter.DpiX, adapter.DpiY);
                adapterRes = 10.0f;
                float screenRes = Utils.GetScreenDpi();

                float zoom = adapterRes / (float)Math.Max(screenRes, 0.0000001);
                zoom = 0.10561666418313964f;
                zoomTo("foo", "center", zoom);

            }
        }

        void _bda_touchValuesChanged(object sender, BrailleIO.Interface.BrailleIO_TouchValuesChanged_EventArgs e)
        {
            if (monitor != null) monitor.SetTouchMatrix(e.touches);
            //throw new NotImplementedException();
        }

        #endregion

        #region Helper functions
        private void interpretGeneralButtons(BrailleIO_DeviceButtonStates states)
        {
            if (states != BrailleIO_DeviceButtonStates.None)
            {
                if ((states & BrailleIO_DeviceButtonStates.AbortDown) == BrailleIO_DeviceButtonStates.AbortDown) { invertImage("foo", "center"); }
                else if ((states & BrailleIO_DeviceButtonStates.AbortUp) == BrailleIO_DeviceButtonStates.AbortUp) { }

                if ((states & BrailleIO_DeviceButtonStates.DownDown) == BrailleIO_DeviceButtonStates.DownDown) { moveVertical("foo", "center", -5); }
                else if ((states & BrailleIO_DeviceButtonStates.DownUp) == BrailleIO_DeviceButtonStates.DownUp) { }

                if ((states & BrailleIO_DeviceButtonStates.EnterDown) == BrailleIO_DeviceButtonStates.EnterDown) { }
                else if ((states & BrailleIO_DeviceButtonStates.EnterUp) == BrailleIO_DeviceButtonStates.EnterUp) { }

                if ((states & BrailleIO_DeviceButtonStates.GestureDown) == BrailleIO_DeviceButtonStates.GestureDown) { /*if (io != null) { io.AllPinsDown(); }*/ }
                else if ((states & BrailleIO_DeviceButtonStates.GestureUp) == BrailleIO_DeviceButtonStates.GestureUp) { /*if (io != null) { io.RestoreLastRendering(); }*/ }

                if ((states & BrailleIO_DeviceButtonStates.LeftDown) == BrailleIO_DeviceButtonStates.LeftDown) { moveHorizontal("foo", "center", 5); }
                else if ((states & BrailleIO_DeviceButtonStates.LeftUp) == BrailleIO_DeviceButtonStates.LeftUp) { }

                if ((states & BrailleIO_DeviceButtonStates.RightDown) == BrailleIO_DeviceButtonStates.RightDown) { moveHorizontal("foo", "center", -5); }
                else if ((states & BrailleIO_DeviceButtonStates.RightUp) == BrailleIO_DeviceButtonStates.RightUp) { }

                if ((states & BrailleIO_DeviceButtonStates.UpDown) == BrailleIO_DeviceButtonStates.UpDown) { moveVertical("foo", "center", 5); }
                else if ((states & BrailleIO_DeviceButtonStates.UpUp) == BrailleIO_DeviceButtonStates.UpUp) { }

                if ((states & BrailleIO_DeviceButtonStates.ZoomInDown) == BrailleIO_DeviceButtonStates.ZoomInDown)
                {
                    zoom("foo", "center", 1.3);
                    //zoomPlus("foo", "center", 0.00005); 
                }
                else if ((states & BrailleIO_DeviceButtonStates.ZoomInUp) == BrailleIO_DeviceButtonStates.ZoomInUp) { }

                if ((states & BrailleIO_DeviceButtonStates.ZoomOutDown) == BrailleIO_DeviceButtonStates.ZoomOutDown)
                {
                    //zoomPlus("foo", "center", -0.00005);
                    zoom("foo", "center", 0.6);
                }
                else if ((states & BrailleIO_DeviceButtonStates.ZoomOutUp) == BrailleIO_DeviceButtonStates.ZoomOutUp) { }
            }
        }


        #endregion

        #endregion

        #endregion


        #region Functions

        AbstractBrailleIOAdapterBase getActiveAdapter()
        {
            if (io != null && io.AdapterManager != null)
            {
                return io.AdapterManager.ActiveAdapter;
            }
            return null;
        }


        void zoom(string viewName, string viewRangeName, double factor)
        {
            if (io == null && io.GetView(viewName) as BrailleIOScreen != null) return;
            // zoom in
            BrailleIOViewRange vr = ((BrailleIOScreen)io.GetView(viewName)).getViewRange(viewRangeName);
            if (vr != null)
            {
                if (vr.getZoom() > 0)
                {
                    //TODO: make zoom to center
                    var oldZoom = vr.getZoom();
                    var newZoom = oldZoom * factor;
                    var oldvrdin = vr.ViewBox;
                    Point oldcenter = new Point(
                        (int)Math.Round(((double)oldvrdin.Width / 2) + (vr.GetXOffset() * -1)),
                        (int)Math.Round(((double)oldvrdin.Height / 2) + (vr.GetYOffset() * -1))
                        );

                    Point newCenter = new Point(
                        (int)Math.Round(oldcenter.X * newZoom / oldZoom),
                        (int)Math.Round(oldcenter.Y * newZoom / oldZoom)
                        );

                    Point newOffset = new Point(
                        (int)Math.Round((newCenter.X - ((double)oldvrdin.Width / 2)) * -1),
                        (int)Math.Round((newCenter.Y - ((double)oldvrdin.Height / 2)) * -1)
                        );

                    vr.setZoom(newZoom);

                    vr.SetXOffset(newOffset.X);
                    vr.SetYOffset(newOffset.Y);
                }
            }

            this.
            io.SendToDevice();
        }
        void zoomTo(string viewName, string viewRangeName, double factor)
        {
            if (io == null && io.GetView(viewName) as BrailleIOScreen != null) return;
            // zoom in
            BrailleIOViewRange vr = ((BrailleIOScreen)io.GetView(viewName)).getViewRange(viewRangeName);
            if (vr != null)
            {
                //TODO: make zoom to center
                var oldZoom = vr.getZoom();
                var newZoom = factor;
                var oldvrdin = vr.ViewBox;
                Point oldcenter = new Point(
                    (int)Math.Round(((double)oldvrdin.Width / 2) + (vr.GetXOffset() * -1)),
                    (int)Math.Round(((double)oldvrdin.Height / 2) + (vr.GetYOffset() * -1))
                    );

                Point newCenter = new Point(
                    (int)Math.Round(oldcenter.X * newZoom / oldZoom),
                    (int)Math.Round(oldcenter.Y * newZoom / oldZoom)
                    );

                Point newOffset = new Point(
                    (int)Math.Round((newCenter.X - ((double)oldvrdin.Width / 2)) * -1),
                    (int)Math.Round((newCenter.Y - ((double)oldvrdin.Height / 2)) * -1)
                    );

                vr.setZoom(newZoom);

                vr.SetXOffset(newOffset.X);
                vr.SetYOffset(newOffset.Y);
            }
            io.SendToDevice();
        }
        void zoomPlus(string viewName, string viewRangeName, double factor)
        {
            if (io == null && io.GetView(viewName) as BrailleIOScreen != null) return;
            // zoom in
            BrailleIOViewRange vr = ((BrailleIOScreen)io.GetView(viewName)).getViewRange(viewRangeName);
            if (vr != null)
            {
                if (vr.getZoom() > 0)
                {
                    //TODO: make zoom to center
                    var oldZoom = vr.getZoom();
                    var newZoom = oldZoom + factor;
                    var oldvrdin = vr.ViewBox;
                    Point oldcenter = new Point(
                        (int)Math.Round(((double)oldvrdin.Width / 2) + (vr.GetXOffset() * -1)),
                        (int)Math.Round(((double)oldvrdin.Height / 2) + (vr.GetYOffset() * -1))
                        );

                    Point newCenter = new Point(
                        (int)Math.Round(oldcenter.X * newZoom / oldZoom),
                        (int)Math.Round(oldcenter.Y * newZoom / oldZoom)
                        );

                    Point newOffset = new Point(
                        (int)Math.Round((newCenter.X - ((double)oldvrdin.Width / 2)) * -1),
                        (int)Math.Round((newCenter.Y - ((double)oldvrdin.Height / 2)) * -1)
                        );

                    vr.setZoom(newZoom);

                    vr.SetXOffset(newOffset.X);
                    vr.SetYOffset(newOffset.Y);
                }
            }
            io.SendToDevice();
        }

        void moveHorizontal(string viewName, string viewRangeName, int steps)
        {
            if (io == null && io.GetView(viewName) as BrailleIOScreen != null) return;
            BrailleIOViewRange vr = ((BrailleIOScreen)io.GetView(viewName)).getViewRange(viewRangeName);
            if (vr != null)
            {
                vr.MoveHorizontal(steps);
            }
            io.SendToDevice();
        }

        void moveVertical(string viewName, string viewRangeName, int steps)
        {
            if (io == null && io.GetView(viewName) as BrailleIOScreen != null) return;
            BrailleIOViewRange vr = ((BrailleIOScreen)io.GetView(viewName)).getViewRange(viewRangeName);
            if (vr != null)
            {
                vr.MoveVertical(steps);
            }
            io.SendToDevice();
        }

        private void invertImage(string viewName, string viewRangeName)
        {
            if (io == null && io.GetView(viewName) as BrailleIOScreen != null) return;
            BrailleIOViewRange vr = ((BrailleIOScreen)io.GetView(viewName)).getViewRange(viewRangeName);
            if (vr != null)
            {
                vr.InvertImage = !vr.InvertImage;
            }
            io.SendToDevice();
        }

        #endregion

        #region Example
        //     string path = "";
        private void showExample()
        {
            BrailleIOScreen s = new BrailleIOScreen();
            #region Center Region

            #region screenshot
            Image bmp = captureScreen();
            #endregion

            BrailleIOViewRange center = new BrailleIOViewRange(0, 7, 120, 46, new bool[120, 40]);
            //center.Move(1,1);

            center.setBitmap(bmp);

            center.setZoom(-1);
            center.SetBorder(0);
            center.SetContrastThreshold(150);

            s.addViewRange("center", center);

            #endregion

            #region Top Reagion
            BrailleIOViewRange top = new BrailleIOViewRange(0, 0, 120, 7, new bool[0, 0]);

            top.SetBorder(0, 0, 1);
            top.SetMargin(0, 0, 1);
            top.SetPadding(0, 0, 1);

            top.setText("ABCDEFGHIJKLMNOPQRSTUVWXYZ\r\nabcdefghijklmnopqrstuvwxyz\r\n0123456789!\"#$%&<=>?@©®\r\n*+-~:;[],.'^_`(){}/|\\r\nß\r\n%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%\r\n");
            top.setText("Tactile screen capture");
            s.addViewRange("top", top);

            #endregion

            #region Bottom Reagion
            var nm_b = new bool[120, 20];
            BrailleIOViewRange bottom = new BrailleIOViewRange(0, 53, 120, 7, nm_b);

            bottom.setMatrix(nm_b);

            bottom.SetBorder(1, 0, 0);
            bottom.SetMargin(1, 0, 0);
            bottom.SetPadding(1, 0, 0);

            bottom.setText("Detail area: status messages can be shown");

            s.addViewRange("bottom", bottom);
            #endregion

            io.AddView("foo", s);
            io.ShowView("foo");
            io.SendToDevice();

        }

        #region Screen capturing

        ScreenCapture sc;
        private Image captureScreen()
        {
            if (sc == null)
                sc = new ScreenCapture();
            // capture entire screen, and save it to a file
            Image bmp = sc.CaptureScreen();
            return bmp;
        }

        internal void updateScreenshotInCenterVr()
        {
            if (io != null)
            {
                var v = io.GetView("foo") as BrailleIOScreen;
                if (v != null)
                {
                    var cs = v.getViewRange("center");
                    if (cs != null)
                    {
                        cs.setBitmap(captureScreen());
                        io.RefreshDisplay(true);
                    }
                }
            }
        }
        #endregion

        #endregion
    }
}
