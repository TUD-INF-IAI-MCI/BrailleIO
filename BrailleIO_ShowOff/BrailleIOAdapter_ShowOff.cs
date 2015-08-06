using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using BrailleIO.Interface;
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
        public MockDriver() { }
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
        IBrailleIOAdapterManager manager;

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

        public override void Synchronize(bool[,] m)
        {
            if(!LockPins)driver.SetMatrix(m);
        }

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
            List<string> pressedKeys,
            List<string> releasedKeys,
            int timeStampTickCount)
        {
            OrderedDictionary raw = new OrderedDictionary();
            raw.Add("pressedKeys", pressedKeys);
            raw.Add("releasedKeys", releasedKeys);
            raw.Add("timeStampTickCount", timeStampTickCount);
            fireKeyStateChanged(states, ref raw);
        }

        internal void firetouchValuesChangedEvent(
            double[,] touches,
            int timeStampTickCount)
        {
            OrderedDictionary raw = new OrderedDictionary();
            fireTouchValuesChanged(touches, timeStampTickCount, ref raw);
        }

        public void startTouch()
        {
        }
    }
}
