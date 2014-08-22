using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using BrailleIO.Interface;
using System.Threading.Tasks;

namespace BrailleIO
{

    public abstract class AbstractBrailleIOAdapterBase : IBrailleIOAdapter
    {
        // display options

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="AbstractBrailleIOAdapterBase"/> is synchronize.
        /// </summary>
        /// <value><c>true</c> if synchronize; otherwise, <c>false</c>.</value>
        public bool Synch
        {
            get;
            set;
        }

        BrailleIODevice _device;
        public BrailleIODevice Device
        {
            get { if (_device == null) _device = createDevice(); return _device; }
            protected set { _device = value; }
        }

        protected virtual BrailleIODevice createDevice()
        {
            return new BrailleIODevice(120, 60, "UNKNOWN", true, true, 10, this.GetType().ToString());
        }

        private float _dpiX = 10;
        /// <summary>
        /// Gets or sets the horizontal resolution of the pin matrix for this device.
        /// </summary>
        /// <value>The horizontal device resolution in dpi.</value>
        public virtual float DpiX
        {
            get { return _dpiX; }
        }

        private float _dpiY = 10;
        /// <summary>
        /// Gets or sets the vertical resolution of the pin matrix for this device.
        /// </summary>
        /// <value>The vertical device resolution in dpi.</value>
        public virtual float DpiY
        {
            get { return _dpiY; }
        }


        private bool _connected = false;
        /// <summary>
        /// Gets a value indicating whether this <see cref="AbstractBrailleIOAdapterBase"/> is connected or not.
        /// </summary>
        /// <value><c>true</c> if connected; otherwise, <c>false</c>.</value>
        public bool Connected
        {
            get
            {
                if (!_connected) _connected = this.Connect();
                return _connected;
            }
            protected set { _connected = value; }
        }

        AbstractBrailleIOAdapterManagerBase manager = null;
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractBrailleIOAdapterBase"/> class.
        /// </summary>
        /// <param name="manager">The manager.</param>
        public AbstractBrailleIOAdapterBase(ref AbstractBrailleIOAdapterManagerBase manager)
        {
            this.manager = manager;
        }

        /// <summary>
        /// Connects this instance.
        /// </summary>
        /// <returns></returns>
        public virtual bool Connect()
        {
            if (manager.ActiveAdapter == null)
            {
                manager.ActiveAdapter = this;
                _connected = true;
            }
            return _connected;
        }

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        /// <returns></returns>
        public virtual bool Disconnect()
        {
            manager.ActiveAdapter = null;
            return true;
        }

        #region on before event handlers
        internal virtual void OnBrailleIO_KeyPressed_EventHandler(BrailleIO_KeyPressed_EventArgs e) { keyPressed(this, e); }
        internal virtual void OnBrailleIO_KeyStateChanged_EventHandler(BrailleIO_KeyStateChanged_EventArgs e) { keyStateChanged(this, e); }
        internal virtual void OnBrailleIO_TouchValuesChanged_EventHandler(BrailleIO_TouchValuesChanged_EventArgs e) { touchValuesChanged(this, e); }
        internal virtual void OnBrailleIO_PinStateChanged_EventHandler(BrailleIO_PinStateChanged_EventArgs e) { pinStateChanged(this, e); }
        internal virtual void OnBrailleIO_ErrorOccured_EventHandler(BrailleIO_ErrorOccured_EventArgs e) { errorOccured(this, e); }
        #endregion

        public event EventHandler<BrailleIO_KeyPressed_EventArgs> keyPressed;
        public event EventHandler<BrailleIO_KeyStateChanged_EventArgs> keyStateChanged;
        public event EventHandler<BrailleIO_Initialized_EventArgs> initialized;
        public event EventHandler<BrailleIO_InputChanged_EventArgs> inputChanged;
        public event EventHandler<BrailleIO_TouchValuesChanged_EventArgs> touchValuesChanged;
        public event EventHandler<BrailleIO_PinStateChanged_EventArgs> pinStateChanged;
        public event EventHandler<BrailleIO_ErrorOccured_EventArgs> errorOccured;

