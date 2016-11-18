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
    }

    /// <summary>
    /// Generic buttons a touch sensitive, two-dimensional tactile pin device 
    /// should have for a proper interaction
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
        Abort = 8,
        /// <summary>
        /// a gesture button to indicate a gesture input to avoid midas-touch-effects
        /// </summary>
        Gesture = 32,
        /// <summary>
        /// a button for 'left' direction indication
        /// </summary>
        Left = 128,
        /// <summary>
        /// a button for 'right' direction indication
        /// </summary>
        Right = 512,
        /// <summary>
        /// a button for 'up' direction indication
        /// </summary>
        Up = 2048,
        /// <summary>
        /// a button for 'down' direction indication
        /// </summary>
        Down = 8192,
        /// <summary>
        /// a button to indicate the intense to magnify the content
        /// </summary>
        ZoomIn = 32768,
        /// <summary>
        /// a button to indicate the intense to minify the content for a better overview
        /// </summary>
        ZoomOut = 131072
    }

    #endregion

    #region Braille keyboard key codes

    /// <summary>
    /// Button states for buttons of a Braille keyboard
    /// </summary>
    [Flags]
    public enum BrailleIO_BrailleKeyboardButtonStates : int
    {
        /// <summary>
        /// no button is pressed or released
        /// </summary>
        None = 0,
        /// <summary>
        /// an unknown keyboard button is pressed or released --> check generic keys
        /// </summary>
        Unknown = 1,        // 0
        /// <summary>
        /// the point 1 button is released
        /// </summary>
        k1Up = 2,           // 1
        /// <summary>
        /// the point 1 button is pressed
        /// </summary>
        k1Down = 4,         // 2
        /// <summary>
        /// the point 2 button is released
        /// </summary>
        k2Up = 8,           // 3
        /// <summary>
        /// the point 2 button is pressed
        /// </summary>
        k2Down = 16,        // 4
        /// <summary>
        /// the point 3 button is released
        /// </summary>
        k3Up = 32,          // 5
        /// <summary>
        /// the point 3 button is pressed
        /// </summary>
        k3Down = 64,        // 6
        /// <summary>
        /// the point 4 button is released
        /// </summary>
        k4Up = 128,         // 7
        /// <summary>
        /// the point 4 button is pressed
        /// </summary>
        k4Down = 256,       // 8
        /// <summary>
        /// the point 5 button is released
        /// </summary>
        k5Up = 512,         // 9
        /// <summary>
        /// the point 5 button is pressed
        /// </summary>
        k5Down = 1024,      // 10
        /// <summary>
        /// the point 6 button is released
        /// </summary>
        k6Up = 2048,        // 11
        /// <summary>
        /// the point 6 button is pressed
        /// </summary>
        k6Down = 4096,      // 12
        /// <summary>
        /// the point 7 button is released
        /// </summary>
        k7Up = 8192,        // 13
        /// <summary>
        /// the point 7 button is pressed
        /// </summary>
        k7Down = 16384,     // 14
        /// <summary>
        /// the point 8 button is released
        /// </summary>
        k8Up = 32768,       // 15
        /// <summary>
        /// the point 8 button is pressed
        /// </summary>
        k8Down = 65536,     // 16
        /// <summary>
        /// the button for the first function key is released. Commonly used for the left thumb.
        /// </summary>
        F1Up = 131072,      // 17
        /// <summary>
        /// the button for the first function key out is pressed. Commonly used for the left thumb.
        /// </summary>
        F1Down = 262144,    // 18
        /// <summary>
        /// additional/optional button to the first function key is released. Commonly used for the left thumb.
        /// </summary>
        F11Up = 524288,     // 19
        /// <summary>
        /// additional/optional button to the first function key is pressed. Commonly used for the left thumb.
        /// </summary>
        F11Down = 1048576,  // 20
        /// <summary>
        /// the button for the second function key is released. Commonly used for the right thumb.
        /// </summary>
        F2Up = 2097152,     // 21
        /// <summary>
        /// the button for the second function key out is pressed. Commonly used for the right thumb.
        /// </summary>
        F2Down = 4194304,   // 22
        /// <summary>
        /// additional/optional button to the second function key is released. Commonly used for the right thumb.
        /// </summary>
        F22Up = 8388608,    // 23
        /// <summary>
        /// additional/optional button to the second function key is pressed. Commonly used for the right thumb.
        /// </summary>
        F22Down = 16777216, // 24
    }

    /// <summary>
    /// Buttons of a Braille keyboard
    /// </summary>
    [Flags]
    public enum BrailleIO_BrailleKeyboardButton : int
    {
        /// <summary>
        /// no button is pressed or released
        /// </summary>
        None = 0,
        /// <summary>
        /// an unknown keyboard button is pressed or released --> check generic keys
        /// </summary>
        Unknown = 1,      // 0
        /// <summary>
        /// the point 1 button
        /// </summary>
        k1 = 2,           // 1
        /// <summary>
        /// the point 2 button
        /// </summary>
        k2 = 8,           // 3
        /// <summary>
        /// the point 3
        /// </summary>
        k3 = 32,          // 5
        /// <summary>
        /// the point 4 button
        /// </summary>
        k4 = 128,         // 7
        /// <summary>
        /// the point 5 button
        /// </summary>
        k5 = 512,         // 9
        /// <summary>
        /// the point 6 button
        /// </summary>
        k6 = 2048,        // 11
        /// <summary>
        /// the point 7 button
        /// </summary>
        k7 = 8192,        // 13
        /// <summary>
        /// the point 8 button
        /// </summary>
        k8 = 32768,       // 15
        /// <summary>
        /// the button for the first function key. Commonly used for the left thumb.
        /// </summary>
        F1 = 131072,      // 17
        /// <summary>
        /// additional/optional button to the first function key. Commonly used for the left thumb.
        /// </summary>
        F11 = 524288,     // 19
        /// <summary>
        /// the button for the second function key. Commonly used for the right thumb.
        /// </summary>
        F2 = 2097152,     // 21
        /// <summary>
        /// additional/optional button to the second function key. Commonly used for the right thumb.
        /// </summary>
        F22 = 8388608,    // 23
    }

    #endregion

    #region additional function key codes

    /// <summary>
    /// Button states for 15 additional function buttons
    /// </summary>
    [Flags]
    public enum BrailleIO_AdditionalButtonStates : int
    {
        /// <summary>
        /// no button is pressed or released
        /// </summary>
        None = 0,
        /// <summary>
        /// an unknown additional button is pressed or released --> check generic keys
        /// </summary>
        Unknown = 1,        // 0
        /// <summary>
        /// the additional function button 1 is released
        /// </summary>
        fn1Up = 2,           // 1
        /// <summary>
        /// the additional function button 1 is pressed
        /// </summary>
        fn1Down = 4,         // 2
        /// <summary>
        /// the additional function button 2 is released
        /// </summary>
        fn2Up = 8,           // 3
        /// <summary>
        /// the additional function button 2 is pressed
        /// </summary>
        fn2Down = 16,        // 4
        /// <summary>
        /// the additional function button 3 is released
        /// </summary>
        fn3Up = 32,          // 5
        /// <summary>
        /// the additional function button 3 is pressed
        /// </summary>
        fn3Down = 64,        // 6
        /// <summary>
        /// the additional function button 4 is released
        /// </summary>
        fn4Up = 128,         // 7
        /// <summary>
        /// the additional function button 4 is pressed
        /// </summary>
        fn4Down = 256,       // 8
        /// <summary>
        /// the additional function button 5 is released
        /// </summary>
        fn5Up = 512,         // 9
        /// <summary>
        /// the additional function button 5 is pressed
        /// </summary>
        fn5Down = 1024,      // 10
        /// <summary>
        /// the additional function button 6 is released
        /// </summary>
        fn6Up = 2048,        // 11
        /// <summary>
        /// the additional function button 6 is pressed
        /// </summary>
        fn6Down = 4096,      // 12
        /// <summary>
        /// the additional function button 7 is released
        /// </summary>
        fn7Up = 8192,        // 13
        /// <summary>
        /// the additional function button 7 is pressed
        /// </summary>
        fn7Down = 16384,     // 14
        /// <summary>
        /// the additional function button 8 is released
        /// </summary>
        fn8Up = 32768,       // 15
        /// <summary>
        /// the additional function button 8 is pressed
        /// </summary>
        fn8Down = 65536,     // 16
        /// <summary>
        /// the additional function button 9 is released
        /// </summary>
        fn9Up = 131072,      // 17
        /// <summary>
        /// the additional function button 9 is pressed
        /// </summary>
        fn9Down = 262144,    // 18
        /// <summary>
        /// the additional function button 10 is released
        /// </summary>
        fn10Up = 524288,     // 19
        /// <summary>
        /// the additional function button 10 is pressed
        /// </summary>
        fn10Down = 1048576,  // 20
        /// <summary>
        /// the additional function button 11 is released
        /// </summary>
        fn11Up = 2097152,     // 21
        /// <summary>
        /// the additional function button 11 is pressed
        /// </summary>
        fn11Down = 4194304,   // 22
        /// <summary>
        /// the additional function button 12 is released
        /// </summary>
        fn12Up = 8388608,    // 23
        /// <summary>
        /// the additional function button 12 is pressed
        /// </summary>
        fn12Down = 16777216, // 24
        /// <summary>
        /// the additional function button 13 is released
        /// </summary>
        fn13Up = 33554432, // 25
        /// <summary>
        /// the additional function button 13 is pressed
        /// </summary>
        fn13Down = 67108864, // 26
        /// <summary>
        /// the additional function button 14 is released
        /// </summary>
        fn14Up = 134217728, // 27
        /// <summary>
        /// the additional function button 14 is pressed
        /// </summary>
        fn14Down = 268435456, // 28
        /// <summary>
        /// the additional function button 15 is released
        /// </summary>
        fn15Up = 536870912, // 29
        /// <summary>
        /// the additional function button 15 is pressed
        /// </summary>
        fn15Down = 1073741824, // 30
    }

    /// <summary>
    /// 15 Buttons for additional functions
    /// </summary>
    [Flags]
    public enum BrailleIO_AdditionalButton : int
    {
        /// <summary>
        /// no button is pressed or released
        /// </summary>
        None = 0,
        /// <summary>
        /// an unknown additional button
        /// </summary>
        Unknown = 1,        // 0
        /// <summary>
        /// the additional function button 1
        /// </summary>
        fn1 = 2,           // 1
        /// <summary>
        /// the additional function button 2
        /// </summary>
        fn2 = 8,           // 3
        /// <summary>
        /// the additional function button 3
        /// </summary>
        fn3 = 32,          // 5
        /// <summary>
        /// the additional function button 4
        /// </summary>
        fn4 = 128,         // 7
        /// <summary>
        /// the additional function button 5
        /// </summary>
        fn5 = 512,         // 9
        /// <summary>
        /// the additional function button 6
        /// </summary>
        fn6 = 2048,        // 11
        /// <summary>
        /// the additional function button 7
        /// </summary>
        fn7 = 8192,        // 13
        /// <summary>
        /// the additional function button 8
        /// </summary>
        fn8 = 32768,       // 15
        /// <summary>
        /// the additional function button 9
        /// </summary>
        fn9 = 131072,      // 17
        /// <summary>
        /// the additional function button 10
        /// </summary>
        fn10 = 524288,     // 19
        /// <summary>
        /// the additional function button 11
        /// </summary>
        fn11 = 2097152,     // 21
        /// <summary>
        /// the additional function button 12
        /// </summary>
        fn12 = 8388608,    // 23
        /// <summary>
        /// the additional function button 13
        /// </summary>
        fn13 = 33554432, // 25
        /// <summary>
        /// the additional function button 14
        /// </summary>
        fn14 = 134217728, // 27
        /// <summary>
        /// the additional function button 15
        /// </summary>
        fn15 = 536870912, // 29
    }

    #endregion

    #region EvetClasses

    public class BrailleIO_KeyEventArgs : System.EventArgs
    {
        /// <summary>
        /// Combined general button states combined from BrailleIO_DeviceButtonStates
        /// </summary>
        public readonly BrailleIO_DeviceButtonStates keyCode;
        /// <summary>
        /// Combined Braille keyboard button states combined from BrailleIO_BrailleKeyboardButtonStates
        /// </summary>
        public readonly BrailleIO_BrailleKeyboardButtonStates keyboardCode;
        /// <summary>
        /// multiple combined additional button states combined from BrailleIO_AdditionalButtonStates
        /// </summary>
        public readonly BrailleIO_AdditionalButtonStates[] additionalKeyCode;

        /// <summary>
        /// The original event args from the device in raw format without interpretation
        /// </summary>
        public readonly OrderedDictionary raw;
        /// <summary>
        /// Initializes a new instance of the <see cref="BrailleIO_KeyPressed_EventArgs" /> class.
        /// </summary>
        /// <param name="keyCode">The key code.</param>
        /// <param name="raw">The original raw event data from the device.</param>
        /// <param name="keyboardCode">optional combined Braille keyboard button states.</param>
        /// <param name="additionalKeyCode">list of optional combined additional button states.</param>
        public BrailleIO_KeyEventArgs(BrailleIO_DeviceButtonStates keyCode, ref OrderedDictionary raw,
            BrailleIO_BrailleKeyboardButtonStates keyboardCode = BrailleIO_BrailleKeyboardButtonStates.None,
            BrailleIO_AdditionalButtonStates[] additionalKeyCode = null)
        {
            this.keyCode = keyCode;
            this.keyboardCode = keyboardCode;
            this.additionalKeyCode = additionalKeyCode;
            this.raw = raw;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="BrailleIO_KeyPressed_EventArgs" /> class.
        /// </summary>
        /// <param name="keyCode">The key code.</param>
        public BrailleIO_KeyEventArgs(BrailleIO_DeviceButtonStates keyCode)
        {
            this.keyCode = keyCode;
            this.keyboardCode = BrailleIO_BrailleKeyboardButtonStates.None;
            this.additionalKeyCode = null;
            this.raw = null;
        }
    }

    /// <summary>
    /// Event arguments for BrailleIO events for pressed keys
    /// </summary>
    public class BrailleIO_KeyPressed_EventArgs : BrailleIO_KeyEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BrailleIO_KeyPressed_EventArgs"/> class.
        /// </summary>
        /// <param name="keyCode">The key code.</param>
        /// <param name="raw">The original raw event data from the device.</param>
        public BrailleIO_KeyPressed_EventArgs(BrailleIO_DeviceButtonStates keyCode, ref OrderedDictionary raw,
            BrailleIO_BrailleKeyboardButtonStates keyboardCode = BrailleIO_BrailleKeyboardButtonStates.None,
            BrailleIO_AdditionalButtonStates[] additionalKeyCode = null)
            : base(keyCode, ref raw, keyboardCode, additionalKeyCode)
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="BrailleIO_KeyPressed_EventArgs" /> class.
        /// </summary>
        /// <param name="keyCode">The key code.</param>
        public BrailleIO_KeyPressed_EventArgs(BrailleIO_DeviceButtonStates keyCode)
            : base(keyCode)
        { }
    }

    /// <summary>
    /// Event arguments for BrailleIO events when ever keys change there states
    /// </summary>
    public class BrailleIO_KeyStateChanged_EventArgs : BrailleIO_KeyEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BrailleIO_KeyStateChanged_EventArgs"/> class.
        /// </summary>
        /// <param name="keyCode">The key code.</param>
        /// <param name="raw">The original raw event data from the device.</param>
        public BrailleIO_KeyStateChanged_EventArgs(BrailleIO_DeviceButtonStates keyCode, ref OrderedDictionary raw,
            BrailleIO_BrailleKeyboardButtonStates keyboardCode = BrailleIO_BrailleKeyboardButtonStates.None,
            BrailleIO_AdditionalButtonStates[] additionalKeyCode = null)
            : base(keyCode, ref raw, keyboardCode, additionalKeyCode)
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="BrailleIO_KeyStateChanged_EventArgs" /> class.
        /// </summary>
        /// <param name="keyCode">The key code.</param>
        public BrailleIO_KeyStateChanged_EventArgs(BrailleIO_DeviceButtonStates keyCode)
            : base(keyCode)
        { }
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
        public BrailleIO_TouchValuesChanged_EventArgs(double[,] touches, int timestamp, ref OrderedDictionary raw) : this(touches, timestamp, ref raw, null) { }
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