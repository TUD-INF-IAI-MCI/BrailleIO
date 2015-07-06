using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrailleIO.Interface
{
    /// <summary>
    /// Enables an implementing instance to give access to properties of there used Threshold
    /// </summary>
    public interface IContrastThreshold
    {
        /// <summary>
        /// Sets the contrast threshold.
        /// </summary>
        /// <param name="threshold">The threshold.</param>
        /// <returns>the new set threshold</returns>
        int SetContrastThreshold(int threshold);
        /// <summary>
        /// Gets the contrast threshold.
        /// </summary>
        /// <returns>the threshold</returns>
        int GetContrastThreshold();
    }
}
