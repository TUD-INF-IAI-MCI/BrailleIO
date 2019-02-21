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
		/// <remarks> </remarks>
    [Flags]
    public enum BrailleIO_DeviceButtonStates : int
    {
        /// <summary>
        /// no button is pressed or released
        /// </summary>
		/// <remarks> </remarks>
        None = 0,
        /// <summary>
        /// an unknown button is pressed or released --> check generic keys
        /// </summary>
		/// <remarks> </remarks>
        Unknown = 1,
        /// <summary>
        /// the enter button is released
        /// </summary>
		/// <remarks> </remarks>
        EnterUp = 2,
        /// <summary>
        /// the enter button is pressed
        /// </summary>
		/// <remarks> </remarks>
        EnterDown = 4,
        /// <summary>
        /// the abort button is released
        /// </summary>
		/// <remarks> </remarks>
        AbortUp = 8,
        /// <summary>
        /// the abort button is pressed
        /// </summary>
		/// <remarks> </remarks>
        AbortDown = 16,
        /// <summary>
        /// the gesture button is released
        /// </summary>
		/// <remarks> </remarks>
        GestureUp = 32,
        /// <summary>
        /// the gesture button is pressed
        /// </summary>
		/// <remarks> </remarks>
        GestureDown = 64,
        /// <summary>
        /// the 'left' button is released
        /// </summary>
		/// <remarks> </remarks>
        LeftUp = 128,
        /// <summary>
        /// the 'left' button is pressed
        /// </summary>
		/// <remarks> </remarks>
        LeftDown = 256,
        /// <summary>
        /// the 'right' button is released
        /// </summary>
		/// <remarks> </remarks>
        RightUp = 512,
        /// <summary>
        /// the 'right' button is pressed
        /// </summary>
		/// <remarks> </remarks>
        RightDown = 1024,
        /// <summary>
        /// the 'up' button is released
        /// </summary>
		/// <remarks> </remarks>
        UpUp = 2048,
        /// <summary>
        /// the 'up' button is pressed
        /// </summary>
		/// <remarks> </remarks>
        UpDown = 4096,
        /// <summary>
        /// the 'down' button is released
        /// </summary>
		/// <remarks> </remarks>
        DownUp = 8192,
        /// <summary>
        /// the 'down' button is pressed
        /// </summary>
		/// <remarks> </remarks>
        DownDown = 16384,
        /// <summary>
        /// the button for zooming in is released
        /// </summary>
		/// <remarks> </remarks>
        ZoomInUp = 32768,
        /// <summary>
        /// the button for zooming in is pressed
        /// </summary>
		/// <remarks> </remarks>
        ZoomInDown = 65536,
        /// <summary>
        /// the button for zooming out is released
        /// </summary>
		/// <remarks> </remarks>
        ZoomOutUp = 131072,
        /// <summary>
        /// the button for zooming out is pressed
        /// </summary>
		/// <remarks> </remarks>
        ZoomOutDown = 262144,
    }

    /// <summary>
    /// Generic buttons a touch sensitive, two-dimensional tactile pin device 
    /// should have for a proper interaction
    /// </summary>
		/// <remarks> </remarks>
    [Flags]
    public enum BrailleIO_DeviceButton : int
    {
        /// <summary>
        /// No button
        /// </summary>
		/// <remarks> </remarks>
        None = 0,
        /// <summary>
        /// an unknown button --> check the generic keys
        /// </summary>
		/// <remarks> </remarks>
        Unknown = 1,
        /// <summary>
        /// the 'Ok' button
        /// </summary>
		/// <remarks> </remarks>
        Enter = 2,
        /// <summary>
        /// the 'cancel' button
        /// </summary>
		/// <remarks> </remarks>
        Abort = 8,
        /// <summary>
        /// a gesture button to indicate a gesture input to avoid midas-touch-effects
        /// </summary>
		/// <remarks> </remarks>
        Gesture = 32,
        /// <summary>
        /// a button for 'left' direction indication
        /// </summary>
		/// <remarks> </remarks>
        Left = 128,
        /// <summary>
        /// a button for 'right' direction indication
        /// </summary>
		/// <remarks> </remarks>
        Right = 512,
        /// <summary>
        /// a button for 'up' direction indication
        /// </summary>
		/// <remarks> </remarks>
        Up = 2048,
        /// <summary>
        /// a button for 'down' direction indication
        /// </summary>
		/// <remarks> </remarks>
        Down = 8192,
        /// <summary>
        /// a button to indicate the intense to magnify the content
        /// </summary>
		/// <remarks> </remarks>
        ZoomIn = 32768,
        /// <summary>
        /// a button to indicate the intense to minify the content for a better overview
        /// </summary>
		/// <remarks> </remarks>
        ZoomOut = 131072
    }

    #endregion

    #region Braille keyboard key codes

    /// <summary>
    /// Button states for buttons of a Braille keyboard
    /// </summary>
		/// <remarks> </remarks>
    [Flags]
    public enum BrailleIO_BrailleKeyboardButtonStates : int
    {
        /// <summary>
        /// no button is pressed or released
        /// </summary>
		/// <remarks> </remarks>
        None = 0,
        /// <summary>
        /// an unknown keyboard button is pressed or released --> check generic keys
        /// </summary>
		/// <remarks> </remarks>
        Unknown = 1,        // 0
        /// <summary>
        /// the point 1 button is released
        /// </summary>
		/// <remarks> </remarks>
        k1Up = 2,           // 1
        /// <summary>
        /// the point 1 button is pressed
        /// </summary>
		/// <remarks> </remarks>
        k1Down = 4,         // 2
        /// <summary>
        /// the point 2 button is released
        /// </summary>
		/// <remarks> </remarks>
        k2Up = 8,           // 3
        /// <summary>
        /// the point 2 button is pressed
        /// </summary>
		/// <remarks> </remarks>
        k2Down = 16,        // 4
        /// <summary>
        /// the point 3 button is released
        /// </summary>
		/// <remarks> </remarks>
        k3Up = 32,          // 5
        /// <summary>
        /// the point 3 button is pressed
        /// </summary>
		/// <remarks> </remarks>
        k3Down = 64,        // 6
        /// <summary>
        /// the point 4 button is released
        /// </summary>
		/// <remarks> </remarks>
        k4Up = 128,         // 7
        /// <summary>
        /// the point 4 button is pressed
        /// </summary>
		/// <remarks> </remarks>
        k4Down = 256,       // 8
        /// <summary>
        /// the point 5 button is released
        /// </summary>
		/// <remarks> </remarks>
        k5Up = 512,         // 9
        /// <summary>
        /// the point 5 button is pressed
        /// </summary>
		/// <remarks> </remarks>
        k5Down = 1024,      // 10
        /// <summary>
        /// the point 6 button is released
        /// </summary>
		/// <remarks> </remarks>
        k6Up = 2048,        // 11
        /// <summary>
        /// the point 6 button is pressed
        /// </summary>
		/// <remarks> </remarks>
        k6Down = 4096,      // 12
        /// <summary>
        /// the point 7 button is released
        /// </summary>
		/// <remarks> </remarks>
        k7Up = 8192,        // 13
        /// <summary>
        /// the point 7 button is pressed
        /// </summary>
		/// <remarks> </remarks>
        k7Down = 16384,     // 14
        /// <summary>
        /// the point 8 button is released
        /// </summary>
		/// <remarks> </remarks>
        k8Up = 32768,       // 15
        /// <summary>
        /// the point 8 button is pressed
        /// </summary>
		/// <remarks> </remarks>
        k8Down = 65536,     // 16
        /// <summary>
        /// the button for the first function key is released. Commonly used for the left thumb.
        /// </summary>
		/// <remarks> </remarks>
        F1Up = 131072,      // 17
        /// <summary>
        /// the button for the first function key out is pressed. Commonly used for the left thumb.
        /// </summary>
		/// <remarks> </remarks>
        F1Down = 262144,    // 18
        /// <summary>
        /// additional/optional button to the first function key is released. Commonly used for the left thumb.
        /// </summary>
		/// <remarks> </remarks>
        F11Up = 524288,     // 19
        /// <summary>
        /// additional/optional button to the first function key is pressed. Commonly used for the left thumb.
        /// </summary>
		/// <remarks> </remarks>
        F11Down = 1048576,  // 20
        /// <summary>
        /// the button for the second function key is released. Commonly used for the right thumb.
        /// </summary>
		/// <remarks> </remarks>
        F2Up = 2097152,     // 21
        /// <summary>
        /// the button for the second function key out is pressed. Commonly used for the right thumb.
        /// </summary>
		/// <remarks> </remarks>
        F2Down = 4194304,   // 22
        /// <summary>
        /// additional/optional button to the second function key is released. Commonly used for the right thumb.
        /// </summary>
		/// <remarks> </remarks>
        F22Up = 8388608,    // 23
        /// <summary>
        /// additional/optional button to the second function key is pressed. Commonly used for the right thumb.
        /// </summary>
		/// <remarks> </remarks>
        F22Down = 16777216, // 24
    }

    /// <summary>
    /// Buttons of a Braille keyboard
    /// </summary>
		/// <remarks> </remarks>
    [Flags]
    public enum BrailleIO_BrailleKeyboardButton : int
    {
        /// <summary>
        /// no button is pressed or released
        /// </summary>
		/// <remarks> </remarks>
        None = 0,
        /// <summary>
        /// an unknown keyboard button is pressed or released --> check generic keys
        /// </summary>
		/// <remarks> </remarks>
        Unknown = 1,      // 0
        /// <summary>
        /// the point 1 button
        /// </summary>
		/// <remarks> </remarks>
        k1 = 2,           // 1
        /// <summary>
        /// the point 2 button
        /// </summary>
		/// <remarks> </remarks>
        k2 = 8,           // 3
        /// <summary>
        /// the point 3
        /// </summary>
		/// <remarks> </remarks>
        k3 = 32,          // 5
        /// <summary>
        /// the point 4 button
        /// </summary>
		/// <remarks> </remarks>
        k4 = 128,         // 7
        /// <summary>
        /// the point 5 button
        /// </summary>
		/// <remarks> </remarks>
        k5 = 512,         // 9
        /// <summary>
        /// the point 6 button
        /// </summary>
		/// <remarks> </remarks>
        k6 = 2048,        // 11
        /// <summary>
        /// the point 7 button
        /// </summary>
		/// <remarks> </remarks>
        k7 = 8192,        // 13
        /// <summary>
        /// the point 8 button
        /// </summary>
		/// <remarks> </remarks>
        k8 = 32768,       // 15
        /// <summary>
        /// the button for the first function key. Commonly used for the left thumb.
        /// </summary>
		/// <remarks> </remarks>
        F1 = 131072,      // 17
        /// <summary>
        /// additional/optional button to the first function key. Commonly used for the left thumb.
        /// </summary>
		/// <remarks> </remarks>
        F11 = 524288,     // 19
        /// <summary>
        /// the button for the second function key. Commonly used for the right thumb.
        /// </summary>
		/// <remarks> </remarks>
        F2 = 2097152,     // 21
        /// <summary>
        /// additional/optional button to the second function key. Commonly used for the right thumb.
        /// </summary>
		/// <remarks> </remarks>
        F22 = 8388608,    // 23
    }

    #endregion

    #region additional function key codes

    /// <summary>
    /// Button states for 15 additional function buttons
    /// </summary>
		/// <remarks> </remarks>
    [Flags]
    public enum BrailleIO_AdditionalButtonStates : int
    {
        /// <summary>
        /// no button is pressed or released
        /// </summary>
		/// <remarks> </remarks>
        None = 0,
        /// <summary>
        /// an unknown additional button is pressed or released --> check generic keys
        /// </summary>
		/// <remarks> </remarks>
        Unknown = 1,        // 0
        /// <summary>
        /// the additional function button 1 is released
        /// </summary>
		/// <remarks> </remarks>
        fn1Up = 2,           // 1
        /// <summary>
        /// the additional function button 1 is pressed
        /// </summary>
		/// <remarks> </remarks>
        fn1Down = 4,         // 2
        /// <summary>
        /// the additional function button 2 is released
        /// </summary>
		/// <remarks> </remarks>
        fn2Up = 8,           // 3
        /// <summary>
        /// the additional function button 2 is pressed
        /// </summary>
		/// <remarks> </remarks>
        fn2Down = 16,        // 4
        /// <summary>
        /// the additional function button 3 is released
        /// </summary>
		/// <remarks> </remarks>
        fn3Up = 32,          // 5
        /// <summary>
        /// the additional function button 3 is pressed
        /// </summary>
		/// <remarks> </remarks>
        fn3Down = 64,        // 6
        /// <summary>
        /// the additional function button 4 is released
        /// </summary>
		/// <remarks> </remarks>
        fn4Up = 128,         // 7
        /// <summary>
        /// the additional function button 4 is pressed
        /// </summary>
		/// <remarks> </remarks>
        fn4Down = 256,       // 8
        /// <summary>
        /// the additional function button 5 is released
        /// </summary>
		/// <remarks> </remarks>
        fn5Up = 512,         // 9
        /// <summary>
        /// the additional function button 5 is pressed
        /// </summary>
		/// <remarks> </remarks>
        fn5Down = 1024,      // 10
        /// <summary>
        /// the additional function button 6 is released
        /// </summary>
		/// <remarks> </remarks>
        fn6Up = 2048,        // 11
        /// <summary>
        /// the additional function button 6 is pressed
        /// </summary>
		/// <remarks> </remarks>
        fn6Down = 4096,      // 12
        /// <summary>
        /// the additional function button 7 is released
        /// </summary>
		/// <remarks> </remarks>
        fn7Up = 8192,        // 13
        /// <summary>
        /// the additional function button 7 is pressed
        /// </summary>
		/// <remarks> </remarks>
        fn7Down = 16384,     // 14
        /// <summary>
        /// the additional function button 8 is released
        /// </summary>
		/// <remarks> </remarks>
        fn8Up = 32768,       // 15
        /// <summary>
        /// the additional function button 8 is pressed
        /// </summary>
		/// <remarks> </remarks>
        fn8Down = 65536,     // 16
        /// <summary>
        /// the additional function button 9 is released
        /// </summary>
		/// <remarks> </remarks>
        fn9Up = 131072,      // 17
        /// <summary>
        /// the additional function button 9 is pressed
        /// </summary>
		/// <remarks> </remarks>
        fn9Down = 262144,    // 18
        /// <summary>
        /// the additional function button 10 is released
        /// </summary>
		/// <remarks> </remarks>
        fn10Up = 524288,     // 19
        /// <summary>
        /// the additional function button 10 is pressed
        /// </summary>
		/// <remarks> </remarks>
        fn10Down = 1048576,  // 20
        /// <summary>
        /// the additional function button 11 is released
        /// </summary>
		/// <remarks> </remarks>
        fn11Up = 2097152,     // 21
        /// <summary>
        /// the additional function button 11 is pressed
        /// </summary>
		/// <remarks> </remarks>
        fn11Down = 4194304,   // 22
        /// <summary>
        /// the additional function button 12 is released
        /// </summary>
		/// <remarks> </remarks>
        fn12Up = 8388608,    // 23
        /// <summary>
        /// the additional function button 12 is pressed
        /// </summary>
		/// <remarks> </remarks>
        fn12Down = 16777216, // 24
        /// <summary>
        /// the additional function button 13 is released
        /// </summary>
		/// <remarks> </remarks>
        fn13Up = 33554432, // 25
        /// <summary>
        /// the additional function button 13 is pressed
        /// </summary>
		/// <remarks> </remarks>
        fn13Down = 67108864, // 26
        /// <summary>
        /// the additional function button 14 is released
        /// </summary>
		/// <remarks> </remarks>
        fn14Up = 134217728, // 27
        /// <summary>
        /// the additional function button 14 is pressed
        /// </summary>
		/// <remarks> </remarks>
        fn14Down = 268435456, // 28
        /// <summary>
        /// the additional function button 15 is released
        /// </summary>
		/// <remarks> </remarks>
        fn15Up = 536870912, // 29
        /// <summary>
        /// the additional function button 15 is pressed
        /// </summary>
		/// <remarks> </remarks>
        fn15Down = 1073741824, // 30
    }

    /// <summary>
    /// 15 Buttons for additional functions
    /// </summary>
		/// <remarks> </remarks>
    [Flags]
    public enum BrailleIO_AdditionalButton : int
    {
        /// <summary>
        /// no button is pressed or released
        /// </summary>
		/// <remarks> </remarks>
        None = 0,
        /// <summary>
        /// an unknown additional button
        /// </summary>
		/// <remarks> </remarks>
        Unknown = 1,        // 0
        /// <summary>
        /// the additional function button 1
        /// </summary>
		/// <remarks> </remarks>
        fn1 = 2,           // 1
        /// <summary>
        /// the additional function button 2
        /// </summary>
		/// <remarks> </remarks>
        fn2 = 8,           // 3
        /// <summary>
        /// the additional function button 3
        /// </summary>
		/// <remarks> </remarks>
        fn3 = 32,          // 5
        /// <summary>
        /// the additional function button 4
        /// </summary>
		/// <remarks> </remarks>
        fn4 = 128,         // 7
        /// <summary>
        /// the additional function button 5
        /// </summary>
		/// <remarks> </remarks>
        fn5 = 512,         // 9
        /// <summary>
        /// the additional function button 6
        /// </summary>
		/// <remarks> </remarks>
        fn6 = 2048,        // 11
        /// <summary>
        /// the additional function button 7
        /// </summary>
		/// <remarks> </remarks>
        fn7 = 8192,        // 13
        /// <summary>
        /// the additional function button 8
        /// </summary>
		/// <remarks> </remarks>
        fn8 = 32768,       // 15
        /// <summary>
        /// the additional function button 9
        /// </summary>
		/// <remarks> </remarks>
        fn9 = 131072,      // 17
        /// <summary>
        /// the additional function button 10
        /// </summary>
		/// <remarks> </remarks>
        fn10 = 524288,     // 19
        /// <summary>
        /// the additional function button 11
        /// </summary>
		/// <remarks> </remarks>
        fn11 = 2097152,     // 21
        /// <summary>
        /// the additional function button 12
        /// </summary>
		/// <remarks> </remarks>
        fn12 = 8388608,    // 23
        /// <summary>
        /// the additional function button 13
        /// </summary>
		/// <remarks> </remarks>
        fn13 = 33554432, // 25
        /// <summary>
        /// the additional function button 14
        /// </summary>
		/// <remarks> </remarks>
        fn14 = 134217728, // 27
        /// <summary>
        /// the additional function button 15
        /// </summary>
		/// <remarks> </remarks>
        fn15 = 536870912, // 29
    }

    #endregion

    #region Event Args

    /// <summary>Event Arguments for Events related to hardware button events</summary>
    public class BrailleIO_KeyEventArgs : System.EventArgs
    {
        /// <summary>
        /// Combined general button states combined from BrailleIO_DeviceButtonStates
        /// </summary>
		/// <remarks> </remarks>
        public readonly BrailleIO_DeviceButtonStates keyCode;
        /// <summary>
        /// Combined Braille keyboard button states combined from BrailleIO_BrailleKeyboardButtonStates
        /// </summary>
		/// <remarks> </remarks>
        public readonly BrailleIO_BrailleKeyboardButtonStates keyboardCode;
        /// <summary>
        /// multiple combined additional button states combined from BrailleIO_AdditionalButtonStates
        /// </summary>
		/// <remarks> </remarks>
        public readonly BrailleIO_AdditionalButtonStates[] additionalKeyCode;

        /// <summary>
        /// The original event args from the device in raw format without interpretation
        /// </summary>
		/// <remarks> </remarks>
        public readonly OrderedDictionary raw;
        /// <summary>
        /// Initializes a new instance of the <see cref="BrailleIO_KeyPressed_EventArgs" /> class.
        /// </summary>
		/// <remarks> </remarks>
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
		/// <remarks> </remarks>
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
		/// <remarks> </remarks>
    public class BrailleIO_KeyPressed_EventArgs : BrailleIO_KeyEventArgs
    {
        /// <summary>Initializes a new instance of the <see cref="BrailleIO_KeyPressed_EventArgs"/> class.</summary>
        /// <param name="keyCode">The key code.</param>
        /// <param name="raw">The original raw event data from the device.</param>
        /// <param name="keyboardCode">the Braille keyboard button states</param>
        /// <param name="additionalKeyCode">the list of additional button states</param>
        public BrailleIO_KeyPressed_EventArgs(BrailleIO_DeviceButtonStates keyCode, ref OrderedDictionary raw,
            BrailleIO_BrailleKeyboardButtonStates keyboardCode = BrailleIO_BrailleKeyboardButtonStates.None,
            BrailleIO_AdditionalButtonStates[] additionalKeyCode = null)
            : base(keyCode, ref raw, keyboardCode, additionalKeyCode)
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="BrailleIO_KeyPressed_EventArgs" /> class.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="keyCode">The key code.</param>
        public BrailleIO_KeyPressed_EventArgs(BrailleIO_DeviceButtonStates keyCode)
            : base(keyCode)
        { }
    }

    /// <summary>
    /// Event arguments for BrailleIO events when ever keys change there states
    /// </summary>
		/// <remarks> </remarks>
    public class BrailleIO_KeyStateChanged_EventArgs : BrailleIO_KeyEventArgs
    {
        /// <summary>Initializes a new instance of the <see cref="BrailleIO_KeyStateChanged_EventArgs"/> class.</summary>
        /// <param name="keyCode">The key code.</param>
        /// <param name="raw">The original raw event data from the device.</param>
        /// <param name="keyboardCode">the Braille keyboard button states</param>
        /// <param name="additionalKeyCode">the list of additional button states</param>
        public BrailleIO_KeyStateChanged_EventArgs(BrailleIO_DeviceButtonStates keyCode, ref OrderedDictionary raw,
            BrailleIO_BrailleKeyboardButtonStates keyboardCode = BrailleIO_BrailleKeyboardButtonStates.None,
            BrailleIO_AdditionalButtonStates[] additionalKeyCode = null)
            : base(keyCode, ref raw, keyboardCode, additionalKeyCode)
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="BrailleIO_KeyStateChanged_EventArgs" /> class.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="keyCode">The key code.</param>
        public BrailleIO_KeyStateChanged_EventArgs(BrailleIO_DeviceButtonStates keyCode)
            : base(keyCode)
        { }
    }

    /// <summary>
    /// Event arguments for BrailleIO events if an adapter was initializes successfully 
    /// </summary>
		/// <remarks> </remarks>
    public class BrailleIO_Initialized_EventArgs : System.EventArgs
    {
        /// <summary>
        /// the device indicator for the initialized adapter
        /// </summary>
		/// <remarks> </remarks>
        public readonly BrailleIODevice device;
        /// <summary>
        /// Initializes a new instance of the <see cref="BrailleIO_Initialized_EventArgs"/> class.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="device">The device.</param>
        public BrailleIO_Initialized_EventArgs(BrailleIODevice device) { this.device = device; }
    }

    /// <summary>
    /// Event arguments for BrailleIO events that indicates a change in the touch detection of an adapter
    /// </summary>
		/// <remarks> </remarks>
    public class BrailleIO_TouchValuesChanged_EventArgs : System.EventArgs
    {
        /// <summary>OPTIONAL list of more detailed touch information.</summary>
        /// <value>The detailed touches.</value>
        public List<Touch> DetailedTouches { get; private set; }
        /// <summary>
        /// the normalized matrix (from 0.0 to 1.0) of detected touch values per pin  
        /// </summary>
		/// <remarks> </remarks>
        public readonly double[,] touches;
        /// <summary>
        /// time stamp of the occurred event for temporal order
        /// </summary>
		/// <remarks> </remarks>
        public readonly int timestamp;
        /// <summary>
        /// The original event args from the device in raw format without interpretation
        /// </summary>
		/// <remarks> </remarks>
        public readonly OrderedDictionary raw;
        /// <summary>
        /// Initializes a new instance of the <see cref="BrailleIO_TouchValuesChanged_EventArgs"/> class.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="touches">the normalized matrix (from 0.0 to 1.0) of detected touch values per pin.</param>
        /// <param name="timestamp">The original event args from the device in raw format without interpretation.
        /// But could also be an own timestamp such as <c>(int)DateTime.UtcNow.Ticks</c>.
        /// </param>
        /// <param name="raw">The original event args from the device in raw format without interpretation.</param>
        public BrailleIO_TouchValuesChanged_EventArgs(double[,] touches, int timestamp, ref OrderedDictionary raw) : this(touches, timestamp, ref raw, null) { }
        /// <summary>Initializes a new instance of the <see cref="BrailleIO_TouchValuesChanged_EventArgs"/> class.</summary>
        /// <param name="touches">the normalized matrix (from 0.0 to 1.0) of detected touch values per pin.</param>
        /// <param name="timestamp">The original event args from the device in raw format without interpretation.
        /// But could also be an own timestamp such as <c>(int)DateTime.UtcNow.Ticks</c>.</param>
        /// <param name="raw">The original event args from the device in raw format without interpretation.</param>
        /// <param name="detailedTouches">list of identified detailed finger touches</param>
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
		/// <remarks> </remarks>
        /// <param name="touches">the normalized matrix (from 0.0 to 1.0) of detected touch values per pin.</param>
        /// <param name="timestamp">The original event args from the device in raw format without interpretation.
        /// But could also be an own timestamp such as <c>(int)DateTime.UtcNow.Ticks</c>.</param>
        public BrailleIO_TouchValuesChanged_EventArgs(double[,] touches, int timestamp) : this(touches, timestamp, null) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="BrailleIO_TouchValuesChanged_EventArgs" /> class.
        /// </summary>
		/// <remarks> </remarks>
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
    /// Event arguments for BrailleIO events for pressed keys
    /// </summary>
		/// <remarks> </remarks>
    public class BrailleIO_KeyCombinationReleased_EventArgs : EventArgs
    {
        /// <summary>
        /// Collector for complex key combinations.
        /// ATTENTION: proprietary key-codes are NOT stored or handled.
        /// </summary>
		/// <remarks> </remarks>
        public KeyCombinationItem KeyCombination;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrailleIO_KeyPressed_EventArgs"/> class.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="kc">The kc.</param>
        public BrailleIO_KeyCombinationReleased_EventArgs(KeyCombinationItem kc)
        {
            KeyCombination = kc;
        }
    }

    /// <summary>
    /// Event for BrailleIO Adapter to indicate that the sate of there presented pins has changed
    /// NOT USED YET
    /// </summary>
		/// <remarks> </remarks>
    public class BrailleIO_PinStateChanged_EventArgs : System.EventArgs
    {
        //TODO: find out what
    }

    /// <summary>
    /// BrailleIO Event for error in the Adapter
    /// </summary>
		/// <remarks> </remarks>
    public class BrailleIO_ErrorOccured_EventArgs : System.EventArgs
    {
        /// <summary>
        /// Code for indicating the class of error that is occurred
        /// </summary>
		/// <remarks> </remarks>
        public readonly ErrorCode errorCode;
        /// <summary>
        /// The original event args from the device in raw format without interpretation
        /// </summary>
		/// <remarks> </remarks>
        public readonly OrderedDictionary raw;
        /// <summary>
        /// Initializes a new instance of the <see cref="BrailleIO_ErrorOccured_EventArgs"/> class.
        /// </summary>
		/// <remarks> </remarks>
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
		/// <remarks> </remarks>
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
		/// <remarks> </remarks>
    public enum ErrorCode
    {
        /// <summary>
        /// everything is fine
        /// </summary>
		/// <remarks> </remarks>
        NONE,
        /// <summary>
        /// sending matrix to device returned FALSE
        /// </summary>
		/// <remarks> </remarks>
        CANT_SENT_MATRIX,
        /// <summary>
        /// device is not available
        /// </summary>
		/// <remarks> </remarks>
        DEVICE_UNAVAILABLE,
        /// <summary>
        /// unknown error occurred
        /// </summary>
		/// <remarks> </remarks>
        UNKNOWN,
        /// <summary>
        /// warning happens
        /// </summary>
		/// <remarks> </remarks>
        WARNING,
    }

    /// <summary>
    /// <seealso cref="BrailleIO_TouchValuesChanged_EventArgs"/> Indicates that some input changes happens
    /// </summary>
		/// <remarks> </remarks>
    public class BrailleIO_InputChanged_EventArgs : System.EventArgs
    {
        /// <summary>
        /// the normalized matrix (from 0.0 to 1.0) of detected touch values per pin  
        /// </summary>
		/// <remarks> </remarks>
        public readonly bool[,] touches;
        /// <summary>
        /// time stamp of the occurred event for temporal order
        /// </summary>
		/// <remarks> </remarks>
        public readonly int timestamp;
        /// <summary>
        /// The original event args from the device in raw format without interpretation
        /// </summary>
		/// <remarks> </remarks>
        public readonly OrderedDictionary raw;
        /// <summary>
        /// Initializes a new instance of the <see cref="BrailleIO_InputChanged_EventArgs"/> class.
        /// </summary>
		/// <remarks> </remarks>
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
		/// <remarks> </remarks>
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
		/// <remarks> </remarks>
    public interface IBrailleIOAdapter
    {
        #region Properties

        /// <summary>
        /// Gets a value indicating whether this <see cref="IBrailleIOAdapter"/> is connected.
        /// </summary>
		/// <remarks> </remarks>
        /// <value>
        ///   <c>true</c> if connected; otherwise, <c>false</c>.
        /// </value>
        bool Connected { get; }

        /// <summary>
        /// Gets the horizontal resolution of this device in Dots Per Inch.
        /// </summary>
		/// <remarks> </remarks>
        /// <value>
        /// The horizontal DPI.
        /// </value>
        float DpiX { get; }
        /// <summary>
        /// Gets the vertical resolution of this device in Dots Per Inch.
        /// </summary>
		/// <remarks> </remarks>
        /// <value>
        /// The vertical DPI.
        /// </value>
        float DpiY { get; }

        /// <summary>
        /// Gets the corresponding hardware device to this wrapper.
        /// </summary>
		/// <remarks> </remarks>
        /// <value>
        /// The device.
        /// </value>
        BrailleIODevice Device { get; }

        #endregion

        /// <summary>
        /// Connects this instance.
        /// </summary>
		/// <remarks> </remarks>
        /// <returns>if the device could been connected or not</returns>
        bool Connect();
        /// <summary>
        /// Disconnects this instance.
        /// </summary>
		/// <remarks> </remarks>
        /// <returns>if the device could been disconnected or not</returns>
        bool Disconnect();

        /// <summary>
        /// Synchronizes the specified matrix. That means the matrix will be send to the hardware device as output
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="matrix">The matrix to show.</param>
        void Synchronize(bool[,] matrix);
        /// <summary>
        /// Recalibrate the touch abilities of the device with  the specified threshold.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="threshold">The threshold.</param>
        /// <returns>if the device could been calibrated or not</returns>
        bool Recalibrate(double threshold);

        #region event handlers
        /// <summary>
        /// Occurs when a key was pressed.
        /// </summary>
		/// <remarks> </remarks>
        event EventHandler<BrailleIO_KeyPressed_EventArgs> keyPressed;
        /// <summary>
        /// Occurs when the state of a key has changed. This can be a pressed or a released
        /// </summary>
		/// <remarks> </remarks>
        event EventHandler<BrailleIO_KeyStateChanged_EventArgs> keyStateChanged;
        /// <summary>
        /// Occurs when the device was successfully initialized.
        /// </summary>
		/// <remarks> </remarks>
        event EventHandler<BrailleIO_Initialized_EventArgs> initialized;
        /// <summary>
        /// Occurs when some properties of the input changes.
        /// </summary>
		/// <remarks> </remarks>
        event EventHandler<BrailleIO_InputChanged_EventArgs> inputChanged;
        /// <summary>
        /// Occurs when some touch values had changed.
        /// </summary>
		/// <remarks> </remarks>
        event EventHandler<BrailleIO_TouchValuesChanged_EventArgs> touchValuesChanged;
        /// <summary>
        /// Occurs when  some pin states had changed.
        /// </summary>
		/// <remarks> </remarks>
        event EventHandler<BrailleIO_PinStateChanged_EventArgs> pinStateChanged;
        /// <summary>
        /// Occurs when an error has occurred.
        /// </summary>
		/// <remarks> </remarks>
        event EventHandler<BrailleIO_ErrorOccured_EventArgs> errorOccurred;
        #endregion
    }

    /// <summary>
    /// Interface an adapter (means a real hardware abstracting implementation for an input or output device) has to implement.
    /// In addition, the current pressed button states are available through this interface.
    /// </summary>
		/// <remarks> </remarks>
    public interface IBrailleIOAdapter2 : IBrailleIOAdapter
    {
        #region ButtonStates

        /// <summary>
        /// Gets all currently pressed device buttons.
        /// </summary>
		/// <remarks> </remarks>
        /// <value>
        /// The currently pressed device buttons.
        /// </value>
        BrailleIO_DeviceButton PressedDeviceButtons { get; }

        /// <summary>
        /// Gets all currently pressed braille keyboard buttons.
        /// </summary>
		/// <remarks> </remarks>
        /// <value>
        /// The currently pressed braille keyboard buttons.
        /// </value>
        BrailleIO_BrailleKeyboardButton PressedBrailleKeyboardButtons { get; }

        /// <summary>
        /// Gets all currently pressed additional buttons.
        /// </summary>
		/// <remarks> </remarks>
        /// <value>
        /// The currently pressed additional buttons.
        /// </value>
        BrailleIO_AdditionalButton[] PressedAdditionalButtons { get; }

        /// <summary>
        /// Occurs when a key combination was released.
        /// ATTENTION: proprietary raw key-codes are NOT stored or handled.
        /// </summary>
		/// <remarks> </remarks>
        event EventHandler<BrailleIO_KeyCombinationReleased_EventArgs> keyCombinationReleased;
        #endregion
    }
}