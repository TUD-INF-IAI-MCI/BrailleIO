using BrailleIO.Interface;
using System;
using System.Collections.Generic;

namespace BrailleIO.Structs
{
    /// <summary>
    /// Bundle of information for a key combination
    /// </summary>
    public struct KeyCombinationItem
    {
        BrailleIO_DeviceButton _pressedGeneralKeys;
        /// <summary>
        /// Enum flag of all currently pressed general buttons
        /// </summary>
        public BrailleIO_DeviceButton PressedGeneralKeys
        {
            get { return _pressedGeneralKeys; }
            set { _pressedGeneralKeys = value; restPressedCache(); }
        }

        BrailleIO_DeviceButton _releasedGeneralKeys;
        /// <summary>
        /// Enum flag of all released general buttons
        /// </summary>
        public BrailleIO_DeviceButton ReleasedGeneralKeys
        {
            get { return _releasedGeneralKeys; }
            set { _releasedGeneralKeys = value; restReleasedCache(); }
        }

        BrailleIO_BrailleKeyboardButton _pressedKeyboardKeys;
        /// <summary>
        /// Enum flag of all currently pressed Braille-keyboard buttons
        /// </summary>
        public BrailleIO_BrailleKeyboardButton PressedKeyboardKeys
        {
            get { return _pressedKeyboardKeys; }
            set { _pressedKeyboardKeys = value; restPressedCache(); }
        }

        BrailleIO_BrailleKeyboardButton _releasedKeyboardKeys;
        /// <summary>
        /// Enum flag of all released Braille-keyboard buttons
        /// </summary>
        public BrailleIO_BrailleKeyboardButton ReleasedKeyboardKeys
        {
            get { return _releasedKeyboardKeys; }
            set { _releasedKeyboardKeys = value; restReleasedCache(); }
        }

        BrailleIO_AdditionalButton[] _pressedAdditionalKeys;
        /// <summary>
        /// List of enum flag of all currently pressed additional button sets
        /// </summary>
        public BrailleIO_AdditionalButton[] PressedAdditionalKeys
        {
            get { return _pressedAdditionalKeys; }
            set { _pressedAdditionalKeys = value; restPressedCache(); }
        }

