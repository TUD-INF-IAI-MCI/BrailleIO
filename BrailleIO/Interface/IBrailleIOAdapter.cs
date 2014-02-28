using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace BrailleIO.Interface
{
    #region generic button codes
    [Flags]
    public enum BrailleIO_DeviceButtonStates : int
    {
        None = 0,
        Unknown = 1,
        EnterUp = 2,
        EnterDown = 4,
        AbortUp = 8,
        AbortDown = 16,
        GestureUp = 32,
        GestureDown = 64,
        LeftUp = 128,
        LeftDown = 256,
        RightUp = 512,
        RightDown = 1024,
        UpUp = 2048,
        UpDown = 4096,
        DownUp = 8192,
        DownDown = 16384,
        ZoomInUp = 32768,
        ZoomInDown = 65536,
        ZoomOutUp = 131072,
        ZoomOutDown = 262144
    }
    
    [Flags]
    public enum BrailleIO_DeviceButton : int
    {
        None = 0,
        Unknown = 1,
        Enter = 2,
        Abort = 4,
        Gesture = 8,
        Left = 16,
        Right = 32,
        Up = 64,
        Down = 128,
        ZoomIn = 256,
        ZoomOut = 512
    }

    #endregion

    #region EvetClasses
    public class BrailleIO_KeyPressed_EventArgs : System.EventArgs
    {
        public readonly BrailleIO_DeviceButtonStates keyCode;
        public readonly Object raw;
        public BrailleIO_KeyPressed_EventArgs(BrailleIO_DeviceButtonStates keyCode, ref Object raw)
        {
            this.keyCode = keyCode;
        }
    }

    public class BrailleIO_KeyStateChanged_EventArgs : System.EventArgs
    {
        public readonly BrailleIO_DeviceButtonStates keyCode;
        public readonly OrderedDictionary raw;
        public BrailleIO_KeyStateChanged_EventArgs(BrailleIO_DeviceButtonStates keyCode, ref OrderedDictionary raw)
        {
            this.keyCode = keyCode;
            this.raw = raw;
        }
    }

    public class BrailleIO_Initialized_EventArgs : System.EventArgs
    {
        public readonly BrailleIODevice device;
        public BrailleIO_Initialized_EventArgs(BrailleIODevice device) { this.device = device; }
    }

    public class BrailleIO_TouchValuesChanged_EventArgs : System.EventArgs
    {
        public readonly double[,] touches;
        public readonly int timestamp;
        public readonly OrderedDictionary raw;
        public BrailleIO_TouchValuesChanged_EventArgs(double[,] touches, int timestamp, ref OrderedDictionary raw)
        {
            this.touches = touches;
            this.timestamp = timestamp;
            this.raw = raw;
        }
    }

    public class BrailleIO_PinStateChanged_EventArgs : System.EventArgs
    {
        //TODO: find out what
    }

    public class BrailleIO_ErrorOccured_EventArgs : System.EventArgs
    {
        public readonly ErrorCode errorCode;
        public readonly OrderedDictionary raw;
        public BrailleIO_ErrorOccured_EventArgs(ErrorCode errorCode, ref OrderedDictionary raw)
        {
            this.errorCode = errorCode;
            this.raw = raw;
        }
    }

    public enum ErrorCode
    {
        //TODO: implement real ErrorCodes. Which I do not have any clue about. Apparently.
        DeviceDestroyedByGozilla = 1
    }

    public class BrailleIO_InputChanged_EventArgs : System.EventArgs
    {
        public readonly bool[,] touches;
        public readonly int timestamp;
        public readonly OrderedDictionary raw;
        public BrailleIO_InputChanged_EventArgs(bool[,] touches, int timestamp, ref OrderedDictionary raw)
        {
            this.touches = touches;
            this.timestamp = timestamp;
            this.raw = raw;
        }
    }
    #endregion

    public interface IBrailleIOAdapter
    {
        #region Properties

        bool Connected { get; }

        float DpiX { get; }
        float DpiY { get; }

        BrailleIODevice Device { get; }

        #endregion

        bool Connect();
        bool Disconnect();

        void Synchronize(bool[,] matrix);
    }
}
