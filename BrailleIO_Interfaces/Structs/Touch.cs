using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrailleIO.Structs
{
    /// <summary>
    /// A structure for holding informations about touches 
    /// </summary>
    public struct Touch
    {
        /// <summary>
        /// The horizontal position of the center of a 
        /// touch in relation to the display matrix.
        /// </summary>
        public readonly double X;
        /// <summary>
        /// The vertical position of the center of a 
        /// touch in relation to the display matrix.
        /// </summary>
        public readonly double Y;
        /// <summary>
        /// The horizontal pin position of the center of a 
        /// touch in relation to the display matrix.
        /// </summary>
        public readonly int PinX;
        /// <summary>
        /// The vertical position of the center of a 
        /// touch in relation to the display matrix.
        /// </summary>
        public readonly int PinY;
        /// <summary>
        /// The intense sensory value of the detected touch between 0 and 1.
        /// </summary>
        public readonly double Intense;
        /// <summary>
        /// The horizontal diameter of the touch.
        /// </summary>
        public readonly double DimX;
        /// <summary>
        /// The vertical diameter of the touch.
        /// </summary>
        public readonly double DimY;

        /// <summary>
        /// Initializes a new instance of the <see cref="Touch" /> struct.
        /// </summary>
        /// <param name="x">The horizontal position of the center of a
        /// touch in relation to the display matrix.</param>
        /// <param name="y">The vertical position of the center of a
        /// touch in relation to the display matrix.</param>
        /// <param name="intense">The intense sensory value of the detected touch between 0 and 1.</param>
        /// <param name="dimX">The horizontal diameter of the touch.</param>
        /// <param name="dimY">The vertical diameter of the touch.</param>
        public Touch(double x, double y, double intense, double dimX, double dimY)
            : this(x, y, intense)
        {
            DimX = dimX;
            DimY = dimY;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Touch"/> struct.
        /// </summary>
        /// <param name="x">
        /// The horizontal position of the center of a 
        /// touch in relation to the display matrix.
        /// </param>
        /// <param name="y">
        /// The vertical position of the center of a 
        /// touch in relation to the display matrix.
        /// </param>
        /// <param name="intense">The intense sensory value of the detected touch between 0 and 1.</param>
        public Touch(double x, double y, double intense)
        {
            this.X = x;
            PinX = (int)Math.Round(x, 0, MidpointRounding.AwayFromZero);
            this.Y = y;
            PinY = (int)Math.Round(y, 0, MidpointRounding.AwayFromZero);
            this.Intense = Math.Min(1, intense);
            DimX = 0.5;
            DimY = 0.5;

        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "X: " + X + " | Y: " + Y + " (" + Intense + ")";
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Touch)) return false;
            Touch t = (Touch)obj;

            return t.X == X && t.Y == Y && t.Intense == Intense && t.DimX == DimX && t.DimY == DimY && t.PinX == PinX && t.PinY == PinY;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }

}
