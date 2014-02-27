using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using BrailleIO.Interface;

namespace BrailleIO
{
    public partial class ShowOff : Form
    {
        #region buttons

        private void button_KEY_RIGHT_ROCKER_SWITCH_UP_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "rsru" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, null, new List<string>() { "rsru" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_RIGHT_ROCKER_SWITCH_DOWN_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "rsrd" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, null, new List<string>() { "rsrd" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_LEFT_CURSORS_UP_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.UpDown, new List<string>() { "clu" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.UpUp, null, new List<string>() { "clu" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_LEFT_CURSORS_LEFT_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.LeftDown, new List<string>() { "cll" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.LeftUp, null, new List<string>() { "cll" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_LEFT_CURSORS_RIGHT_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.RightDown, new List<string>() { "clr" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.RightUp, null, new List<string>() { "clr" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_LEFT_CURSORS_DOWN_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.DownDown, new List<string>() { "cld" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.DownUp, null, new List<string>() { "cld" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_RIGHT_CURSORS_UP_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "cru" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, null, new List<string>() { "cru" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_RIGHT_CURSORS_RIGHT_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "crr" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, null, new List<string>() { "crr" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_RIGHT_CURSORS_DOWN_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "crd" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, null, new List<string>() { "crd" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_RIGHT_CURSORS_LEFT_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "crl" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, null, new List<string>() { "crl" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_LEFT_ROCKER_SWITCH_UP_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.ZoomInDown, new List<string>() { "rslu" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.ZoomInUp, null, new List<string>() { "rslu" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_LEFT_ROCKER_SWITCH_DOWN_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.ZoomOutDown, new List<string>() { "rsld" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.ZoomOutUp, null, new List<string>() { "rsld" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_LEFT_CURSORS_CENTER_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.EnterDown, new List<string>() { "clc" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.EnterUp, null, new List<string>() { "clc" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_HYPERBRAILLE_KEY_RIGHT_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.AbortDown, new List<string>() { "hbr" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.AbortUp, null, new List<string>() { "hbr" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_DOT7_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "k7" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, null, new List<string>() { "k7" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_DOT3_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "k3" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, null, new List<string>() { "k3" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_DOT2_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "k2" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, null, new List<string>() { "k2" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_DOT1_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "k1" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, null, new List<string>() { "k1" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_THUMB_LEFT_HAND_LEFT_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "l" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, null, new List<string>() { "l" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_THUMB_LEFT_HAND_RIGHT_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "lr" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, null, new List<string>() { "lr" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_THUMB_RIGHT_HAND_LEFT_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "rl" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, null, new List<string>() { "rl" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_THUMB_RIGHT_HAND_RIGHT_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "r" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, null, new List<string>() { "r" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_DOT4_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "k4" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, null, new List<string>() { "k4" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_DOT5_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "k5" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, null, new List<string>() { "k5" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_DOT6_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "k6" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, null, new List<string>() { "k6" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_DOT8_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "k8" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, null, new List<string>() { "k8" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_RIGHT_CURSORS_CENTER_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "crc" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, null, new List<string>() { "crc" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_NAV_LEFT_2_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "nsll" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, null, new List<string>() { "nsll" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_NAV_LEFT_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "nsl" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, null, new List<string>() { "nsl" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_NAV_RIGHT_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "nsr" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, null, new List<string>() { "nsr" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_NAV_RIGHT_2_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "nsrr" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, null, new List<string>() { "nsrr" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_NAV_UP_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "nsu" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, null, new List<string>() { "nsu" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_NAV_UP_2_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "nsuu" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, null, new List<string>() { "nsuu" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_NAV_DOWN_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "nsd" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, null, new List<string>() { "nsd" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_NAV_DOWN_2_Click(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, new List<string>() { "nsdd" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.Unknown, null, new List<string>() { "nsdd" }, (int)DateTime.UtcNow.Ticks);
        }

        private void button_KEY_HYPERBRAILLE_KEY_LEFT_Click_1(object sender, EventArgs e)
        {
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.GestureDown, new List<string>() { "hbl" }, null, (int)DateTime.UtcNow.Ticks);
            fireKeyStateChangeEvent(Interface.BrailleIO_DeviceButtonStates.GestureUp, null, new List<string>() { "hbl" }, (int)DateTime.UtcNow.Ticks);
        }

        #endregion

        #region key combinations


        #region Application keys

        protected volatile bool ctr = false;

        void ShowOff_KeyUp(object sender, KeyEventArgs e)
        {
            if (e != null)
            {
                if (!e.Control && ctr)
                {
                    ctr = false;
                    fireKeyStateChangeEvent(BrailleIO_DeviceButtonStates.None, null, null, 0);
                }
            }
        }

        void ShowOff_KeyDown(object sender, KeyEventArgs e)
        {
            if (e != null)
            {
                if (e.Control && !ctr)
                {
                    ctr = true;
                }
            }
        }

        void hk_Pressed(object sender, System.ComponentModel.HandledEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion



        volatile List<BrailleIO_DeviceButtonStates> _pressedStates = new List<BrailleIO_DeviceButtonStates>();
        volatile List<BrailleIO_DeviceButtonStates> _releasedStates = new List<BrailleIO_DeviceButtonStates>();
        volatile List<String> _pressedButtons = new List<String>();
        volatile List<String> _releasedButtons = new List<String>();

        internal void fireKeyStateChangeEvent(BrailleIO_DeviceButtonStates states,
            List<string> pressedKeys,
            List<string> releasedKeys,
            int timeStampTickCount)
        {


            if (ctr)
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
                            }
                            else
                            {
                                _pressedButtons.Add(pk);
                                _pressedStates.Add(states);
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
                            }
                            else
                            {
                                _releasedButtons.Add(rk);
                                _releasedStates.Add(states);
                            }
                        }
                    }
                }

                MarkButtonAsPressed(_pressedButtons);

                return;
            }

            if (states == BrailleIO_DeviceButtonStates.None && pressedKeys == null && releasedKeys == null && timeStampTickCount == 0)
            {
                //check if this is because the ctr is released
                if (showOffAdapter != null)
                {
                    BrailleIO_DeviceButtonStates ps = BrailleIO_DeviceButtonStates.None;
                    foreach (BrailleIO_DeviceButtonStates s in _pressedStates) { ps = ps & s; }

                    BrailleIO_DeviceButtonStates rs = BrailleIO_DeviceButtonStates.None;
                    foreach (BrailleIO_DeviceButtonStates s in _releasedStates) { rs = rs & s; }

                    showOffAdapter.firekeyStateChangedEvent(ps, _pressedButtons, new List<String>(), timeStampTickCount);
                    showOffAdapter.firekeyStateChangedEvent(rs, new List<String>(), _releasedButtons, timeStampTickCount);

                    UnmarkButtons(_releasedButtons);

                    _pressedButtons.Clear();
                    _releasedButtons.Clear();
                    _pressedStates.Clear();
                    _releasedStates.Clear();
                }
            }
            else
            {
                if (showOffAdapter != null)
                {
                    showOffAdapter.firekeyStateChangedEvent(states, pressedKeys, releasedKeys, timeStampTickCount);
                }
            }
        }


        #endregion

        #region Button marking

        private readonly Color NormalButtonBgColor = Color.FromArgb(0, Color.Gray);
        private readonly Color MarkedButtonBgColor = Color.FromArgb(100, Color.DarkOrange);
        private readonly Color ReleasingButtonBgColor = Color.FromArgb(100, Color.ForestGreen);

        public void MarkButtonAsPressed(List<String> pressedButtons)
        {
            if (pressedButtons != null && pressedButtons.Count > 0)
                foreach (var buttonName in pressedButtons)
                {
                    Control button = getButtonFromGenericName(buttonName.ToString().Trim());
                    if (button != null)
                    {
                        button.BackColor = MarkedButtonBgColor;
                    }
                }
        }

        public void UnmarkButtons(List<String> releasedButtons)
        {
            if (releasedButtons != null && releasedButtons.Count > 0)
                foreach (var buttonName in releasedButtons)
                {
                    Control button = getButtonFromGenericName(buttonName.ToString().Trim());
                    if (button != null)
                    {
                        button.BackColor = ReleasingButtonBgColor;
                        System.Threading.Thread.Sleep(100);
                        button.BackColor = NormalButtonBgColor;
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

        #endregion

    }
}
