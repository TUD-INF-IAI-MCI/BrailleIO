using System;
using System.Collections.Generic;

namespace BrailleIO.Interface
{
    /// <summary>
    /// Utils for some working with the interface definitions
    /// </summary>
    public static class Utils
    {
        #region DeviceButtons

        /// <summary>
        /// Gets the 'up' state value for a certain device button.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns>the enum value for the 'up' state of the button.</returns>
        public static BrailleIO_DeviceButtonStates GetUpStateForDeviceButton(BrailleIO_DeviceButton button)
        {
            int val = (int)button;
            BrailleIO_DeviceButtonStates result = BrailleIO_DeviceButtonStates.None;
            try
            {
                result = (BrailleIO_DeviceButtonStates)GetButtonUpFlags(val);
            }
            catch (Exception) { }
            return result;
        }

        /// <summary>
        /// Gets the 'down' state value for a certain device button.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns>the enum value for the 'down' state of the button</returns>
        public static BrailleIO_DeviceButtonStates GetDownStateForDeviceButton(BrailleIO_DeviceButton button)
        {
            int val = (int)button;
            BrailleIO_DeviceButtonStates result = BrailleIO_DeviceButtonStates.None;
            try
            {
                result = (BrailleIO_DeviceButtonStates)ShiftUpButtonsToUpDownStates(GetButtonUpFlags(val));
            }
            catch (Exception) { }
            return result;
        }

