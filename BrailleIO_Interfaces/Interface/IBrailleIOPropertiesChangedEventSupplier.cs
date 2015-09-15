using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrailleIO.Interface
{
    public interface IBrailleIOPropertiesChangedEventSupplier
    {
        /// <summary>
        /// Occurs when a property has changed.
        /// </summary>
        event EventHandler<BrailleIOPropertyChangedEventArgs> PropertyChanged;
    }

    public class BrailleIOPropertyChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The name of the changed property
        /// </summary>
        public readonly String PropertyName = String.Empty;

        public BrailleIOPropertyChangedEventArgs(string propertyName)
        {
            PropertyName = propertyName;
        }
    } 
}
