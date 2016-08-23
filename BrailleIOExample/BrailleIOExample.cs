using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrailleIO;
using System.Drawing;
using BrailleIO.Interface;
using Gestures.Recognition;
using Gestures.Recognition.Preprocessing;
using Gestures.Recognition.Interfaces;
using Gestures.Recognition.GestureData;

namespace BraillIOExample
{
    class BrailleIOExample
    {
        #region Members
        AbstractBrailleIOAdapterBase showOff;
        BrailleIOMediator io;

        BrailleIO.IBrailleIOShowOffMonitor Monitor;

        const String BS_MAIN_NAME = "Mainscreen";

        GestureRecognizer showOffGestureRecognizer;

        #endregion

        public BrailleIOExample()
        {
            io = BrailleIOMediator.Instance;
            io.AdapterManager = new ShowOffBrailleIOAdapterManager();
            Monitor = ((ShowOffBrailleIOAdapterManager)io.AdapterManager).Monitor;
            showOff = io.AdapterManager.ActiveAdapter as AbstractBrailleIOAdapterBase;
            registerToEvents();
            showExample();
        }

        private void registerToEvents()
        {
            if (showOff != null)
            {
                #region events

                showOff.touchValuesChanged += new EventHandler<BrailleIO_TouchValuesChanged_EventArgs>(_bda_touchValuesChanged);
                showOff.keyStateChanged += new EventHandler<BrailleIO_KeyStateChanged_EventArgs>(_bda_keyStateChanged);

                showOffGestureRecognizer = registerGestureRecognizer(showOff);

                #endregion
            }
        }

        #region Events

        void _bda_keyStateChanged(object sender, BrailleIO.Interface.BrailleIO_KeyStateChanged_EventArgs e)
        {
            //throw new NotImplementedException();
            interpretGeneralButtons(e.keyCode, sender as IBrailleIOAdapter);
            if ((e.keyCode & BrailleIO_DeviceButtonStates.Unknown) == BrailleIO_DeviceButtonStates.Unknown
                || ((e.keyCode & BrailleIO_DeviceButtonStates.None) == BrailleIO_DeviceButtonStates.None && e.raw != null)
                ) { interpretGenericButtons(sender, e.raw); }
        }

        void _bda_touchValuesChanged(object sender, BrailleIO.Interface.BrailleIO_TouchValuesChanged_EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("---> touch values changed");

            if (e != null)
            {
                // add touches to the gesture recognizers
                if (sender == showOff && interpretShowOfGesture && showOffGestureRecognizer != null)
                {
                    showOffGestureRecognizer.AddFrame(new Frame(e.touches));
                }
            }

        }

        #endregion

        #region Button Handling

        private void interpretGeneralButtons(BrailleIO_DeviceButtonStates states, IBrailleIOAdapter sender)
        {
            if (states != BrailleIO_DeviceButtonStates.None)
            {
                if ((states & BrailleIO_DeviceButtonStates.AbortDown) == BrailleIO_DeviceButtonStates.AbortDown) { invertImage(BS_MAIN_NAME, "center"); }
                else if ((states & BrailleIO_DeviceButtonStates.AbortUp) == BrailleIO_DeviceButtonStates.AbortUp) { }

                if ((states & BrailleIO_DeviceButtonStates.DownDown) == BrailleIO_DeviceButtonStates.DownDown) { moveVertical(BS_MAIN_NAME, "center", -5); }
                else if ((states & BrailleIO_DeviceButtonStates.DownUp) == BrailleIO_DeviceButtonStates.DownUp) { }

                if ((states & BrailleIO_DeviceButtonStates.EnterDown) == BrailleIO_DeviceButtonStates.EnterDown) { }
                else if ((states & BrailleIO_DeviceButtonStates.EnterUp) == BrailleIO_DeviceButtonStates.EnterUp) { }

                if ((states & BrailleIO_DeviceButtonStates.GestureDown) == BrailleIO_DeviceButtonStates.GestureDown)
                {
                    /*if (io != null) { io.AllPinsDown(); }*/
                    if (sender == showOff) { interpretShowOfGesture = true; }
                }
                else if ((states & BrailleIO_DeviceButtonStates.GestureUp) == BrailleIO_DeviceButtonStates.GestureUp)
                {
                    /*if (io != null) { io.RestoreLastRendering(); }*/
                    // evaluate the result
                    if (sender == showOff) { interpretShowOfGesture = false; }
                    recognizeGesture(sender);
                }

                if ((states & BrailleIO_DeviceButtonStates.LeftDown) == BrailleIO_DeviceButtonStates.LeftDown) { moveHorizontal(BS_MAIN_NAME, "center", 5); }
                else if ((states & BrailleIO_DeviceButtonStates.LeftUp) == BrailleIO_DeviceButtonStates.LeftUp) { }

                if ((states & BrailleIO_DeviceButtonStates.RightDown) == BrailleIO_DeviceButtonStates.RightDown) { moveHorizontal(BS_MAIN_NAME, "center", -5); }
                else if ((states & BrailleIO_DeviceButtonStates.RightUp) == BrailleIO_DeviceButtonStates.RightUp) { }

                if ((states & BrailleIO_DeviceButtonStates.UpDown) == BrailleIO_DeviceButtonStates.UpDown) { moveVertical(BS_MAIN_NAME, "center", 5); }
                else if ((states & BrailleIO_DeviceButtonStates.UpUp) == BrailleIO_DeviceButtonStates.UpUp) { }

                if ((states & BrailleIO_DeviceButtonStates.ZoomInDown) == BrailleIO_DeviceButtonStates.ZoomInDown)
                {
                    zoom(BS_MAIN_NAME, "center", 1.3);
                    //zoomPlus(BS_MAIN_NAME, "center", 0.00005); 
                }
                else if ((states & BrailleIO_DeviceButtonStates.ZoomInUp) == BrailleIO_DeviceButtonStates.ZoomInUp) { }

                if ((states & BrailleIO_DeviceButtonStates.ZoomOutDown) == BrailleIO_DeviceButtonStates.ZoomOutDown)
                {
                    //zoomPlus(BS_MAIN_NAME, "center", -0.00005);
                    zoom(BS_MAIN_NAME, "center", 0.6);
                }
                else if ((states & BrailleIO_DeviceButtonStates.ZoomOutUp) == BrailleIO_DeviceButtonStates.ZoomOutUp) { }
            }
        }

