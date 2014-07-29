using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Concurrent;

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

        internal readonly ConcurrentStack<bool[,]> MartixStack = new ConcurrentStack<bool[,]>();

        Bitmap _baseImg;

        readonly static System.Timers.Timer renderTimer = new System.Timers.Timer(100);

        #endregion

        void renderTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {

                if (MartixStack.Count > 0)
                {

                    bool[,] rm;
                    int c = 0;

                    while (!MartixStack.TryPop(out rm) && (++c < 10)) { rm = null; }

                    if (this.pictureBoxMatrix.Image == null)
                        this.pictureBoxMatrix.Image = generateBaseImage(120, 60);
                    this.pictureBoxPins.Image = getPinMatrixImage(rm);
                    MartixStack.Clear();
                }

            }
            catch (Exception ex){
                _baseImg = null;
                _touchbmp = null;
                _matrixbmp = null;
                System.Diagnostics.Debug.WriteLine("Exception in renderer Timer Elapsed\n" + ex);
            }
        }


        /// <summary>
        /// Generates a base image of this virtual pin matrix.
        /// </summary>
        /// <param name="rerender">if set to <c>true</c> [rerender].</param>
        /// <param name="Width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns></returns>
        Bitmap generateBaseImage(int Width, int height)
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

        partial void _dispose() { renderTimer.Stop(); }

        /// <summary>
        /// Paints the specified matrix to the GUI.
        /// </summary>
        /// <param name="m">The pin matrix.</param>
        public void paint(bool[,] m)
        {
            MartixStack.Push(m);
            //FIXME: for fixing
            //BrailleIO.Renderer.GraphicUtils.PaintBoolMatrixToImage(m, @"C:\Users\Admin\Desktop\tmp\matrixes\m_" + DateTime.UtcNow.ToString("yyyy_MM_dd-HH_mm_ss_fff",
            //                                System.Globalization.CultureInfo.InvariantCulture) + ".bmp" );


        }
        #region Matrix Image

        Bitmap _matrixbmp;
        readonly object _matrixGraphicLock = new object();
        Graphics _matrixGraphics;
        Graphics matrixGraphics
        {
            get
            {
                lock (_matrixGraphicLock)
                {
                    if (_matrixGraphics == null)
                    {
                        if (_matrixbmp == null) _matrixbmp = new Bitmap(this.pictureBoxTouch.Width, this.pictureBoxTouch.Height);
                        if (_matrixbmp != null) _matrixGraphics = Graphics.FromImage(_matrixbmp);
                    }
                    return _matrixGraphics;
                }
            }
        }

        private Bitmap getPinMatrixImage(bool[,] m)
        {
            if (m != null && matrixGraphics != null)
            {
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
            return _matrixbmp != null ? _matrixbmp.Clone() as Bitmap : null;
        }

        #endregion

        #region Touch Image

        Bitmap _touchbmp;
        readonly object _touchGraphicLock = new object();
        Graphics _touchGraphics;
        Graphics touchGraphics
        {
            get
            {
                lock (_touchGraphicLock)
                {
                    if (_touchGraphics == null)
                    {
                        if (_touchbmp == null) _touchbmp = new Bitmap(this.pictureBoxTouch.Width, this.pictureBoxTouch.Height);
                        if (_touchbmp != null) _touchGraphics = Graphics.FromImage(_touchbmp);
                    }
                    return _touchGraphics;
                }
            }
        }


        private Image _lastTouchImage;
        /// <summary>
        /// Gets a image representing the touched pins.
        /// </summary>
        /// <returns></returns>
        private Bitmap getTouchImage()
        {
            if (touchStack.Count > 0)
            {
                double[,] tm;
                int c = 0;

                while (!touchStack.TryPop(out tm) && (++c < 10)) { tm = null; }

                if (tm != null)
                {
                    if (touchGraphics != null)
                    {
                        touchGraphics.Clear(Color.Transparent);
                        int rows = 60;
                        int cols = 120;

                        for (int i = 0; i < rows; i++)
                            for (int j = 0; j < cols; j++)
                            {
                                double t = 0;
                                //touch paint
                                if (tm != null && tm.GetLength(0) > i && tm.GetLength(1) > j && tm[i, j] > 0)
                                {
                                    t = tm[i, j];
                                }
                                //TODO:System.InvalidOperationException wurde nicht von Benutzercode behandelt.
  //HResult=-2146233079
  //Message=Das Objekt wird bereits an anderer Stelle verwendet.
  //Source=System.Drawing
  //StackTrace:
  //     bei System.Drawing.Graphics.CheckErrorStatus(Int32 status)
  //     bei System.Drawing.Graphics.FillEllipse(Brush brush, Int32 x, Int32 y, Int32 width, Int32 height)
  //     bei BrailleIO.ShowOff.getTouchImage() in E:\Tangram\Tool\BrailleIO\BrailleIO_ShowOff\ShowOff_GuiRenderinge.cs:Zeile 209.
  //     bei BrailleIO.ShowOff.PaintTouchMatrix(Double[,] touchMatrix) in E:\Tangram\Tool\BrailleIO\BrailleIO_ShowOff\ShowOff.cs:Zeile 101.
  //     bei tud.mci.tangram.TangramLector.LectorGUI._bda_touchValuesChanged(Object sender, BrailleIO_TouchValuesChanged_EventArgs e) in E:\Tangram\Tool\OOo Draw Extension\TangramLector\BrailleIO\LectorBIO.cs:Zeile 144.
  //     bei BrailleIO.AbstractBrailleIOAdapterBase.BrailleIO_TouchValuesChanged_EventHandler.Invoke(Object sender, BrailleIO_TouchValuesChanged_EventArgs e)
  //     bei BrailleIO.AbstractBrailleIOAdapterBase.fireTouchValuesChanged(Double[,] touches, Int32 timestamp, OrderedDictionary& raw) in E:\Tangram\Tool\BrailleIO\BrailleIO\Adapter\BrailleIOAdapter.cs:Zeile 177.
  //     bei BrailleIOBraillDisAdapter.BrailleIOAdapter_BrailleDisNet.BrailleDis_touchValuesChangedEvent(BrailleDisModuleState[] changedModules, BrailleDisModuleState[] activeModules, Int32 timeStampTickCount) in E:\Tangram\Tool\BrailleIOPrivate\BrailleIOBraillDisAdapter\BrailleIOAdapter_BrailleDisNet.cs:Zeile 266.
  //InnerException: 


                                if (t > 0) touchGraphics.FillEllipse(Brushes.Red, j * (pixelFactor + 1), i * (pixelFactor + 1), pixelFactor - 1, pixelFactor - 1);
                            }
                    }
                }
                touchStack.Clear();
            }

            return _touchbmp != null ? _touchbmp.Clone() as Bitmap : null;
        }
        #endregion


        #endregion


    }


}
