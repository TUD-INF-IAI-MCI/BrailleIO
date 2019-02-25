using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrailleIO.Interface
{
    /// <summary>
    /// get Access to the properties for zooming
    /// </summary>
    public interface IZoomable
    {
        /// <summary>
        /// Gets the actual zoom-level (zoom factor).
        /// </summary>
        /// <returns>Zoom value as ratio</returns>
        double GetZoom();
        /// <summary>
        /// Sets the actual zoom.
        /// </summary>
        /// <param name="zoom">The zoom value as ratio (zoom factor).</param>
        void SetZoom(double zoom);
    }
}
