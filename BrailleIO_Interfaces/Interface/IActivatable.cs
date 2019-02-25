using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Collection of interfaces for the Braille I/O framework
/// </summary>
namespace BrailleIO.Interface
{
    /// <summary>
    /// Interface for objects which can toggle their activation status
    /// </summary>
    public interface IActivatable
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IActivatable"/> is active or not.
        /// </summary>
        /// <value>
        ///   <c>true</c> if active; otherwise, <c>false</c>.
        /// </value>
        bool Active {get; set;}
    }
}