        /// <summary>
        /// Gets the device buttons contained in the states flags.
        /// </summary>
        /// <param name="states">The states.</param>
        /// <returns>List of buttons contained in that state flag</returns>
        /// <remarks>DEPRECATED! use flag stuff instead</remarks>
        public static List<BrailleIO_DeviceButton> GetDeviceButtonsOfStates(BrailleIO_DeviceButtonStates states)
        {
            List<BrailleIO_DeviceButton> result = new List<BrailleIO_DeviceButton>();
            foreach (BrailleIO_DeviceButtonStates r in Enum.GetValues(typeof(BrailleIO_DeviceButtonStates)))
            {
                if (states.HasFlag(r))
                {
                    var b = GetDeviceButtonFlagsOfState(r);
                    if (!result.Contains(b)) result.Add(b);
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the device button for one certain button state.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>the button related to this state</returns>
        public static BrailleIO_DeviceButton GetDeviceButtonFlagsOfState(BrailleIO_DeviceButtonStates state)
        {
            BrailleIO_DeviceButton buttons = BrailleIO_DeviceButton.None;

            try
            {
                buttons = (BrailleIO_DeviceButton)ShiftDownButtonsToUpButtonStates((int)state);
            }
            catch (Exception) { }
            return buttons;
        }

        /// <summary>
        /// Gets all released device buttons.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>Flag of all released device buttons</returns>
        public static BrailleIO_DeviceButton GetAllUpDeviceButtons(BrailleIO_DeviceButtonStates state)
        {
            BrailleIO_DeviceButton result = BrailleIO_DeviceButton.None;
            try
            {
                result = (BrailleIO_DeviceButton)GetButtonUpFlags((int)state);
            }
            catch (Exception) { }
            return result;
        }

        /// <summary>
        /// Gets all pressed device buttons.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>Flag of all pressed device buttons</returns>
        public static BrailleIO_DeviceButton GetAllDownDeviceButtons(BrailleIO_DeviceButtonStates state)
        {
            BrailleIO_DeviceButton result = BrailleIO_DeviceButton.None;
            try
            {
                result = (BrailleIO_DeviceButton)ShiftDownButtonsToUpButtonStates(GetButtonDownFlags((int)state));
            }
            catch (Exception) { }
            return result;
        }

        #endregion

        #region BrailleKeyboardButton

        /// <summary>
        /// Gets the 'up' state value for a certain Braille keyboard button.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns>the enum value for the 'up' state of the button.</returns>
        public static BrailleIO_BrailleKeyboardButtonStates GetUpStateForDeviceButton(BrailleIO_BrailleKeyboardButton button)
        {
            int val = (int)button;
            BrailleIO_BrailleKeyboardButtonStates result = BrailleIO_BrailleKeyboardButtonStates.None;
            try
            {
                result = (BrailleIO_BrailleKeyboardButtonStates)GetButtonUpFlags(val);
            }
            catch (Exception) { }
            return result;
        }

        /// <summary>
        /// Gets the 'down' state value for a certain Braille keyboard button.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns>the enum value for the 'down' state of the button</returns>
        public static BrailleIO_BrailleKeyboardButtonStates GetDownStateForDeviceButton(BrailleIO_BrailleKeyboardButton button)
        {
            int val = (int)button;
            BrailleIO_BrailleKeyboardButtonStates result = BrailleIO_BrailleKeyboardButtonStates.None;
            try
            {
                result = (BrailleIO_BrailleKeyboardButtonStates)ShiftUpButtonsToUpDownStates(GetButtonUpFlags(val));
            }
            catch (Exception) { }
            return result;
        }

        /// <summary>
        /// Gets the device button for one Braille keyboard button state.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>the button related to this state</returns>
        public static BrailleIO_BrailleKeyboardButton GetDeviceButtonFlagsOfState(BrailleIO_BrailleKeyboardButtonStates state)
        {
            BrailleIO_BrailleKeyboardButton buttons = BrailleIO_BrailleKeyboardButton.None;

            try
            {
                buttons = (BrailleIO_BrailleKeyboardButton)ShiftDownButtonsToUpButtonStates((int)state);
            }
            catch (Exception) { }
            return buttons;
        }


        /// <summary>
        /// Gets all released Braille keyboard buttons.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>Flag of all released Braille keyboard buttons</returns>
        public static BrailleIO_BrailleKeyboardButton GetAllUpBrailleKeyboardButtons(BrailleIO_BrailleKeyboardButtonStates state)
        {
            BrailleIO_BrailleKeyboardButton result = BrailleIO_BrailleKeyboardButton.None;
            try
            {
                result = (BrailleIO_BrailleKeyboardButton)GetButtonUpFlags((int)state);
            }
            catch (Exception) { }
            return result;
        }

        /// <summary>
        /// Gets all pressed Braille keyboard buttons.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>Flag of all pressed Braille keyboard buttons</returns>
        public static BrailleIO_BrailleKeyboardButton GetAllDownBrailleKeyboardButtons(BrailleIO_BrailleKeyboardButtonStates state)
        {
            BrailleIO_BrailleKeyboardButton result = BrailleIO_BrailleKeyboardButton.None;
            try
            {
                result = (BrailleIO_BrailleKeyboardButton)ShiftDownButtonsToUpButtonStates(GetButtonDownFlags((int)state));
            }
            catch (Exception) { }
            return result;
        }

        #endregion

        #region AdditionalButton

        /// <summary>
        /// Gets the 'up' state value for a certain additional device button.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns>the enum value for the 'up' state of the button.</returns>
        public static BrailleIO_AdditionalButtonStates GetUpStateForDeviceButton(BrailleIO_AdditionalButton button)
        {
            int val = (int)button;
            BrailleIO_AdditionalButtonStates result = BrailleIO_AdditionalButtonStates.None;
            try
            {
                result = (BrailleIO_AdditionalButtonStates)GetButtonUpFlags(val);
            }
            catch (Exception) { }
            return result;
        }

        /// <summary>
        /// Gets the 'down' state value for a certain additional device button.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns>the enum value for the 'down' state of the button</returns>
        public static BrailleIO_AdditionalButtonStates GetDownStateForDeviceButton(BrailleIO_AdditionalButton button)
        {
            int val = (int)button;
            BrailleIO_AdditionalButtonStates result = BrailleIO_AdditionalButtonStates.None;
            try
            {
                result = (BrailleIO_AdditionalButtonStates)ShiftUpButtonsToUpDownStates(GetButtonUpFlags(val));
            }
            catch (Exception) { }
            return result;
        }

        /// <summary>
        /// Gets the additional device button for one certain button state.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>the button related to this state</returns>
        public static BrailleIO_AdditionalButton GetDeviceButtonFlagsOfState(BrailleIO_AdditionalButtonStates state)
        {
            BrailleIO_AdditionalButton buttons = BrailleIO_AdditionalButton.None;

            try
            {
                buttons = (BrailleIO_AdditionalButton)ShiftDownButtonsToUpButtonStates((int)state);
            }
            catch (Exception) { }
            return buttons;
        }

        /// <summary>
        /// Gets all released additional device buttons.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>Flag of all released additional device buttons</returns>
        public static BrailleIO_AdditionalButton GetAllUpAdditionalButtons(BrailleIO_AdditionalButtonStates state)
        {
            BrailleIO_AdditionalButton result = BrailleIO_AdditionalButton.None;
            try
            {
                result = (BrailleIO_AdditionalButton)GetButtonUpFlags((int)state);
            }
            catch (Exception) { }
            return result;
        }

        /// <summary>
        /// Gets all pressed additional device buttons.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>Flag of all pressed additional device buttons</returns>
        public static BrailleIO_AdditionalButton GetAllDownAdditionalButtons(BrailleIO_AdditionalButtonStates state)
        {
            BrailleIO_AdditionalButton result = BrailleIO_AdditionalButton.None;
            try
            {
                result = (BrailleIO_AdditionalButton)ShiftDownButtonsToUpButtonStates(GetButtonDownFlags((int)state));
            }
            catch (Exception) { }
            return result;
        }

        /// <summary>
        /// Combines two collections of additional buttons.
        /// </summary>
        /// <param name="dict1">The first collection.</param>
        /// <param name="dict2">The second collection.</param>
        /// <returns>A collection containing all buttons from both additional button collections.</returns>
        public static IDictionary<int, BrailleIO_AdditionalButton> CombineAdditionalButtonCollections(IDictionary<int, BrailleIO_AdditionalButton> dict1, IDictionary<int, BrailleIO_AdditionalButton> dict2)
        {
            if (dict1 == null) dict1 = new Dictionary<int, BrailleIO_AdditionalButton>();

            var result = dict1;

            if (dict2 != null && dict2.Count > 0)
            {
                foreach (var kvPair in dict2)
                {
                    if (result.ContainsKey(kvPair.Key))
                    {
                        result[kvPair.Key] |= kvPair.Value;
                    }
                    else
                    {
                        if (kvPair.Value != BrailleIO_AdditionalButton.None)
                        {
                            result.Add(kvPair.Key, kvPair.Value);
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Combines two collections of additional buttons.
        /// </summary>
        /// <param name="arr">An array of additional buttons.</param>
        /// <param name="dict">A dictionary of additional buttons.</param>
        /// <returns>An combined array of additional buttons.</returns>
        public static BrailleIO_AdditionalButton[] CombineAdditionalButtonCollections(BrailleIO_AdditionalButton[] arr, IDictionary<int, BrailleIO_AdditionalButton> dict)
        {
            // build list out of the array
            List<BrailleIO_AdditionalButton> resultList = new List<BrailleIO_AdditionalButton>(arr != null ? arr : new BrailleIO_AdditionalButton[0]);

            // add dictionary
            if (dict != null && dict.Count > 0)
            {
                foreach (var kvPair in dict)
                {
                    if (kvPair.Value != BrailleIO_AdditionalButton.None)
                    {
                        int i = kvPair.Key;
                        // extend list
                        while (i >= resultList.Count)
                        {
                            resultList.Add(BrailleIO_AdditionalButton.None);
                        }
                        // combine
                        resultList[i] |= kvPair.Value;
                    }
                }
            }
            return resultList.ToArray();
        }

        /// <summary>
        /// Combines two collections of additional buttons.
        /// </summary>
        /// <param name="arr1">The first array of additional buttons.</param>
        /// <param name="arr2">The second array of additional buttons.</param>
        /// <returns>An combined array of additional buttons.</returns>
        public static BrailleIO_AdditionalButton[] CombineAdditionalButtonCollections(BrailleIO_AdditionalButton[] arr1, BrailleIO_AdditionalButton[] arr2)
        {
            if (arr1 == null) return arr2;

            BrailleIO_AdditionalButton[] result = arr1;

            if (arr2.Length > 0)
            {
                BrailleIO_AdditionalButton[] inserter = arr2;
                // check for the larger one
                if (arr1.Length < arr2.Length) { result = arr2; inserter = arr1; }

                for (int i = 0; i < inserter.Length; i++)
                {
                    result[i] |= inserter[i];
                }
            }
            return result;
        }

        #endregion

        #region General Handling of Button Enums

        /// <summary>
        /// Gets all flags indicating a released button in this enum flag value.
        /// </summary>
        /// <param name="i">The combined flag enum value.</param>
        /// <returns>a flag enum value containing only released buttons.</returns>
        /// <remarks>Does only work for the button flags defined in BrailleIO.Interface. 
        /// Here the 0x0 = NONE, 0x1 = UNKOWN, 'up' starts from 0x2 directly followed by its related 'down' state (0x4)!</remarks>
        public static int GetButtonUpFlags(int i)
        {
            int flags = 0;

            if ((i & 1) == 1) flags = 1;

            int f = 2;
            while (f > 0 && f <= i)
            {
                if ((f & i) == f)
                {
                    flags = flags | f;
                }
                f = f << 2; // shift through all flags
            }
            return flags;
        }

        /// <summary>
        /// Gets all flags indicating a pressed button in this enum flag value.
        /// </summary>
        /// <param name="i">The combined flag enum value.</param>
        /// <returns>a flag enum value containing only pressed buttons.</returns>
        /// <remarks>Does only work for the button flags defined in BrailleIO.Interface. 
        /// Here the 0x0 = NONE, 0x1 = UNKOWN, 'up' starts from 0x2 directly followed by its related 'down' state (0x4)!</remarks>
        public static int GetButtonDownFlags(int i)
        {
            int flags = 0;

            int f = 1;
            while (f > 0 && f <= i)
            {
                if ((f & i) == f)
                {
                    flags = flags | f;
                }
                f = f << 2; // shift through all flags
            }

            return flags;
        }

        /// <summary>
        /// Shifts button states from a 'down' state to its related 'up' state.
        /// Contained 'up' states are kept.
        /// </summary>
        /// <param name="i">The button States.</param>
        /// <returns>The new button states with all states are up states</returns>
        /// <remarks>Does only work for the button flags defined in BrailleIO.Interface. 
        /// Here the 0x0 = NONE, 0x1 = UNKOWN, 'up' starts from 0x2 directly followed by its related 'down' state (0x4)!</remarks>
        public static int ShiftDownButtonsToUpButtonStates(int i)
        {
            int result = i;
            int f = 4;
            while (f > 0 && f <= i)
            {
                if ((f & i) == f)
                {
                    result = result ^ f;
                    result = result | f >> 1;
                }
                f = f << 2; // shift through all flags
            }

            return result;
        }

        /// <summary>
        /// Shifts button states from a up state to its related down state.
        /// Contained down states are kept.
        /// </summary>
        /// <param name="i">The button States.</param>
        /// <returns>The new button states with all states are down states</returns>
        /// <remarks>Does only work for the button flags defined in BrailleIO.Interface. 
        /// Here the 0x0 = NONE, 0x1 = UNKOWN, 'up' starts from 0x2 directly followed by its related 'down' state (0x4)!</remarks>
        public static int ShiftUpButtonsToUpDownStates(int i)
        {
            int result = i;
            int f = 2;
            while (f > 0 && f <= i)
            {
                if ((f & i) == f)
                {
                    result = result ^ f;
                    result = result | f << 1;
                }
                f = f << 2; // shift through all flags
            }

            return result;
        }


        #endregion

    }
}