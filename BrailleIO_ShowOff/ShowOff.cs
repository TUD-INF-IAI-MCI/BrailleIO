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
            ShowOffAdapter = new BrailleIOAdapter_ShowOff(ref adapterManager, this);
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

        public AbstractBrailleIOAdapterBase GetAdapter(ref IBrailleIOAdapterManager manager)
        {
            ShowOffAdapter = new BrailleIOAdapter_ShowOff(ref manager);
            return ShowOffAdapter;
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Paints the touch matrix over the matrix image.
        /// </summary>
        /// <param name="touchMatrix">The touch matrix.</param>
        public void PaintTouchMatrix(double[,] touchMatrix)
        {
            if (!mouseToGetureMode) // maybe show the modules in background?!
            {
                addMatrixToStack(touchMatrix); try
                {
                    this.pictureBoxTouch.Image = getTouchImage();
                }
                catch (System.Exception ex)
                {

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



    }
}
