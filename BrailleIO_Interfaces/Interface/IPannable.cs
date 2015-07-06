using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace BrailleIO.Interface
{
    /// <summary>
    /// Enables an implementing instance to indicate the possibilities for panning operation 
    /// </summary>
    public interface IPannable
    {
        /// <summary>
        /// Gets or sets a value indicating whether to show some scrollbars.
        /// </summary>
        /// <value><c>true</c> if to show scrollbars; otherwise, <c>false</c>.</value>
        bool ShowScrollbars { get; set; }
        /// <summary>
        /// Gets the offset in horizontal direction.
        /// Should normally be negative. Moves the position of the content by the offset.
        /// E.g. a negative offset should move the content to the left and the viewable 
        /// region to the right.
        /// </summary>
        /// <returns>the horizontal offset</returns>
        int GetXOffset();
        /// <summary>
        /// Sets the offset in horizontal direction.
        /// Should normally be negative. Moves the position of the content by the offset.
        /// E.g. a negative offset should move the content to the left and the viewable 
        /// region to the right.
        /// </summary>
        /// <param name="x">The offset in horizontal direction.</param>
        void SetXOffset(int x);
        /// <summary>
        /// Gets the offset in vertical direction.
        /// Should normally be negative. Moves the position of the content by the offset.
        /// E.g. a negative offset should move the content to up and the viewable 
        /// region down.
        /// </summary>
        /// <returns>the vertical offset</returns>
        int GetYOffset();
        /// <summary>
        /// Sets the offset in vertical direction.
        /// Should normally be negative. Moves the position of the content by the offset.
        /// E.g. a negative offset should move the content to up and the viewable
        /// region down.
        /// </summary>
        /// <param name="x">The offset in vertical direction.</param>
        void SetYOffset(int y);
        /// <summary>
        /// Move the content in both direction simultaneous. 
        /// So the vertical and horizontal offset will been set at the same time.
        /// </summary>
        /// <param name="p">The offsets in vertical and horizontal direction to move.</param>
        /// <returns></returns>
        Point Move(Point p);
    }
}
