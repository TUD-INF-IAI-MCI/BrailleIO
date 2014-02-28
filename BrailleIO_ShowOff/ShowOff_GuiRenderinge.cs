using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;

namespace BrailleIO
{
    public partial class ShowOff : Form
    {
        #region GUI rendering

        #region Members

        #region constants
        readonly Pen strokePen = new Pen(Brushes.LightGray, 0.4F);
        readonly Pen gridLinePen = new Pen(Brushes.Thistle, 1);
        /// <summary>
        /// Factor for pins to pixels
        /// </summary>
        readonly int pixelFactor = 5;
        #endregion

        volatile bool _run = true;

        volatile bool[,] rm;

        Graphics _g;
        readonly Object gLock = new Object();
        Graphics PinGraphic
        {
            get
            {
                lock (gLock)
                {
                if (_g == null)
                {
                    _g = Graphics.FromImage(bmp);
                }
                return _g;
                }
            }
            set
            {
                lock (gLock)
                {
                _g = value;
                }
            }
        }

        Bitmap _bmp;
        private readonly object _bmpLock = new object();
        Bitmap bmp
        {
            get
            {
                lock (_bmpLock)
                {
                if (_bmp == null)
                    _bmp = new Bitmap(panel_MatrixPanel.Width, panel_MatrixPanel.Height);

                return _bmp;
                }
            }
        }

        Graphics _pg;
        Graphics PG
        {
            get { if (_pg == null && panel_MatrixPanel != null) { _pg = panel_MatrixPanel.CreateGraphics(); } return _pg; }
        }
        
        object graphicsLock = new object();
        
        Bitmap _baseImg;

        #endregion

        /// <summary>
        /// Generates a base image of this virtual pin matrix.
        /// </summary>
        /// <param name="rerender">if set to <c>true</c> [rerender].</param>
        /// <param name="Width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns></returns>
        Bitmap generateBaseImage(bool rerender, int Width, int height)
        {
            if (_baseImg == null || rerender)
            {
                _baseImg = new Bitmap(panel_MatrixPanel.Width, panel_MatrixPanel.Height);
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
        
        partial void _dispose() { _run = false; }

        /// <summary>
        /// Paints the specified matrix to the GUI.
        /// </summary>
        /// <param name="m">The pin matrix.</param>
        public void paint(bool[,] m)
        {
            startRendering();
            rm = m;
        }

        /// <summary>
        /// Starts the rendering thread if necessary.
        /// </summary>
        private void startRendering()
        {
            try
            {
                if (renderingThread == null || !renderingThread.IsAlive)
                {
                    renderingThread = new Thread(threadPaint);
                    renderingThread.Name = "renderingThread";
                    // renderingThread.Priority = ThreadPriority.BelowNormal;
                    renderingThread.Start();
                }
            }
            catch (System.Threading.ThreadStartException e) { System.Diagnostics.Debug.WriteLine(e); }
            catch (System.Threading.ThreadStateException e) { System.Diagnostics.Debug.WriteLine(e); }
        }

        /// <summary>
        /// Paints the pin matrix to the GUI.
        /// </summary>
        private void threadPaint()
        {
            bool[,] m1 = null;
            while (_run)
            {
                try
                {
                    m1 = rm;
                    if (m1 != null)
                    {
                        PinGraphic.DrawImageUnscaled(generateBaseImage(false, 120, 60), 0, 0);
                        for (int i = 0; i < m1.GetLength(0); i++)
                            for (int j = 0; j < m1.GetLength(1); j++)
                            {

                                double t = 0;
                                //touch paint
                                if (touchMatrix != null && touchMatrix.GetLength(0) > i && touchMatrix.GetLength(1) > j && touchMatrix[i, j] > 0)
                                {
                                    t = touchMatrix[i, j];
                                }

                                if (m1[i, j])
                                {
                                    PinGraphic.FillRectangle(Brushes.Black, j * (pixelFactor + 1), i * (pixelFactor + 1), pixelFactor, pixelFactor);
                                    if (t > 0) PinGraphic.FillEllipse(Brushes.LightPink, (j * (pixelFactor + 1)) + 1, (i * (pixelFactor + 1)) + 1, pixelFactor - 2, pixelFactor - 2);
                                }
                                else
                                {
                                    if (t > 0) PinGraphic.FillEllipse(Brushes.Red, j * (pixelFactor + 1), i * (pixelFactor + 1), pixelFactor, pixelFactor);
                                    //    PinGraphic.DrawEllipse(strokePen, j * (pixelFactor + 1), i * (pixelFactor + 1), pixelFactor - 1, pixelFactor - 1);
                                }
                            }
                        rm = null;
                        try
                        {
                            if (PG != null) PG.DrawImage(bmp, 0, 0);
                        }
                        catch (System.ObjectDisposedException) { return; }
                        catch (System.ComponentModel.Win32Exception) { return; }
                    }
                    else { Thread.Sleep(5); }
                }
                catch (System.Exception) { }
                finally { /*Thread.Sleep(10);*/ }
            }
        }

        #endregion


    }


}
