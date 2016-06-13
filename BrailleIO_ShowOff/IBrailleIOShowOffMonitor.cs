using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrailleIO.Interface;
using System.Drawing;

namespace BrailleIO
{
    /// <summary>
    /// A interface for monitoring other adapterse.g. for debugging or monitoring / mirroring.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IBrailleIOShowOffMonitor : IDisposable
    {
        /// <summary>
        /// Initializes the BrailleIO framework. Build a new BrailleIOAdapter_ShowOff, and add it to the IBrailleIOAdapterManager.
        /// </summary>
        /// <param name="adapterManager">The adapter manager to use for managing devices.</param>
        /// <param name="setAsActiveAdapter">if set to <c>true</c> [set this as active adapter].</param>
        /// <returns>
        /// The created BrailleIOAdapter_ShowOff, that was build with this instance
        /// </returns>
        AbstractBrailleIOAdapterBase InitializeBrailleIO(IBrailleIOAdapterManager adapterManager, bool setAsActiveAdapter = false);

        /// <summary>
        /// Initializes the BrailleIO framework. Build a new BrailleIOAdapter_ShowOff, and add it to the global IBrailleIOAdapterManager.
        /// </summary>
        /// <returns>The created BrailleIOAdapter_ShowOff, that was build with this instance</returns>
        AbstractBrailleIOAdapterBase InitializeBrailleIO(bool setAsActiveAdapter = false);

        /// <summary>
        /// creates a new <see cref="BrailleIOAdapter_ShowOff"/> and returns it
        /// </summary>
        /// <param name="manager">the corresponding adapter manager</param>
        /// <returns>a new "BrailleIOAdapter_ShowOff adapter</returns>
        AbstractBrailleIOAdapterBase GetAdapter(IBrailleIOAdapterManager manager);

        /// <summary>
        /// Paints the touch matrix over the matrix image.
        /// </summary>
        /// <param name="touchMatrix">The touch matrix.</param>
        void PaintTouchMatrix(double[,] touchMatrix);

        /// <summary>
        /// Sets an overlay picture will be displayed as topmost 
        /// - so beware to use a transparent background when using this 
        /// overlay functionality.
        /// </summary>
        /// <param name="image">The image to be displayed as an overlay.</param>
        /// <returns><c>true</c> if the image could been set, otherwise <c>false</c></returns>
        bool SetPictureOverlay(Image image);
        
        /// <summary>
        /// Gets the current overlay image.
        /// </summary>
        /// <returns>the current set overlay image or <c>null</c></returns>
        Image GetPictureOverlay();

        /// <summary>
        /// Gets the size of the picture overlay image.
        /// </summary>
        /// <value>The size of the overlay image.</value>
        Size PictureOverlaySize{get;}

        /// <summary>
        /// Sets the text in the status bar.
        /// </summary>
        /// <param name="text">The text to display in the status bar.</param>
        void SetStatusText(string text);

        /// <summary>
        /// Resets the text in the status bar.
        /// </summary>
        void ResetStatusText();
        
        /// <summary>
        /// Marks the button as pressed.
        /// </summary>
        /// <param name="pressedButtons">The pressed buttons.</param>
        void MarkButtonAsPressed(List<String> pressedButtons);

        /// <summary>
        /// Reset the buttons to normal mode.
        /// </summary>
        /// <param name="releasedButtons">The released buttons.</param>
        void UnmarkButtons(List<String> releasedButtons);

        /// <summary>
        /// Paints the specified matrix to the GUI.
        /// </summary>
        /// <param name="m">The pin matrix.</param>
        void Paint(bool[,] m);

        /// <summary>
        /// Occurs when this instance was disposed.
        /// </summary>
        event EventHandler Disposed;
    }
}
