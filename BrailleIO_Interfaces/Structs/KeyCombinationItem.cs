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
        /// <summary>
        /// Enum flag of all currently pressed general buttons
        /// </summary>
        public BrailleIO_DeviceButton PressedGeneralKeys;
        /// <summary>
        /// Enum flag of all released general buttons
        /// </summary>
        public BrailleIO_DeviceButton ReleasedGeneralKeys;
        /// <summary>
        /// Enum flag of all currently pressed Braille-keyboard buttons
        /// </summary>
        public BrailleIO_BrailleKeyboardButton PressedKeyboardKeys;
        /// <summary>
        /// Enum flag of all released Braille-keyboard buttons
        /// </summary>
        public BrailleIO_BrailleKeyboardButton ReleasedKeyboardKeys;
        /// <summary>
        /// List of enum flag of all currently pressed additional button sets
        /// </summary>
        public BrailleIO_AdditionalButton[] PressedAdditionalKeys;
        /// <summary>
        /// List of enum flag of all released additional button sets
        /// </summary>
        public BrailleIO_AdditionalButton[] ReleasedAdditionalKeys;

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
            PressedGeneralKeys = pressedGeneralKeys;
            ReleasedGeneralKeys = releasedGeneralKeys;

            PressedKeyboardKeys = pressedKeyboardKeys;
            ReleasedKeyboardKeys = releasedKeyboardKeys;

            PressedAdditionalKeys = pressedAdditionalKeys;
            ReleasedAdditionalKeys = releasedAdditionalKeys;
        }


        /// <summary>
        /// Determines whether some currently pressed buttons are detected or not.
        /// </summary>
        /// <returns><c>true</c> if some pressed buttons are registered; otherwise, <c>false</c>.</returns>
        public bool AreButtonsPressed()
        {
            if (PressedGeneralKeys != BrailleIO_DeviceButton.None ||
                PressedKeyboardKeys != BrailleIO_BrailleKeyboardButton.None)
            {
                return true;
            }
            else // check additional buttons
            {
                if (PressedAdditionalKeys != null && PressedAdditionalKeys.Length > 0)
                {
                    foreach (var item in PressedAdditionalKeys)
                    {
                        if (item != BrailleIO_AdditionalButton.None)
                            return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether some released buttons are detected or not.
        /// </summary>
        /// <returns><c>true</c> if some released buttons are registered; otherwise, <c>false</c>.</returns>
        public bool AreButtonsReleased()
        {
            if (ReleasedGeneralKeys != BrailleIO_DeviceButton.None ||
                ReleasedKeyboardKeys != BrailleIO_BrailleKeyboardButton.None)
            {
                return true;
            }
            else // check additional buttons
            {
                if (ReleasedAdditionalKeys != null && ReleasedAdditionalKeys.Length > 0)
                {
                    foreach (var item in ReleasedAdditionalKeys)
                    {
                        if (item != BrailleIO_AdditionalButton.None)
                            return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Returns a comma separated list of all currently pressed buttons.
        /// </summary>
        /// <returns>String of currently pressed buttons.</returns>
        public string PressedButtonsToString()
        {
            string result = String.Empty;

            if (PressedGeneralKeys != BrailleIO_DeviceButton.None)
            {
                string pgs = PressedGeneralKeys.ToString();
                if (!String.IsNullOrWhiteSpace(pgs))
                {
                    if (!String.IsNullOrWhiteSpace(result)) result += ", ";
                    result += pgs;
                }
            }

            if (PressedKeyboardKeys != BrailleIO_BrailleKeyboardButton.None)
            {
                string pkbs = PressedKeyboardKeys.ToString();
                if (!String.IsNullOrWhiteSpace(pkbs))
                {
                    if (!String.IsNullOrWhiteSpace(result)) result += ", ";
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
                        if (i > 0) pAddBtns = pAddBtns.Replace(",", "_" + i + ",");
                        if (!String.IsNullOrWhiteSpace(pAddBtns))
                        {
                            if (!String.IsNullOrWhiteSpace(result)) result += ", ";
                        }
                        result += pAddBtns;
                        if (i > 0) result += "_" + i;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Returns a comma separated list of all released buttons.
        /// </summary>
        /// <returns>String of released buttons.</returns>
        public string ReleasedButtonsToString()
        {
            string result = String.Empty;

            if (ReleasedGeneralKeys != BrailleIO_DeviceButton.None)
            {
                string pgs = ReleasedGeneralKeys.ToString();
                if (!String.IsNullOrWhiteSpace(pgs))
                {
                    if (!String.IsNullOrWhiteSpace(result)) result += ", ";
                    result += pgs;
                }
            }

            if (ReleasedKeyboardKeys != BrailleIO_BrailleKeyboardButton.None)
            {
                string pkbs = ReleasedKeyboardKeys.ToString();
                if (!String.IsNullOrWhiteSpace(pkbs))
                {
                    if (!String.IsNullOrWhiteSpace(result)) result += ", ";
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
                        if (i > 0) rAddBtns = rAddBtns.Replace(",", "_" + i + ",");
                        if (!String.IsNullOrWhiteSpace(result))
                        {
                            result += ", ";
                        }
                        result += rAddBtns;
                        if (i > 0) result += "_" + i;
                    }
                }
            }

            return result;
        }

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
