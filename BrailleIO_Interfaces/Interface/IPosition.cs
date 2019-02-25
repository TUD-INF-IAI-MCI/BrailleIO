namespace BrailleIO.Interface
{
    /// <summary>
    /// Enables an implementing instance to enable access to there position properties
    /// </summary>
    /// <remarks> </remarks>
    public interface IPosition
    {
        /// <summary>
        /// Sets the horizontal, left start position left ==> The X position.
        /// </summary>
        /// <param name="left">The left.</param>
        void SetLeft(int left);
        /// <summary>
        /// Gets the horizontal, left start position ==> the X position.
        /// </summary>
        /// <returns>The position of the left side in pins (X)</returns>
        int GetLeft();
        /// <summary>
        /// Sets the vertical, top position 00> the Y position.
        /// </summary>
        /// <param name="top">The top.</param>
        void SetTop(int top);
        /// <summary>
        /// Gets the vertical, top position ==> the Y position./// </summary>
        /// <returns>The position from the top in pins (Y)</returns>
        int GetTop();
        /// <summary>
        /// Sets the width.
        /// </summary>
        /// <param name="width">The width.</param>
        void SetWidth(int width);
        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <returns>the width</returns>
        int GetWidth();
        /// <summary>
        /// Sets the height.
        /// </summary>
        /// <param name="height">The height.</param>
        void SetHeight(int height);
        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <returns>the height</returns>
        int GetHeight();
    }
}
