using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace BrailleIO
{
    public partial class ShowOff : Form, IBrailleIOShowOffMonitor
    {
        #region GUI rendering

        #region Members

        #region constants

        readonly Pen strokePen = new Pen(Brushes.LightGray, 0.4F);
        readonly Pen gridLinePen = new Pen(Brushes.Thistle, 1);
        /// <summary>
        /// Factor for pins to pixels
        /// </summary>
        const int pixelFactor = 5;

        #endregion

        /// <summary>
        /// Stack for incoming pin matrix stats to display on the 'device'
        /// </summary>
        internal readonly ConcurrentStack<bool[,]> MartixStack = new ConcurrentStack<bool[,]>();

        /// <summary>
        /// Image of the device dot matrix and the touch module matrix - basement layer
        /// </summary>
        Bitmap _baseImg;

        /// <summary>
        /// a which init a new paint of the pin state matrix
        /// </summary>
        readonly static System.Timers.Timer renderTimer = new System.Timers.Timer(50);

        private readonly Object _renderLock = new Object();

        #region Picture Box Images

        const int MAX_TRYS = 3;

        readonly object _pbPinsLock = new Object();
        Image pictureBoxPinsImage
        {
            get
            {
                lock (_pbPinsLock)
                {
                    Image im = null; ;
                    this.Invoke((MethodInvoker)delegate
                    {
                        im = this.pictureBoxPins.Image;
                    });
                    return im;
                }
            }
            set
            {
                lock (_pbPinsLock)
                {
                    this.Invoke((MethodInvoker)delegate{
                          int i = 0;
                          while (i++ < MAX_TRYS)
                          {
                              try
                              {
                                  if (this.pictureBoxPins.Image != null) this.pictureBoxPins.Image.Dispose();
                                  this.pictureBoxPins.Image = value;
                                  pictureBoxPins.Refresh();
                                  return;
                              }
                              catch (ObjectDisposedException) { return; }
                              catch (Exception e)
                              {
                                  System.Diagnostics.Debug.WriteLine("Exception in setting the pins image " + e);
                                  pictureBoxPins.Image = pictureBoxPins.ErrorImage; 
                                  pictureBoxPins.Update();
                              }
                          }
                      });
                }
            }
        }

        readonly object _pbTouchLock = new Object();
        Image pictureBoxTouchImage
        {
            get
            {
                lock (_pbTouchLock)
                {Image im = null; ;
                this.Invoke((MethodInvoker)delegate
                {
                    im = this.pictureBoxTouch.Image;
                });
                return im;
                }
            }
            set
            {
                lock (_pbTouchLock)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        int i = 0;
                        while (i++ < MAX_TRYS)
                        {
                            try
                            {
                                if (this.pictureBoxTouch.Image != null) this.pictureBoxTouch.Image.Dispose();
                                this.pictureBoxTouch.Image = value;
                                pictureBoxTouch.Refresh();
                                return;
                            }
                            catch (ObjectDisposedException) { return; }
                            catch (Exception e)
                            {
                                System.Diagnostics.Debug.WriteLine("Exception in setting the touch image " + e);
                                pictureBoxTouch.Image = pictureBoxTouch.ErrorImage; 
                                pictureBoxTouch.Update();
                            }
                        }
                    });
                }
            }
        }

        readonly object _pbOverlayLock = new Object();
        Image pictureBoxOverlayImage
        {
            get
            {
                lock (_pbOverlayLock)
                {
                    Image im = null; ;
                    this.Invoke((MethodInvoker)delegate
                    {
                        im = this.pictureBox_overAllOverlay.Image;
                    });
                    return im;
                }
            }
            set
            {
                lock (_pbOverlayLock)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        int i = 0;
                        while (i++ < MAX_TRYS)
                        {
                            try
                            {
                                if (this.pictureBox_overAllOverlay.Image != null) this.pictureBox_overAllOverlay.Image.Dispose();
                                this.pictureBox_overAllOverlay.Image = value;
                                 pictureBox_overAllOverlay.Refresh();
                                return;
                            }
                            catch (ObjectDisposedException) { return; }
                            catch (Exception e)
                            {
                                System.Diagnostics.Debug.WriteLine("Exception in setting the overlay image " + e);
                                pictureBox_overAllOverlay.Image = pictureBox_overAllOverlay.ErrorImage; 
                                pictureBox_overAllOverlay.Update();
                            }
                        }
                    });
                }
            }
        }

        #endregion

        #endregion

        /// <summary>
        /// Handles the Elapsed event of the renderTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Timers.ElapsedEventArgs"/> instance containing the event data.</param>
        void renderTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Task t = new Task(
                () =>
                {
                    try
                    {
                        if (MartixStack.Count > 0)
                        {
                            bool[,] rm;
                            int c = 0;

                            while (!MartixStack.TryPop(out rm) && (++c < 10)) { rm = null; }

                            Image im = null; ;
                            this.Invoke((MethodInvoker)delegate
                            {
                                if (this.pictureBoxMatrix.Image == null)
                                    this.pictureBoxMatrix.Image = generateBaseImage(120, 60);
                            });
                            var image = getPinMatrixImage(rm);
                            if (image != null)
                            {
                                int trys = 0;
                                while (trys++ < 5)
                                {
                                    try
                                    {
                                        pictureBoxPinsImage = image;
                                        break;
                                    }
                                    catch (ObjectDisposedException) { return; }
                                    catch (Exception ex)
                                    {
                                        System.Diagnostics.Debug.WriteLine("Exception in setting pins image " + ex);
                                        Thread.Sleep(5);
                                    }
                                }
                            }
                            MartixStack.Clear();
                        }
                    }
                    catch (ObjectDisposedException) { return; }
                    catch (Exception ex)
                    {
                        _baseImg = null;
                        System.Diagnostics.Debug.WriteLine("Exception in renderer Timer Elapsed\n" + ex);
                    }
                });
            t.Start();
        }

        readonly object baseImgPaintLock = new object();
        /// <summary>
        /// Generates a base image of this virtual pin matrix.
        /// </summary>
        /// <param name="rerender">if set to <c>true</c> [rerender].</param>
        /// <param name="Width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns></returns>
        Bitmap generateBaseImage(int Width, int height)
        {
            lock (baseImgPaintLock)
            {
                if (_baseImg == null)
                {
                    _baseImg = new Bitmap(pictureBoxMatrix.Width, pictureBoxMatrix.Height);
                    using (Graphics big = Graphics.FromImage(_baseImg))
                    {
                        big.FillRectangle(Brushes.White, 0, 0, _baseImg.Width, _baseImg.Height);
                        bool[,] m = new bool[height, Width];
                        for (int i = 0; i < m.GetLength(0); i++)
                        {
                            if ((i % pixelFactor) == 0)
                            {
                                big.DrawLine(gridLinePen, new Point(0, i * (pixelFactor + 1) - 1), new Point(_baseImg.Width, i * (pixelFactor + 1) - 1));
                                big.DrawLine(gridLinePen, new Point(0, (int)Math.Round((i + 2.5) * (pixelFactor + 1) - 1)), new Point(_baseImg.Width, (int)Math.Round((i + 2.5) * (pixelFactor + 1) - 1)));
                            }

                            for (int j = 0; j < m.GetLength(1); j++)
                            {
                                if ((j % 2) == 0)
                                {
                                    big.DrawLine(gridLinePen, new Point(j * (pixelFactor + 1) - 1, 0), new Point(j * (pixelFactor + 1) - 1, _baseImg.Height));
                                }
                                big.DrawEllipse(strokePen, j * (pixelFactor + 1), i * (pixelFactor + 1), pixelFactor - 1, pixelFactor - 1);
                            }
                        }
                    }
                }
                return _baseImg != null ? _baseImg.Clone() as Bitmap : null;
            }
        }

        partial void _dispose() { renderTimer.Stop(); }

        /// <summary>
        /// Paints the specified matrix to the GUI.
        /// </summary>
        /// <param name="m">The pin matrix.</param>
        public new void Paint(bool[,] m)
        {
            MartixStack.Push(m);
        }

        #region Matrix Image

        readonly object matrixPaintLock = new object();
        /// <summary>
        /// Renders the pin matrix to an image.
        /// </summary>
        /// <param name="m">The martix to render.</param>
        /// <returns>a Bitmap of the matrix to render</returns>
        private Bitmap getPinMatrixImage(bool[,] m)
        {
            lock (matrixPaintLock)
            {
                Bitmap _matrixbmp = new Bitmap(pictureBoxMatrix.Width, pictureBoxMatrix.Height);
                if (_matrixbmp != null)
                {
                    using (Graphics matrixGraphics = Graphics.FromImage(_matrixbmp))
                    {
                        try
                        {
                            if (m != null && matrixGraphics != null)
                            {
                                // matrixGraphics.Flush(System.Drawing.Drawing2D.FlushIntention.Flush);
                                matrixGraphics.Clear(Color.Transparent);
                                int rows = m.GetLength(0);
                                int cols = m.GetLength(1);

                                for (int i = 0; i < rows; i++)
                                    for (int j = 0; j < cols; j++)
                                    {
                                        if (m[i, j])
                                        {
                                            matrixGraphics.FillRectangle(Brushes.Black, j * (pixelFactor + 1), i * (pixelFactor + 1), pixelFactor, pixelFactor);
                                        }
                                    }
                            }
                        }
                        catch (System.InvalidOperationException ex)
                        {
                            System.Diagnostics.Debug.WriteLine("Exception in rendering the pin matrix image: " + ex);
                        }
                    }
                }
                return _matrixbmp;
            }
        }

        #endregion

        #region Touch Image

        int rows = 60;
        int cols = 120;

        readonly object touchImgPaintLock = new object();
        /// <summary>
        /// Gets a image representing the touched pins.
        /// </summary>
        /// <returns></returns>
        private Bitmap getTouchImage()
        {
            lock (touchImgPaintLock)
            {
                if (touchStack.Count > 0)
                {
                    double[,] tm;
                    int c = 0;

                    while (!touchStack.TryPop(out tm) && (++c < 10)) { tm = null; }

                    if (tm != null)
                    {
                        touchStack.Clear(); // clear the stack because a touch image will be created now

                        Bitmap _touchbmp = new Bitmap(pictureBoxMatrix.Width, pictureBoxMatrix.Height);

                        if (_touchbmp != null)
                        {
                            using (Graphics touchGraphics = Graphics.FromImage(_touchbmp))
                            {
                                try
                                {
                                    touchGraphics.Clear(Color.Transparent);

                                    for (int i = 0; i < rows; i++)
                                        for (int j = 0; j < cols; j++)
                                        {
                                            double t = 0;
                                            //touch paint
                                            if (tm != null && tm.GetLength(0) > i && tm.GetLength(1) > j && tm[i, j] > 0)
                                            {
                                                t = tm[i, j];
                                            }
                                            if (t > 0)
                                            {
                                                touchGraphics.FillEllipse(Brushes.Red, j * (pixelFactor + 1), i * (pixelFactor + 1), pixelFactor - 1, pixelFactor - 1);
                                            }
                                        }
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine("Exception in rendering touch image " + ex);
                                }
                            }
                            return _touchbmp;
                        }
                    }
                }
                return null;
            }

        }
        #endregion

        #endregion
    }
}
