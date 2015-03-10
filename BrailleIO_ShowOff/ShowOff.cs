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
using System.Threading.Tasks;


namespace BrailleIO
{
    public partial class ShowOff : Form, IBrailleIOShowOffMonitor
    {
        #region Members
        internal readonly ConcurrentStack<double[,]> touchStack = new ConcurrentStack<double[,]>();

        BrailleIOMediator io;
        public BrailleIOAdapter_ShowOff ShowOffAdapter { get; private set; }

        #endregion

        /// <summary>
        /// Important function! Call this if you don't rum the ShowOffAdapter out of an windows form application.
        /// </summary>
        public void InitForm()
        {
            try
            {
                Application.EnableVisualStyles();
            }
            catch (System.InvalidOperationException e)
            {
                //System.Diagnostics.Debug.WriteLine("Exception  in Init show off form\n" + e);
            }
        }

        public ShowOff()
        {
            InitForm();
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
            this.KeyDown += new KeyEventHandler(showOff_KeyDown);
            this.KeyUp += new KeyEventHandler(showOff_KeyUp);

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
            Task pT = new Task(() => { paintTouchImage(); });
            pT.Start();
        }

        private readonly Object touchMatrixLock = new Object();
        private void paintTouchImage()
        {
            lock (touchMatrixLock)
            {
                Bitmap touchImage = null;
                try
                {
                    touchImage = getTouchImage();
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Exception in getting touch image " + ex);
                }
                int trys = 0;
                while (++trys < 5)
                    try
                    {
                        if (touchImage != null )
                        {
                            this.pictureBoxTouch.BeginInvoke(
                                (MethodInvoker)delegate { 
                                    if(this.pictureBoxTouch != null && this.pictureBoxTouch.Handle != null && this.pictureBoxTouch.Visible && !this.IsDisposed && !this.pictureBoxTouch.IsDisposed)
                                        pictureBoxTouchImage = touchImage;
                                }
                                );
                            break;
                        }
                        else { return; }
                    }
                    catch(Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Exception in paint touch image " + ex);
                        Thread.Sleep(1);
                    }
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
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine("Exception in setting picture overlay");
                return false; 
            }
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
                        pictureBoxOverlayImage = image;
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Exception in private set picture overlay " + e);
                }
            }
        }

        /// <summary>
        /// Resets the picture overlay to an invisible overlay.
        /// </summary>
        public void ResetPictureOverlay()
        {
            try
            {
                if (this.pictureBox_overAllOverlay != null)
                {
                    if (this.InvokeRequired)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            resetPictureOverlay();
                        });
                    }

                }
            }
            catch (Exception e) {
                System.Diagnostics.Debug.WriteLine("Exception in reset picture overlay " +e );
            }
        }

        void resetPictureOverlay()
        {
            lock (overlayLock)
            {
                try
                {
                    if (this.pictureBox_overAllOverlay != null)
                    {
                        this.pictureBox_overAllOverlay.Enabled = false;
                        pictureBoxOverlayImage = null;
                        this.pictureBox_overAllOverlay.BackColor = Color.Transparent;
                    }
                }
                catch (Exception ex) {
                    System.Diagnostics.Debug.WriteLine("Exception in private reset picture overlay " + ex);
                }
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
                        if (pictureBoxOverlayImage != null)
                        {
                            return pictureBoxOverlayImage.Clone() as Image;
                        }
                    }
                }
                catch (Exception ex) {
                    System.Diagnostics.Debug.WriteLine("Exception in getting picture overlay " + ex);
                }
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
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Exception in getting picture overlay size " + ex);
                    }
                    return new Size(-1, -1);
                }
            }
        }

        #endregion

        #region Status Text

        private readonly object statusLock = new Object();

        /// <summary>
        /// Sets the text in the status bar.
        /// </summary>
        /// <param name="text">The text to display in the status bar.</param>
        public void SetStatusText(string text)
        {
            try
            {
                if (this.toolStripStatusLabel_Messages != null)
                {
                    if (this.InvokeRequired)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            setStatusText(text);
                        });
                    }
                }
            }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine("Exception in setting the status text message " + ex);
            }
        }

        private void setStatusText(string text)
        {
            lock (statusLock)
            {
                if (this.toolStripStatusLabel_Messages != null)
                {
                    this.toolStripStatusLabel_Messages.Text = text;
                }
            }
        }

        /// <summary>
        /// Resets the text in the status bar.
        /// </summary>
        public void ResetStatusText()
        {
            try
            {
                if (this.toolStripStatusLabel_Messages != null)
                {
                    if (this.InvokeRequired)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            resetStatusText();
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception in resetting status text. " + ex);
            }
        }

        private void resetStatusText()
        {
            lock (statusLock)
            {
                if (this.toolStripStatusLabel_Messages != null)
                {
                    this.toolStripStatusLabel_Messages.Text = string.Empty;
                }
            }
        }

        #endregion

        #endregion
    }
}
