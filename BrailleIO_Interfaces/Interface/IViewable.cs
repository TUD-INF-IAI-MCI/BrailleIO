using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrailleIO.Interface
{
    /// <summary>
    /// Enables an implementing instance to give access to properties of there visibility
    /// </summary>
    public interface IViewable
    {
        /// <summary>
        /// Sets the visibility.
        /// </summary>
        /// <param name="visible">if set to <c>true</c> the instance shoulb bee visible; otherwise the instance is hidden</param>
        void SetVisibility(bool visible);
        /// <summary>
        /// Determines whether this instance is visible.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance is visible; otherwise, <c>false</c> if the instance is hidden.
        /// </returns>
        bool IsVisible();
    }
}
