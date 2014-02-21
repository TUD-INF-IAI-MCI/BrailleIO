using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrailleIO.Structs
{
    public struct BoxModel
    {
        public uint Top;
        public uint Bottom;
        public uint Left;
        public uint Right;

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "Box: " + Top + "," + Right + "," + Bottom + "," + Left;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return (obj is BoxModel) && (Top == ((BoxModel)obj).Top) && (Right == ((BoxModel)obj).Right) && (Bottom == ((BoxModel)obj).Bottom) && (Left == ((BoxModel)obj).Left);
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public BoxModel(uint top, uint right, uint bottom, uint left)
        {
            Top = top; Right = right; Bottom = bottom; Left = left;
        }
        public BoxModel(uint top, uint horizontal, uint bottom) : this(top, horizontal, bottom, horizontal) { }
        public BoxModel(uint vertical, uint horizontal) : this(vertical, horizontal, vertical) { }
        public BoxModel(uint width) : this(width, width) { }

        public void Clear() { Top = Right = Bottom = Left = 0; }
        public bool HasBox() { return (Top > 0) || (Right > 0) || (Bottom > 0) || (Left > 0); }
    }
}
