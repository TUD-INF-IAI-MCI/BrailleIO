using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrailleIO.Interface
{
    /// <summary>
    /// Supplier for content changed events.
    /// </summary>
    public interface IBrailleIOContentChangedEventSupplier
    {
        /// <summary>
        /// Occurs when the content has been changed.
        /// </summary>
        event EventHandler<EventArgs> ContentChanged;
    }
}
