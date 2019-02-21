using System;
using System.Collections.Concurrent;
using System.Linq;
using BrailleIO.Interface;

namespace BrailleIO
{
    /// <summary>
    /// Basic abstract <see cref="IBrailleIOAdapterManager"/> implementation to handle all generic adapters.
    /// </summary>
		/// <remarks> </remarks>
    /// <seealso cref="BrailleIO.Interface.IBrailleIOAdapterManager" />
    abstract public class AbstractBrailleIOAdapterManagerBase : IBrailleIOAdapterManager, IDisposable
    {
        IBrailleIOAdapter _activeAdapter;
        /// <summary>initialize all supported devices and wait for connection.</summary>
        /// <value>The active adapter.</value>
        public IBrailleIOAdapter ActiveAdapter
        {
            get { return _activeAdapter; }
            set { AddAdapter(value); _activeAdapter = value; fire_ActiveAdapterChanged(); }
        }
        private readonly Object _adapterLock = new Object();
        private readonly ConcurrentBag<IBrailleIOAdapter> _adapters = new ConcurrentBag<IBrailleIOAdapter>();
        /// <summary>
        /// Gets the adapters.
        /// </summary>
		/// <remarks> </remarks>
        /// <value>
        /// The adapters.
        /// </value>
        protected ConcurrentBag<IBrailleIOAdapter> Adapters
        {
            get
            {
                lock (_adapterLock) { return _adapters; }
            }
        }


        /// <summary>
        /// The BrailleIOMediator
        /// </summary>
		/// <remarks> </remarks>
        protected BrailleIOMediator io;
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractBrailleIOAdapterManagerBase"/> class.
        /// </summary>
		/// <remarks> </remarks>
        public AbstractBrailleIOAdapterManagerBase()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractBrailleIOAdapterManagerBase"/> class.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="io">The <see cref="BrailleIOMediator"/> this manager is related to.</param>
        public AbstractBrailleIOAdapterManagerBase(ref BrailleIOMediator io)
            : this()
        {
            this.io = io;
        }

        /// <summary>
        /// Adds a new adapter to the manager.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="adapter">The adapter.</param>
        /// <returns>True if the adapter could be added to the manager otherwise false. It also returns false if the adapter is already added.</returns>
        public virtual bool AddAdapter(IBrailleIOAdapter adapter)
        {
            if (Adapters.Contains(adapter)) return false;
            try
            {
                Adapters.Add(adapter);
                fire_NewAdapterRegistered(adapter);
            }
            catch { return false; }
            return true;
        }

        /// <summary>
        /// Removes a new adapter from the manager.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="adapter">The adapter.</param>
        /// <returns>True if the adapter could be removed from the manager otherwise false.</returns>
        public virtual bool RemoveAdapter(IBrailleIOAdapter adapter)
        {
            if (Adapters.Contains(adapter))
            {
                try
                {
                    Adapters.TryTake(out adapter);
                    fire_AdapterRemoved(adapter);
                    return true;
                }
                catch { }
            }
            return false;
        }

        /// <summary>
        /// Gets the adapters.
        /// </summary>
		/// <remarks> </remarks>
        /// <returns>array of currently registered adapters (hardware abstraction)</returns>
        public virtual IBrailleIOAdapter[] GetAdapters()
        {
            return Adapters.ToArray();
        }

