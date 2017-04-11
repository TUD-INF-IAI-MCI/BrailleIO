using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using BrailleIO.Structs;

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

        // const int MAX_TRYS = 3;

        readonly object _pbPinsLock = new Object();
        Image pictureBoxPinsImage
        {
            get
            {
                lock (_pbPinsLock)
                {
                    return invokeGetImageOfPictureBox(pictureBoxPins);
                }
            }
            set
            {
                if (!this.Disposing)
                {
                    lock (_pbPinsLock)
                    {
                        invokePictureBoxImageChange(this.pictureBoxPins, value);
                    }
                }
            }
        }

        readonly object _pbTouchLock = new Object();
        Image pictureBoxTouchImage
        {
            get
            {
                lock (_pbTouchLock)
                {
                    return invokeGetImageOfPictureBox(this.pictureBoxTouch);
                }
            }
            set
            {
                if (!this.Disposing)
                {
                    lock (_pbTouchLock)
                    {
                        invokePictureBoxImageChange(pictureBoxTouch, value);
                    }
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
                    return invokeGetImageOfPictureBox(this.pictureBox_overAllOverlay);
                }
            }
            set
            {
                if (!this.Disposing)
                {
                    lock (_pbOverlayLock)
                    {
                        invokePictureBoxImageChange(pictureBox_overAllOverlay, value);
                    }
                }
            }
        }

        /// <summary>
        /// Invokes the picture box image change.
        /// </summary>
        /// <param name="picBox">The picture box.</param>
        /// <param name="im">The image to show.</param>
        /// <returns></returns>
        private bool invokePictureBoxImageChange(PictureBox picBox, Image im)
        {
            bool success = true;
            this.Invoke((MethodInvoker)delegate
            {
                try
                {
                    //Set the image into the picture box
                    this.Invoke((MethodInvoker)delegate
                    {
                        picBox.SuspendLayout();
                        try
                        {
                            if (picBox.Image != null)
                            {
                                picBox.Image.Dispose(); //Without this, memory goes nuts
                            }
                        }
                        catch (Exception)
                        {
                            success = false;
                        }
                        picBox.Image = im;
                        picBox.ResumeLayout();
                    });
                }
                catch (Exception)
                {
                    success = false;
                }
            });

            return success;
        }

        /// <summary>
        /// Invokes the getter  for the image of a picture box.
        /// </summary>
        /// <param name="picBox">The picture box.</param>
        /// <returns>the image of the picture box or <c>null</c></returns>
        private Image invokeGetImageOfPictureBox(PictureBox picBox)
        {
            Image bitmap = null;
            //Set the image into the picture box
            this.Invoke((MethodInvoker)delegate
            {
                try
                {
                    bitmap = picBox.Image;
                }
                catch (Exception)
                {
                }
            });
            return bitmap;
        }

        #endregion

        #endregion

        volatile bool _rendering = false;
        bool[,] _renderedMatrix;
        /// <summary>
        /// Handles the Elapsed event of the renderTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Timers.ElapsedEventArgs"/> instance containing the event data.</param>
        void renderTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_rendering) return;
            Task t = new Task(
                () =>
                {
                    _rendering = true;
                    try
                    {
                        if (MartixStack.Count > 0)
                        {

                            int c = 0;

                            while (!MartixStack.TryPop(out _renderedMatrix) && (++c < 10)) { _renderedMatrix = null; }

                            this.Invoke((MethodInvoker)delegate
                            {
                                if (this.pictureBoxMatrix.Image == null)
                                    this.pictureBoxMatrix.Image = generateBaseImage(120, 60);
                            });

                            var image = getPinMatrixImage(_renderedMatrix);
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
                    finally { _rendering = false; }
                });
            t.Start();
        }

        readonly object baseImgPaintLock = new object();
        /// <summary>
        /// Generates a base image of this virtual pin matrix.
        /// </summary>
        /// <param name="Width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>Basic Bitmap structure for an empty dot pattern simulating a pin-matrix surface.</returns>
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

        partial void _dispose() { _run = false; renderTimer.Stop(); }

        /// <summary>
        /// Paints the specified matrix to the GUI.
        /// </summary>
        /// <param name="m">The pin matrix.</param>
        public new void Paint(bool[,] m)
        {
            if (!this.Disposing)
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

        static Pen _dTpen;
        static Pen dTpen
        {
            get
            {
                if (_dTpen == null)
                {
                    _dTpen = new Pen(Brushes.LightGreen, 1.8F); //Pens.LightGreen;
                    //_dTpen.Width = 1.8F;
                }
                return _dTpen;
            }
        }


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
                    List<Touch> dT;
                    int c = 0;

                    while (!touchStack.TryPop(out tm) && (++c < 10)) { tm = null; }
                    c = 0;
                    while (!detailedTouchStack.TryPop(out dT) && (++c < 10)) { dT = null; }

                    touchStack.Clear();
                    detailedTouchStack.Clear();

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

                                    // paint detailed touches (clustered)
                                    if (dT != null && dT.Count > 0)
                                    {
                                        foreach (var t in dT)
                                        {
                                            if (t.X >= 0 && t.Y >= 0 && t.DimX > 0 && t.DimY > 0)
                                            {

                                                double height = Math.Max(17, t.DimY * pixelFactor);
                                                double width = Math.Max(17, t.DimX * pixelFactor);

                                                double cX = t.X * (pixelFactor + 1);
                                                double cY = t.Y * (pixelFactor + 1);

                                                touchGraphics.DrawEllipse(dTpen,
                                                    (int)Math.Round(cX - width * 0.5),
                                                    (int)Math.Round(cY - height * 0.5),
                                                    (int)Math.Round(width),
                                                    (int)Math.Round(height));
                                            }
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

        #region Screenshot Export

        #region Menu entry

        ToolStripMenuItem _screenShotEntryMatrix;
        protected ToolStripMenuItem screenShotEntryMatrix
        {
            get
            {
                if (_screenShotEntryMatrix == null)
                    _screenShotEntryMatrix = new ToolStripMenuItem(
                        "Store &Tactile Matrix",
                        null,
                        screenShotEntryMatrix_Click,
                        Keys.Control | Keys.M);
                return _screenShotEntryMatrix;
            }
        }

        ToolStripMenuItem _screenShotEntryMatrixImage;
        protected ToolStripMenuItem screenShotEntryMatrixImage
        {
            get
            {
                if (_screenShotEntryMatrixImage == null)
                    _screenShotEntryMatrixImage = new ToolStripMenuItem(
                        "Paint Matrix as Image",
                        null,
                        screenShotEntryMatrixImage_Click,
                        Keys.Control | Keys.I);
                return _screenShotEntryMatrixImage;
            }
        }

        ToolStripMenuItem _screenShotEntry;
        public ToolStripMenuItem screenShotMenuEntry
        {
            get
            {
                if (_screenShotEntry == null)
                {
                    _screenShotEntry = new ToolStripMenuItem("&Screen Shot");
                    _screenShotEntry.DropDownItems.Add(screenShotEntryMatrix);
                    _screenShotEntry.DropDownItems.Add(screenShotEntryMatrixImage);
                }
                return _screenShotEntry;
            }
        }


        /// <summary>
        /// The show screenshot menu configuration key to search for in the app.config file.
        /// </summary>
        internal const String SHOW_SCREENSHOT_MNU_CONFIG_KEY = "ShowOff_ShowScreenshotMenu";
        /// <summary>
        /// Shows the screenshot menu if the corresponding key <see cref="SHOW_SCREENSHOT_MNU_CONFIG_KEY"/> 
        /// was set to <c>true</c> in the app.config of the current running application.
        /// </summary>
        internal void showScreenshotMenuFromConfig()
        {
            try
            {
                var config = System.Configuration.ConfigurationManager.AppSettings;
                if (config != null && config.Count > 0)
                {
                    var value = config[SHOW_SCREENSHOT_MNU_CONFIG_KEY];
                    bool val = Convert.ToBoolean(value);
                    if (val) {
                            ShowScreenshotMenu(); 
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Shows the menu for storing screen shots of the current rendered matrix.
        /// </summary>
        /// <param name="strip">The strip.</param>
        /// <returns><c>true</c> if the screen shot menu could be added.</returns>
        public bool ShowScreenshotMenu(MenuStrip strip = null)
        {

            try
            {
                if (strip == null) strip = this.menuStripMain;
                if (strip != null)
                {
                    if (strip.InvokeRequired) strip.Invoke(new Action(() => { strip.Items.Add(screenShotMenuEntry); }));
                    else strip.Items.Add(screenShotMenuEntry);
                    int i = 0;
                    while (!ShowMenuStrip() && i++ < 10) { Thread.Sleep(100); }
                }

                return true;
            }
            catch (Exception)
            {

            }

            return false;
        }

        void screenShotEntryMatrix_Click(object sender, EventArgs e)
        {
            // Displays a SaveFileDialog so the user can save the Image
            // assigned to Button2.
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "PNG Image|*.png";
            saveFileDialog1.Title = "Save the tactile matrix";
            saveFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.
            if (saveFileDialog1.FileName != "")
            {
                ExportTactileMatrix(saveFileDialog1.FileName, _renderedMatrix, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        void screenShotEntryMatrixImage_Click(object sender, EventArgs e)
        {
            // Displays a SaveFileDialog so the user can save the Image
            // assigned to Button2.
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "PNG Image|*.png";
            saveFileDialog1.Title = "Save the tactile matrix as image File";
            saveFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.
            if (saveFileDialog1.FileName != "")
            {
                PaintBoolMatrixToImage(_renderedMatrix, saveFileDialog1.FileName);
            }
        }

        #endregion

        /// <summary>
        /// Exports the tactile matrix.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="m">The matrix.</param>
        /// <param name="format">The image format for storing - default is PNG.</param>
        public void ExportTactileMatrix(string filePath, bool[,] m = null, System.Drawing.Imaging.ImageFormat format = null)
        {
            if (m == null) m = _renderedMatrix;
            if (format == null) format = System.Drawing.Imaging.ImageFormat.Png;
            if (String.IsNullOrWhiteSpace(filePath))
                try
                {
                    filePath = "Matrix_export_" + DateTime.Now.Ticks.ToString() + "." +
                            (format.ToString().Substring(format.ToString().LastIndexOf('.')).ToLower());
                }
                catch (Exception)
                {
                    filePath = "Matrix_export_" + DateTime.Now.Ticks.ToString() + ".png";
                }

            if (m != null)
            {
                // transform the matrix into an image
                Image img = BoolMatrixToImage(m);
                if (img != null)
                {
                    img.Save(filePath, format);
                }
            }
        }

        //paints display!
        /// <summary>
        /// Paints the bool matrix into an BMP image.
        /// </summary>
        /// <param name="m">The matrix.</param>
        /// <param name="filePath">The file path.</param>
        public static Image BoolMatrixToImage(bool[,] m)
        {
            if (m == null || m.GetLength(0) < 1 || m.GetLength(1) < 1) return null;

            Bitmap bmp = new Bitmap(m.GetLength(1), m.GetLength(0));

            using (Graphics PinGraphic = Graphics.FromImage(bmp))
            {
                PinGraphic.FillRectangle(Brushes.White, 0, 0, bmp.Width, bmp.Height);
                PinGraphic.Flush();

                for (int i = 0; i < m.GetLength(0); i++)
                    for (int j = 0; j < m.GetLength(1); j++)
                    {
                        try
                        {
                            if (m[i, j])
                            {
                                bmp.SetPixel(j, i, Color.Black);
                            }
                        }
                        catch { }
                    }
            }
            return bmp;
        }

        private static Object gLock = new Object();
        private static Pen _p = new Pen(Brushes.LightGray, 0.4F);
        private static Object pLock = new Object();
        private static Pen Stroke
        {
            get
            {
                lock (gLock)
                {
                    return _p;
                }
            }
        }
        private static Object graphicsLock = new Object();
        const int pixel = 5;

        //paints display!
        /// <summary>
        /// Paints the bool matrix into an BMP image.
        /// </summary>
        /// <param name="m">The matrix.</param>
        /// <param name="filePath">The file path.</param>
        public static void PaintBoolMatrixToImage(bool[,] m, string filePath)
        {
            if (m == null || m.GetLength(0) < 1 || m.GetLength(1) < 1) return;

            Image bmp = new Bitmap(m.GetLength(1) * (pixel + 1), m.GetLength(0) * (pixel + 1));
            lock (graphicsLock)
            {
                using (Graphics PinGraphic = Graphics.FromImage(bmp))
                {
                    PinGraphic.FillRectangle(Brushes.White, 0, 0, bmp.Width, bmp.Height);

                    for (int i = 0; i < m.GetLength(0); i++)
                        for (int j = 0; j < m.GetLength(1); j++)
                        {
                            lock (graphicsLock)
                            {
                                if (m[i, j])
                                {
                                    PinGraphic.FillRectangle(Brushes.Black, j * (pixel + 1), i * (pixel + 1), pixel, pixel);
                                }
                                else
                                {
                                    PinGraphic.DrawEllipse(Stroke, j * (pixel + 1), i * (pixel + 1), pixel - 1, pixel - 1);
                                }
                            }
                        }
                    try
                    {
                        PinGraphic.Flush();
                        if (string.IsNullOrEmpty(filePath) || string.IsNullOrWhiteSpace(filePath)) return;
                        bmp.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                    }
                    catch { }
                }
            }
        }

        #endregion

    }
}
