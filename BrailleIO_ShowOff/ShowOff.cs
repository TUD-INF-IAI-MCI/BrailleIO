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
        BrailleIOMediator io;

        private volatile double[,] touchMatrix;
        Thread renderingThread;

        public bool[,] m
        {
            get;
            set;
        }

        BrailleIOAdapter_ShowOff showOffAdapter;

        #endregion

        public ShowOff()
        {
            InitializeComponent();
            this.Activate();
            this.Show();
        }

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

        public void SetTouchMatrix(double[,] touchMatrix) { this.touchMatrix = touchMatrix; this.paint(m); }

        #endregion
        
        #region GUI rendering

        #region Members
        private volatile bool _run = true;

        private volatile bool[,] rm;

        private Graphics _g;
        private readonly Object gLock = new Object();
        private Graphics PinGraphic
        {
            get
            {
                //lock (gLock)
                //{
                if (_g == null)
                {
                    _g = Graphics.FromImage(bmp);
                }
                return _g;
                //}
            }
            set
            {
                lock (gLock)
                {
                    _g = value;
                }
            }
        }

        private Bitmap _bmp;
        //private readonly object _bmpLock = new object();
        private Bitmap bmp
        {
            get
            {
                //lock (_bmpLock)
                //{
                if (_bmp == null)
                    _bmp = new Bitmap(panel1.Width, panel1.Height);

                return _bmp;
                //}
            }
        }

        private Graphics _pg;
        private Graphics PG
        {
            get { if (_pg == null && panel1 != null) { _pg = panel1.CreateGraphics(); } return _pg; }
        }

        private readonly Pen Stroke = new Pen(Brushes.LightGray, 0.4F);

        const int pixel = 5;

        private object graphicsLock = new object();

        Pen gridLinePen = new Pen(Brushes.Thistle, 1);
        Bitmap _baseImg;

        #endregion

        Bitmap generateBaseImage(bool rerender, int Width, int height)
        {
            if (_baseImg == null || rerender)
            {
                _baseImg = new Bitmap(panel1.Width, panel1.Height);
                using (Graphics big = Graphics.FromImage(_baseImg))
                {
                    big.FillRectangle(Brushes.White, 0, 0, _baseImg.Width, _baseImg.Height);
                    bool[,] m = new bool[height, Width];
                    for (int i = 0; i < m.GetLength(0); i++)
                    {
                        if ((i % 5) == 0)
                        {
                            big.DrawLine(gridLinePen, new Point(0, i * (pixel + 1) - 1), new Point(_baseImg.Width, i * (pixel + 1) - 1));
                            big.DrawLine(gridLinePen, new Point(0, (int)Math.Round((i + 2.5) * (pixel + 1) - 1)), new Point(_baseImg.Width, (int)Math.Round((i + 2.5) * (pixel + 1) - 1)));
                        }

                        for (int j = 0; j < m.GetLength(1); j++)
                        {
                            if ((j % 2) == 0)
                            {
                                big.DrawLine(gridLinePen, new Point(j * (pixel + 1) - 1, 0), new Point(j * (pixel + 1) - 1, _baseImg.Height));
                            }
                            big.DrawEllipse(Stroke, j * (pixel + 1), i * (pixel + 1), pixel - 1, pixel - 1);
                        }
                    }
                }
            }
            return _baseImg != null ? _baseImg.Clone() as Bitmap : null;
        }
        
        //paints display!
        public void paint(bool[,] m)
        {
            startRendering();
            rm = m;
        }

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
            catch (System.Threading.ThreadStartException) { }
            catch (System.Threading.ThreadStateException) { }
        }

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
                                    PinGraphic.FillRectangle(Brushes.Black, j * (pixel + 1), i * (pixel + 1), pixel, pixel);
                                    if (t > 0) PinGraphic.FillEllipse(Brushes.LightPink, (j * (pixel + 1)) + 1, (i * (pixel + 1)) + 1, pixel - 2, pixel - 2);
                                }
                                else
                                {
                                    if (t > 0) PinGraphic.FillEllipse(Brushes.Red, j * (pixel + 1), i * (pixel + 1), pixel, pixel);
                                    //    PinGraphic.DrawEllipse(Stroke, j * (pixel + 1), i * (pixel + 1), pixel - 1, pixel - 1);
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

        #region GUI Events

        #region mouse

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {

        }

        #endregion

        #region buttons

        private void button_KEY_RIGHT_ROCKER_SWITCH_UP_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "rsru" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_RIGHT_ROCKER_SWITCH_DOWN_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "rsrd" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_LEFT_CURSORS_UP_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.UpDown, new List<string>() { "clu" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_LEFT_CURSORS_LEFT_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.LeftDown, new List<string>() { "cll" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_LEFT_CURSORS_RIGHT_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.RightDown, new List<string>() { "clr" }, null, (int)DateTime.UtcNow.Ticks); }
        }


        private void button_KEY_LEFT_CURSORS_DOWN_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.DownDown, new List<string>() { "cld" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_RIGHT_CURSORS_UP_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "cru" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_RIGHT_CURSORS_RIGHT_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "crr" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_RIGHT_CURSORS_DOWN_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "crd" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_RIGHT_CURSORS_LEFT_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "crl" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_LEFT_ROCKER_SWITCH_UP_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.ZoomInDown, new List<string>() { "nsdd" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_LEFT_ROCKER_SWITCH_DOWN_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.ZoomOutDown, new List<string>() { "rsld" }, null, (int)DateTime.UtcNow.Ticks); }
        }        

        private void button_KEY_LEFT_CURSORS_CENTER_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.EnterDown, new List<string>() { "clc" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_HYPERBRAILLE_KEY_RIGHT_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.AbortDown, new List<string>() { "hbr" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_DOT7_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>(){"k7"}, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_DOT3_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "k3" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_DOT2_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "k2" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_DOT1_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "k1" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_THUMB_LEFT_HAND_LEFT_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "l" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_THUMB_LEFT_HAND_RIGHT_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "lr" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_THUMB_RIGHT_HAND_LEFT_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "rl" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_THUMB_RIGHT_HAND_RIGHT_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "r" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_DOT4_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "k4" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_DOT5_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "k5" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_DOT6_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "k6" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_DOT8_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "k8" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_RIGHT_CURSORS_CENTER_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "crc" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_NAV_LEFT_2_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "nsll" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_NAV_LEFT_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "nsl" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_NAV_RIGHT_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "nsr" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_NAV_RIGHT_2_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "nsrr" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_NAV_UP_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "nsu" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_NAV_UP_2_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "nsuu" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_NAV_DOWN_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "nsd" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_NAV_DOWN_2_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "nsdd" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_HYPERBRAILLE_KEY_LEFT_Click_1(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.GestureDown, new List<string>() { "hbl" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        #endregion

        #endregion

    }
}
