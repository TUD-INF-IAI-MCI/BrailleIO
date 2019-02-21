using BrailleIO.Structs;
using System.Collections.Generic;

namespace BrailleIO.Interface
{
    /// <summary>
    /// Interface for requesting the current touch data status beside event notification.
    /// </summary>
		/// <remarks> </remarks>
    public interface ITouchDataAdapter
    {
        /// <summary>
        /// return the current touch data matrix
        /// </summary>
		/// <remarks> </remarks>
        /// <returns>the current touch data sensory matrix</returns>
        double[,] GetCurrentTouchDataMatrix();

        /// <summary>
        /// return the current touches.
        /// </summary>
		/// <remarks> </remarks>
        /// <returns>List of currently active touches</returns>
        List<Touch> GetCurrentTouchData();

    }
}