        protected virtual void fireInitialized(BrailleIO_Initialized_EventArgs e)
        {
            if (initialized != null)
                initialized(this, e);
        }
        protected virtual void fireKeyStateChanged(BrailleIO_DeviceButtonStates keyCode, ref OrderedDictionary raw)
        {
            if (keyStateChanged != null)
                keyStateChanged(this, new BrailleIO_KeyStateChanged_EventArgs(keyCode, ref raw));
        }
        protected virtual void fireInputChanged(bool[,] touches, int timestamp, ref OrderedDictionary raw)
        {
            if (inputChanged != null)
                inputChanged(this, new BrailleIO_InputChanged_EventArgs(touches, timestamp, ref raw));
        }
        protected virtual void fireTouchValuesChanged(double[,] touches, int timestamp, ref OrderedDictionary raw)
        {
            if (touchValuesChanged != null)
                touchValuesChanged(this, new BrailleIO_TouchValuesChanged_EventArgs(touches, timestamp, ref raw));
        }
        public virtual void fireErrorOccured(ErrorCode errorCode, ref OrderedDictionary raw)
        {
            if (errorOccured != null)
                errorOccured(this, new BrailleIO_ErrorOccured_EventArgs(errorCode, ref raw));
        }

        public virtual void Synchronize(bool[,] matrix)
        {
            manager.ActiveAdapter.Synchronize(matrix);
        }

        #region Touch


        public virtual bool Recalibrate(double threshold)
        {
            if (Device != null)
            {
                touchValuesChanged += new EventHandler<BrailleIO_TouchValuesChanged_EventArgs>(AbstractBrailleIOAdapterBase_touchValuesChanged);
                bool[,] empty = new bool[Device.DeviceSizeY, Device.DeviceSizeX];
                bool[,] full = GetFullSetMatrix();
                //clear the correction matrix 
                touchCorrectionMatrix = new double[Device.DeviceSizeY, Device.DeviceSizeX];

                Synchronize(empty);
                System.Threading.Thread.Sleep(1000);
                Synchronize(full);
                System.Threading.Thread.Sleep(1000);
                Synchronize(empty);
                System.Threading.Thread.Sleep(1000);
                Synchronize(full);
                System.Threading.Thread.Sleep(1000);
                Synchronize(empty);

                touchValuesChanged -= new EventHandler<BrailleIO_TouchValuesChanged_EventArgs>(AbstractBrailleIOAdapterBase_touchValuesChanged);
                touchCorrectionMatrix = tempTouchCorrectionMatrix;
            }

            return true;
        }

        void AbstractBrailleIOAdapterBase_touchValuesChanged(object sender, BrailleIO_TouchValuesChanged_EventArgs e)
        {
            addToFlickeringTouchMatrix(e.touches);
        }

        double[,] _touchCorrectionMatrix;
        private static readonly object tcmLock = new object();
        protected double[,] touchCorrectionMatrix
        {
            get { return _touchCorrectionMatrix; }
            set { lock (tcmLock) { _touchCorrectionMatrix = value; } }
        }

        double[,] _tmpTouchCorrectionMatrix;
        static readonly object ttcmLock = new object();
        double[,] tempTouchCorrectionMatrix
        {
            get { return _tmpTouchCorrectionMatrix; }
            set { lock (ttcmLock) { _tmpTouchCorrectionMatrix = value; } }
        }

        private double[,] addToFlickeringTouchMatrix(double[,] tMatrix)
        {
            double[,] ftm = tempTouchCorrectionMatrix;

            if (ftm != null && tMatrix != null)
            {
                int rows = tMatrix.GetLength(0);
                int cols = tMatrix.GetLength(1);

                Parallel.For(0, rows, i =>
                {
                    Parallel.For(0, cols, j =>
                    {
                        try
                        {
                            // Use a temporary to improve parallel performance. 
                            double temp = 0;
                            temp += Math.Max(tMatrix[i, j], ftm[i, j]);
                            ftm[i, j] = temp;
                        }
                        catch (System.Exception ex)
                        {

                        }

                    }); // Parallel.For cols

                }); // Parallel.For rows
            }
            else
            {
                ftm = tMatrix;
            }
            tempTouchCorrectionMatrix = ftm;
            return ftm;
        }


        private bool[,] _fullMatrix;
        public bool[,] GetFullSetMatrix()
        {
            if (_fullMatrix == null || (Device != null && (_fullMatrix.GetLength(0) != Device.DeviceSizeY || _fullMatrix.GetLength(1) != Device.DeviceSizeX)))
            {
                int rows = Device.DeviceSizeY;
                int cols = Device.DeviceSizeX;
                _fullMatrix = new bool[rows, cols];
                Parallel.For(0, rows, i => {
                    Parallel.For(0, cols, j => { try { _fullMatrix[i, j] = true; } catch{} });
                });
            }
            return _fullMatrix;
        }
        #endregion

    }


}
