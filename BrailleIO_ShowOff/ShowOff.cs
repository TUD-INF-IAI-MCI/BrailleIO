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

        public bool[,] m { get; set; }

        BrailleIOMediator io;
        volatile double[,] touchMatrix;
        Thread renderingThread;
        BrailleIOAdapter_ShowOff showOffAdapter;

        #endregion

        /// <summary>
        /// Important function! Call this if you don't rum the ShowOffAdapter out of an windows form application.
        /// </summary>
        public static void InitForm()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
        }

        public ShowOff()
        {
            InitializeComponent();
            this.Activate();
            this.Show();
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

        public void PaintTouchMatrix(double[,] touchMatrix) { this.touchMatrix = touchMatrix; this.paint(m); }

        #endregion

    }
}
