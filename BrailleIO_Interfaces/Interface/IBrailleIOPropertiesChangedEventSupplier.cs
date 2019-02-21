using System;

namespace BrailleIO.Interface
{
    /// <summary>
    /// Event supplier for property changed events.
    /// </summary>
    /// <remarks> </remarks>
    public interface IBrailleIOPropertiesChangedEventSupplier
    {
        /// <summary>
        /// Occurs when a property has changed.
        /// </summary>
		/// <remarks> </remarks>
        event EventHandler<BrailleIOPropertyChangedEventArgs> PropertyChanged;
    }

    /// <summary>
    /// Event arguments for changed properties.
    /// </summary>
    /// <remarks> </remarks>
    /// <seealso cref="System.EventArgs" />
    public class BrailleIOPropertyChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The name of the changed property
        /// </summary>
		/// <remarks> </remarks>
        public readonly String PropertyName = String.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrailleIOPropertyChangedEventArgs"/> class.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="propertyName">Name of the property that has changed.</param>
        public BrailleIOPropertyChangedEventArgs(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}