        /// <summary>Synchronizes the specified matrix.</summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns>
        ///   <c>true</c> if the matrix could been displayed successfully on the output device</returns>
        public bool Synchronize(bool[,] matrix)
        {
            bool success = true;
            if (ActiveAdapter != null)
            {
                System.Threading.Tasks.Task T = new System.Threading.Tasks.Task(() =>
                {
                    ActiveAdapter.Synchronize(matrix);
                });
                T.Start();
            }

            if (Adapters.Count > 0)
            {
                if (Adapters.Count > 1)
                {
                    System.Threading.Tasks.Parallel.ForEach<IBrailleIOAdapter>(Adapters, (item) =>
                    {
                        try
                        {
                            if (item != null && item != ActiveAdapter && item is AbstractBrailleIOAdapterBase && ((AbstractBrailleIOAdapterBase)item).Synch)
                                item.Synchronize(matrix);
                        }
                        catch { success = false; }
                    });
                }
                else if (ActiveAdapter == null)
                {
                    System.Threading.Tasks.Task T2 = new System.Threading.Tasks.Task(() =>
                    {
                        try
                        {
                            IBrailleIOAdapter item = null;
                            item = Adapters.ElementAt(0);

                            if (item != null && item != ActiveAdapter && item is AbstractBrailleIOAdapterBase && ((AbstractBrailleIOAdapterBase)item).Synch)
                                item.Synchronize(matrix);
                        }
                        catch { success = false; }
                    });
                    T2.Start();
                }
            }
            return success;
        }

        /// <summary>
        /// Occurs when a new adapter was registered.
        /// </summary>
		/// <remarks> </remarks>
        public event EventHandler<IBrailleIOAdapterEventArgs> NewAdapterRegistered;
        /// <summary>
        /// Occurs when an adapter was removed.
        /// </summary>
		/// <remarks> </remarks>
        public event EventHandler<IBrailleIOAdapterEventArgs> AdapterRemoved;
        /// <summary>
        /// Occurs when the active adapter changed.
        /// </summary>
		/// <remarks> </remarks>
        public event EventHandler<IBrailleIOAdapterEventArgs> ActiveAdapterChanged;

        /// <summary>
        /// Fires the new adapter registered event.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="adapter">The adapter.</param>
        protected virtual void fire_NewAdapterRegistered(IBrailleIOAdapter adapter)
        {
            if (NewAdapterRegistered != null)
            {
                try
                {
                    NewAdapterRegistered.Invoke(this, new IBrailleIOAdapterEventArgs(adapter));
                }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine("ERROR in AbstractBrailleIOAdapterManagerBase fire_NewAdapterRegistered:\r\n" + ex); }
            }
        }

        /// <summary>
        /// Fires the adapter removed event.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="adapter">The adapter.</param>
        protected virtual void fire_AdapterRemoved(IBrailleIOAdapter adapter)
        {
            if (AdapterRemoved != null)
            {
                try
                {
                    AdapterRemoved.Invoke(this, new IBrailleIOAdapterEventArgs(adapter));
                }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine("ERROR in AbstractBrailleIOAdapterManagerBase fire_AdapterRemoved:\r\n" + ex); }
            }
        }

        /// <summary>
        /// Fires the active adapter changed event.
        /// </summary>
		/// <remarks> </remarks>
        protected virtual void fire_ActiveAdapterChanged()
        {
            if (ActiveAdapterChanged != null)
            {
                try
                {
                    ActiveAdapterChanged.Invoke(this, new IBrailleIOAdapterEventArgs(ActiveAdapter));
                }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine("ERROR in AbstractBrailleIOAdapterManagerBase fire_ActiveAdapterChanged:\r\n" + ex); }
            }
        }

        #region IDisposable

        /// <summary>
        /// Disposes this element and disconnects and disposes all registered adapters.
        /// </summary>
		/// <remarks> </remarks>
        public void Dispose()
        {
            try
            {
                foreach (var item in Adapters)
                {
                    try
                    {
                        if (item != null)
                        {
                            item.Disconnect();
                            if (item is IDisposable) ((IDisposable)item).Dispose();
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }

        #endregion
    }

    /// <summary>
    /// Basic generic adapter manager for handling hardware or software adapters as in- and output for the <see cref="BrailleIOMediator"/>
    /// </summary>
		/// <remarks> </remarks>
    /// <seealso cref="BrailleIO.AbstractBrailleIOAdapterManagerBase" />
    public class BasicBrailleIOAdapterManager : AbstractBrailleIOAdapterManagerBase { }
}
