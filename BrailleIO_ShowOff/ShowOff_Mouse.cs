using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace BrailleIO
{
    public partial class ShowOff : Form
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
            touchGraphics.Clear(Color.Transparent);
            this.pictureBoxTouch.Image = _touchbmp;
            mouseToGetureMode = false;
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

                // fire event
                fireTouchEvent(new List<Point>() { pin });

                touchGraphics.FillEllipse(Brushes.Red, new Rectangle(pin.X * (pixelFactor + 1), pin.Y * (pixelFactor + 1), pixelFactor - 1, pixelFactor - 1));
                this.pictureBoxTouch.Image = _touchbmp;
            }
            catch (Exception)
            {
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


        void fireTouchEvent(List<Point>touches)
        {
            if (showOffAdapter != null)
            {
                double[,] touchM = new double[rows, cols];

                foreach (var p in touches)
                {
                    if (p.X >= 0 && p.Y >= 0 && p.X < cols && p.Y < rows)
                    { touchM[p.Y, p.X] = 1; }
                }
    
                showOffAdapter.firetouchValuesChangedEvent(touchM, (int)DateTime.UtcNow.Ticks);
            }
        }


    }
}