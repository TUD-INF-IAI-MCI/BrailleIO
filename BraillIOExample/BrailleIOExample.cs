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

        BrailleIO.IBrailleIOShowOffMonitor monitor;

        const String BS_MAIN_NAME = "Mainscreen";

        #endregion

        public BrailleIOExample()
        {
            io = BrailleIOMediator.Instance;
            io.AdapterManager = new ShowOffBrailleIOAdapterManager();
            monitor = ((ShowOffBrailleIOAdapterManager)io.AdapterManager).Monitor;
            showOff = io.AdapterManager.ActiveAdapter as AbstractBrailleIOAdapterBase;
            showExample();
        }

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
