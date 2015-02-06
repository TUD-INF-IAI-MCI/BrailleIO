using System;
using System.Collections.Specialized;
using BrailleIO.Interface;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace BrailleIO
{
    /// <summary>
    /// A container for <see cref="BrailleIOViewRange"/> regions. So you can combine complex displays. 
    /// You can add an unlimited number of screen to your <see cref="BrailleIOMediator"/> instance. 
    /// But only one Screen can be visible at the same time.
    /// Width this container you can build multi screen applications
    /// </summary>
    public class BrailleIOScreen : AbstractViewBoxModelBase, IViewable
    {
        #region Members
        //FIXME: remove this if we can trust the orderedConcurrentDictionarry 
        private OrderedDictionary view_ranges = new OrderedDictionary();

        private OrderedConcurentDictionary<String, BrailleIOViewRange> viewRanges = new OrderedConcurentDictionary<String, BrailleIOViewRange>(new BrailleIOViewRangeComparer());


        private bool is_visible = true;
        /// <summary>
        /// Gets or sets the name of the Screen.
        /// Can change when adding it with a different name to a collection
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set; }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="BrailleIOScreen"/> class.
        /// </summary>
        public BrailleIOScreen()
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="BrailleIOScreen"/> class.
        /// </summary>
        /// <param name="name">The name of the screen. Shoulb be unique. Can be used to find the screen (view) 
        /// in the list of all available screen of the <see cref="BrailleIOMediator"/> instance.</param>
        public BrailleIOScreen(String name) { Name = name; }


        /// <summary>
        /// Returns an ordered list of the added view ranges.
        /// The order is from the earliest added to the latest added but width respect of the set zOrder from the lowest to the highest.
        /// </summary>
        /// <returns>Returns an ordered list of the added view ranges.
        /// The order is from the earliest added to the latest added but width respect of the set zOrder from the lowest to the highest.</returns>
        public List<KeyValuePair<String, BrailleIOViewRange>> GetOrderedViewRanges()
        {
            return viewRanges.GetSortedValues();
        }

        /// <summary>
        /// add ViewRange to screen
        /// </summary>
        /// <param name="name">
        /// name of ViewRange
        /// </param>
        /// <param name="_view_range">
        /// ViewRange
        /// </param>
        public void AddViewRange(String name, BrailleIOViewRange _view_range)
        {
            if (_view_range != null)
            {
                _view_range.Name = name;

                //FIXME: add to ordered
                viewRanges.Add(name, _view_range);

                if (!this.view_ranges.Contains(name)) this.view_ranges.Add(name, _view_range);
                else
                {
                    if (!this.view_ranges[name].Equals(_view_range))
                    {
                        this.view_ranges.Remove(name); this.view_ranges.Add(name, _view_range);
                    }
                }
                _view_range.SetParent(this);
            }
        }

        /// <summary>
        /// remove ViewRange
        /// </summary>
        /// <param name="name">
        /// name of ViewRange
        /// </param>
        public void RemoveViewRange(String name)
        {
            this.view_ranges.Remove(name);
            viewRanges.Remove(name);
        }

        /// <summary>
        /// rename ViewRange
        /// </summary>
        /// <param name="from">
        /// old name of ViewRange
        /// </param>
        /// <param name="to">
        /// new name of ViewRange
        /// </param>
        public void RenameViewRange(String from, String to)
        {
            this.view_ranges.Add(to, this.view_ranges[from]);
            this.view_ranges.Remove(from);
        }

        /// <summary>
        /// get All ViewRanges in Screen
        /// </summary>
        /// <returns>
        /// OrderedDictionary&lt;ViewRange&gt;
        /// </returns>
        public OrderedDictionary GetViewRanges()
        {
            //return this.view_ranges;
            
            OrderedDictionary result = new OrderedDictionary();
            try
            {
                var list = viewRanges.GetSortedValues();
                foreach (var pair in list)
                {
                    result.Add(pair.Key, pair.Value);
                }
            }
            catch { }

            return result;

        }

        /// <summary>
        /// Gets the view range width a specific name.
        /// </summary>
        /// <param name="name">The name of the viewRange to seach for.</param>
        /// <returns>the view range or <c>null</c></returns>
        public BrailleIOViewRange GetViewRange(String name)
        {
            if (HasViewRange(name))
                return (BrailleIOViewRange)this.view_ranges[name];
            else
                return null;
        }

        /// <summary>
        /// has specific ViewRange?
        /// </summary>
        /// <param name="name">
        /// name of ViewRange
        /// </param>
        /// <returns>
        /// bool has ViewRange?
        /// </returns>
        public bool HasViewRange(String name)
        {
            return this.view_ranges.Contains(name);
        }

        /// <summary>
        /// has any ViewRanges?
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return (this.view_ranges.Count > 0) ? false : true;
        }

        /// <summary>
        /// amount of ViewRanges
        /// </summary>
        /// <returns>
        /// int count
        /// </returns>
        public int Count()
        {
            return this.view_ranges.Count;
        }

        /// <summary>
        /// set Visibility of Screen
        /// </summary>
        /// <param name="which">
        /// bool desired visibility
        /// </param>
        public void SetVisibility(bool visible)
        {
            this.is_visible = visible;
        }

        /// <summary>
        /// Determines whether this instance is visible.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance is visible; otherwise, <c>false</c> if the instance is hidden.
        /// </returns>
        public bool IsVisible() { return this.is_visible; }

    }

    /// <summary>
    /// A Dictionary that can be sorted
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    class OrderedConcurentDictionary<TKey, TValue> : IDictionary<TKey, TValue>,
    ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>,
    IDictionary, ICollection, IEnumerable, IComparer<KeyValuePair<TKey, TValue>>
    {
        #region Member

        private readonly IComparer<KeyValuePair<TKey, TValue>> comparer;

        private ConcurrentDictionary<TKey, TValue> dic;

        private readonly ConcurrentDictionary<TKey, long> timedic = new ConcurrentDictionary<TKey, long>();

        #endregion

        public OrderedConcurentDictionary(IComparer<KeyValuePair<TKey, TValue>> comparer)
        {
            dic = new ConcurrentDictionary<TKey, TValue>();
            this.comparer = comparer;
        }

        #region timeStamp list

        volatile int z = 0;

        private void addToTimeDic(TKey key) { timedic[key] = z++; }
        private void updateInTimeDic(TKey key) { timedic[key] = z++; }
        private void removeFromTimeDic(TKey key)
        {
            long trash;
            timedic.TryRemove(key, out trash);
        }

        public List<KeyValuePair<TKey, TValue>> GetSortedValues()
        {
            List<KeyValuePair<TKey, TValue>> myList = dic.ToArray().ToList();
            myList.Sort(this);
            return myList;
        }

        #endregion

        #region interface implementations

        public bool ContainsKey(TKey key)
        {
            return dic.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get
            {
                //TODO: sort
                return dic.Keys;
            }
        }

        public bool Remove(TKey key)
        {
            TValue trash;
            return dic.TryRemove(key, out trash);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return dic.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values
        {
            get
            {
                //TODO: sort

                //SortedList<

                return dic.Values;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                return dic[key];
            }
            set
            {
                dic[key] = value;
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            addToTimeDic(item.Key);
            dic.AddOrUpdate(item.Key, item.Value, (k, oldValue) => oldValue = item.Value);
        }

        public void Clear()
        {
            dic.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            if (dic.ContainsKey(item.Key))
            {
                return dic[item.Key].Equals(item.Value);
            }
            return false;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            //TODO:
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return dic.Count; }
        }

        public bool IsReadOnly
        {
            get { return ((IDictionary)dic).IsReadOnly; }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (Contains(item))
            {
                TValue trash;
                return dic.TryRemove(item.Key, out trash);
            }
            return false;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return dic.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dic.GetEnumerator();
        }

        public void Add(object key, object value)
        {
            if (key is TKey && value is TValue)
            {
                Add(new KeyValuePair<TKey, TValue>((TKey)key, (TValue)value));
            }
        }

        public void Add(TKey key, TValue value)
        {
            Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        public bool Contains(object key)
        {
            return ((IDictionary)dic).Contains(key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IDictionary)dic).GetEnumerator();
        }

        public bool IsFixedSize
        {
            get { return ((IDictionary)dic).IsFixedSize; }
        }

        ICollection IDictionary.Keys
        {
            get
            {
                //TODO: sort
                return ((IDictionary)dic).Keys;
            }
        }

        public void Remove(object key)
        {
            if (key is TKey) { Remove((TKey)key); }
        }

        ICollection IDictionary.Values
        {
            get
            {
                //TODO: sort
                return ((IDictionary)dic).Values;
            }
        }

        public object this[object key]
        {
            get
            {
                if (key is TKey) return dic[(TKey)key];
                else return null;
            }
            set
            {
                if (key is TKey && value is TValue) dic[(TKey)key] = (TValue)value;
            }
        }

        public void CopyTo(Array array, int index)
        {
            //TODO:
            throw new NotImplementedException();
        }

        public bool IsSynchronized
        {
            get
            {
                //TODO
                return false;
            }
        }

        public object SyncRoot
        {
            get
            {
                //TODO:
                return null;
            }
        }

        #endregion

        #region internal sorter


        public int Compare(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
        {
            int result = this.comparer.Compare(x, y);

            if (result == 0)
            {
                // try find the tstamps
                long tx = 0;
                long ty = 0;

                if (timedic.ContainsKey(x.Key)) { tx = timedic[x.Key]; }
                if (timedic.ContainsKey(y.Key)) { ty = timedic[y.Key]; }

                try { return (int)(tx - ty); }
                catch { }
            }

            return result;
        }

        #endregion

    }

    /// <summary>
    /// Comparer for <see cref="BrailleIOViewRange"/> width respect to their adding time stamp and their zIndex.
    /// </summary>
    class BrailleIOViewRangeComparer : Comparer<KeyValuePair<String, BrailleIOViewRange>>,
        IComparer<KeyValuePair<String, object>>
    {

        /*
         * return:
         *      < 0 |   x < y
         *      = 0 |   x == x
         *      > 0 |   x > y
         * */

        public override int Compare(KeyValuePair<String, BrailleIOViewRange> x, KeyValuePair<String, BrailleIOViewRange> y)
        {
            int zx = 0;
            int zy = 0;

            if (x.Value is BrailleIOViewRange) zx = ((BrailleIOViewRange)x.Value).GetZIndex();
            if (y.Value is BrailleIOViewRange) zy = ((BrailleIOViewRange)y.Value).GetZIndex();

            return zx - zy;
        }

        public int Compare(KeyValuePair<String, object> x, KeyValuePair<String, object> y)
        {

            if (x.Value is BrailleIOViewRange && y.Value is BrailleIOViewRange)
                return Compare(
                    new KeyValuePair<String, BrailleIOViewRange>(x.Key, ((BrailleIOViewRange)x.Value)),
                    new KeyValuePair<string, BrailleIOViewRange>(y.Key, ((BrailleIOViewRange)x.Value)));

            return 0;
        }
    }

}
