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


namespace BrailleIO
{
    public partial class ShowOff : Form
    {
        #region Members
        internal readonly ConcurrentStack<double[,]> touchStack = new ConcurrentStack<double[,]>();

        BrailleIOMediator io;
        BrailleIOAdapter_ShowOff showOffAdapter;

        #endregion

        /// <summary>
        /// Important function! Call this if you don't rum the ShowOffAdapter out of an windows form application.
        /// </summary>
        public static void InitForm()
        {
            try
            {
            	Application.SetCompatibleTextRenderingDefault(false);
            }
            catch (System.InvalidOperationException e)
            {
                System.Diagnostics.Debug.WriteLine("Exception  in Init show off form\n" + e);
            }
            Application.EnableVisualStyles();
            
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

        public AbstractBrailleIOAdapterBase InitalizeBrailleIO()
        {
            io = BrailleIOMediator.Instance;
            io.AdapterManager = new ShowOffBrailleIOAdapterManager();

            showOffAdapter = GetAdapter(ref io.AdapterManager) as BrailleIOAdapter_ShowOff;
            showOffAdapter.Synch = true;
            io.AdapterManager.AddAdapter(showOffAdapter);

            return showOffAdapter;
        }

        public AbstractBrailleIOAdapterBase GetAdapter(ref AbstractBrailleIOAdapterManagerBase manager)
        {
            showOffAdapter = new BrailleIOAdapter_ShowOff(ref manager);
            return showOffAdapter;
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Paints the touch matrix over the matrix image.
        /// </summary>
        /// <param name="touchMatrix">The touch matrix.</param>
        public void PaintTouchMatrix(double[,] touchMatrix) { addMatrixToStack(touchMatrix); this.pictureBoxTouch.Image = getTouchImage(); }

        private void addMatrixToStack(double[,] touchMatrix)
        {
            if (touchMatrix != null)
            {
                touchStack.Push(touchMatrix);
            }
        }


        #endregion

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {

        }

    }
}
