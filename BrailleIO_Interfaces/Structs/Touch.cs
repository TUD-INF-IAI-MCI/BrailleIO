using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrailleIO.Structs
{
    /// <summary>
    /// A structure for holding informations about touches 
    /// </summary>
    public class Touch
    {
        private int _id = int.MinValue;
        /// <summary>The identifier</summary>
        /// <value>The identifier.</value>
        public int ID
        {
            get
            {
                if (_id == int.MinValue)
                {
                    _id = (int)(DateTime.Now.Ticks % int.MaxValue);
                }
                return _id;
            }
            set { _id = value; }
        }

        private double _x = -1;
        /// <summary>The horizontal position of the center of a
        /// touch in relation to the display matrix.</summary>
        /// <value>The x.</value>
        public double X
        {
            get { return _x; }
            private set
            {
                _x = value;
                int px = (int)Math.Round(_x, 0, MidpointRounding.AwayFromZero);
                if (PinX != px) PinX = px;
            }
        }

        private double _y = -1;
        /// <summary>The vertical position of the center of a
        /// touch in relation to the display matrix.</summary>
        /// <value>The y.</value>
        public double Y
        {
            get { return _y; }
            private set
            {
                _y = value;
                int py = (int)Math.Round(_y, 0, MidpointRounding.AwayFromZero);
                if (PinY != py) PinY = py;
            }
        }

        private int _pinX = -1;
        /// <summary>The horizontal pin position of the center of a
        /// touch in relation to the display matrix.</summary>
        /// <value>The pin x.</value>
        public int PinX
        {
            get { return _pinX; }
            private set
            {
                _pinX = value;
                if (X < _pinX - 0.5 || X > _pinX + 0.5) X = _pinX;
            }
        }

        private int _pinY = -1;
        /// <summary>The vertical position of the center of a
        /// touch in relation to the display matrix.</summary>
        /// <value>The pin y.</value>
        public int PinY
        {
            get { return _pinY; }
            private set
            {
                _pinY = value;
                if (Y < _pinY - 0.5 || Y > _pinY + 0.5) Y = _pinY;
            }
        }

        private double _intense = 0;
        /// <summary>The intense sensory value of the detected touch between 0 and 1.</summary>
        /// <value>The intense of the touch.</value>
        public double Intense
        {
            get { return _intense; }
            private set { _intense = value; }
        }

        private double _dimX = 0.5;
        /// <summary>The horizontal diameter of the touch.</summary>
        /// <value>The horizontal diameter.</value>
        public double DimX
        {
            get { return _dimX; }
            private set { _dimX = value; }
        }

        private double _dimY = 0.5;
        /// <summary>The vertical diameter of the touch.</summary>
        /// <value>The vertical diameter.</value>
        public double DimY
        {
            get { return _dimY; }
            private set { _dimY = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Touch"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="x">The horizontal x position of the touch.</param>
        /// <param name="y">The vertical y position of the touch.</param>
        /// <param name="cx">The horizontal diameter of the touch.</param>
        /// <param name="cy">The vertical diameter of the touch.</param>
        /// <param name="intense">The intense of the sensory data.</param>
        public Touch(int id, double x, double y, double cx, double cy, double intense)
            : this(x, y, cx, cy, intense)
        {
            ID = id;
        }

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
            ID = this.GetHashCode();

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
