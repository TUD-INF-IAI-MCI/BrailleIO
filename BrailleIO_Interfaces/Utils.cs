using System;
using System.Collections.Generic;

namespace BrailleIO.Interface
{
    /// <summary>
    /// Utils for some working with the interface definitions
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Gets the 'up' state value for a certain device button.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns>the enum value for the 'up' state of the button</returns>
        public static BrailleIO_DeviceButtonStates GetUpStateForDeviceButton(BrailleIO_DeviceButton button)
        {
            string name = button.ToString() + "Up";
            if (Enum.IsDefined(typeof(BrailleIO_DeviceButtonStates), name))
                return (BrailleIO_DeviceButtonStates)Enum.Parse(typeof(BrailleIO_DeviceButtonStates), name);
            return BrailleIO_DeviceButtonStates.Unknown;
        }

        /// <summary>
        /// Gets the 'down' state value for a certain device button.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns>the enum value for the 'down' state of the button</returns>
        public static BrailleIO_DeviceButtonStates GetDownStateForDeviceButton(BrailleIO_DeviceButton button)
        {
            string name = button.ToString() + "Down";
            if (Enum.IsDefined(typeof(BrailleIO_DeviceButtonStates), name))
                return (BrailleIO_DeviceButtonStates)Enum.Parse(typeof(BrailleIO_DeviceButtonStates), name);
            return BrailleIO_DeviceButtonStates.Unknown;
        }

        /// <summary>
        /// Gets the device buttons contained in the states flags.
        /// </summary>
        /// <param name="states">The states.</param>
        /// <returns>List of buttons contained in that state flag</returns>
        public static List<BrailleIO_DeviceButton> GetDeviceButtonsOfStates(BrailleIO_DeviceButtonStates states)
        {
            List<BrailleIO_DeviceButton> result = new List<BrailleIO_DeviceButton>();
            foreach (BrailleIO_DeviceButtonStates r in Enum.GetValues(typeof(BrailleIO_DeviceButtonStates)))
            {
                if (states.HasFlag(r))
                {
                    var b = GetDeviceButtonOfState(r);
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
        public static BrailleIO_DeviceButton GetDeviceButtonOfState(BrailleIO_DeviceButtonStates state)
        {
            return GetDeviceButtonOfState(state.ToString());
        }

        /// <summary>
        /// Gets the device button for one certain button state.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>the button related to this state</returns>
        public static BrailleIO_DeviceButton GetDeviceButtonOfState(String stateName)
        {
            if (Enum.IsDefined(typeof(BrailleIO_DeviceButton), stateName))
            {
                return (BrailleIO_DeviceButton)Enum.Parse(typeof(BrailleIO_DeviceButton), stateName);
            }
            return BrailleIO_DeviceButton.Unknown;
        }
    }
}