        /// <summary>
        /// Interprets the generic buttons. This are buttons that are not modeled as one of the standard buttons.
        /// This buttons have to be extracted from the raw data sent by the corresponding device and his 
        /// interpreting adapter implementation
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="orderedDictionary">The ordered dictionary.</param>
        private void interpretGenericButtons(object sender, System.Collections.Specialized.OrderedDictionary orderedDictionary)
        {
            List<String> pressedKeys = new List<String>();
            List<String> releasedKeys = new List<String>();

            // here you have to check for what kind of device which buttons are placed in the raw data sent by the device

            if (sender is BrailleIO.BrailleIOAdapter_ShowOff)
            {
                //interpret the raw data as data from as ShowOffAdapter
                pressedKeys = orderedDictionary["pressedKeys"] as List<String>;
                releasedKeys = orderedDictionary["releasedKeys"] as List<String>;
            }
            else
            {
                // ... check for other device types
            }

            if (releasedKeys != null && releasedKeys.Count > 0)
            {
                if (releasedKeys.Contains("nsrr")) { moveHorizontal(BS_MAIN_NAME, "center", -25); }
                if (releasedKeys.Contains("nsr")) { moveHorizontal(BS_MAIN_NAME, "center", -5); }
                if (releasedKeys.Contains("nsll")) { moveHorizontal(BS_MAIN_NAME, "center", 25); }
                if (releasedKeys.Contains("nsl")) { moveHorizontal(BS_MAIN_NAME, "center", 5); }
                if (releasedKeys.Contains("nsuu")) { moveVertical(BS_MAIN_NAME, "center", 25); }
                if (releasedKeys.Contains("nsu")) { moveVertical(BS_MAIN_NAME, "center", 5); }
                if (releasedKeys.Contains("nsdd")) { moveVertical(BS_MAIN_NAME, "center", -25); }
                if (releasedKeys.Contains("nsd")) { moveVertical(BS_MAIN_NAME, "center", -5); }

            }

        }

        #endregion

        #region Gesture Recognizer

        #region private Fields

        /// <summary>
        /// flag for decision if touch values of the ShowOff adapter should bee interpreted by the related gesture recognizer
        /// </summary>
        volatile bool interpretShowOfGesture = false;

        #endregion

        /// <summary>
        /// gesture recognizer registration for the device
        /// </summary>
        /// <param name="adapter">The adapter.</param>
        /// <returns>A gesture recognizer for recognizing gestures</returns>
        private GestureRecognizer registerGestureRecognizer(AbstractBrailleIOAdapterBase adapter)
        {
            if (adapter != null)
            {
                var blobTracker = new BlobTracker();
                var gestureRecognizer = new GestureRecognizer(blobTracker);

                var tabClassifier = new TapClassifier();
                var multitouchClassifier = new MultitouchClassifier();

                // add several classifiers to interpret the tracked blobs
                gestureRecognizer.AddClassifier(tabClassifier);
                gestureRecognizer.AddClassifier(multitouchClassifier);

                // start tracking fo blobs
                blobTracker.InitiateTracking();
                return gestureRecognizer;
            }

            return null;
        }

        /// <summary>
        /// Recognizes the gesture.
        /// </summary>
        /// <param name="sender">The sender.</param>
        private void recognizeGesture(IBrailleIOAdapter sender)
        {
            IClassificationResult gesture = null;
            if (sender != null)
            {
                if (sender == showOff && showOffGestureRecognizer != null)
                {
                    gesture = showOffGestureRecognizer.FinishEvaluation();
                }
            }

            //TODO: do whatever you want with this gesture result
            if (Monitor != null)
            {
                if (gesture != null) { Monitor.SetStatusText("GESTURE form '" + sender + "' :" + gesture.ToString()); }
                else { Monitor.SetStatusText("No gesture recognized"); }
            }

        }

