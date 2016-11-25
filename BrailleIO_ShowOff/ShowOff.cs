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
        internal readonly ConcurrentStack<List<Touch>> detailedTouchStack = new ConcurrentStack<List<Touch>>();

        BrailleIOMediator io;
        /// <summary>
        /// Gets the show off adapter. This is the <see cref="IBrailleIOAdapter"/> implementation simulating a hardware in- output adapter.
        /// </summary>
        /// <value>
        /// The show off adapter.
        /// </value>
        public BrailleIOAdapter_ShowOff ShowOffAdapter { get; private set; }

        #endregion

        /// <summary>
        /// Important function! Call this if you don't rum the ShowOffAdapter out of an windows form application.
        /// </summary>
        public void InitForm()
        {
            try
            {
                this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);

                Application.EnableVisualStyles();
            }
            catch (System.InvalidOperationException e)
            {
                System.Diagnostics.Debug.WriteLine("Exception  in Init show off form\n" + e);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowOff"/> class.
        /// </summary>
        public ShowOff()
        {
            InitForm();
            InitializeComponent();
            AddContextMenu();

            initPictureBoxes();

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

        private void initPictureBoxes()
        {
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
        }
        // has to be called in ShowOff.Designer.cs -> protected override void Dispose(bool disposing)
        partial void _dispose();

        #region initialization

        /// <summary>
        /// Initializes the BrailleIO framework. Build a new BrailleIOAdapter_ShowOff, and add it to the IBrailleIOAdapterManager.
        /// </summary>
        /// <param name="adapterManager">The adapter manager to use for managing devices.</param>
        /// <param name="setAsActiveAdapter">if set to <c>true</c> [set this as active adapter].</param>
        /// <returns>
        /// The created BrailleIOAdapter_ShowOff, that was build with this instance
        /// </returns>
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

        /// <summary>
        /// creates a new <see cref="BrailleIOAdapter_ShowOff" /> and returns it
        /// </summary>
        /// <param name="manager">the corresponding adapter manager</param>
        /// <returns>
        /// a new "BrailleIOAdapter_ShowOff adapter
        /// </returns>
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
        /// <param name="detailedTouches">The detailed touches.</param>
        public void PaintTouchMatrix(double[,] touchMatrix) { PaintTouchMatrix(touchMatrix, null); }

        /// <summary>
        /// Paints the touch matrix over the matrix image.
        /// </summary>
        /// <param name="touchMatrix">The touch matrix.</param>
        /// <param name="detailedTouches">The detailed touches.</param>
        public void PaintTouchMatrix(double[,] touchMatrix, List<Touch> detailedTouches = null)
        {
            addMatrixToStack(touchMatrix, detailedTouches);
            Task pT = new Task(() => { paintTouchImage(); });
            pT.Start();
        }

        private readonly Object touchMatrixLock = new Object();
        private void paintTouchImage()
        {
            if (!this.Disposing)
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
                            if (touchImage != null)
                            {
                                this.pictureBoxTouch.BeginInvoke(
                                    (MethodInvoker)delegate
                                {
                                    if (this.pictureBoxTouch != null && this.pictureBoxTouch.Handle != null && this.pictureBoxTouch.Visible && !this.IsDisposed && !this.pictureBoxTouch.IsDisposed)
                                        pictureBoxTouchImage = touchImage;
                                }
                                    );
                                break;
                            }
                            else { return; }
                        }
                        catch (InvalidOperationException)
                        {
                            if (this.Disposing) { break; }
                            else
                            {
                                if (this.pictureBoxTouch.IsDisposed)
                                {
                                    this.mnuItemReset_Click(null, null);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine("Exception in paint touch image " + ex);
                            Thread.Sleep(1);
                        }
                }
            }
        }
        private void addMatrixToStack(double[,] touchMatrix, List<Touch> detailedTouches)
        {
            if (touchMatrix != null)
            {
                touchStack.Push(touchMatrix);
            }

            detailedTouchStack.Push(detailedTouches);
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
                    //if (this.InvokeRequired)
                    //{
                        this.Invoke((MethodInvoker)delegate
                        {
                            setPictureOverlay(image);
                        });
                    //}

                }
                return true;
            }
            catch (InvalidOperationException) { }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception in setting picture overlay:\r\n" + ex);
            }
            return false;
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
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception in reset picture overlay " + e);
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
                catch (Exception ex)
                {
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
                catch (Exception ex)
                {
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
            if (!this.Disposing)
            {
                try
                {
                    if (this.toolStripStatusLabel_Messages != null)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            setStatusText(text);
                        });
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Exception in setting the status text message " + ex);
                }
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
            if (!this.Disposing)
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

        #region Main Menu

        /// <summary>
        /// Shows the menu strip.
        /// </summary>
        /// <returns><c>true</c> if the menu strip is visible.</returns>
        public bool ShowMenuStrip()
        {
            try
            {
                this.menuStripMain.Invoke(new Action(() => { this.menuStripMain.Visible = true; }));
                return this.menuStripMain.Visible;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Hides the menu strip.
        /// </summary>
        /// <returns><c>true</c> if the menu strip is not visible.</returns>
        public bool HideMenuStrip()
        {
            try
            {
                this.menuStripMain.Invoke(new Action(() => { this.menuStripMain.Visible = false; }));
                return !this.menuStripMain.Visible;
            }
            catch (Exception) { }
            return false;
        }

        /// <summary>
        /// Adds a menu item to the main menu strip.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="setAsMdiWindowListItem">if set to <c>true</c> the item will be registered as the MDI window list item.</param>
        /// <returns></returns>
        public bool AddMenuItem(ToolStripItem item, bool setAsMdiWindowListItem = false)
        {
            try
            {
                if (item != null)
                    this.menuStripMain.Invoke(
                        new Action(() =>
                        {
                            if (setAsMdiWindowListItem && item is ToolStripMenuItem)
                                this.menuStripMain.MdiWindowListItem = item as ToolStripMenuItem;
                            this.menuStripMain.Items.Add(item);
                        }));
            }
            catch (Exception) { }
            return false;
        }

        /// <summary>
        /// Removes a certain menu item from the main menu strip.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns><c>true</c> if the item was removed.</returns>
        public bool RemoveMenuItem(ToolStripItem item)
        {
            bool success = false;
            try
            {
                if (item != null)
                    this.menuStripMain.Invoke(new Action(() =>
                    {
                        if (this.menuStripMain.Items.Contains(item))
                        {
                            this.menuStripMain.Items.Remove(item);
                            success = !this.menuStripMain.Items.Contains(item);
                        }
                    }));
            }
            catch { }
            return success;
        }

        #endregion

        /// <summary>
        /// Gets the title of the window.
        /// </summary>
        /// <returns>the title of the window</returns>
        public String GetTitle()
        {
            String title = String.Empty;
            try
            {
                this.Invoke(new Action(() =>
                {
                    title = this.Text;
                }));
            }
            catch (Exception) { }
            return title;
        }

        /// <summary>
        /// Sets the title of the window.
        /// </summary>
        /// <param name="title">The new title.</param>
        /// <returns><c>true</c> if the title was changed</returns>
        public bool SetTitle(String title)
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    this.Text = title;
                }));
            }
            catch (Exception) { }
            return this.Text.Equals(title);
        }

        #endregion

        #region Context Menu

        private void AddContextMenu()
        {
            ContextMenu mnuContextMenu = new ContextMenu();
            this.ContextMenu = mnuContextMenu;
            //add menu item
            MenuItem mnuItemReset = new MenuItem();
            mnuItemReset.Text = "&Reset Monitor";
            mnuContextMenu.MenuItems.Add(mnuItemReset);

            mnuItemReset.Click += new EventHandler(mnuItemReset_Click);

        }

        void mnuItemReset_Click(object sender, EventArgs e)
        {
            //InitializeComponent();

            if (!this.Disposing)
            {
                try
                {
                    this.Invoke((MethodInvoker)delegate
                        {
                            //save for killing the picture boxes afterwards
                            System.Windows.Forms.PictureBox pb1 = this.pictureBox_overAllOverlay;
                            System.Windows.Forms.PictureBox pb2 = this.pictureBoxMatrix;
                            System.Windows.Forms.PictureBox pb3 = this.pictureBoxPins;
                            System.Windows.Forms.PictureBox pb4 = this.pictureBoxTouch;

                            //rebuild the picture boxes
                            this.pictureBoxPins = new System.Windows.Forms.PictureBox();
                            this.pictureBoxMatrix = new System.Windows.Forms.PictureBox();
                            this.pictureBox_overAllOverlay = new System.Windows.Forms.PictureBox();
                            this.pictureBoxTouch = new System.Windows.Forms.PictureBox();
                            this.statusStrip1.SuspendLayout();
                            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTouch)).BeginInit();
                            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPins)).BeginInit();
                            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMatrix)).BeginInit();
                            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_overAllOverlay)).BeginInit();
                            this.SuspendLayout();

                            // 
                            // pictureBoxTouch
                            // 
                            this.pictureBoxTouch.ErrorImage = global::BrailleIO_ShowOff.Properties.Resources.touch_error;
                            this.pictureBoxTouch.Location = new System.Drawing.Point(117, 118);
                            this.pictureBoxTouch.Name = "pictureBoxTouch";
                            this.pictureBoxTouch.Size = new System.Drawing.Size(721, 363);
                            this.pictureBoxTouch.TabIndex = 52;
                            this.pictureBoxTouch.TabStop = false;
                            this.pictureBoxTouch.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBoxTouch_MouseDown);
                            this.pictureBoxTouch.MouseEnter += new System.EventHandler(this.pictureBoxTouch_MouseEnter);
                            this.pictureBoxTouch.MouseLeave += new System.EventHandler(this.pictureBoxTouch_MouseLeave);
                            this.pictureBoxTouch.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBoxTouch_MouseMove);
                            this.pictureBoxTouch.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBoxTouch_MouseUp);
                            // 
                            // pictureBoxPins
                            // 
                            this.pictureBoxPins.ErrorImage = global::BrailleIO_ShowOff.Properties.Resources.pin_error;
                            this.pictureBoxPins.Location = new System.Drawing.Point(107, 109);
                            this.pictureBoxPins.Name = "pictureBoxPins";
                            this.pictureBoxPins.Size = new System.Drawing.Size(721, 363);
                            this.pictureBoxPins.TabIndex = 48;
                            this.pictureBoxPins.TabStop = false;
                            // 
                            // pictureBoxMatrix
                            // 
                            this.pictureBoxMatrix.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                            this.pictureBoxMatrix.ErrorImage = global::BrailleIO_ShowOff.Properties.Resources.base_error;
                            this.pictureBoxMatrix.Location = new System.Drawing.Point(94, 98);
                            this.pictureBoxMatrix.Name = "pictureBoxMatrix";
                            this.pictureBoxMatrix.Size = new System.Drawing.Size(721, 363);
                            this.pictureBoxMatrix.TabIndex = 47;
                            this.pictureBoxMatrix.TabStop = false;
                            // 
                            // pictureBox_overAllOverlay
                            // 
                            this.pictureBox_overAllOverlay.BackColor = System.Drawing.Color.Transparent;
                            this.pictureBox_overAllOverlay.Cursor = System.Windows.Forms.Cursors.Default;
                            this.pictureBox_overAllOverlay.Enabled = false;
                            this.pictureBox_overAllOverlay.ErrorImage = global::BrailleIO_ShowOff.Properties.Resources.overlay_error;
                            this.pictureBox_overAllOverlay.InitialImage = null;
                            this.pictureBox_overAllOverlay.Location = new System.Drawing.Point(94, 119);
                            this.pictureBox_overAllOverlay.Name = "pictureBox_overAllOverlay";
                            this.pictureBox_overAllOverlay.Size = new System.Drawing.Size(721, 363);
                            this.pictureBox_overAllOverlay.TabIndex = 50;
                            this.pictureBox_overAllOverlay.TabStop = false;
                            this.pictureBox_overAllOverlay.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBoxTouch_MouseDown);
                            this.pictureBox_overAllOverlay.MouseEnter += new System.EventHandler(this.pictureBoxTouch_MouseEnter);
                            this.pictureBox_overAllOverlay.MouseLeave += new System.EventHandler(this.pictureBoxTouch_MouseLeave);
                            this.pictureBox_overAllOverlay.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBoxTouch_MouseMove);
                            this.pictureBox_overAllOverlay.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBoxTouch_MouseUp);

                            this.Controls.Add(this.pictureBox_overAllOverlay);
                            this.Controls.Add(this.pictureBoxTouch);
                            this.Controls.Add(this.pictureBoxPins);
                            this.Controls.Add(this.pictureBoxMatrix);

                            this.statusStrip1.PerformLayout();
                            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTouch)).EndInit();
                            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPins)).EndInit();
                            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMatrix)).EndInit();
                            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_overAllOverlay)).EndInit();
                            this.ResumeLayout(false);
                            this.PerformLayout();

                            this.pictureBoxTouch.CreateControl();
                            this.pictureBoxPins.CreateControl();
                            this.pictureBoxMatrix.CreateControl();
                            this.pictureBox_overAllOverlay.CreateControl();

                            initPictureBoxes();

                            try
                            {
                                pb1.Dispose();
                                pb2.Dispose();
                                pb3.Dispose();
                                pb4.Dispose();
                            }
                            catch { }

                        });
                }
                catch (ObjectDisposedException) { this.Dispose(); }
                catch (InvalidOperationException) { }
                catch { }
            }
        }

        #endregion
    }
}
