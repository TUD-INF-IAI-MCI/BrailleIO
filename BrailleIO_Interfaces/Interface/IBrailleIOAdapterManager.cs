
using System;
namespace BrailleIO.Interface
{
    /// <summary>
    /// Interface an manager for in- and output adapters have to implement.
    /// </summary>
		/// <remarks> </remarks>
    public interface IBrailleIOAdapterManager
    {
        /// <summary>
        /// Gets or sets the active adapter.
        /// This is the main Adapter the output is sent to.
        /// At least one Adapter has to be active. Otherwise no rendering will happen.
        /// </summary>
		/// <remarks> </remarks>
        /// <value>The active adapter.</value>
        IBrailleIOAdapter ActiveAdapter
        {
            get;
            set;
        }

        /// <summary>
        /// Adds an adapter to the list of available adapters.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="adapter">The adapter.</param>
        /// <returns>if the Adapter could been added or not</returns>
        bool AddAdapter(IBrailleIOAdapter adapter);
        /// <summary>
        /// Removes an adapter form the list of available adapters.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="adapter">The adapter to remove.</param>
        /// <returns>if the specific adapter could been removed or not</returns>
        bool RemoveAdapter(IBrailleIOAdapter adapter);
        /// <summary>
        /// Gets a list of all registered adapters.
        /// </summary>
		/// <remarks> </remarks>
        /// <returns>List of available adapters</returns>
        IBrailleIOAdapter[] GetAdapters();
        /// <summary>
        /// Synchronizes the specified matrix with the Adapters.
        /// That means the given matrix will been send to the adapters.
        /// At least the active Adapter should display the matrix. Adapters that are able to set a "Synch" flag also should mirrow (display) the matrix.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="matrix">The matrix to show on the output device.</param>
        /// <returns><c>true</c> if the matrix could been displayed successfully on the output device</returns>
        bool Synchronize(bool[,] matrix);

        /// <summary>
        /// Occurs when a new adapter was registered.
        /// </summary>
		/// <remarks> </remarks>
        event EventHandler<IBrailleIOAdapterEventArgs> NewAdapterRegistered;
        /// <summary>
        /// Occurs when an adapter was removed.
        /// </summary>
		/// <remarks> </remarks>
        event EventHandler<IBrailleIOAdapterEventArgs> AdapterRemoved;

        /// <summary>
        /// Occurs when the active adapter changed.
        /// </summary>
		/// <remarks> </remarks>
        event EventHandler<IBrailleIOAdapterEventArgs> ActiveAdapterChanged;
    }

    /// <summary>
    /// Event arguments for submitting <see cref="IBrailleIOAdapter"/> to event handler.
    /// </summary>
		/// <remarks> </remarks>
    /// <seealso cref="System.EventArgs" />
    public class IBrailleIOAdapterEventArgs : EventArgs
    {
        /// <summary>
        /// The adapter raising this event.
        /// </summary>
		/// <remarks> </remarks>
        public readonly IBrailleIOAdapter Adapter;

        /// <summary>
        /// Initializes a new instance of the <see cref="IBrailleIOAdapterEventArgs"/> class.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="adapter">The adapter.</param>
        public IBrailleIOAdapterEventArgs(IBrailleIOAdapter adapter)
        {
            Adapter = adapter;
        }
    }

}
