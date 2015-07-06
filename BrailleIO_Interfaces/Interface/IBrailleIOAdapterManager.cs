
namespace BrailleIO.Interface
{
    public interface IBrailleIOAdapterManager
    {
        /// <summary>
        /// Gets or sets the active adapter.
        /// This is the main Adapter the output is sent to.
        /// At least one Adapter has to be active. Otherwise no rendering will happen.
        /// </summary>
        /// <value>The active adapter.</value>
        IBrailleIOAdapter ActiveAdapter
        {
            get;
            set;
        }

        /// <summary>
        /// Adds an adapter to the list of available adapters.
        /// </summary>
        /// <param name="adapter">The adapter.</param>
        /// <returns>if the Adapter could been added or not</returns>
        bool AddAdapter(IBrailleIOAdapter adapter);
        /// <summary>
        /// Removes an adapter form the list of available adapters.
        /// </summary>
        /// <param name="adapter">The adapter to remove.</param>
        /// <returns>if the specific adapter could been removed or not</returns>
        bool RemoveAdapter(IBrailleIOAdapter adapter);
        /// <summary>
        /// Gets a list of all registered adapters.
        /// </summary>
        /// <returns>List of available adapters</returns>
        IBrailleIOAdapter[] GetAdapters();
        /// <summary>
        /// Synchronizes the specified matrix with the Adapters.
        /// That means the given matrix will been send to the adapters.
        /// At least the active Adapter should display the matrix. Adapters that are able to set a "Synch" flag also should mirrow (display) the matrix.
        /// </summary>
        /// <param name="matrix">The matrix to show on the output device.</param>
        /// <returns><c>true</c> if the matrix could been displayed successfully on the output device</returns>
        bool Synchronize(bool[,] matrix);

    }
}
