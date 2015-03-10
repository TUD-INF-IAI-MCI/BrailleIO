using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Concurrent;

namespace BrailleIO
{
    public partial class ShowOff : Form, IBrailleIOShowOffMonitor
    {
        //TODO: make gesture emulation
        #region Mouse events

        volatile bool mouseToGetureMode = false;

        void pictureBoxTouch_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("mouse down");
            if (e.Button.HasFlag(MouseButtons.Left))
                startMouseGestureMode(e);
        }

        void pictureBoxTouch_MouseEnter(object sender, System.EventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("mouse entered");
            resetMouseGestureMode();
        }

        void pictureBoxTouch_MouseLeave(object sender, System.EventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("mouse left");
            {
                resetMouseGestureMode();
            }
        }

        void pictureBoxTouch_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (mouseToGetureMode)
            {
                //System.Diagnostics.Debug.WriteLine("mouse move: " + e.Location);
                paintMousePosition(e.Location);
            }
        }

        void pictureBoxTouch_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("mouse up");
            if (e.Button.HasFlag(MouseButtons.Left))
                resetMouseGestureMode();
        }

        #region utils

        void resetMouseGestureMode()
        {
            try
            {
                //touchGraphics.Clear(Color.Transparent);
                //this.pictureBoxTouch.Image = _touchbmp;

                this.PaintTouchMatrix(buildTouchMatrix(null));
                mouseToGetureMode = false;
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception in resetting the mouse position " + ex);
            }
        }


        void startMouseGestureMode(System.Windows.Forms.MouseEventArgs e)
        {
            mouseToGetureMode = true;
            paintMousePosition(e.Location);
        }

        private void paintMousePosition(Point p)
        {
            try
            {
                Point pin = getPinForPoint(p);
                var touchPoints = handleEllipsePoints(pin);
                this.PaintTouchMatrix(buildTouchMatrix(touchPoints));
                // fire event
                fireTouchEvent(touchPoints);

                //foreach (var touch in touchPoints)
                //{
                //    touchGraphics.FillEllipse(Brushes.Red, new Rectangle(touch.X * (pixelFactor + 1), touch.Y * (pixelFactor + 1), pixelFactor - 1, pixelFactor - 1));
                //}

                //this.pictureBoxTouch.Image = _touchbmp;
            }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine("Exception in painting mouse position " + ex);
            }
        }

        /// <summary>
        /// Converts a pixel point into a pin
        /// </summary>
        /// <param name="p">The mouse point in pixel.</param>
        /// <returns></returns>
        private Point getPinForPoint(Point p)
        {
            Point pin = new Point(0, 0);
            if (this.pictureBoxTouch != null)
            {
                Size pbs = this.pictureBoxTouch.Size;

                double ratioX = (double)p.X / (double)pbs.Width;
                double ratioY = (double)p.Y / (double)pbs.Height;

                pin.X = (int)Math.Round((ratioX * cols));
                pin.Y = (int)Math.Round((ratioY * rows));
            }
            return pin;
        }

        #endregion

        #endregion

        void fireTouchEvent(List<Touch> touches)
        {
            if (ShowOffAdapter != null)
            {
                double[,] touchM = buildTouchMatrix(touches);
                ShowOffAdapter.firetouchValuesChangedEvent(touchM, (int)DateTime.UtcNow.Ticks);
            }
        }

        /// <summary>
        /// Builds the touch matrix from a list of points.
        /// </summary>
        /// <param name="touches">The touches.</param>
        /// <returns></returns>
        private double[,] buildTouchMatrix(List<Touch> touches)
        {
            double[,] touchM = new double[rows, cols];

            if (touches != null)
            {
                foreach (var p in touches)
                {
                    if (p.X >= 0 && p.Y >= 0 && p.X < cols && p.Y < rows)
                    { touchM[p.Y, p.X] = p.Intense; }
                }
            }
            return touchM;
        }

        public double TouchSizeRadiusX = 1;
        public double TouchSizeRadiusY = 1;

        /// <summary>
        /// Handles the ellipse points.
        /// </summary>
        /// <param name="p">The touch.</param>
        /// <param name="matrix">The matrix where the points where added.</param>
        private List<Touch> handleEllipsePoints(Point p)
        {
            ConcurrentBag<Touch> touchValues = new ConcurrentBag<Touch>();

            Point pos = new Point((int)Math.Round(TouchSizeRadiusX), (int)Math.Round(TouchSizeRadiusY));

            int width = (int)Math.Round(TouchSizeRadiusX * 2);
            int height = (int)Math.Round(TouchSizeRadiusY * 2);

            //check every element of the bonding box if inside or not


            Parallel.For(0, width + 1, x =>
            {
                Parallel.For(0, height + 1, y =>
                {
                    double touch = PointIsInsideEllipse(new Point(x, y), TouchSizeRadiusX, TouchSizeRadiusY, TouchSizeRadiusX, TouchSizeRadiusY);
                    if (touch <= 1)
                    {
                        touchValues.Add(new Touch(
                            x + p.X - (int)Math.Round(TouchSizeRadiusX),
                            y + p.Y - (int)Math.Round(TouchSizeRadiusY),
                            Math.Max(0.1, 1 - touch)));
                    }
                });
            });
            return touchValues.ToList();
        }

        /// <summary>
        /// Determines whether [the specified pointToCheck] [is inside the ellipse].
        /// The region (disk) bounded by the ellipse is given by the equation:
        /// 
        /// having an ellipse centered at (c_x,c_y), with semi-major axis r_x, semi-minor axis r_y, 
        /// both aligned with the Cartesian plane.
        /// 
        ///     (x−c_x)^2         (y−c_y)^2
        ///    ___________   +   ___________   ≤   1      (1)
        ///      r_x ^2            r_y ^2     
        /// 
        /// So given a test point (x,y), plug it in (1). If the inequality is satisfied, 
        /// then it is inside the ellipse; otherwise it is outside the ellipse. 
        /// 
        /// Moreover, the point is on the boundary of the region (i.e., on the ellipse) 
        /// if and only if the inequality is satisfied tightly 
        /// (i.e., the left hand side evaluates to 1)
        /// 
        /// </summary>
        /// <param name="pointToCheck">The point to check.</param>
        /// <param name="ellipsePos">The ellipse pos.</param>
        /// <param name="r_x">1/2 width of the ellipse.</param>
        /// <param name="r_y">1/2 height of the ellipse.</param>
        /// <returns>Value must be smaller or equal to 1 - than the point is inside the ellipse, otherwise it is outside</returns>
        public static double PointIsInsideEllipse(Point pointToCheck, double c_x, double c_y, double r_x, double r_y)
        {
            if (r_x == 0 || r_y == 0)
                return 2;
            double xComponent =
                Math.Pow((double)(pointToCheck.X - c_x), 2)
                /
                Math.Pow(r_x, 2);

            double yComponent =
                    Math.Pow((double)(pointToCheck.Y - c_y), 2)
                    /
                    Math.Pow(r_y, 2);

            double value = xComponent + yComponent;
            return value;
        }

    }


    struct Touch
    {
        public readonly int X;
        public readonly int Y;
        public readonly double Intense;

        public Touch(int x, int y, double intense)
        {
            this.X = x;
            this.Y = y;
            this.Intense = Math.Min(1, intense);
        }

    }

}