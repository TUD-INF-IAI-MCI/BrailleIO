using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrailleIO.Interface
{
    interface IBrailleIOContentChangedEventSupplier
    {
        event EventHandler<EventArgs> ContentChanged;
    }
}
