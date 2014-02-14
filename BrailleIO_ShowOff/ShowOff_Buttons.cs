using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BrailleIO
{
    public partial class ShowOff : Form
    {
        #region buttons

        private void button_KEY_RIGHT_ROCKER_SWITCH_UP_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "rsru" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_RIGHT_ROCKER_SWITCH_DOWN_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "rsrd" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_LEFT_CURSORS_UP_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.UpDown, new List<string>() { "clu" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_LEFT_CURSORS_LEFT_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.LeftDown, new List<string>() { "cll" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_LEFT_CURSORS_RIGHT_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.RightDown, new List<string>() { "clr" }, null, (int)DateTime.UtcNow.Ticks); }
        }


        private void button_KEY_LEFT_CURSORS_DOWN_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.DownDown, new List<string>() { "cld" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_RIGHT_CURSORS_UP_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "cru" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_RIGHT_CURSORS_RIGHT_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "crr" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_RIGHT_CURSORS_DOWN_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "crd" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_RIGHT_CURSORS_LEFT_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "crl" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_LEFT_ROCKER_SWITCH_UP_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.ZoomInDown, new List<string>() { "nsdd" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_LEFT_ROCKER_SWITCH_DOWN_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.ZoomOutDown, new List<string>() { "rsld" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_LEFT_CURSORS_CENTER_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.EnterDown, new List<string>() { "clc" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_HYPERBRAILLE_KEY_RIGHT_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.AbortDown, new List<string>() { "hbr" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_DOT7_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "k7" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_DOT3_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "k3" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_DOT2_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "k2" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_DOT1_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "k1" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_THUMB_LEFT_HAND_LEFT_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "l" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_THUMB_LEFT_HAND_RIGHT_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "lr" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_THUMB_RIGHT_HAND_LEFT_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "rl" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_THUMB_RIGHT_HAND_RIGHT_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "r" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_DOT4_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "k4" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_DOT5_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "k5" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_DOT6_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "k6" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_DOT8_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "k8" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_RIGHT_CURSORS_CENTER_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "crc" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_NAV_LEFT_2_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "nsll" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_NAV_LEFT_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "nsl" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_NAV_RIGHT_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "nsr" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_NAV_RIGHT_2_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "nsrr" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_NAV_UP_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "nsu" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_NAV_UP_2_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "nsuu" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_NAV_DOWN_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "nsd" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_NAV_DOWN_2_Click(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "nsdd" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        private void button_KEY_HYPERBRAILLE_KEY_LEFT_Click_1(object sender, EventArgs e)
        {
            if (showOffAdapter != null) { showOffAdapter.firekeyStateChangedEvent(Interface.BrailleIO_DeviceButtonStates.GestureDown, new List<string>() { "hbl" }, null, (int)DateTime.UtcNow.Ticks); }
        }

        #endregion
    }
}
