/// <summary>
/// Collection of Structs for the Braille I/O framework
/// </summary>
namespace BrailleIO.Structs
{
    /// <summary>
    /// A Struct wrapping the four dimensions of a box model member
    /// </summary>
		/// <remarks> </remarks>
    public struct BoxModel
    {
        /// <summary>
        /// dimension/width to the top
        /// </summary>
		/// <remarks> </remarks>
        public uint Top;
        /// <summary>
        /// dimension/width to the bottom
        /// </summary>
		/// <remarks> </remarks>
        public uint Bottom;
        /// <summary>
        /// dimension/width to the left
        /// </summary>
		/// <remarks> </remarks>
        public uint Left;
        /// <summary>
        /// dimension/width to the right
        /// </summary>
		/// <remarks> </remarks>
        public uint Right;

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
		/// <remarks> </remarks>
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
		/// <remarks> </remarks>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return (obj is BoxModel) && (Top == ((BoxModel)obj).Top) && (Right == ((BoxModel)obj).Right) && (Bottom == ((BoxModel)obj).Bottom) && (Left == ((BoxModel)obj).Left);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
		/// <remarks> </remarks>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode() { return base.GetHashCode(); }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoxModel"/> struct.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="top">The dimension/width to the top.</param>
        /// <param name="right">The dimension/width to the right.</param>
        /// <param name="bottom">The dimension/width to the bottom.</param>
        /// <param name="left">The dimension/width to the left.</param>
        public BoxModel(uint top, uint right, uint bottom, uint left)
        {
            Top = top; Right = right; Bottom = bottom; Left = left;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="BoxModel"/> struct.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="top">The dimension/width to the top.</param>
        /// <param name="horizontal">The dimension/width to the horizontal values (left + right).</param>
        /// <param name="bottom">The dimension/width to the bottom.</param>
        public BoxModel(uint top, uint horizontal, uint bottom) : this(top, horizontal, bottom, horizontal) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="BoxModel"/> struct.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="vertical">The dimension/width to the vertical values (top + bottom).</param>
        /// <param name="horizontal">The dimension/width to the horizontal values (left + right).</param>
        public BoxModel(uint vertical, uint horizontal) : this(vertical, horizontal, vertical) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="BoxModel"/> struct.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="width">The dimension/width to all values (top, bottom, left and right).</param>
        public BoxModel(uint width) : this(width, width) { }

        /// <summary>
        /// Set all dimension/width to 0
        /// </summary>
		/// <remarks> </remarks>
        public void Clear() { Top = Right = Bottom = Left = 0; }
        /// <summary>
        /// Determines whether this instance has box Which means that at least one dimension is larger than 0.
        /// </summary>
		/// <remarks> </remarks>
        /// <returns>
        /// 	<c>true</c> if this instance has box; otherwise, <c>false</c>.
        /// </returns>
        public bool HasBox() { return (Top > 0) || (Right > 0) || (Bottom > 0) || (Left > 0); }
    }
}
