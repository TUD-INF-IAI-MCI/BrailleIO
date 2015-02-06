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
        public ShowOff form = ShowOff.ActiveForm as ShowOff;
        public MockDriver() { }
        public MockDriver(ShowOff gui) { form = gui; }
        /// <summary>
        /// Sends the Matrix to the windows forms application to display
        /// </summary>
        /// <param name="m">The m.</param>
        public void SetMatrix(bool[,] m)
        {
            if (form != null && m != null)
                form.paint(m);
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
            driver.form = gui;
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
            Connect();
        }

        public override void Synchronize(bool[,] m)
        {
            driver.SetMatrix(m);
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

        //private void driver_inputChangedEvent(
        //    bool touchInputAvailable,
        //    int[,] valueMatrix,
        //    HyperBraille.HBBrailleDis.BrailleDisKeyboard keyboardState,
        //    int timeStampTickCount)
        //{
        //    OrderedDictionary raw = new OrderedDictionary();
        //    raw.Add("touchInputAvailable", touchInputAvailable);
        //    raw.Add("valueMatrix", valueMatrix);
        //    raw.Add("keyBoardState", keyboardState);
        //    raw.Add("timeStampTickCount", timeStampTickCount);
        //    bool[,] touches = new bool[valueMatrix.GetLength(0), valueMatrix.GetLength(1)];
        //    for (int i = 0; i < valueMatrix.GetLength(0); i++)
        //        for (int j = 0; j < valueMatrix.GetLength(1); j++)
        //            touches[i, j] = ((double)Math.Round((double)valueMatrix[i, j]) != 0) ? true : false;
        //    fireInputChanged(touches, timeStampTickCount, ref raw);
        //}

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

        //private void driver_pinStateChangedEvent(HyperBraille.HBBrailleDis.BrailleDisPinState[] changedPins)
        //{
        //    OrderedDictionary raw = new OrderedDictionary();
        //    raw.Add("changedPins", changedPins);

        //    foreach (HyperBraille.HBBrailleDis.BrailleDisPinState p in changedPins)
        //    {

        //    }
        //}

        internal void firetouchValuesChangedEvent(
            double[,] touches,
            int timeStampTickCount)
        {
            OrderedDictionary raw = new OrderedDictionary();
            fireTouchValuesChanged(touches, timeStampTickCount, ref raw);
        }

        public void startTouch()
        {
            //TODO: do touches
        }
    }
}
