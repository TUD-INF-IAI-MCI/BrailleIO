using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace BrailleRenderer
{
    public interface ITouchableRenderer
    {

        /// <summary>
        /// Gets the Object at position x,y in the content.
        /// </summary>
        /// <param name="x">The x position in the content matrix.</param>
        /// <param name="y">The y position in the content matrix.</param>
        /// <returns>An object at the requester position in the content or <c>null</c></returns>
        Object GetContentAtPosition(int x, int y);

        /// <summary>
        /// Get all Objects inside (or at least partial) the given area.
        /// </summary>
        /// <param name="left">Left border of the region to test (X).</param>
        /// <param name="right">Right border of the region to test (X + width).</param>
        /// <param name="top">Top border of the region to test (Y).</param>
        /// <param name="bottom">Bottom border of the region to test (Y + heigh).</param>
        /// <returns>A list of elements inside or at least partial inside the requested area.</returns>
        IList GetAllContentInArea(int left, int right, int top, int bottom);
    }
}