        #endregion

        #region Functions

        IBrailleIOAdapter getActiveAdapter()
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
            BrailleIOViewRange vr = ((BrailleIOScreen)io.GetView(viewName)).GetViewRange(viewRangeName);
            if (vr != null)
            {
                if (vr.GetZoom() > 0)
                {
                    //TODO: make zoom to center
                    var oldZoom = vr.GetZoom();
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

                    vr.SetZoom(newZoom);

                    vr.SetXOffset(newOffset.X);
                    vr.SetYOffset(newOffset.Y);
                }
            }

            this.
            io.RenderDisplay();
        }
        void zoomTo(string viewName, string viewRangeName, double factor)
        {
            if (io == null && io.GetView(viewName) as BrailleIOScreen != null) return;
            // zoom in
            BrailleIOViewRange vr = ((BrailleIOScreen)io.GetView(viewName)).GetViewRange(viewRangeName);
            if (vr != null)
            {
                //TODO: make zoom to center
                var oldZoom = vr.GetZoom();
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

                vr.SetZoom(newZoom);

                vr.SetXOffset(newOffset.X);
                vr.SetYOffset(newOffset.Y);
            }
            io.RenderDisplay();
        }
        void zoomPlus(string viewName, string viewRangeName, double factor)
        {
            if (io == null && io.GetView(viewName) as BrailleIOScreen != null) return;
            // zoom in
            BrailleIOViewRange vr = ((BrailleIOScreen)io.GetView(viewName)).GetViewRange(viewRangeName);
            if (vr != null)
            {
                if (vr.GetZoom() > 0)
                {
                    //TODO: make zoom to center
                    var oldZoom = vr.GetZoom();
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

                    vr.SetZoom(newZoom);

                    vr.SetXOffset(newOffset.X);
                    vr.SetYOffset(newOffset.Y);
                }
            }
            io.RenderDisplay();
        }

        void moveHorizontal(string viewName, string viewRangeName, int steps)
        {
            if (io == null && io.GetView(viewName) as BrailleIOScreen != null) return;
            BrailleIOViewRange vr = ((BrailleIOScreen)io.GetView(viewName)).GetViewRange(viewRangeName);
            if (vr != null)
            {
                vr.MoveHorizontal(steps);
            }
            io.RenderDisplay();
        }
        void moveVertical(string viewName, string viewRangeName, int steps)
        {
            if (io == null && io.GetView(viewName) as BrailleIOScreen != null) return;
            BrailleIOViewRange vr = ((BrailleIOScreen)io.GetView(viewName)).GetViewRange(viewRangeName);
            if (vr != null)
            {
                vr.MoveVertical(steps);
            }
            io.RenderDisplay();
        }

        private void invertImage(string viewName, string viewRangeName)
        {
            if (io == null && io.GetView(viewName) as BrailleIOScreen != null) return;
            BrailleIOViewRange vr = ((BrailleIOScreen)io.GetView(viewName)).GetViewRange(viewRangeName);
            if (vr != null)
            {
                vr.InvertImage = !vr.InvertImage;
            }
            io.RenderDisplay();
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

            center.SetBitmap(bmp);

            center.SetZoom(-1);
            center.SetBorder(0);
            center.SetContrastThreshold(150);
            center.ShowScrollbars = true;

            s.AddViewRange("center", center);

            #endregion

            #region Top Reagion
            BrailleIOViewRange top = new BrailleIOViewRange(0, 0, 120, 7);

            top.SetBorder(0, 0, 1);
            top.SetMargin(0, 0, 1);
            top.SetPadding(0, 0, 1);

            top.SetText("ABCDEFGHIJKLMNOPQRSTUVWXYZ\r\nabcdefghijklmnopqrstuvwxyz\r\n0123456789!\"#$%&<=>?@©®\r\n*+-~:;[],.'^_`(){}/|\\r\nß\r\n%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%\r\n");
            top.SetText("Tactile screen capture");
            s.AddViewRange("top", top);

            #endregion

            #region Bottom Reagion
            BrailleIOViewRange bottom = new BrailleIOViewRange(0, 53, 120, 7);

            bottom.SetBorder(1, 0, 0);
            bottom.SetMargin(1, 0, 0);
            bottom.SetPadding(1, 0, 0);

            bottom.SetText("Detail area: status messages can be shown");

            s.AddViewRange("bottom", bottom);
            #endregion

            io.AddView(BS_MAIN_NAME, s);
            io.ShowView(BS_MAIN_NAME);
            io.RenderDisplay();

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
                var v = io.GetView(BS_MAIN_NAME) as BrailleIOScreen;
                if (v != null)
                {
                    var cs = v.GetViewRange("center");
                    if (cs != null)
                    {
                        cs.SetBitmap(captureScreen());
                        io.RefreshDisplay(true);
                    }
                }
            }
        }
        #endregion

        #endregion
    }
}
