using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using BrailleIO.Renderer;
using BrailleIO.Structs;
using System.Collections.Concurrent;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using BrailleIO.Interface;


namespace BrailleIO
{
    public partial class ShowOff : Form
    {
        #region Members
        internal readonly ConcurrentStack<double[,]> touchStack = new ConcurrentStack<double[,]>();

        BrailleIOMediator io;
        public BrailleIOAdapter_ShowOff ShowOffAdapter { get; private set; }

        #endregion

        /// <summary>
        /// Important function! Call this if you don't rum the ShowOffAdapter out of an windows form application.
        /// </summary>
        public static void InitForm()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
            }
            catch (System.InvalidOperationException e)
            {
                //System.Diagnostics.Debug.WriteLine("Exception  in Init show off form\n" + e);
            }
        }

        public ShowOff()
        {
            InitializeComponent();

            this.pictureBoxMatrix.BackColor = Color.White;
            this.pictureBoxMatrix.Image = generateBaseImage(120, 60);
            this.pictureBoxPins.BackColor = Color.Transparent;
            pictureBoxPins.Parent = pictureBoxMatrix;
            pictureBoxPins.Location = new Point(0, 0);
            this.pictureBoxTouch.BackColor = Color.Transparent;
            pictureBoxTouch.Parent = pictureBoxPins;
            pictureBoxTouch.Location = new Point(0, 0);
            pictureBoxTouch.Image = null;


            this.pictureBox_overAllOverlay.BackColor = Color.Transparent;
            pictureBox_overAllOverlay.Parent = pictureBoxTouch;
            pictureBox_overAllOverlay.Location = new Point(0, 0);
            pictureBox_overAllOverlay.Image = null;

            renderTimer.Elapsed += new System.Timers.ElapsedEventHandler(renderTimer_Elapsed);
            renderTimer.Start();

            this.Activate();
            this.Show();

            //register Ctr. Button listener
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(ShowOff_KeyDown);
            this.KeyUp += new KeyEventHandler(ShowOff_KeyUp);

            //TODO: Register hotkeys;
        }

        // has to be called in ShowOff.Designer.cs -> protected override void Dispose(bool disposing)
        partial void _dispose();

        #region initalization

        /// <summary>
        /// Initializes the BrailleIO framework. Build a new BrailleIOAdapter_ShowOff, and add it to the IBrailleIOAdapterManager.
        /// </summary>
        /// <param name="adapterManager">The adapter manager to use for managing devices.</param>
        /// <returns>The created BrailleIOAdapter_ShowOff, that was build with this instance</returns>
        public AbstractBrailleIOAdapterBase InitializeBrailleIO(IBrailleIOAdapterManager adapterManager, bool setAsActiveAdapter = false)
        {
            ShowOffAdapter = new BrailleIOAdapter_ShowOff(adapterManager, this);
            ShowOffAdapter.Synch = true;

            adapterManager.AddAdapter(ShowOffAdapter);

            return ShowOffAdapter;
        }

        /// <summary>
        /// Initializes the BrailleIO framework. Build a new BrailleIOAdapter_ShowOff, and add it to the global IBrailleIOAdapterManager.
        /// </summary>
        /// <returns>The created BrailleIOAdapter_ShowOff, that was build with this instance</returns>
        public AbstractBrailleIOAdapterBase InitializeBrailleIO(bool setAsActiveAdapter = false)
        {
            io = BrailleIOMediator.Instance;
            if (io != null)
            {
                if (io.AdapterManager == null)
                {
                    io.AdapterManager = new ShowOffBrailleIOAdapterManager();
                    setAsActiveAdapter = true;
                }
                return InitializeBrailleIO(io.AdapterManager, setAsActiveAdapter);
            }
            return null;
        }

        public AbstractBrailleIOAdapterBase GetAdapter(IBrailleIOAdapterManager manager)
        {
            ShowOffAdapter = new BrailleIOAdapter_ShowOff(manager);
            return ShowOffAdapter;
        }

        #endregion

        #region Public Functions

        #region Touch Image Overlay

        /// <summary>
        /// Paints the touch matrix over the matrix image.
        /// </summary>
        /// <param name="touchMatrix">The touch matrix.</param>
        public void PaintTouchMatrix(double[,] touchMatrix)
        {
            addMatrixToStack(touchMatrix);
            Thread.Sleep(2);
            Bitmap touchImage = null;

            try
            {
                touchImage = getTouchImage();
            }
            catch (System.Exception ex)
            {

            }
            int trys = 0;
            while (++trys < 5)
                try
                {
                    if (touchImage != null)
                    {
                        this.pictureBoxTouch.Image = touchImage;
                        break;
                    }
                }
                catch
                {
                    Thread.Sleep(1);
                }
        }

        private void addMatrixToStack(double[,] touchMatrix)
        {
            if (touchMatrix != null)
            {
                touchStack.Push(touchMatrix);
            }
        }

        #endregion

        #region Picture Overlay

        private readonly object overlayLock = new Object();
        /// <summary>
        /// Sets an overlay picture will be displayed as topmost 
        /// - so beware to use a transparent background when using this 
        /// overlay functionality.
        /// </summary>
        /// <param name="image">The image to be displayed as an overlay.</param>
        /// <returns><c>true</c> if the image could been set, otherwise <c>false</c></returns>
        public bool SetPictureOverlay(Image image)
        {
            try
            {
                if (this.pictureBox_overAllOverlay != null)
                {
                    if (this.InvokeRequired)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            setPictureOverlay(image);
                        });
                    }

                }
                return true;
            }
            catch (Exception) { return false; }
        }


        void setPictureOverlay(Image image)
        {
            lock (overlayLock)
            {
                try
                {
                    if (this.pictureBox_overAllOverlay != null)
                    {
                        this.pictureBox_overAllOverlay.Enabled = true;
                        this.pictureBox_overAllOverlay.Image = image;
                    }
                }
                catch (Exception e) { }
            }
        }


        /// <summary>
        /// Resets the picture overlay to an invisible overlay.
        /// </summary>
        public void ResetPictureOverlay()
        {
            lock (overlayLock)
            {
                try
                {
                    if (this.pictureBox_overAllOverlay != null)
                    {
                        this.pictureBox_overAllOverlay.Enabled = false;
                        this.pictureBox_overAllOverlay.Image = null;
                        this.pictureBox_overAllOverlay.BackColor = Color.Transparent;
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Gets the current overlay image.
        /// </summary>
        /// <returns>the current set overlay image or <c>null</c></returns>
        public Image GetPictureOverlay()
        {
            lock (overlayLock)
            {
                try
                {
                    if (this.pictureBox_overAllOverlay != null)
                    {
                        if (this.pictureBox_overAllOverlay.Image != null)
                        {
                            return this.pictureBox_overAllOverlay.Image.Clone() as Image;
                        }
                    }
                }
                catch { }
            }
            return null;
        }

        /// <summary>
        /// Gets the size of the picture overlay image.
        /// </summary>
        /// <value>The size of the overlay image.</value>
        public Size PictureOverlaySize
        {
            get
            {
                lock (overlayLock)
                {
                    try
                    {
                        if (this.pictureBox_overAllOverlay != null)
                        {
                            return this.pictureBox_overAllOverlay.Size;
                        }
                    }
                    catch { }
                    return new Size(-1, -1);
                }
            }
        }

        #endregion

        #endregion
    }
}
