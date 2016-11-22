using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using BrailleIO.Interface;
using BrailleIO.Structs;
namespace BrailleIO
{
    /// <summary>
    /// Driver to emulate an real hardware driver that is responsible for sending the bool matrix to the hardware device
    /// </summary>
    public class MockDriver
    {
        /// <summary>
        /// The Windows Forms Application that should display the matrix. It is the Emulation of the presenting hardware.
        /// </summary>
        public IBrailleIOShowOffMonitor Monitor = ShowOff.ActiveForm as IBrailleIOShowOffMonitor;
        /// <summary>
        /// Initializes a new instance of the <see cref="MockDriver"/> class. This is a simple wrapper for the software adapter itself connected to this GUI.
        /// </summary>
        public MockDriver() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="MockDriver"/> class. This is a simple wrapper for the software adapter itself connected to this GUI.
        /// </summary>
        /// <param name="gui">The GUI.</param>
        public MockDriver(ShowOff gui) { Monitor = gui; }
        /// <summary>
        /// Sends the Matrix to the windows forms application to display
        /// </summary>
        /// <param name="m">The m.</param>
        public void SetMatrix(bool[,] m)
        {
            if (Monitor != null && m != null)
                Monitor.Paint(m);
        }
    }

    /// <summary>
    /// Software emulation of a Metec BrailleDis 7200 display. 
    /// It enables Developers to emulate a real pin matrix device or can be used 
    /// as debug monitor for displaying inputs on a real connected BrailleDis device.
    /// </summary>
    public class BrailleIOAdapter_ShowOff : AbstractBrailleIOAdapterBase
    {
        /// <summary>
        /// The driver emulator that send the matrix to the windows forms application
        /// </summary>
        public MockDriver driver = new MockDriver();

        /// <summary>
        /// Initializes a new instance of the <see cref="BrailleIOAdapter_ShowOff"/> class.
        /// </summary>
        /// <param name="manager">The IBrailleIOAdapterManager the device hase to be registerd to.</param>
        /// <param name="gui">The ShowOff windows forms application that is used as displaying or user interaction GUI.</param>
        public BrailleIOAdapter_ShowOff(IBrailleIOAdapterManager manager, ShowOff gui)
            : this(manager)
        {
            driver.Monitor = gui;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BrailleIOAdapter_ShowOff"/> class.
        /// </summary>
        /// <param name="manager">The IBrailleIOAdapterManager the device hase to be registerd to.</param>
        public BrailleIOAdapter_ShowOff(IBrailleIOAdapterManager manager)
            : base(manager)
        {
            this.manager = manager;
            this.Device = new BrailleIODevice(120, 60, "ShowOFF_" + this.GetHashCode(), true, false, 30, this);
            if (manager != null)
            {
                manager.AddAdapter(this);
            }
            Connect();
        }

        /// <summary>
        /// Synchronizes the specified matrix. 
        /// That means the Adapter try to sent the given Matrix to the real hardware 
        /// device as an output.
        /// </summary>
        /// <param name="m">The matrix.</param>
        public override void Synchronize(bool[,] m)
        {
            if(!LockPins)driver.SetMatrix(m);
        }

        /// <summary>
        /// Connects this instance.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if connected successfully; otherwise, <c>false</c>.
        /// </returns>
        public override bool Connect()
        {
            if (base.Connect())
            {
                this.Connected = true;
                fireInitialized(new BrailleIO_Initialized_EventArgs(Device));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if disconnected successfully; otherwise, <c>false</c>.
        /// </returns>
        public override bool Disconnect()
        {
            if (base.Disconnect())
            {
                return true;
            }
            return false;
        }

        internal void sendAttached(BrailleIODevice device)
        {
            fireInitialized(new BrailleIO_Initialized_EventArgs(device));
        }

        internal void firekeyStateChangedEvent(BrailleIO_DeviceButtonStates states,
            BrailleIO_BrailleKeyboardButtonStates keyboardCode,
            BrailleIO_AdditionalButtonStates[] additionalKeyCode,
            List<string> pressedKeys,
            List<string> releasedKeys,
            int timeStampTickCount)
        {
            OrderedDictionary raw = new OrderedDictionary();
            raw.Add("pressedKeys", pressedKeys);
            raw.Add("releasedKeys", releasedKeys);
            raw.Add("timeStampTickCount", timeStampTickCount);
            fireKeyStateChanged(states, ref raw, keyboardCode, additionalKeyCode);
            //fireKeyStateChanged(states, ref raw);
        }

        internal void firetouchValuesChangedEvent(
            double[,] touches,
            int timeStampTickCount, List<Touch> detailedTouches)
        {
            OrderedDictionary raw = new OrderedDictionary();
            fireTouchValuesChanged(touches, timeStampTickCount, ref raw, detailedTouches);
        }

        ///// <summary>
        ///// Starts the touch evaluation.
        ///// </summary>
        //public void StartTouch()
        //{
        //}
    }
}
