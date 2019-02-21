using BrailleIO.Interface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Input;

namespace BrailleIO
{
    /// <summary>
    /// A software adapter simulating a Metec BarilleDis 7200.
    /// </summary>
		/// <remarks> </remarks>
    /// <seealso cref="System.Windows.Forms.Form" />
    /// <seealso cref="BrailleIO.IBrailleIOShowOffMonitor" />
    public partial class ShowOff : Form, IBrailleIOShowOffMonitor
    {
        #region buttons

        private void button_KEY_RIGHT_ROCKER_SWITCH_UP_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown,
                BrailleIO_BrailleKeyboardButtonStates.None,
                new BrailleIO_AdditionalButtonStates[1] { BrailleIO_AdditionalButtonStates.fn2Down },
                new List<string>() { "rsru" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown,
                BrailleIO_BrailleKeyboardButtonStates.None,
                new BrailleIO_AdditionalButtonStates[1] { BrailleIO_AdditionalButtonStates.fn2Up },
                null, new List<string>() { "rsru" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_RIGHT_ROCKER_SWITCH_DOWN_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown,
                BrailleIO_BrailleKeyboardButtonStates.None,
                new BrailleIO_AdditionalButtonStates[1] { BrailleIO_AdditionalButtonStates.fn3Down },
                new List<string>() { "rsrd" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown,
                BrailleIO_BrailleKeyboardButtonStates.None,
                new BrailleIO_AdditionalButtonStates[1] { BrailleIO_AdditionalButtonStates.fn3Up },
                null, new List<string>() { "rsrd" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_LEFT_CURSORS_UP_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.None,
                BrailleIO_BrailleKeyboardButtonStates.None, new BrailleIO_AdditionalButtonStates[1] { BrailleIO_AdditionalButtonStates.fn4Down },
                new List<string>() { "clu" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.None,
                BrailleIO_BrailleKeyboardButtonStates.None, new BrailleIO_AdditionalButtonStates[1] { BrailleIO_AdditionalButtonStates.fn4Up },
                null, new List<string>() { "clu" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_LEFT_CURSORS_LEFT_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.None,
                BrailleIO_BrailleKeyboardButtonStates.None, new BrailleIO_AdditionalButtonStates[1] { BrailleIO_AdditionalButtonStates.fn6Down },
                new List<string>() { "cll" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.None,
                BrailleIO_BrailleKeyboardButtonStates.None, new BrailleIO_AdditionalButtonStates[1] { BrailleIO_AdditionalButtonStates.fn6Up },
                null, new List<string>() { "cll" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_LEFT_CURSORS_RIGHT_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.None,
                BrailleIO_BrailleKeyboardButtonStates.None, new BrailleIO_AdditionalButtonStates[1] { BrailleIO_AdditionalButtonStates.fn7Down },
                new List<string>() { "clr" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.None,
                BrailleIO_BrailleKeyboardButtonStates.None, new BrailleIO_AdditionalButtonStates[1] { BrailleIO_AdditionalButtonStates.fn7Up },
                null, new List<string>() { "clr" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_LEFT_CURSORS_DOWN_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown,
                BrailleIO_BrailleKeyboardButtonStates.None, new BrailleIO_AdditionalButtonStates[1] { BrailleIO_AdditionalButtonStates.fn5Down },
                new List<string>() { "cld" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown,
                BrailleIO_BrailleKeyboardButtonStates.None, new BrailleIO_AdditionalButtonStates[1] { BrailleIO_AdditionalButtonStates.fn5Up },
                null, new List<string>() { "cld" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_RIGHT_CURSORS_UP_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown,
                BrailleIO_BrailleKeyboardButtonStates.None,
                new BrailleIO_AdditionalButtonStates[1] { BrailleIO_AdditionalButtonStates.fn12Down },
                new List<string>() { "cru" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown,
                BrailleIO_BrailleKeyboardButtonStates.None,
                new BrailleIO_AdditionalButtonStates[1] { BrailleIO_AdditionalButtonStates.fn12Up },
                null, new List<string>() { "cru" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_RIGHT_CURSORS_RIGHT_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown,
                BrailleIO_BrailleKeyboardButtonStates.None,
                new BrailleIO_AdditionalButtonStates[1] { BrailleIO_AdditionalButtonStates.fn15Down },
                new List<string>() { "crr" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown,
                BrailleIO_BrailleKeyboardButtonStates.None,
                new BrailleIO_AdditionalButtonStates[1] { BrailleIO_AdditionalButtonStates.fn15Up },
                null, new List<string>() { "crr" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_RIGHT_CURSORS_DOWN_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown,
                BrailleIO_BrailleKeyboardButtonStates.None,
                new BrailleIO_AdditionalButtonStates[1] { BrailleIO_AdditionalButtonStates.fn13Down },
                new List<string>() { "crd" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown,
                BrailleIO_BrailleKeyboardButtonStates.None,
                new BrailleIO_AdditionalButtonStates[1] { BrailleIO_AdditionalButtonStates.fn13Up },
                null, new List<string>() { "crd" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_RIGHT_CURSORS_LEFT_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown,
                BrailleIO_BrailleKeyboardButtonStates.None,
                new BrailleIO_AdditionalButtonStates[1] { BrailleIO_AdditionalButtonStates.fn14Down },
                new List<string>() { "crl" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown,
                BrailleIO_BrailleKeyboardButtonStates.None,
                new BrailleIO_AdditionalButtonStates[1] { BrailleIO_AdditionalButtonStates.fn14Up },
                 null, new List<string>() { "crl" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_LEFT_ROCKER_SWITCH_UP_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.ZoomInDown,
                BrailleIO_BrailleKeyboardButtonStates.None, null, new List<string>() { "rslu" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.ZoomInUp,
                BrailleIO_BrailleKeyboardButtonStates.None, null, null, new List<string>() { "rslu" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_LEFT_ROCKER_SWITCH_DOWN_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.ZoomOutDown,
                BrailleIO_BrailleKeyboardButtonStates.None, null, new List<string>() { "rsld" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.ZoomOutUp,
                BrailleIO_BrailleKeyboardButtonStates.None, null, null, new List<string>() { "rsld" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_LEFT_CURSORS_CENTER_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.None,
                BrailleIO_BrailleKeyboardButtonStates.None, new BrailleIO_AdditionalButtonStates[1] { BrailleIO_AdditionalButtonStates.fn1Down },
                new List<string>() { "clc" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.None,
                BrailleIO_BrailleKeyboardButtonStates.None, new BrailleIO_AdditionalButtonStates[1] { BrailleIO_AdditionalButtonStates.fn1Up },
                null, new List<string>() { "clc" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_HYPERBRAILLE_KEY_RIGHT_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.AbortDown,
                BrailleIO_BrailleKeyboardButtonStates.None, null, new List<string>() { "hbr" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.AbortUp,
                BrailleIO_BrailleKeyboardButtonStates.None, null, null, new List<string>() { "hbr" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_DOT7_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown,
                BrailleIO_BrailleKeyboardButtonStates.k7Down, null,
                new List<string>() { "k7" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown,
                BrailleIO_BrailleKeyboardButtonStates.k7Up, null,
                 null, new List<string>() { "k7" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_DOT3_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown,
                BrailleIO_BrailleKeyboardButtonStates.k3Down, null,
                new List<string>() { "k3" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown,
                BrailleIO_BrailleKeyboardButtonStates.k3Up, null,
                null, new List<string>() { "k3" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_DOT2_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown,
                BrailleIO_BrailleKeyboardButtonStates.k2Down, null,
                new List<string>() { "k2" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown,
                BrailleIO_BrailleKeyboardButtonStates.k2Up, null,
                null, new List<string>() { "k2" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_DOT1_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown,
                BrailleIO_BrailleKeyboardButtonStates.k1Down, null,
                new List<string>() { "k1" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown,
                BrailleIO_BrailleKeyboardButtonStates.k1Up, null,
                null, new List<string>() { "k1" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_THUMB_LEFT_HAND_LEFT_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown,
                BrailleIO_BrailleKeyboardButtonStates.F1Down, null,
                new List<string>() { "l" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown,
                BrailleIO_BrailleKeyboardButtonStates.F1Up, null,
                null, new List<string>() { "l" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_THUMB_LEFT_HAND_RIGHT_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown,
                BrailleIO_BrailleKeyboardButtonStates.F11Down, null,
                new List<string>() { "lr" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown,
                BrailleIO_BrailleKeyboardButtonStates.F11Up, null,
                null, new List<string>() { "lr" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_THUMB_RIGHT_HAND_LEFT_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown,
                BrailleIO_BrailleKeyboardButtonStates.F22Down, null,
                new List<string>() { "rl" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown,
                BrailleIO_BrailleKeyboardButtonStates.F22Up, null,
                null, new List<string>() { "rl" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_THUMB_RIGHT_HAND_RIGHT_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown,
                BrailleIO_BrailleKeyboardButtonStates.F2Down, null,
                new List<string>() { "r" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown,
                BrailleIO_BrailleKeyboardButtonStates.F2Up, null,
                null, new List<string>() { "r" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_DOT4_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown,
                BrailleIO_BrailleKeyboardButtonStates.k4Down, null,
                new List<string>() { "k4" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown,
                BrailleIO_BrailleKeyboardButtonStates.k4Up, null,
                 null, new List<string>() { "k4" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_DOT5_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown,
                BrailleIO_BrailleKeyboardButtonStates.k5Down, null,
                new List<string>() { "k5" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown,
                BrailleIO_BrailleKeyboardButtonStates.k5Up, null,
                null, new List<string>() { "k5" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_DOT6_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown,
                BrailleIO_BrailleKeyboardButtonStates.k6Down, null,
                new List<string>() { "k6" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown,
                BrailleIO_BrailleKeyboardButtonStates.k6Up, null,
                null, new List<string>() { "k6" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_DOT8_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown,
                BrailleIO_BrailleKeyboardButtonStates.k8Down, null,
                new List<string>() { "k8" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown,
                BrailleIO_BrailleKeyboardButtonStates.k8Up, null,
                null, new List<string>() { "k8" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_RIGHT_CURSORS_CENTER_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.EnterDown,
                BrailleIO_BrailleKeyboardButtonStates.None, null,
                new List<string>() { "crc" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.EnterUp,
                BrailleIO_BrailleKeyboardButtonStates.None, null,
                null, new List<string>() { "crc" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_NAV_LEFT_2_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown,
                BrailleIO_BrailleKeyboardButtonStates.None,
                new BrailleIO_AdditionalButtonStates[1] { BrailleIO_AdditionalButtonStates.fn10Down },
                new List<string>() { "nsll" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown,
                BrailleIO_BrailleKeyboardButtonStates.None,
                new BrailleIO_AdditionalButtonStates[1] { BrailleIO_AdditionalButtonStates.fn10Up },
                null, new List<string>() { "nsll" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_NAV_LEFT_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.LeftDown,
                BrailleIO_BrailleKeyboardButtonStates.None, null,
                new List<string>() { "nsl" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.LeftUp,
                BrailleIO_BrailleKeyboardButtonStates.None, null,
                null, new List<string>() { "nsl" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_NAV_RIGHT_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.RightDown,
                BrailleIO_BrailleKeyboardButtonStates.None, null,
                new List<string>() { "nsr" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.RightUp,
                BrailleIO_BrailleKeyboardButtonStates.None, null,
                null, new List<string>() { "nsr" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_NAV_RIGHT_2_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown,
                BrailleIO_BrailleKeyboardButtonStates.None,
                new BrailleIO_AdditionalButtonStates[1] { BrailleIO_AdditionalButtonStates.fn11Down },
                new List<string>() { "nsrr" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown,
                BrailleIO_BrailleKeyboardButtonStates.None,
                new BrailleIO_AdditionalButtonStates[1] { BrailleIO_AdditionalButtonStates.fn11Up },
                null, new List<string>() { "nsrr" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_NAV_UP_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.UpDown,
                BrailleIO_BrailleKeyboardButtonStates.None, null,
                new List<string>() { "nsu" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.UpUp,
                BrailleIO_BrailleKeyboardButtonStates.None, null,
                null, new List<string>() { "nsu" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_NAV_UP_2_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown,
                BrailleIO_BrailleKeyboardButtonStates.None,
                new BrailleIO_AdditionalButtonStates[1] { BrailleIO_AdditionalButtonStates.fn8Down },
                new List<string>() { "nsuu" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown,
                BrailleIO_BrailleKeyboardButtonStates.None,
                new BrailleIO_AdditionalButtonStates[1] { BrailleIO_AdditionalButtonStates.fn8Up },
                null, new List<string>() { "nsuu" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_NAV_DOWN_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.DownDown,
                BrailleIO_BrailleKeyboardButtonStates.None, null,
                new List<string>() { "nsd" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.DownUp,
                BrailleIO_BrailleKeyboardButtonStates.None, null,
                null, new List<string>() { "nsd" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_NAV_DOWN_2_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown,
                BrailleIO_BrailleKeyboardButtonStates.None,
                new BrailleIO_AdditionalButtonStates[1] { BrailleIO_AdditionalButtonStates.fn9Down },
                new List<string>() { "nsdd" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown,
                BrailleIO_BrailleKeyboardButtonStates.None,
                new BrailleIO_AdditionalButtonStates[1] { BrailleIO_AdditionalButtonStates.fn9Up },
                null, new List<string>() { "nsdd" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_HYPERBRAILLE_KEY_LEFT_Click_1(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.GestureDown,
                BrailleIO_BrailleKeyboardButtonStates.None, null,
                new List<string>() { "hbl" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.GestureUp,
                BrailleIO_BrailleKeyboardButtonStates.None, null, null, new List<string>() { "hbl" }, (int)DateTime.UtcNow.Ticks);
        }

        #endregion

        #region key combinations

        #region Application keys
        private volatile bool _ctr = false;
        /// <summary>
        /// determines if the CTR-key is pressed or not
        /// </summary>
		/// <remarks> </remarks>
        /// <value>
        ///   <c>true</c> if CTR-Key is pressed; otherwise, <c>false</c>.
        /// </value>
        protected bool Ctr
        {
            get
            {
                return _ctr || Keyboard.IsKeyDown(System.Windows.Input.Key.LeftCtrl) || Keyboard.IsKeyDown(System.Windows.Input.Key.RightCtrl);
            }
            set { _ctr = value; }
        }

        void showOff_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e != null)
            {
                if (!e.Control)
                {
                    Ctr = false;
                    // FIXME: make this working for key combinations
                    fireKeyStateChangeEvent(BrailleIO_DeviceButtonStates.None,
                        BrailleIO_BrailleKeyboardButtonStates.None, null,
                        null, null, 0);
                }
            }
        }

        void showOff_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e != null)
            {
                if (e.Control)
                {
                    Ctr = true;
                }
            }
        }

        void hk_Pressed(object sender, System.ComponentModel.HandledEventArgs e)
        {

        }

        #endregion

        volatile List<BrailleIO_DeviceButtonStates> _pressedStates = new List<BrailleIO_DeviceButtonStates>();
        volatile List<BrailleIO_DeviceButtonStates> _releasedStates = new List<BrailleIO_DeviceButtonStates>();

        volatile List<BrailleIO_BrailleKeyboardButtonStates> _pressedKeyboradStates = new List<BrailleIO_BrailleKeyboardButtonStates>();
        volatile List<BrailleIO_BrailleKeyboardButtonStates> _releasedKeybordStates = new List<BrailleIO_BrailleKeyboardButtonStates>();

        volatile List<BrailleIO_AdditionalButtonStates> _pressedFncStates = new List<BrailleIO_AdditionalButtonStates>();
        volatile List<BrailleIO_AdditionalButtonStates> _releasedFncStates = new List<BrailleIO_AdditionalButtonStates>();

        volatile List<String> _pressedButtons = new List<String>();
        volatile List<String> _releasedButtons = new List<String>();

        /// <summary>Fires the key state change event.</summary>
        /// <param name="states">The states.</param>
        /// <param name="keyboardCode">The keyboard code.</param>
        /// <param name="additionalKeyCode">The additional key code.</param>
        /// <param name="pressedKeys">The pressed keys.</param>
        /// <param name="releasedKeys">The released keys.</param>
        /// <param name="timeStampTickCount">The time stamp tick count.</param>
        internal void fireKeyStateChangeEvent(BrailleIO_DeviceButtonStates states,
            BrailleIO_BrailleKeyboardButtonStates keyboardCode,
            BrailleIO_AdditionalButtonStates[] additionalKeyCode,
            List<string> pressedKeys,
            List<string> releasedKeys,
            int timeStampTickCount)
        {
            if (Ctr)
            {
                //check if pressed or released
                if (releasedKeys == null || releasedKeys.Count < 1) //pressed keys
                {
                    if (pressedKeys != null && pressedKeys.Count > 0)
                    {
                        foreach (string pk in pressedKeys)
                        {
                            if (_pressedButtons.Contains(pk))
                            {
                                UnmarkButtons(new List<String> { pk });
                                _pressedButtons.Remove(pk);
                                _pressedStates.Remove(states);
                                _pressedKeyboradStates.Remove(keyboardCode);
                                if (additionalKeyCode != null && additionalKeyCode.Length > 0) _pressedFncStates.Remove(additionalKeyCode[0]);
                            }
                            else
                            {
                                _pressedButtons.Add(pk);
                                _pressedStates.Add(states);
                                _pressedKeyboradStates.Add(keyboardCode);
                                if (additionalKeyCode != null && additionalKeyCode.Length > 0) _pressedFncStates.Add(additionalKeyCode[0]);
                            }
                        }
                    }
                }
                else // released keys
                {
                    if (releasedKeys != null && releasedKeys.Count > 0)
                    {
                        foreach (string rk in releasedKeys)
                        {
                            if (_releasedButtons.Contains(rk))
                            {
                                UnmarkButtons(new List<String> { rk });
                                _releasedButtons.Remove(rk);
                                _releasedStates.Remove(states);
                                _releasedKeybordStates.Remove(keyboardCode);
                                if (additionalKeyCode != null && additionalKeyCode.Length > 0) _releasedFncStates.Remove(additionalKeyCode[0]);
                            }
                            else
                            {
                                _releasedButtons.Add(rk);
                                _releasedStates.Add(states);
                                _releasedKeybordStates.Add(keyboardCode);
                                if (additionalKeyCode != null && additionalKeyCode.Length > 0) _releasedFncStates.Add(additionalKeyCode[0]);
                            }
                        }
                    }
                }

                MarkButtonAsPressed(_pressedButtons);
            }

            BrailleIO_DeviceButtonStates ps = BrailleIO_DeviceButtonStates.None;
            foreach (BrailleIO_DeviceButtonStates s in _pressedStates) { ps = ps | s; }
            BrailleIO_BrailleKeyboardButtonStates ks = BrailleIO_BrailleKeyboardButtonStates.None;
            foreach (BrailleIO_BrailleKeyboardButtonStates item in _pressedKeyboradStates) { ks |= item; }
            BrailleIO_AdditionalButtonStates ads = BrailleIO_AdditionalButtonStates.None;
            foreach (BrailleIO_AdditionalButtonStates item in _pressedFncStates) { ads |= item; }

            // fire always button pressed but wait for release
            if (ShowOffAdapter != null && pressedKeys != null && pressedKeys.Count > 0)
                ShowOffAdapter.firekeyStateChangedEvent(ps, ks, new BrailleIO_AdditionalButtonStates[1] { ads }, _pressedButtons, new List<String>(), timeStampTickCount);
            //              ShowOffAdapter.firekeyStateChangedEvent(ps, _pressedButtons, new List<String>(), timeStampTickCount);


            if (Ctr) return; // break the release or reset functions 


            if (states == BrailleIO_DeviceButtonStates.None && pressedKeys == null && releasedKeys == null && timeStampTickCount == 0)
            {
                //check if this is because the ctr is released
                if (ShowOffAdapter != null)
                {
                    BrailleIO_DeviceButtonStates rs = BrailleIO_DeviceButtonStates.None;
                    foreach (BrailleIO_DeviceButtonStates s in _releasedStates) { rs = rs | s; }
                    BrailleIO_BrailleKeyboardButtonStates rks = BrailleIO_BrailleKeyboardButtonStates.None;
                    foreach (BrailleIO_BrailleKeyboardButtonStates item in _releasedKeybordStates) { rks |= item; }
                    // TODO: do for additional buttons
                    BrailleIO_AdditionalButtonStates rads = BrailleIO_AdditionalButtonStates.None;
                    foreach (BrailleIO_AdditionalButtonStates item in _releasedFncStates) { rads |= item; }

                    ShowOffAdapter.firekeyStateChangedEvent(rs, rks, new BrailleIO_AdditionalButtonStates[1] { rads }, new List<String>(), _releasedButtons, timeStampTickCount);
                    //ShowOffAdapter.firekeyStateChangedEvent(rs, new List<String>(), _releasedButtons, timeStampTickCount);


                    UnmarkButtons(_releasedButtons);

                    _pressedButtons.Clear();
                    _releasedButtons.Clear();
                    _pressedStates.Clear();
                    _pressedKeyboradStates.Clear();
                    _pressedFncStates.Clear();
                    _releasedStates.Clear();
                    _releasedKeybordStates.Clear();
                    _releasedFncStates.Clear();
                }
            }
            else
            {
                if (ShowOffAdapter != null)
                {
                    ShowOffAdapter.firekeyStateChangedEvent(states, keyboardCode, additionalKeyCode, pressedKeys, releasedKeys, timeStampTickCount);
                    //ShowOffAdapter.firekeyStateChangedEvent(states, pressedKeys, releasedKeys, timeStampTickCount);
                }
            }
        }

        #endregion

        #region Button marking

        private readonly Color normalButtonBgColor = Color.FromArgb(0, Color.Gray);
        private readonly Color markedButtonBgColor = Color.FromArgb(100, Color.DarkOrange);
        private readonly Color releasingButtonBgColor = Color.FromArgb(100, Color.ForestGreen);

        /// <summary>
        /// Marks the button as pressed.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="pressedButtons">The pressed buttons.</param>
        public void MarkButtonAsPressed(List<String> pressedButtons)
        {
            if (pressedButtons != null && pressedButtons.Count > 0)
                foreach (var buttonName in pressedButtons)
                {
                    Control button = getButtonFromGenericName(buttonName.ToString().Trim());
                    if (button != null)
                    {
                        button.BackColor = markedButtonBgColor;
                    }
                }
        }

        /// <summary>Marks the button as pressed.</summary>
        /// <param name="pressedGeneralKeys">newly pressed general keys</param>
        /// <param name="pressedBrailleKeyboardKeys">newly pressed Braille-keyboard keys</param>
        /// <param name="pressedAdditionalKeys">the newly pressed additional keys</param>
        public void MarkButtonAsPressed(BrailleIO_DeviceButton pressedGeneralKeys,
            BrailleIO_BrailleKeyboardButton pressedBrailleKeyboardKeys = BrailleIO_BrailleKeyboardButton.None,
            BrailleIO_AdditionalButton[] pressedAdditionalKeys = null)
        {
            List<Control> ctr = new List<Control>();
            if(pressedGeneralKeys != BrailleIO_DeviceButton.None || pressedGeneralKeys != BrailleIO_DeviceButton.Unknown)
            {
                ctr.AddRange(getButtonFromGenerral(pressedGeneralKeys));
            }
            if (pressedBrailleKeyboardKeys != BrailleIO_BrailleKeyboardButton.None || pressedBrailleKeyboardKeys != BrailleIO_BrailleKeyboardButton.Unknown)
            {
                ctr.AddRange(getButtonFromBrailleKeyboard(pressedBrailleKeyboardKeys));
            }
            if(pressedAdditionalKeys != null && pressedAdditionalKeys.Length > 0 && pressedAdditionalKeys[0] != BrailleIO_AdditionalButton.None)
            {
                ctr.AddRange(getButtonFromAdditional0(pressedAdditionalKeys[0]));
            }

            if(ctr.Count > 0)
            {
                foreach (var button in ctr)
                {
                    if (button != null)
                    {
                        button.BackColor = markedButtonBgColor;
                    }
                }
            }
        }



        /// <summary>
        /// Reset the buttons to normal mode.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="releasedButtons">The released buttons.</param>
        public void UnmarkButtons(List<String> releasedButtons)
        {
            if (releasedButtons != null && releasedButtons.Count > 0)
                try
                {
                    foreach (var buttonName in releasedButtons)
                    {
                        Control button = getButtonFromGenericName(buttonName.ToString().Trim());
                        if (button != null)
                        {
                            button.BackColor = releasingButtonBgColor;
                            System.Threading.Thread.Sleep(100);
                            button.BackColor = normalButtonBgColor;
                        }
                    }
                }
                catch { }
        }


        /// <summary>Unmarks the buttons.</summary>
        /// <param name="releasedGeneralKeys">The released general keys.</param>
        /// <param name="releasedBrailleKeyboardKeys">The released braille keyboard keys.</param>
        /// <param name="releasedAdditionalKeys">The released additional keys.</param>
        public void UnmarkButtons(BrailleIO_DeviceButton releasedGeneralKeys,
            BrailleIO_BrailleKeyboardButton releasedBrailleKeyboardKeys = BrailleIO_BrailleKeyboardButton.None,
            BrailleIO_AdditionalButton[] releasedAdditionalKeys = null)
        {
            List<Control> ctr = new List<Control>();
            if (releasedGeneralKeys != BrailleIO_DeviceButton.None || releasedGeneralKeys != BrailleIO_DeviceButton.Unknown)
            {
                ctr.AddRange(getButtonFromGenerral(releasedGeneralKeys));
            }
            if (releasedBrailleKeyboardKeys != BrailleIO_BrailleKeyboardButton.None || releasedBrailleKeyboardKeys != BrailleIO_BrailleKeyboardButton.Unknown)
            {
                ctr.AddRange(getButtonFromBrailleKeyboard(releasedBrailleKeyboardKeys));
            }
            if (releasedAdditionalKeys != null && releasedAdditionalKeys.Length > 0 && releasedAdditionalKeys[0] != BrailleIO_AdditionalButton.None)
            {
                ctr.AddRange(getButtonFromAdditional0(releasedAdditionalKeys[0]));
            }

            if (ctr.Count > 0)
            {
                foreach (var button in ctr)
                {
                    if (button != null)
                    {
                        button.BackColor = releasingButtonBgColor;
                        System.Threading.Thread.Sleep(100);
                        button.BackColor = normalButtonBgColor;
                    }
                }
            }
        }




        private Control getButtonFromGenericName(String name)
        {
            switch (name)
            {
                case "hbl":
                    return this.button_KEY_HYPERBRAILLE_KEY_LEFT;
                case "rslu":
                    return this.button_KEY_LEFT_ROCKER_SWITCH_UP;
                case "rsld":
                    return this.button_KEY_LEFT_ROCKER_SWITCH_DOWN;
                case "clu":
                    return this.button_KEY_LEFT_CURSORS_UP;
                case "cll":
                    return this.button_KEY_LEFT_CURSORS_LEFT;
                case "clc":
                    return this.button_KEY_LEFT_CURSORS_CENTER;
                case "cld":
                    return this.button_KEY_LEFT_CURSORS_DOWN;
                case "clr":
                    return this.button_KEY_LEFT_CURSORS_RIGHT;
                case "nsll":
                    return this.button_KEY_NAV_LEFT_2;
                case "nsl":
                    return this.button_KEY_NAV_LEFT;
                case "nsuu":
                    return this.button_KEY_NAV_UP_2;
                case "nsu":
                    return this.button_KEY_NAV_UP;
                case "nsdd":
                    return this.button_KEY_NAV_DOWN_2;
                case "nsd":
                    return this.button_KEY_NAV_DOWN;
                case "nsr":
                    return this.button_KEY_NAV_RIGHT;
                case "nsrr":
                    return this.button_KEY_NAV_RIGHT_2;
                case "crl":
                    return this.button_KEY_RIGHT_CURSORS_LEFT;
                case "crd":
                    return this.button_KEY_RIGHT_CURSORS_DOWN;
                case "crc":
                    return this.button_KEY_RIGHT_CURSORS_CENTER;
                case "crr":
                    return this.button_KEY_RIGHT_CURSORS_RIGHT;
                case "cru":
                    return this.button_KEY_RIGHT_CURSORS_UP;
                case "rsrd":
                    return this.button_KEY_RIGHT_ROCKER_SWITCH_DOWN;
                case "rsru":
                    return this.button_KEY_RIGHT_ROCKER_SWITCH_UP;
                case "hbr":
                    return this.button_KEY_HYPERBRAILLE_KEY_RIGHT;
                case "k8":
                    return this.button_KEY_DOT8;
                case "k6":
                    return this.button_KEY_DOT6;
                case "k5":
                    return this.button_KEY_DOT5;
                case "k4":
                    return this.button_KEY_DOT4;
                case "r":
                    return this.button_KEY_THUMB_RIGHT_HAND_RIGHT;
                case "rl":
                    return this.button_KEY_THUMB_RIGHT_HAND_LEFT;
                case "lr":
                    return this.button_KEY_THUMB_LEFT_HAND_RIGHT;
                case "l":
                    return this.button_KEY_THUMB_LEFT_HAND_LEFT;
                case "k1":
                    return this.button_KEY_DOT1;
                case "k2":
                    return this.button_KEY_DOT2;
                case "k3":
                    return this.button_KEY_DOT3;
                case "k7":
                    return this.button_KEY_DOT7;
                default:
                    break;
            }

            return null;

        }


        private List<Control> getButtonFromGenerral(BrailleIO_DeviceButton btn)
        {
            List<Control> ctr = new List<Control>();
            if (btn.HasFlag(BrailleIO_DeviceButton.Enter))
                ctr.Add(this.button_KEY_RIGHT_CURSORS_CENTER);
            if (btn.HasFlag(BrailleIO_DeviceButton.Abort))
                ctr.Add(this.button_KEY_HYPERBRAILLE_KEY_RIGHT);
            if (btn.HasFlag(BrailleIO_DeviceButton.Gesture))
                ctr.Add(this.button_KEY_HYPERBRAILLE_KEY_LEFT);
            if (btn.HasFlag(BrailleIO_DeviceButton.Left))
                ctr.Add(this.button_KEY_NAV_LEFT);
            if (btn.HasFlag(BrailleIO_DeviceButton.Right))
                ctr.Add(this.button_KEY_NAV_RIGHT);
            if (btn.HasFlag(BrailleIO_DeviceButton.Up))
                ctr.Add(this.button_KEY_NAV_UP);
            if (btn.HasFlag(BrailleIO_DeviceButton.Down))
                ctr.Add(this.button_KEY_NAV_DOWN);
            if (btn.HasFlag(BrailleIO_DeviceButton.ZoomIn))
                ctr.Add(this.button_KEY_LEFT_ROCKER_SWITCH_UP);
            if (btn.HasFlag(BrailleIO_DeviceButton.ZoomOut))
                ctr.Add(this.button_KEY_LEFT_ROCKER_SWITCH_DOWN);



            return ctr;


        }

        private List<Control> getButtonFromBrailleKeyboard(BrailleIO_BrailleKeyboardButton btn)
        {
            List<Control> ctr = new List<Control>();
            if (btn.HasFlag(BrailleIO_BrailleKeyboardButton.k1))
                ctr.Add(this.button_KEY_DOT1);
            if (btn.HasFlag(BrailleIO_BrailleKeyboardButton.k2))
                ctr.Add(this.button_KEY_DOT2);
            if (btn.HasFlag(BrailleIO_BrailleKeyboardButton.k3))
                ctr.Add(this.button_KEY_DOT3);
            if (btn.HasFlag(BrailleIO_BrailleKeyboardButton.k4))
                ctr.Add(this.button_KEY_DOT4);
            if (btn.HasFlag(BrailleIO_BrailleKeyboardButton.k5))
                ctr.Add(this.button_KEY_DOT5);
            if (btn.HasFlag(BrailleIO_BrailleKeyboardButton.k6))
                ctr.Add(this.button_KEY_DOT6);
            if (btn.HasFlag(BrailleIO_BrailleKeyboardButton.k7))
                ctr.Add(this.button_KEY_DOT7);
            if (btn.HasFlag(BrailleIO_BrailleKeyboardButton.k8))
                ctr.Add(this.button_KEY_DOT8);
            if (btn.HasFlag(BrailleIO_BrailleKeyboardButton.F1))
                ctr.Add(this.button_KEY_THUMB_LEFT_HAND_LEFT);
            if (btn.HasFlag(BrailleIO_BrailleKeyboardButton.F11))
                ctr.Add(this.button_KEY_THUMB_LEFT_HAND_RIGHT);
            if (btn.HasFlag(BrailleIO_BrailleKeyboardButton.F2))
                ctr.Add(this.button_KEY_THUMB_RIGHT_HAND_RIGHT);
            if (btn.HasFlag(BrailleIO_BrailleKeyboardButton.F22))
                ctr.Add(this.button_KEY_THUMB_RIGHT_HAND_LEFT);


            return ctr;
        }

        private List<Control> getButtonFromAdditional0(BrailleIO_AdditionalButton btn)
        {
            List<Control> ctr = new List<Control>();
            if (btn.HasFlag(BrailleIO_AdditionalButton.fn1))
                ctr.Add(this.button_KEY_LEFT_CURSORS_CENTER);
            if (btn.HasFlag(BrailleIO_AdditionalButton.fn2))
                ctr.Add(this.button_KEY_RIGHT_ROCKER_SWITCH_UP);
            if (btn.HasFlag(BrailleIO_AdditionalButton.fn3))
                ctr.Add(this.button_KEY_RIGHT_ROCKER_SWITCH_DOWN);
            if (btn.HasFlag(BrailleIO_AdditionalButton.fn4))
                ctr.Add(this.button_KEY_LEFT_CURSORS_UP);
            if (btn.HasFlag(BrailleIO_AdditionalButton.fn5))
                ctr.Add(this.button_KEY_LEFT_CURSORS_DOWN);
            if (btn.HasFlag(BrailleIO_AdditionalButton.fn6))
                ctr.Add(this.button_KEY_LEFT_CURSORS_LEFT);
            if (btn.HasFlag(BrailleIO_AdditionalButton.fn7))
                ctr.Add(this.button_KEY_LEFT_CURSORS_RIGHT);
            if (btn.HasFlag(BrailleIO_AdditionalButton.fn8))
                ctr.Add(this.button_KEY_NAV_UP_2);
            if (btn.HasFlag(BrailleIO_AdditionalButton.fn9))
                ctr.Add(this.button_KEY_NAV_DOWN_2);
            if (btn.HasFlag(BrailleIO_AdditionalButton.fn10))
                ctr.Add(this.button_KEY_NAV_LEFT_2);
            if (btn.HasFlag(BrailleIO_AdditionalButton.fn11))
                ctr.Add(this.button_KEY_NAV_RIGHT_2);
            if (btn.HasFlag(BrailleIO_AdditionalButton.fn12))
                ctr.Add(this.button_KEY_RIGHT_CURSORS_UP);
            if (btn.HasFlag(BrailleIO_AdditionalButton.fn13))
                ctr.Add(this.button_KEY_RIGHT_CURSORS_DOWN);
            if (btn.HasFlag(BrailleIO_AdditionalButton.fn14))
                ctr.Add(this.button_KEY_RIGHT_CURSORS_LEFT);
            if (btn.HasFlag(BrailleIO_AdditionalButton.fn15))
                ctr.Add(this.button_KEY_RIGHT_CURSORS_RIGHT);

            return ctr;
        }

        #endregion
    }
}