        BrailleIO_AdditionalButton[] _releasedAdditionalKeys;
        /// <summary>
        /// List of enum flag of all released additional button sets
        /// </summary>
        public BrailleIO_AdditionalButton[] ReleasedAdditionalKeys
        {
            get { return _releasedAdditionalKeys; }
            set { _releasedAdditionalKeys = value; restReleasedCache(); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyCombinationItem" /> struct.
        /// </summary>
        /// <param name="pressedGeneralKeys">The pressed general keys.</param>
        /// <param name="releasedGeneralKeys">The released general keys.</param>
        /// <param name="pressedKeyboardKeys">The pressed keyboard keys.</param>
        /// <param name="releasedKeyboardKeys">The released keyboard keys.</param>
        /// <param name="pressedAdditionalKeys">The pressed additional keys.</param>
        /// <param name="releasedAdditionalKeys">The released additional keys.</param>
        public KeyCombinationItem(
            BrailleIO_DeviceButton pressedGeneralKeys,
            BrailleIO_DeviceButton releasedGeneralKeys,
            BrailleIO_BrailleKeyboardButton pressedKeyboardKeys,
            BrailleIO_BrailleKeyboardButton releasedKeyboardKeys,
            BrailleIO_AdditionalButton[] pressedAdditionalKeys,
            BrailleIO_AdditionalButton[] releasedAdditionalKeys
            )
        {
            _pressedGeneralKeys = pressedGeneralKeys;
            _releasedGeneralKeys = releasedGeneralKeys;

            _pressedKeyboardKeys = pressedKeyboardKeys;
            _releasedKeyboardKeys = releasedKeyboardKeys;

            _pressedAdditionalKeys = pressedAdditionalKeys;
            _releasedAdditionalKeys = releasedAdditionalKeys;

            _pressedString = _releasedString = "-";
            _pressed = _released = 0;
        }

        #region Functions

        short _pressed;
        /// <summary>
        /// Determines whether some currently pressed buttons are detected or not.
        /// </summary>
        /// <returns><c>true</c> if some pressed buttons are registered; otherwise, <c>false</c>.</returns>
        public bool AreButtonsPressed()
        {
            if (_pressed != 0) return _pressed > 0;
            if (PressedGeneralKeys != BrailleIO_DeviceButton.None ||
                PressedKeyboardKeys != BrailleIO_BrailleKeyboardButton.None)
            {
                _pressed = 1;
                return true;
            }
            else // check additional buttons
            {
                if (PressedAdditionalKeys != null && PressedAdditionalKeys.Length > 0)
                {
                    foreach (var item in PressedAdditionalKeys)
                    {
                        if (item != BrailleIO_AdditionalButton.None)
                        {
                            _pressed = 1;
                            return true;
                        }
                    }
                }
            }
            _pressed = -1;
            return false;
        }

        short _released;
        /// <summary>
        /// Determines whether some released buttons are detected or not.
        /// </summary>
        /// <returns><c>true</c> if some released buttons are registered; otherwise, <c>false</c>.</returns>
        public bool AreButtonsReleased()
        {
            if (_released != 0) return _released > 0;
            if (ReleasedGeneralKeys != BrailleIO_DeviceButton.None ||
                ReleasedKeyboardKeys != BrailleIO_BrailleKeyboardButton.None)
            {
                _released = 1;
                return true;
            }
            else // check additional buttons
            {
                if (ReleasedAdditionalKeys != null && ReleasedAdditionalKeys.Length > 0)
                {
                    foreach (var item in ReleasedAdditionalKeys)
                    {
                        if (item != BrailleIO_AdditionalButton.None)
                        {
                            _released = 1;
                            return true;
                        }
                    }
                }
            }
            _released = -1;
            return false;
        }


        private string _pressedString;
        /// <summary>
        /// Returns a comma separated list of all currently pressed buttons.
        /// </summary>
        /// <returns>String of currently pressed buttons.</returns>
        public string PressedButtonsToString()
        {
            if (!_pressedString.Equals("-")) return _pressedString;
            string result = String.Empty;

            if (PressedGeneralKeys != BrailleIO_DeviceButton.None)
            {
                string pgs = PressedGeneralKeys.ToString();
                if (!String.IsNullOrWhiteSpace(pgs))
                {
                    if (!String.IsNullOrWhiteSpace(result)) result += ",";
                    result += pgs;
                }
            }

            if (PressedKeyboardKeys != BrailleIO_BrailleKeyboardButton.None)
            {
                string pkbs = PressedKeyboardKeys.ToString();
                if (!String.IsNullOrWhiteSpace(pkbs))
                {
                    if (!String.IsNullOrWhiteSpace(result)) result += ",";
                    result += pkbs;
                }
            }

            // check additional buttons
            if (PressedAdditionalKeys != null && PressedAdditionalKeys.Length > 0)
            {
                for (int i = 0; i < PressedAdditionalKeys.Length; i++)
                {
                    var addBtns = PressedAdditionalKeys[i];

                    if (addBtns != BrailleIO_AdditionalButton.None)
                    {
                        string pAddBtns = addBtns.ToString();
                        if (i > 0) pAddBtns = pAddBtns.Replace(",","_" + i + ",");
                        if (!String.IsNullOrWhiteSpace(pAddBtns))
                        {
                            if (!String.IsNullOrWhiteSpace(result)) result += ",";
                        }
                        result += pAddBtns;
                        if (i > 0) result += "_" + i;
                    }
                }
            }

            result = result.Replace(" ", "");
            _pressedString = result;
            return result;
        }

        private string _releasedString;
        /// <summary>
        /// Returns a comma separated list of all released buttons.
        /// </summary>
        /// <returns>String of released buttons.</returns>
        public string ReleasedButtonsToString()
        {
            if (!_releasedString.Equals("-")) return _releasedString;
            string result = String.Empty;

            if (ReleasedGeneralKeys != BrailleIO_DeviceButton.None)
            {
                string pgs = ReleasedGeneralKeys.ToString();
                if (!String.IsNullOrWhiteSpace(pgs))
                {
                    if (!String.IsNullOrWhiteSpace(result)) result += ",";
                    result += pgs;
                }
            }

            if (ReleasedKeyboardKeys != BrailleIO_BrailleKeyboardButton.None)
            {
                string pkbs = ReleasedKeyboardKeys.ToString();
                if (!String.IsNullOrWhiteSpace(pkbs))
                {
                    if (!String.IsNullOrWhiteSpace(result)) result += ",";
                    result += pkbs;
                }
            }

            // check additional buttons
            if (ReleasedAdditionalKeys != null && ReleasedAdditionalKeys.Length > 0)
            {
                for (int i = 0; i < ReleasedAdditionalKeys.Length; i++)
                {
                    var addBtns = ReleasedAdditionalKeys[i];

                    if (addBtns != BrailleIO_AdditionalButton.None)
                    {
                        string rAddBtns = addBtns.ToString();
                        if (i > 0) rAddBtns = rAddBtns.Replace(",","_" + i + ",");
                        if (!String.IsNullOrWhiteSpace(result))
                        {
                            result += ",";
                        }
                        result += rAddBtns;
                        if (i > 0) result += "_" + i;
                    }
                }
            }

            result = result.Replace(" ", "");
            _releasedString = result;
            return result;
        }

        #region Caching
        
        private void restPressedCache()
        {
            _pressedString = "-";
            _pressed = 0;
        }

        private void restReleasedCache()
        {
            _releasedString = "-";
            _pressed = 0;
        }

        #endregion

        #endregion

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "Pressed:'" + PressedButtonsToString() + "' Released:'" + ReleasedButtonsToString() + "'";
        }

    }

}
