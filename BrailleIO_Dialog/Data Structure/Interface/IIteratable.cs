using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrailleIO.Dialogs
{
    /// <summary>
    /// Instances that implement this interface allows for easy handling 
    /// common child/member list operations.
    /// </summary>
    /// <seealso cref="System.Collections.IEnumerable" />
    public interface IIteratable : IEnumerable
    {
        /// <summary>
        /// Gets or sets the current index of the iterator.
        /// </summary>
        /// <value>
        /// The current iterator index of the current.
        /// </value>
        int CurrentIndex { get; set; }
        /// <summary>
        /// Get the amount of child elements.
        /// </summary>
        /// <value>The count of child elements.</value>
        int ElementCount { get; }

        /// <summary>
        /// Determines whether this instance has child elements.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance has elements; otherwise, <c>false</c>.
        /// </returns>
        bool HasChildren();


        /// <summary>
        /// Determines whether this instance has a next element.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance has a next element; otherwise, <c>false</c>.
        /// </returns>
        bool HasNext();
        /// <summary>
        /// Determines whether this instance has a next element.
        /// </summary>
        /// <param name="_current">The element to be searched from.</param>
        /// <returns>
        ///   <c>true</c> if this instance has a next element; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>Does not set the iterator index to the index of the current element.</remarks>
        bool HasNext(IDialogComponent _current);
        /// <summary>
        /// Determines whether this instance has a next element.
        /// </summary>
        /// <param name="_currentIndex">Index of the element to be searched from.</param>
        /// <returns>
        ///   <c>true</c> if this instance has a next element; otherwise, <c>false</c>.
        /// </return>
        /// <remarks>Does not set the iterator index to the index of the current element.</remarks>
        bool HasNext(int _currentIndex);

        /// <summary>
        /// Determines whether this instance has a previous element.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance has a previous element; otherwise, <c>false</c>.
        /// </returns>
        bool HasPrevious();
        /// <summary>
        /// Determines whether this instance has a previous element.
        /// </summary>
        /// <param name="_current">The element to be searched from.</param>
        /// <returns>
        ///   <c>true</c> if this instance has a previous element; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>Does not set the iterator index to the index of the current element.</remarks>
        bool HasPrevious(IDialogComponent _current);
        /// <summary>
        /// Determines whether this instance has a previous element.
        /// </summary>
        /// <param name="_currentIndex">Index of the element to be searched from.</param>
        /// <returns>
        ///   <c>true</c> if this instance has a previous element; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>Does not set the iterator index to the index of the current element.</remarks>
        bool HasPrevious(int _currentIndex);

        /// <summary>
        /// Gets the next element.
        /// </summary>
        /// <returns>The next element if exists or <c>null</c>.</returns>
        /// <remarks>Changes the <see cref="CurrentIndex"/> in case of success.</remarks>
        IDialogComponent GetNext();
        /// <summary>
        /// Gets the next element, starting from the given element.
        /// </summary>
        /// <param name="_current">The current element to search from.</param>
        /// <returns>The next element from the given one if exist, or <c>null</c>.</returns>
        /// <remarks>Changes the <see cref="CurrentIndex"/> in case of success.</remarks>
        IDialogComponent GetNext(IDialogComponent _current);
        /// <summary>
        /// Gets the next element, starting from the given index.
        /// </summary>
        /// <param name="_current">The current index to search from.</param>
        /// <returns>The next element from the given index if exist, or <c>null</c>.</returns>
        /// <remarks>Changes the <see cref="CurrentIndex"/> in case of success.</remarks>
        IDialogComponent GetNext(int _currentIndex);

        /// <summary>
        /// Gets the previous element.
        /// </summary>
        /// <returns>The previous element if exists or <c>null</c>.</returns>
        /// <remarks>Changes the <see cref="CurrentIndex"/> in case of success.</remarks>
        IDialogComponent GetPrevious();
        /// <summary>
        /// Gets the next previous, starting from the given element.
        /// </summary>
        /// <param name="_current">The current element to search from.</param>
        /// <returns>The previous element from the given one if exist, or <c>null</c>.</returns>
        /// <remarks>Changes the <see cref="CurrentIndex"/> in case of success.</remarks>
        IDialogComponent GetPrevious(IDialogComponent _current);
        /// <summary>
        /// Gets the previous element, starting from the given index.
        /// </summary>
        /// <param name="_current">The current index to search from.</param>
        /// <returns>The previous element from the given index if exist, or <c>null</c>.</returns>
        /// <remarks>Changes the <see cref="CurrentIndex"/> in case of success.</remarks>
        IDialogComponent GetPrevious(int _currentIndex);

        /// <summary>
        /// Gets the first element.
        /// </summary>
        /// <returns>The first element if exist; otherwise <c>null</c></returns>
        /// <remarks>resets the <see cref="CurrentIndex"/> to 0.</remarks>
        IDialogComponent GetFirst();
        /// <summary>
        /// Gets the last element.
        /// </summary>
        /// <returns>The last element if exist; otherwise <c>null</c></returns>
        /// <remarks>resets the <see cref="CurrentIndex"/> to the maximum index.</remarks>
        IDialogComponent GetLast();

        /// <summary>
        /// Indexes the of element.
        /// </summary>
        /// <param name="_current">The current to search it's index.</param>
        /// <returns>The index of the element inside the list if it is contained; otherwise -1.</returns>
        int IndexOfElement(IDialogComponent _current);

    }
}
