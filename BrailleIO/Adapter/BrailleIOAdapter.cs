using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using BrailleIO.Interface;

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

        ///// <summary>
        ///// Gets the UID of the device.
        ///// </summary>
        ///// <value>The UID.</value>
        //public string UID { get; protected set;}

        ///// <summary>
        ///// Gets or sets the horizontal dimension of the pin matrix for this device.
        ///// </summary>
        ///// <value>The device size X.</value>
        //public int DeviceSizeX
        //{
        //    get;
        //    set;
        //}
        ///// <summary>
        ///// Gets or sets the vertical dimension of the pin matrix for this device.
        ///// </summary>
        ///// <value>The device size Y.</value>
        //public int DeviceSizeY
        //{
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// Gets or sets the maximal refresh rate for the device.
        ///// </summary>
        ///// <value>The refresh rate in milliseconds.</value>
        //public int RefreshRate
        //{
        //    get;
        //    set;
        //}
        BrailleIODevice _device;
        public BrailleIODevice Device
        {
            get { if (_device == null) _device = createDevice(); return _device; }
            protected set { _device = value; }
        }

        protected virtual BrailleIODevice createDevice()
        {
            return new BrailleIODevice(120, 60, "UNKNOWN", true, true, 10);
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

        #region event delegates
        public delegate void BrailleIO_KeyPressed_EventHandler(object sender, BrailleIO_KeyPressed_EventArgs e);
        public delegate void BrailleIO_KeyStateChanged_EventHandler(object sender, BrailleIO_KeyStateChanged_EventArgs e);
        public delegate void BrailleIO_Initialized_EventHandler(object sender, BrailleIO_Initialized_EventArgs e);
        public delegate void BrailleIO_InputChanged_EventHandler(object sender, BrailleIO_InputChanged_EventArgs e);
        public delegate void BrailleIO_TouchValuesChanged_EventHandler(object sender, BrailleIO_TouchValuesChanged_EventArgs e);
        public delegate void BrailleIO_PinStateChanged_EventHandler(object sender, BrailleIO_PinStateChanged_EventArgs e);
        public delegate void BrailleIO_ErrorOccured_EventHandler(object sender, BrailleIO_ErrorOccured_EventArgs e);
        #endregion

        #region event handlers
        public event BrailleIO_KeyPressed_EventHandler keyPressed;
        public event BrailleIO_KeyStateChanged_EventHandler keyStateChanged;
        public event BrailleIO_Initialized_EventHandler initialized;
        public event BrailleIO_InputChanged_EventHandler inputChanged;
        public event BrailleIO_TouchValuesChanged_EventHandler touchValuesChanged;
        public event BrailleIO_PinStateChanged_EventHandler pinStateChanged;
        public event BrailleIO_ErrorOccured_EventHandler errorOccured;
        #endregion

        #region on before event handlers
        internal virtual void OnBrailleIO_KeyPressed_EventHandler(BrailleIO_KeyPressed_EventArgs e) { keyPressed(this, e); }
        internal virtual void OnBrailleIO_KeyStateChanged_EventHandler(BrailleIO_KeyStateChanged_EventArgs e) { keyStateChanged(this, e); }
        internal virtual void OnBrailleIO_TouchValuesChanged_EventHandler(BrailleIO_TouchValuesChanged_EventArgs e) { touchValuesChanged(this, e); }
        internal virtual void OnBrailleIO_PinStateChanged_EventHandler(BrailleIO_PinStateChanged_EventArgs e) { pinStateChanged(this, e); }
        internal virtual void OnBrailleIO_ErrorOccured_EventHandler(BrailleIO_ErrorOccured_EventArgs e) { errorOccured(this, e); }
        #endregion

        #region subscribe to eventhandlers // called and specified and wrapped in driver specific adapters
        public virtual void subscribe_BrailleIO_KeyPressed_EventHandler(BrailleIO_KeyPressed_EventHandler handler)
        {
            this.keyPressed += handler;
        }
        public virtual void subscribe_BrailleIO_KeyStateChanged_EventHandler(BrailleIO_KeyStateChanged_EventHandler handler)
        {
            this.keyStateChanged += handler;
        }
        public virtual void subscribe_BrailleIO_TouchValuesChanged_EventHandler(BrailleIO_TouchValuesChanged_EventHandler handler)
        {
            this.touchValuesChanged += handler;
        }
        public virtual void subscribe_BrailleIO_PinStateChanged_EventHandler(BrailleIO_PinStateChanged_EventHandler handler)
        {
            this.pinStateChanged += handler;
        }
        public virtual void subscribe_BrailleIO_ErrorOccured_EventHandler(BrailleIO_ErrorOccured_EventHandler handler)
        {
            this.errorOccured += handler;
        }
        #endregion

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
        public virtual void Synchronize(bool[,] matrix)
        {
            manager.ActiveAdapter.Synchronize(matrix);
        }
        public virtual void fireErrorOccured(ErrorCode errorCode, ref OrderedDictionary raw)
        {
            if (errorOccured != null)
                errorOccured(this, new BrailleIO_ErrorOccured_EventArgs(errorCode, ref raw));
        }



    }


}
