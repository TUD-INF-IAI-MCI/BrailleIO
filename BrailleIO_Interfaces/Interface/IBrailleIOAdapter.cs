using BrailleIO.Structs;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace BrailleIO.Interface
{
    #region generic button codes  

    /// <summary>
    /// Button states for the general buttons
    /// </summary>
    [Flags]
    public enum BrailleIO_DeviceButtonStates : int
    {
        /// <summary>
        /// no button is pressed or released
        /// </summary>
        None = 0,
        /// <summary>
        /// an unknown button is pressed or released --> check generic keys
        /// </summary>
        Unknown = 1,
        /// <summary>
        /// the enter button is released
        /// </summary>
        EnterUp = 2,
        /// <summary>
        /// the enter button is pressed
        /// </summary>
        EnterDown = 4,
        /// <summary>
        /// the abort button is released
        /// </summary>
        AbortUp = 8,
        /// <summary>
        /// the abort button is pressed
        /// </summary>
        AbortDown = 16,
        /// <summary>
        /// the gesture button is released
        /// </summary>
        GestureUp = 32,
        /// <summary>
        /// the gesture button is pressed
        /// </summary>
        GestureDown = 64,
        /// <summary>
        /// the 'left' button is released
        /// </summary>
        LeftUp = 128,
        /// <summary>
        /// the 'left' button is pressed
        /// </summary>
        LeftDown = 256,
        /// <summary>
        /// the 'right' button is released
        /// </summary>
        RightUp = 512,
        /// <summary>
        /// the 'right' button is pressed
        /// </summary>
        RightDown = 1024,
        /// <summary>
        /// the 'up' button is released
        /// </summary>
        UpUp = 2048,
        /// <summary>
        /// the 'up' button is pressed
        /// </summary>
        UpDown = 4096,
        /// <summary>
        /// the 'down' button is released
        /// </summary>
        DownUp = 8192,
        /// <summary>
        /// the 'down' button is pressed
        /// </summary>
        DownDown = 16384,
        /// <summary>
        /// the button for zooming in is released
        /// </summary>
        ZoomInUp = 32768,
        /// <summary>
        /// the button for zooming in is pressed
        /// </summary>
        ZoomInDown = 65536,
        /// <summary>
        /// the button for zooming out is released
        /// </summary>
        ZoomOutUp = 131072,
        /// <summary>
        /// the button for zooming out is pressed
        /// </summary>
        ZoomOutDown = 262144,


        k1Up,
        k1Down,
        k2Up,
        k2Down,
        k3Up,
        k3Down,
        k4Up,
        k4Down,
        k5Up,
        k5Down,
        k6Up,
        k6Down,
        k7Up,
        k7Down,
        k8Up,
        k8Down,


    }

    /// <summary>
    /// Generic buttons a touch sensitive, two-dimensional tactile pin device should have for a propper interaction
    /// </summary>
    [Flags]
    public enum BrailleIO_DeviceButton : int
    {
        /// <summary>
        /// No button
        /// </summary>
        None = 0,
        /// <summary>
        /// an unknown button --> check the generic keys
        /// </summary>
        Unknown = 1,
        /// <summary>
        /// the 'Ok' button
        /// </summary>
        Enter = 2,
        /// <summary>
        /// the 'cancel' button
        /// </summary>
        Abort = 4,
        /// <summary>
        /// a gesture button to indicate a gesture input to avoid midas-touch-effects
        /// </summary>
        Gesture = 8,
        /// <summary>
        /// a button for 'left' direction indication
        /// </summary>
        Left = 16,
        /// <summary>
        /// a button for 'right' direction indication
        /// </summary>
        Right = 32,
        /// <summary>
        /// a button for 'up' direction indication
        /// </summary>
        Up = 64,
        /// <summary>
        /// a button for 'down' direction indication
        /// </summary>
        Down = 128,
        /// <summary>
        /// a button to indicate the intense to magnify the content
        /// </summary>
        ZoomIn = 256,
        /// <summary>
        /// a button to indicate the intense to minify the content for a better overview
        /// </summary>
        ZoomOut = 512
    }

    #endregion

    #region EvetClasses

    /// <summary>
    /// Event arguments for BrailleIO events for pressed keys
    /// </summary>
    public class BrailleIO_KeyPressed_EventArgs : System.EventArgs
    {
        /// <summary>
        /// Combined general button states combined from BrailleIO_DeviceButtonStates
        /// </summary>
        public readonly BrailleIO_DeviceButtonStates keyCode;
        /// <summary>
        /// The original event args from the device in raw format without interpretation
        /// </summary>
        public readonly Object raw;
        /// <summary>
        /// Initializes a new instance of the <see cref="BrailleIO_KeyPressed_EventArgs"/> class.
        /// </summary>
        /// <param name="keyCode">The key code.</param>
        /// <param name="raw">The original raw event data from the device.</param>
        public BrailleIO_KeyPressed_EventArgs(BrailleIO_DeviceButtonStates keyCode, ref Object raw)
        {
            this.keyCode = keyCode;
            this.raw = raw;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="BrailleIO_KeyPressed_EventArgs" /> class.
        /// </summary>
        /// <param name="keyCode">The key code.</param>
        public BrailleIO_KeyPressed_EventArgs(BrailleIO_DeviceButtonStates keyCode)
        {
            this.keyCode = keyCode;
            this.raw = null;
        }
    }

    /// <summary>
    /// Event arguments for BrailleIO events when ever keys change there states
    /// </summary>
    public class BrailleIO_KeyStateChanged_EventArgs : System.EventArgs
    {
        /// <summary>
        /// Combined general button states combined from BrailleIO_DeviceButtonStates
        /// </summary>
        public readonly BrailleIO_DeviceButtonStates keyCode;
        /// <summary>
        /// The original event args from the device in raw format without interpretation
        /// </summary>
        public readonly OrderedDictionary raw;
        /// <summary>
        /// Initializes a new instance of the <see cref="BrailleIO_KeyStateChanged_EventArgs"/> class.
        /// </summary>
        /// <param name="keyCode">The key code.</param>
        /// <param name="raw">The original raw event data from the device.</param>
        public BrailleIO_KeyStateChanged_EventArgs(BrailleIO_DeviceButtonStates keyCode, ref OrderedDictionary raw)
        {
            this.keyCode = keyCode;
            this.raw = raw;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="BrailleIO_KeyStateChanged_EventArgs" /> class.
        /// </summary>
        /// <param name="keyCode">The key code.</param>
        public BrailleIO_KeyStateChanged_EventArgs(BrailleIO_DeviceButtonStates keyCode)
        {
            this.keyCode = keyCode;
            this.raw = null;
        }
    }

    /// <summary>
    /// Event arguments for BrailleIO events if an adapter was initializes successfully 
    /// </summary>
    public class BrailleIO_Initialized_EventArgs : System.EventArgs
    {
        /// <summary>
        /// the device indicator for the initialized adapter
        /// </summary>
        public readonly BrailleIODevice device;
        /// <summary>
        /// Initializes a new instance of the <see cref="BrailleIO_Initialized_EventArgs"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        public BrailleIO_Initialized_EventArgs(BrailleIODevice device) { this.device = device; }
    }

    /// <summary>
    /// Event arguments for BrailleIO events that indicates a change in the touch detection of an adapter
    /// </summary>
    public class BrailleIO_TouchValuesChanged_EventArgs : System.EventArgs
    {
        /// <summary>
        /// OPTIONAL list of more detailed touch information.
        /// </summary>
        public List<Touch> DetailedTouches { get; private set; }
        /// <summary>
        /// the normalized matrix (from 0.0 to 1.0) of detected touch values per pin  
        /// </summary>
        public readonly double[,] touches;
        /// <summary>
        /// time stamp of the occurred event for temporal order
        /// </summary>
        public readonly int timestamp;
        /// <summary>
        /// The original event args from the device in raw format without interpretation
        /// </summary>
        public readonly OrderedDictionary raw;
        /// <summary>
        /// Initializes a new instance of the <see cref="BrailleIO_TouchValuesChanged_EventArgs"/> class.
        /// </summary>
        /// <param name="touches">the normalized matrix (from 0.0 to 1.0) of detected touch values per pin.</param>
        /// <param name="timestamp">The original event args from the device in raw format without interpretation.
        /// But could also be an own timestamp such as <c>(int)DateTime.UtcNow.Ticks</c>.
        /// </param>
        /// <param name="raw">The original event args from the device in raw format without interpretation.</param>
        public BrailleIO_TouchValuesChanged_EventArgs(double[,] touches, int timestamp, ref OrderedDictionary raw):this(touches, timestamp, ref raw, null) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="BrailleIO_TouchValuesChanged_EventArgs"/> class.
        /// </summary>
        /// <param name="touches">the normalized matrix (from 0.0 to 1.0) of detected touch values per pin.</param>
        /// <param name="timestamp">The original event args from the device in raw format without interpretation.
        /// But could also be an own timestamp such as <c>(int)DateTime.UtcNow.Ticks</c>.
        /// </param>
        /// <param name="raw">The original event args from the device in raw format without interpretation.</param>
        public BrailleIO_TouchValuesChanged_EventArgs(double[,] touches, int timestamp, ref OrderedDictionary raw, List<Touch> detailedTouches)
        {
            this.touches = touches;
            this.timestamp = timestamp;
            this.raw = raw;
            this.DetailedTouches = detailedTouches;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="BrailleIO_TouchValuesChanged_EventArgs" /> class.
        /// </summary>
        /// <param name="touches">the normalized matrix (from 0.0 to 1.0) of detected touch values per pin.</param>
        /// <param name="timestamp">The original event args from the device in raw format without interpretation.
        /// But could also be an own timestamp such as <c>(int)DateTime.UtcNow.Ticks</c>.</param>
        public BrailleIO_TouchValuesChanged_EventArgs(double[,] touches, int timestamp) : this(touches, timestamp, null) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="BrailleIO_TouchValuesChanged_EventArgs" /> class.
        /// </summary>
        /// <param name="touches">the normalized matrix (from 0.0 to 1.0) of detected touch values per pin.</param>
        /// <param name="timestamp">The original event args from the device in raw format without interpretation.
        /// But could also be an own timestamp such as <c>(int)DateTime.UtcNow.Ticks</c>.</param>
        /// <param name="detailedTouches">The detailed touches.</param>
        public BrailleIO_TouchValuesChanged_EventArgs(double[,] touches, int timestamp, List<Touch> detailedTouches)
        {
            this.touches = touches;
            this.timestamp = timestamp;
            this.raw = null;
            this.DetailedTouches = detailedTouches;
        }
    }

    /// <summary>
    /// Event for BrailleIO Adapter to indicate that the sate of there presented pins has changed
    /// NOT USED YET
    /// </summary>
    public class BrailleIO_PinStateChanged_EventArgs : System.EventArgs
    {
        //TODO: find out what
    }

    /// <summary>
    /// BrailleIO Event for error in the Adapter
    /// </summary>
    public class BrailleIO_ErrorOccured_EventArgs : System.EventArgs
    {
        /// <summary>
        /// Code for indicating the class of error that is occurred
        /// </summary>
        public readonly ErrorCode errorCode;
        /// <summary>
        /// The original event args from the device in raw format without interpretation
        /// </summary>
        public readonly OrderedDictionary raw;
        /// <summary>
        /// Initializes a new instance of the <see cref="BrailleIO_ErrorOccured_EventArgs"/> class.
        /// </summary>
        /// <param name="errorCode">Code for indicating the class of error that is occurred.</param>
        /// <param name="raw">The original event args from the device in raw format without interpretation.</param>
        public BrailleIO_ErrorOccured_EventArgs(ErrorCode errorCode, ref OrderedDictionary raw)
        {
            this.errorCode = errorCode;
            this.raw = raw;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="BrailleIO_ErrorOccured_EventArgs" /> class.
        /// </summary>
        /// <param name="errorCode">Code for indicating the class of error that is occurred.</param>
        public BrailleIO_ErrorOccured_EventArgs(ErrorCode errorCode)
        {
            this.errorCode = errorCode;
            this.raw = null;
        }
    }

    /// <summary>
    /// General groups for errors that can occur
    /// </summary>
    public enum ErrorCode
    {
        /// <summary>
        /// everything is fine
        /// </summary>
        NONE,
        /// <summary>
        /// sending matrix to device returned FALSE
        /// </summary>
        CANT_SENT_MATRIX,
        /// <summary>
        /// device is not available
        /// </summary>
        DEVICE_UNAVAILABLE,
        /// <summary>
        /// unknown error occurred
        /// </summary>
        UNKNOWN,
        /// <summary>
        /// warning happens
        /// </summary>
        WARNING,
    }

    /// <summary>
    /// <seealso cref="BrailleIO_TouchValuesChanged_EventArgs"/> Indicates that some input changes happens
    /// </summary>
    public class BrailleIO_InputChanged_EventArgs : System.EventArgs
    {
        /// <summary>
        /// the normalized matrix (from 0.0 to 1.0) of detected touch values per pin  
        /// </summary>
        public readonly bool[,] touches;
        /// <summary>
        /// time stamp of the occurred event for temporal order
        /// </summary>
        public readonly int timestamp;
        /// <summary>
        /// The original event args from the device in raw format without interpretation
        /// </summary>
        public readonly OrderedDictionary raw;
        /// <summary>
        /// Initializes a new instance of the <see cref="BrailleIO_InputChanged_EventArgs"/> class.
        /// </summary>
        /// <param name="touches">the normalized matrix (from 0.0 to 1.0) of detected touch values per pin  .</param>
        /// <param name="timestamp">time stamp of the occurred event for temporal order.</param>
        /// <param name="raw">The original event args from the device in raw format without interpretation.</param>
        public BrailleIO_InputChanged_EventArgs(bool[,] touches, int timestamp, ref OrderedDictionary raw)
        {
            this.touches = touches;
            this.timestamp = timestamp;
            this.raw = raw;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="BrailleIO_InputChanged_EventArgs" /> class.
        /// </summary>
        /// <param name="touches">the normalized matrix (from 0.0 to 1.0) of detected touch values per pin  .</param>
        /// <param name="timestamp">time stamp of the occurred event for temporal order.</param>
        public BrailleIO_InputChanged_EventArgs(bool[,] touches, int timestamp)
        {
            this.touches = touches;
            this.timestamp = timestamp;
            this.raw = null;
        }
    }
    #endregion

    /// <summary>
    /// Interface an adapter (means a real hardware abstracting implementation for an input or output device) has to implement.
    /// </summary>
    public interface IBrailleIOAdapter
    {
        #region Properties

        /// <summary>
        /// Gets a value indicating whether this <see cref="IBrailleIOAdapter"/> is connected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if connected; otherwise, <c>false</c>.
        /// </value>
        bool Connected { get; }

        /// <summary>
        /// Gets the horizontal resolution of this device in Dots Per Inch.
        /// </summary>
        /// <value>
        /// The horizontal DPI.
        /// </value>
        float DpiX { get; }
        /// <summary>
        /// Gets the vertical resolution of this device in Dots Per Inch.
        /// </summary>
        /// <value>
        /// The vertical DPI.
        /// </value>
        float DpiY { get; }

        /// <summary>
        /// Gets the corresponding hardware device to this wrapper.
        /// </summary>
        /// <value>
        /// The device.
        /// </value>
        BrailleIODevice Device { get; }

        #endregion

        /// <summary>
        /// Connects this instance.
        /// </summary>
        /// <returns>if the device could been connected or not</returns>
        bool Connect();
        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        /// <returns>if the device could been disconnected or not</returns>
        bool Disconnect();

        /// <summary>
        /// Synchronizes the specified matrix. That means the matrix will be send to the hardware device as output
        /// </summary>
        /// <param name="matrix">The matrix to show.</param>
        void Synchronize(bool[,] matrix);
        /// <summary>
        /// Recalibrate the touch abilities of the device with  the specified threshold.
        /// </summary>
        /// <param name="threshold">The threshold.</param>
        /// <returns>if the device could been calibrated or not</returns>
        bool Recalibrate(double threshold);

        #region event handlers
        /// <summary>
        /// Occurs when a key was pressed.
        /// </summary>
        event EventHandler<BrailleIO_KeyPressed_EventArgs> keyPressed;
        /// <summary>
        /// Occurs when the state of a key has changed. This can be a pressed or a released
        /// </summary>
        event EventHandler<BrailleIO_KeyStateChanged_EventArgs> keyStateChanged;
        /// <summary>
        /// Occurs when the device was successfully initialized.
        /// </summary>
        event EventHandler<BrailleIO_Initialized_EventArgs> initialized;
        /// <summary>
        /// Occurs when some properties of the input changes.
        /// </summary>
        event EventHandler<BrailleIO_InputChanged_EventArgs> inputChanged;
        /// <summary>
        /// Occurs when some touch values had changed.
        /// </summary>
        event EventHandler<BrailleIO_TouchValuesChanged_EventArgs> touchValuesChanged;
        /// <summary>
        /// Occurs when  some pin states had changed.
        /// </summary>
        event EventHandler<BrailleIO_PinStateChanged_EventArgs> pinStateChanged;
        /// <summary>
        /// Occurs when an error has occurred.
        /// </summary>
        event EventHandler<BrailleIO_ErrorOccured_EventArgs> errorOccurred;
        #endregion
    }
}