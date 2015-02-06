using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using BrailleIO.Interface;

namespace BrailleIO
{
    abstract public class AbstractBrailleIOAdapterManagerBase : IBrailleIOAdapterManager
    {
        IBrailleIOAdapter _activeAdapter;
        /// <summary>
        /// initialize all supported devices and wait for connection.
        /// </summary>
        public IBrailleIOAdapter ActiveAdapter
        {
            get { return _activeAdapter; }
            set { AddAdapter(value); _activeAdapter = value; }
        }
        private Object _adapterLock = new Object();
        private ConcurrentBag<IBrailleIOAdapter> _adapters = new ConcurrentBag<IBrailleIOAdapter>();
        protected ConcurrentBag<IBrailleIOAdapter> Adapters
        {
            get
            {
                lock (_adapterLock) { return _adapters; }
            }
        }


        protected BrailleIOMediator io;
        public AbstractBrailleIOAdapterManagerBase()
        {}

        public AbstractBrailleIOAdapterManagerBase(ref BrailleIOMediator io)
            : this()
        {
            this.io = io;
        }

        /// <summary>
        /// Adds a new adapter to the manager.
        /// </summary>
        /// <param name="adapter">The adapter.</param>
        /// <returns>True if the adapter could be added to the manager otherwise false. It also returns false if the adapter is already added.</returns>
        public virtual bool AddAdapter(IBrailleIOAdapter adapter)
        {
            if (Adapters.Contains(adapter)) return false;
            try
            {
                Adapters.Add(adapter);
            }
            catch { return false; }
            return true;
        }

        /// <summary>
        /// Removes a new adapter from the manager.
        /// </summary>
        /// <param name="adapter">The adapter.</param>
        /// <returns>True if the adapter could be removed from the manager otherwise false.</returns>
        public virtual bool RemoveAdapter(IBrailleIOAdapter adapter)
        {
            if (Adapters.Contains(adapter))
            {
                try
                {
                    Adapters.TryTake(out adapter);
                    return true;
                }
                catch { }
            }
            return false;
        }

        /// <summary>
        /// Gets the adapters.
        /// </summary>
        /// <returns></returns>
        public virtual IBrailleIOAdapter[] GetAdapters()
        {
            return Adapters.ToArray();
        }

        /// <summary>
        /// Synchronizes the specified matrix.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns></returns>
        public bool Synchronize(bool[,] matrix)
        {
            ActiveAdapter.Synchronize(matrix);
            foreach (var item in Adapters)
            {
                if (item != null && item != ActiveAdapter && item is AbstractBrailleIOAdapterBase && ((AbstractBrailleIOAdapterBase)item).Synch)
                    item.Synchronize(matrix);
            }
            return false;
        }


    }

    public class BasicBrailleIOAdapterManager : AbstractBrailleIOAdapterManagerBase { }
}
