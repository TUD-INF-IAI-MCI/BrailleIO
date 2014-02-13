using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BraillIOExample
{
    internal static class Utils
    {
        public static float GetResoultion(float dpiX, float dpiY)
        {
            return (dpiX + dpiY) / 2;
        }
        
        [System.Runtime.InteropServices.DllImport("gdi32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        private static extern int GetDeviceCaps(IntPtr hDC, int nIndex);
        private enum DeviceCap
        {
            /// <summary>
            /// Logical pixels inch in X
            /// </summary>
            LOGPIXELSX = 88,
            /// <summary>
            /// Logical pixels inch in Y
            /// </summary>
            LOGPIXELSY = 90

            // Other constants may be founded on pinvoke.net
        }

        /// <summary>
        /// Gets the screen dpi.
        /// </summary>
        /// <returns></returns>
        public static float GetScreenDpi()
        {
            System.Drawing.Graphics g = System.Drawing.Graphics.FromHwnd(IntPtr.Zero);
            IntPtr desktop = g.GetHdc();

            int dpiX = GetDeviceCaps(desktop, (int)DeviceCap.LOGPIXELSX);
            int dpiY = GetDeviceCaps(desktop, (int)DeviceCap.LOGPIXELSY);

            return GetResoultion((float)dpiX, (float)dpiY);
        }
    }
}
