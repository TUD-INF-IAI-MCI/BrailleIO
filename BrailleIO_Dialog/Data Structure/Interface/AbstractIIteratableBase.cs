using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrailleIO.Dialogs
{
    /// <summary>
    /// Abstract implementation of the <see cref="BrailleIO.Dialogs.IIteratable"/> interface.
    /// </summary>
    /// <seealso cref="BrailleIO.Dialogs.IIteratable" />
    /// <seealso cref="System.Collections.IEnumerable" />
    public abstract class AbstractIIteratableBase : IIteratable, IEnumerable
    {
        #region Member

        /// <summary>
        /// An object for synchronizing the access to critical parts.
        /// </summary>
        public readonly Object SyncLock = new Object();


        virtual protected List<IDialogComponent> ChildEntries
        {
            get;
            set;
        }

        #endregion

        #region IIteratable

        int _childIteratorIndex = 0;
        /// <summary>
        /// Gets or sets the current index of the iterator.
        /// </summary>
        /// <value>
        /// The current iterator index of the current.
        /// </value>
        virtual public int CurrentIndex
        {
            get
            {
                lock (SyncLock)
                {
                    if (_childIteratorIndex < 0)
                        _childIteratorIndex = 0;
                    else if (ChildEntries != null && _childIteratorIndex >= ChildEntries.Count)
                        _childIteratorIndex = ChildEntries.Count - 1;
                    return _childIteratorIndex;
                }
            }
            set
            {
                lock (SyncLock)
                {
                    _childIteratorIndex = Math.Max(
                Math.Min(value,
                ChildEntries != null ? ChildEntries.Count - 1 : 0),
                0);
                }
            }
        }

        /// <summary>
        /// Get the amount of child elements.
        /// </summary>
        /// <value>
        /// The count of child elements.
        /// </value>
        virtual public int ElementCount
        {
            get
            {
                lock (SyncLock)
                {
                    return HasChildren() ? ChildEntries.Count : 0;
                }
            }
        }

        /// <summary>
        /// Determines whether this instance has child elements.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance has elements; otherwise, <c>false</c>.
        /// </returns>
        virtual public bool HasChildren()
        {
            return ChildEntries != null && ChildEntries.Count > 0;
        }

        #region Next Element

        /// <summary>
        /// Determines whether this instance has a next element.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance has a next element; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        virtual public bool HasNext()
        {
            // if this element is another group etc ...
            if (HasChildren() &&
                ChildEntries[CurrentIndex] is IIteratable &&
                ((IIteratable)ChildEntries[CurrentIndex]).HasChildren())
            {
                return true;
            }
            return HasNext(CurrentIndex);
        }
        /// <summary>
        /// Determines whether this instance has a next element.
        /// </summary>
        /// <param name="_current">The element to be searched from.</param>
        /// <returns>
        ///   <c>true</c> if this instance has a next element; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// Does not set the iterator index to the index of the current element.
        /// </remarks>
        virtual public bool HasNext(IDialogComponent _current)
        {
            if (HasChildren())
            {
                lock (SyncLock)
                {
                    var i = IndexOfElement(_current);
                    if (i >= 0)
                    {
                        // if this element is another group etc ...
                        if (_current is IIteratable &&
                            ((IIteratable)_current).HasChildren())
                        {
                            return true; //((IIteratable)_current).GetNext();
                        }

                        return (HasNext(i));
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// Determines whether the specified current index has next.
        /// </summary>
        /// <param name="_currentIndex">Index of the current.</param>
        /// <returns>
        ///   <c>true</c> if the specified current index has next; otherwise, <c>false</c>.
        /// </returns>
        virtual public bool HasNext(int _currentIndex)
        {
            if (HasChildren())
            {
                // if this element is another group etc ...
                if (ChildEntries[CurrentIndex] is IIteratable &&
                ((IIteratable)ChildEntries[CurrentIndex]).HasChildren())
                {
                    return true;
                }
                else
                {
                    return _currentIndex < (ElementCount - 1);
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the next element.
        /// </summary>
        /// <returns>
        /// The next element if exists or <c>null</c>.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <remarks>
        /// Changes the <see cref="CurrentIndex" /> in case of success.
        /// </remarks>
        virtual public IDialogComponent GetNext()
        {
            if (HasNext())
            {
                lock (SyncLock)
                {
                    try
                    {

                        // if this element is another group etc ...
                        if (ChildEntries[CurrentIndex] is IIteratable)
                        {
                            var next = GetNextOfIIteratableChild(ChildEntries[CurrentIndex] as IIteratable);
                            if (next != null) return next;
                        }

                        CurrentIndex++;
                        return ChildEntries[CurrentIndex];
                    }
                    catch { }
                    finally { }
                }
            }
            return null;
        }
        /// <summary>
        /// Gets the next element, starting from the given element.
        /// </summary>
        /// <param name="_current">The current element to search from.</param>
        /// <returns>
        /// The next element from the given one if exist, or <c>null</c>.
        /// </returns>
        /// <remarks>
        /// Changes the <see cref="CurrentIndex" /> in case of success.
        /// </remarks>
        virtual public IDialogComponent GetNext(IDialogComponent _current)
        {
            if (HasChildren())
            {
                lock (SyncLock)
                {
                    try
                    {
                        var i = IndexOfElement(_current);
                        if (i >= 0)
                        {
                            // if this element is another group etc ...
                            if (_current is IIteratable)
                            {
                                var next = GetNextOfIIteratableChild(_current as IIteratable);
                                if (next != null) return next;
                            }

                            if (i < (ChildEntries.Count - 1))
                            {
                                CurrentIndex = i + 1;
                                return ChildEntries[CurrentIndex];
                            }
                        }
                    }
                    catch { }
                    finally { }
                }
            }
            return null;
        }


        /// <summary>
        /// Gets the next element, starting from the given index.
        /// </summary>
        /// <param name="_currentIndex"></param>
        /// <returns>
        /// The next element from the given index if exist, or <c>null</c>.
        /// </returns>
        /// <remarks>
        /// Changes the <see cref="CurrentIndex" /> in case of success.
        /// </remarks>
        virtual public IDialogComponent GetNext(int _currentIndex)
        {
            if (HasNext(_currentIndex))
            {
                lock (SyncLock)
                {
                    try
                    {
                        CurrentIndex = _currentIndex;

                        // if this element is another group etc ...
                        if (ChildEntries[CurrentIndex] is IIteratable)
                        {
                            var next = GetNextOfIIteratableChild(ChildEntries[CurrentIndex] as IIteratable);
                            if (next != null) return next;
                        }


                        CurrentIndex = _currentIndex + 1;
                        return ChildEntries[CurrentIndex];
                    }
                    catch { }
                    finally { }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the next child of an group.
        /// </summary>
        /// <param name="_current">The current child element which is a group.</param>
        /// <returns>
        /// The next element of the child group if available; 
        /// otherwise the first element if available; 
        /// otherwise <c>null</c>.</returns>
        protected static IDialogComponent GetNextOfIIteratableChild(IIteratable _current)
        {
            if (_current != null && _current.HasChildren())
            {
                var next = _current.GetNext();
                if (next != null)
                    return next;
                else
                {
                    next = _current.GetFirst();
                    if (next != null)
                        return next;
                }
            }
            return null;
        }


        #endregion

        #region Previous Element

        /// <summary>
        /// Determines whether this instance has a previous element.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance has a previous element; otherwise, <c>false</c>.
        /// </returns>
        virtual public bool HasPrevious()
        {
            // if this element is another group etc ...
            if (HasChildren() &&
                ChildEntries[CurrentIndex] is IIteratable &&
                ((IIteratable)ChildEntries[CurrentIndex]).HasChildren())
            {
                return true;
            }
            return HasPrevious(CurrentIndex);
        }
        /// <summary>
        /// Determines whether this instance has a previous element.
        /// </summary>
        /// <param name="_current">The element to be searched from.</param>
        /// <returns>
        ///   <c>true</c> if this instance has a previous element; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// Does not set the iterator index to the index of the current element.
        /// </remarks>
        virtual public bool HasPrevious(IDialogComponent _current)
        {
            if (HasChildren())
            {
                lock (SyncLock)
                {
                    // if this element is another group etc ...
                    if (_current is IIteratable &&
                        ((IIteratable)_current).HasChildren())
                    {
                        return true; //((IIteratable)_current).GetNext();
                    }

                    var i = IndexOfElement(_current);
                    return (HasPrevious(i));
                }
            }
            return false;
        }
        /// <summary>
        /// Determines whether this instance has a previous element.
        /// </summary>
        /// <param name="_currentIndex">Index of the element to be searched from.</param>
        /// <returns>
        ///   <c>true</c> if this instance has a previous element; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// Does not set the iterator index to the index of the current element.
        /// </remarks>
        virtual public bool HasPrevious(int _currentIndex)
        {
            if (HasChildren())
            {
                // if this element is another group etc ...
                if (ChildEntries[CurrentIndex] is IIteratable &&
                ((IIteratable)ChildEntries[CurrentIndex]).HasChildren())
                {
                    return true;
                }
                else
                {
                    return _currentIndex > 0;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the previous element.
        /// </summary>
        /// <returns>
        /// The previous element if exists or <c>null</c>.
        /// </returns>
        /// <remarks>
        /// Changes the <see cref="CurrentIndex" /> in case of success.
        /// </remarks>
        virtual public IDialogComponent GetPrevious()
        {
            if (HasPrevious())
            {
                lock (SyncLock)
                {
                    try
                    {

                        // if this element is another group etc ...
                        if (ChildEntries[CurrentIndex] is IIteratable)
                        {
                            var prev = GetPreviouseOfIIteratableChild(ChildEntries[CurrentIndex] as IIteratable);
                            if (prev != null) return prev;
                        }

                        CurrentIndex--;
                        return ChildEntries[CurrentIndex];
                    }
                    catch { }
                    finally { }
                }
            }
            return null;
        }
        /// <summary>
        /// Gets the next previous, starting from the given element.
        /// </summary>
        /// <param name="_current">The current element to search from.</param>
        /// <returns>
        /// The previous element from the given one if exist, or <c>null</c>.
        /// </returns>
        /// <remarks>
        /// Changes the <see cref="CurrentIndex" /> in case of success.
        /// </remarks>
        virtual public IDialogComponent GetPrevious(IDialogComponent _current)
        {
            if (HasChildren())
            {
                lock (SyncLock)
                {
                    try
                    {
                        var i = IndexOfElement(_current);
                        if (i > 0)
                        {
                            CurrentIndex = i;

                            // if this element is another group etc ...
                            if (ChildEntries[CurrentIndex] is IIteratable)
                            {
                                var prev = GetPreviouseOfIIteratableChild(ChildEntries[CurrentIndex] as IIteratable);
                                if (prev != null) return prev;
                            }
                        }

                        if (i > 1)
                        {
                            CurrentIndex = i - 1;
                            return ChildEntries[i];
                        }
                    }
                    catch { }
                    finally { }
                }
            }
            return null;
        }
        /// <summary>
        /// Gets the previous element, starting from the given index.
        /// </summary>
        /// <param name="_currentIndex"></param>
        /// <returns>
        /// The previous element from the given index if exist, or <c>null</c>.
        /// </returns>
        /// <remarks>
        /// Changes the <see cref="CurrentIndex" /> in case of success.
        /// </remarks>
        virtual public IDialogComponent GetPrevious(int _currentIndex)
        {
            if (HasPrevious(_currentIndex))
            {
                lock (SyncLock)
                {
                    try
                    {
                        CurrentIndex = _currentIndex;

                        // if this element is another group etc ...
                        if (ChildEntries[CurrentIndex] is IIteratable)
                        {
                            var prev = GetPreviouseOfIIteratableChild(ChildEntries[CurrentIndex] as IIteratable);
                            if (prev != null) return prev;
                        }


                        CurrentIndex = _currentIndex - 1;
                        return ChildEntries[CurrentIndex];
                    }
                    catch { }
                    finally { }
                }
            }
            return null;
        }


        /// <summary>
        /// Gets the previous child of an group.
        /// </summary>
        /// <param name="_current">The current child element which is a group.</param>
        /// <returns>
        /// The previous element of the child group if available; 
        /// otherwise the last element if available; 
        /// otherwise <c>null</c>.</returns>
        protected static IDialogComponent GetPreviouseOfIIteratableChild(IIteratable _current)
        {
            if (_current != null && _current.HasChildren())
            {
                var prev = _current.GetPrevious();
                if (prev != null)
                    return prev;
                else
                {
                    prev = _current.GetLast();
                    if (prev != null)
                        return prev;
                }
            }
            return null;
        }

        #endregion

        #region Helper Functions

        /// <summary>
        /// Gets the first element.
        /// </summary>
        /// <returns>
        /// The first element if exist; otherwise <c>null</c>
        /// </returns>
        /// <remarks>
        /// resets the <see cref="CurrentIndex" /> to 0.
        /// </remarks>
        virtual public IDialogComponent GetFirst()
        {
            if (HasChildren())
            {
                lock (SyncLock)
                {
                    try
                    {
                        CurrentIndex = 0;

                        // if this element is another group etc ...
                        if (ChildEntries[CurrentIndex] is IIteratable)
                        {
                            var first = GetFirstOfIIteratableChild(ChildEntries[CurrentIndex] as IIteratable);
                            if (first != null) return first;
                        }

                        return ChildEntries[CurrentIndex];
                    }
                    catch { }
                    finally { }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the last element.
        /// </summary>
        /// <returns>
        /// The last element if exist; otherwise <c>null</c>
        /// </returns>
        /// <remarks>
        /// resets the <see cref="CurrentIndex" /> to the maximum index.
        /// </remarks>
        virtual public IDialogComponent GetLast()
        {
            if (HasChildren())
            {
                lock (SyncLock)
                {
                    try
                    {
                        CurrentIndex = ElementCount - 1;

                        // if this element is another group etc ...
                        if (ChildEntries[CurrentIndex] is IIteratable)
                        {
                            var last = GetLastOfIIteratableChild(ChildEntries[CurrentIndex] as IIteratable);
                            if (last != null) return last;
                        }

                        return ChildEntries[CurrentIndex];
                    }
                    catch { }
                    finally { }
                }
            }
            return null;
        }

        /// <summary>
        /// Indexes the of element.
        /// </summary>
        /// <param name="_current">The current to search it's index.</param>
        /// <returns>
        /// The index of the element inside the list if it is contained; otherwise -1.
        /// </returns>
        virtual public int IndexOfElement(IDialogComponent _current)
        {
            if (_current != null && HasChildren())
            {
                lock (SyncLock)
                {
                    try
                    {
                        for (int i = 0; i < ChildEntries.Count; i++)
                        {
                            if (ChildEntries[i] != null && ChildEntries[i] == _current)
                            {
                                return i;
                            }
                        }
                    }
                    catch { }
                    finally { }
                }
            }

            return -1;
        }

        /// <summary>
        /// Gets the first child of an group.
        /// </summary>
        /// <param name="_current">The current child element which is a group.</param>
        /// <returns>
        /// The first element of the child group if available; 
        /// otherwise <c>null</c>.</returns>
        protected static IDialogComponent GetFirstOfIIteratableChild(IIteratable _current)
        {
            if (_current != null && _current.HasChildren())
            {
                return _current.GetFirst();
            }
            return null;
        }

        /// <summary>
        /// Gets the first child of an group.
        /// </summary>
        /// <param name="_current">The current child element which is a group.</param>
        /// <returns>
        /// The first element of the child group if available; 
        /// otherwise <c>null</c>.</returns>
        protected static IDialogComponent GetLastOfIIteratableChild(IIteratable _current)
        {
            if (_current != null && _current.HasChildren())
            {
                return _current.GetLast();
            }
            return null;
        }

        /// <summary>
        /// Searches for a child in the entries and child entries of 
        /// an entry for an entry with the given status.
        /// </summary>
        /// <param name="statusFlag">The status flag to search for.</param>
        /// <returns>The first entry having set this flag or <c>null</c>.</returns>
        protected DialogEntry getChildElementByStatus(DialogEntryStatus statusFlag)
        {
            // TODO: handle children of groups ...

            if (HasChildren())
            {
                {
                    lock (SyncLock)
                    {
                        try
                        {
                            foreach (var item in ChildEntries)
                            {
                                if (item != null && item is DialogEntry)
                                {
                                    if (((DialogEntry)item).Status.HasFlag(statusFlag))
                                        return item as DialogEntry;

                                    // child handling
                                    // if this element is another group etc ...
                                    if (((DialogEntry)item).Type == DialogEntryType.Group)
                                    {
                                        if (item is Group_DialogEntry)
                                        {
                                            return ((Group_DialogEntry)item).GetActiveGroupEntry();
                                        }
                                        else
                                        {
                                            foreach (var childItem in ((DialogEntry)item).GetChildEntryList())
                                            {
                                                if (childItem != null &&
                                                    childItem is DialogEntry &&
                                                    ((DialogEntry)childItem).Status.HasFlag(statusFlag))
                                                    return childItem as DialogEntry;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                }
            }
            return null;
        }


        #endregion

        #endregion

        #region IEnumerator

        /// <summary>
        /// Gets the child <see cref="IDialogComponent"/> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="IDialogComponent"/>.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns>the child entry at the specific index.</returns>
        public IDialogComponent this[int index]
        {
            get
            {
                return ChildEntries[index];
            }
        }

        /// <summary>
        /// Exposes an enumerator, which supports a simple iteration over a non-generic collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.IEnumerator" />-object.
        /// </returns>
        virtual public System.Collections.IEnumerator GetEnumerator()
        {
            if (ChildEntries != null)
                return ChildEntries.GetEnumerator();
            return null;
        }

        #endregion


    }
}
