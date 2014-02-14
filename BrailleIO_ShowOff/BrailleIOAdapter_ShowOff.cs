using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using BrailleIO.Interface;
namespace BrailleIO
{
    class MockDriver
    {
        ShowOff form = ShowOff.ActiveForm as ShowOff;
        public MockDriver()
        {
        }

        public void setMatrix(bool[,] m)
        {
           if(form != null && m != null) 
               form.paint(m);
        }
    }

    public class BrailleIOAdapter_ShowOff : AbstractBrailleIOAdapterBase
    {
        MockDriver driver = new MockDriver();
        AbstractBrailleIOAdapterManagerBase manager;
        public BrailleIOAdapter_ShowOff(ref AbstractBrailleIOAdapterManagerBase manager)
            : base(ref manager)
        {
            this.RefreshRate = 30;
            this.DeviceSizeX = 120;
            this.DeviceSizeY = 60;
            this.manager = manager;
            Connect();
        }

        public override void Synchronize(bool[,] m)
        {
            driver.setMatrix(m);
        }

        public override bool Connect()
        {
            if (base.Connect())
            {
                //driver.setMatrix(new bool[,] { { true, false, true, false, true, false, true, false, true, false, true, true }, { true, false, true, false, true, false, true, false, true, false, true, true } });
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

        protected void sendAttached(BrailleIODevice device)
        {
            //TODO : wrap information
            //fireInitialized(new BrailleIO_Initialized_EventArgs(device));
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

        //private void driver_touchValuesChangedEvent(
        //    HyperBraille.HBBrailleDis.BrailleDisModuleState[] changedModules,
        //    HyperBraille.HBBrailleDis.BrailleDisModuleState[] activeModules,
        //    int timeStampTickCount)
        //{

        //}

        public void startTouch()
        {

        }
    }
}
