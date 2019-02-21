using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrailleIO.Interface
{
    /// <summary>
    /// Supplier for content changed events.
    /// </summary>
		/// <remarks> </remarks>
    public interface IBrailleIOContentChangedEventSupplier
    {
        /// <summary>
        /// Occurs when the content has been changed.
        /// </summary>
		/// <remarks> </remarks>
        event EventHandler<EventArgs> ContentChanged;
    }
}
