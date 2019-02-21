using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using BrailleIO.Interface;

namespace BrailleIO
{
    /// <summary>
    /// A container for <see cref="BrailleIOViewRange"/> regions. So you can combine complex displays. 
    /// You can add an unlimited number of screen to your <see cref="BrailleIOMediator"/> instance. 
    /// But only one Screen can be visible at the same time.
    /// Width this container you can build multi screen applications
    /// </summary>
		/// <remarks> </remarks>
    public class BrailleIOScreen : AbstractViewBoxModelBase
    {
        #region Members
        ////FIXME: remove this if we can trust the orderedConcurrentDictionarry 
        //private OrderedDictionary view_ranges = new OrderedDictionary();

        private OrderedConcurentDictionary<String, BrailleIOViewRange> viewRanges = new OrderedConcurentDictionary<String, BrailleIOViewRange>(new BrailleIOViewRangeComparer());
        
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="BrailleIOScreen"/> class.
        /// </summary>
		/// <remarks> </remarks>
        public BrailleIOScreen()
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="BrailleIOScreen"/> class.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="name">The name of the screen. Should be unique. Can be used to find the screen (view) 
        /// in the list of all available screen of the <see cref="BrailleIOMediator"/> instance.</param>
        public BrailleIOScreen(String name) { Name = name; }


        /// <summary>
        /// Returns an ordered list of the added view ranges.
        /// The order is from the earliest added to the latest added but width respect of the set zOrder from the lowest to the highest.
        /// </summary>
		/// <remarks> </remarks>
        /// <returns>Returns an ordered list of the added view ranges.
        /// The order is from the earliest added to the latest added but width respect of the set zOrder from the lowest to the highest.</returns>
        public List<KeyValuePair<String, BrailleIOViewRange>> GetOrderedViewRanges()
        {
            return viewRanges.GetSortedValues();
        }

        /// <summary>
        /// add ViewRange to screen
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="_view_range">ViewRange</param>
        public void AddViewRange(BrailleIOViewRange _view_range)
        {
            if (_view_range != null) 
                AddViewRange(
                    String.IsNullOrWhiteSpace(_view_range.Name) 
                    ? _view_range.GetHashCode().ToString() : _view_range.Name
                    , _view_range);
        }
        /// <summary>
        /// add ViewRange to screen
        /// </summary>
		/// <remarks> </remarks>
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
                //viewRanges.Add(name, _view_range);

                if (!this.viewRanges.ContainsKey(name))
                    this.viewRanges.Add(name, _view_range);
                else
                {
                    if (!this.viewRanges[name].Equals(_view_range))
                    {
                        this.viewRanges.Remove(name);
                        this.viewRanges.Add(name, _view_range);
                    }
                }
                _view_range.SetParent(this);
            }
        }

        /// <summary>
        /// remove ViewRange
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="name">
        /// name of ViewRange
        /// </param>
        public void RemoveViewRange(String name)
        {
            this.viewRanges.Remove(name);
            //viewRanges.Remove(name);
        }

        /// <summary>
        /// rename ViewRange
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="from">
        /// old name of ViewRange
        /// </param>
        /// <param name="to">
        /// new name of ViewRange
        /// </param>
        public void RenameViewRange(String from, String to)
        {
            this.viewRanges.Add(to, this.viewRanges[from]);
            this.viewRanges.Remove(from);
        }

        /// <summary>
        /// get All ViewRanges in Screen
        /// </summary>
		/// <remarks> </remarks>
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
		/// <remarks> </remarks>
        /// <param name="name">The name of the viewRange to search for.</param>
        /// <returns>the view range or <c>null</c></returns>
        public BrailleIOViewRange GetViewRange(String name)
        {
            if (HasViewRange(name))
                return (BrailleIOViewRange)this.viewRanges[name];
            else
                return null;
        }

        /// <summary>
        /// has specific ViewRange?
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="name">
        /// name of ViewRange
        /// </param>
        /// <returns>
        /// <c>true</c> if the screen contains the requested viewRange name; otherwise <c>false</c>.
        /// </returns>
        public bool HasViewRange(String name)
        {
            return this.viewRanges.ContainsKey(name);
        }

        /// <summary>
        /// Gets the visible view range at a position.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="x">The horizontal position on the device.</param>
        /// <param name="y">The vertical position on the device.</param>
        /// <returns>The view range at the requested pin-position or <c>null</c>.</returns>
        public BrailleIOViewRange GetVisibleViewRangeAtPosition(int x, int y)
        {
            if (!IsEmpty() && x >= 0 && y >= 0 && viewRanges != null && viewRanges.Count > 0)
            {
                try
                {

                    var sortedViews = viewRanges.GetSortedValues();
                    var kvPair = sortedViews.FindLast(
                        (item) => { 
                            return item.Value != null 
                                && item.Value.IsVisible() 
                                && item.Value.ContainsPoint(x, y); });
                    if (kvPair.Value != null)
                    {
                        return kvPair.Value;
                    }
                }
                catch (Exception) { }
            }

            return null;
        }


        /// <summary>has any ViewRanges?</summary>
        /// <returns>
        ///   <c>true</c> if this there are no <see cref="BrailleIOViewRange"/> inside; otherwise, <c>false</c>.</returns>
        public bool IsEmpty()
        {
            return (this.viewRanges.Count > 0) ? false : true;
        }

        /// <summary>
        /// amount of ViewRanges
        /// </summary>
		/// <remarks> </remarks>
        /// <returns>
        /// int count
        /// </returns>
        public int Count()
        {
            return this.viewRanges.Count;
        }
    }

    /// <summary>
    /// A Dictionary that can be sorted
    /// </summary>
		/// <remarks> </remarks>
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

        /// <summary>Initializes a new instance of the <see cref="OrderedConcurentDictionary{TKey, TValue}"/> class.</summary>
        /// <param name="comparer">The used comparer for sorting.</param>
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

        /// <summary>Gets the sorted values.</summary>
        /// <returns>List of sorted values</returns>
        public List<KeyValuePair<TKey, TValue>> GetSortedValues()
        {
            List<KeyValuePair<TKey, TValue>> myList = dic.ToArray().ToList();
            myList.Sort(this);
            return myList;
        }

        #endregion

        #region interface implementations

        /// <summary>Ermittelt, ob das <see cref="T:System.Collections.Generic.IDictionary`2"/> ein Element mit dem angegebenen Schlüssel enthält.</summary>
        /// <param name="key">Der im <see cref="T:System.Collections.Generic.IDictionary`2"/> zu suchende Schlüssel.</param>
        /// <returns>
        ///   <span class="keyword">
        ///     <span class="languageSpecificText">
        ///       <span class="cs">true</span>
        ///       <span class="vb">True</span>
        ///       <span class="cpp">true</span>
        ///     </span>
        ///   </span>
        ///   <span class="nu">
        ///     <span class="keyword">true</span> (<span class="keyword">True</span> in Visual Basic)</span>, wenn das <see cref="T:System.Collections.Generic.IDictionary`2"/> ein Element mit dem Schlüssel enthält, andernfalls <span class="keyword"><span class="languageSpecificText"><span class="cs">false</span><span class="vb">False</span><span class="cpp">false</span></span></span><span class="nu"><span class="keyword">false</span> (<span class="keyword">False</span> in Visual Basic)</span>.
        /// </returns>
        public bool ContainsKey(TKey key)
        {
            return dic.ContainsKey(key);
        }

        /// <summary>Ruft eine <see cref="T:System.Collections.Generic.ICollection`1"/> ab, die die Schlüssel des <see cref="T:System.Collections.Generic.IDictionary`2"/> enthält.</summary>
        public ICollection<TKey> Keys
        {
            get
            {
                //TODO: sort
                return dic.Keys;
            }
        }

        /// <summary>Entfernt das Element mit dem angegebenen Schlüssel aus dem <see cref="T:System.Collections.Generic.IDictionary`2"/>.</summary>
        /// <param name="key">Der Schlüssel des zu entfernenden Elements.</param>
        /// <returns>
        ///   <span class="keyword">
        ///     <span class="languageSpecificText">
        ///       <span class="cs">true</span>
        ///       <span class="vb">True</span>
        ///       <span class="cpp">true</span>
        ///     </span>
        ///   </span>
        ///   <span class="nu">
        ///     <span class="keyword">true</span> (<span class="keyword">True</span> in Visual Basic)</span>, wenn das Element erfolgreich entfernt wurde, andernfalls <span class="keyword"><span class="languageSpecificText"><span class="cs">false</span><span class="vb">False</span><span class="cpp">false</span></span></span><span class="nu"><span class="keyword">false</span> (<span class="keyword">False</span> in Visual Basic)</span>.
        /// Diese Methode gibt auch dann <span class="keyword"><span class="languageSpecificText"><span class="cs">false</span><span class="vb">False</span><span class="cpp">false</span></span></span><span class="nu"><span class="keyword">false</span> (<span class="keyword">False</span> in Visual Basic)</span> zurück, wenn <paramref name="key" /> nicht im ursprünglichen <see cref="T:System.Collections.Generic.IDictionary`2"/> gefunden wurde.
        /// </returns>
        public bool Remove(TKey key)
        {
            TValue trash;
            return dic.TryRemove(key, out trash);
        }

        /// <summary>Ruft den dem angegebenen Schlüssel zugeordneten Wert ab.</summary>
        /// <param name="key">Der Schlüssel, dessen Wert abgerufen werden soll.</param>
        /// <param name="value">
        /// Wenn diese Methode zurückgegeben wird, enthält sie den dem angegebenen Schlüssel zugeordneten Wert, wenn der Schlüssel gefunden wird, andernfalls enthält sie den Standardwert für den Typ des <paramref name="value" />-Parameters.
        /// Dieser Parameter wird nicht initialisiert übergeben.
        /// </param>
        /// <returns>
        ///   <span class="keyword">
        ///     <span class="languageSpecificText">
        ///       <span class="cs">true</span>
        ///       <span class="vb">True</span>
        ///       <span class="cpp">true</span>
        ///     </span>
        ///   </span>
        ///   <span class="nu">
        ///     <span class="keyword">true</span> (<span class="keyword">True</span> in Visual Basic)</span>, wenn das Objekt, das <see cref="T:System.Collections.Generic.IDictionary`2"/> implementiert, ein Element mit dem angegebenen Schlüssel enthält, andernfalls <span class="keyword"><span class="languageSpecificText"><span class="cs">false</span><span class="vb">False</span><span class="cpp">false</span></span></span><span class="nu"><span class="keyword">false</span> (<span class="keyword">False</span> in Visual Basic)</span>.
        /// </returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return dic.TryGetValue(key, out value);
        }

        /// <summary>Ruft eine <see cref="T:System.Collections.Generic.ICollection`1"/> ab, die die Werte im <see cref="T:System.Collections.Generic.IDictionary`2"/> enthält.</summary>
        public ICollection<TValue> Values
        {
            get
            {
                //TODO: sort

                //SortedList<

                return dic.Values;
            }
        }

        /// <summary>Gets or sets the <see cref="TValue"/> with the specified key.</summary>
        /// <param name="key">The key.</param>
        /// <value>The <see cref="TValue"/>.</value>
        /// <returns>the value to the requested key</returns>
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

        /// <summary>Fügt der <see cref="T:System.Collections.Generic.ICollection`1"/> ein Element hinzu.</summary>
        /// <param name="item">Das Objekt, das <see cref="T:System.Collections.Generic.ICollection`1"/> hinzugefügt werden soll.</param>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            addToTimeDic(item.Key);
            dic.AddOrUpdate(item.Key, item.Value, (k, oldValue) => oldValue = item.Value);
        }

        /// <summary>Entfernt alle Elemente aus <see cref="T:System.Collections.Generic.ICollection`1"/>.</summary>
        public void Clear()
        {
            dic.Clear();
        }

        /// <summary>Ermittelt, ob die <see cref="T:System.Collections.Generic.ICollection`1"/> einen bestimmten Wert enthält.</summary>
        /// <param name="item">Das im <see cref="T:System.Collections.Generic.ICollection`1"/> zu suchende Objekt.</param>
        /// <returns>
        ///   <span class="keyword">
        ///     <span class="languageSpecificText">
        ///       <span class="cs">true</span>
        ///       <span class="vb">True</span>
        ///       <span class="cpp">true</span>
        ///     </span>
        ///   </span>
        ///   <span class="nu">
        ///     <span class="keyword">true</span> (<span class="keyword">True</span> in Visual Basic)</span>, wenn das <paramref name="item" /> in der <see cref="T:System.Collections.Generic.ICollection`1"/> gefunden wird, andernfalls <span class="keyword"><span class="languageSpecificText"><span class="cs">false</span><span class="vb">False</span><span class="cpp">false</span></span></span><span class="nu"><span class="keyword">false</span> (<span class="keyword">False</span> in Visual Basic)</span>.
        /// </returns>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            if (dic.ContainsKey(item.Key))
            {
                return dic[item.Key].Equals(item.Value);
            }
            return false;
        }

        /// <summary>
        /// Kopiert die Elemente der <see cref="T:System.Collections.Generic.ICollection`1"/> in ein <see cref="T:System.Array"/>, beginnend bei einem bestimmten <see cref="T:System.Array"/>-Index.
        /// </summary>
        /// <param name="array">
        /// Das eindimensionale <see cref="T:System.Array"/>, das das Ziel der aus der <see cref="T:System.Collections.Generic.ICollection`1"/> kopierten Elemente ist.
        /// Für das <see cref="T:System.Array"/> muss eine nullbasierte Indizierung verwendet werden.
        /// </param>
        /// <param name="arrayIndex">Der nullbasierte Index im <paramref name="array" />, bei dem der Kopiervorgang beginnt.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            //TODO:
            throw new NotImplementedException();
        }

        /// <summary>Ruft die Anzahl der Elemente ab, die in <see cref="T:System.Collections.Generic.ICollection`1"/> enthalten sind.</summary>
        public int Count
        {
            get { return dic.Count; }
        }

        /// <summary>Ruft einen Wert ab, der angibt, ob das <see cref="T:System.Collections.Generic.ICollection`1"/> schreibgeschützt ist.</summary>
        public bool IsReadOnly
        {
            get { return ((IDictionary)dic).IsReadOnly; }
        }

        /// <summary>Entfernt das erste Vorkommen eines angegebenen Objekts aus der <see cref="T:System.Collections.Generic.ICollection`1"/>.</summary>
        /// <param name="item">Das aus der <see cref="T:System.Collections.Generic.ICollection`1"/> zu entfernende Objekt.</param>
        /// <returns>
        ///   <span class="keyword">
        ///     <span class="languageSpecificText">
        ///       <span class="cs">true</span>
        ///       <span class="vb">True</span>
        ///       <span class="cpp">true</span>
        ///     </span>
        ///   </span>
        ///   <span class="nu">
        ///     <span class="keyword">true</span> (<span class="keyword">True</span> in Visual Basic)</span>, wenn <paramref name="item" /> erfolgreich aus der <see cref="T:System.Collections.Generic.ICollection`1"/> entfernt wurde, andernfalls <span class="keyword"><span class="languageSpecificText"><span class="cs">false</span><span class="vb">False</span><span class="cpp">false</span></span></span><span class="nu"><span class="keyword">false</span> (<span class="keyword">False</span> in Visual Basic)</span>.
        /// Diese Methode gibt auch dann <span class="keyword"><span class="languageSpecificText"><span class="cs">false</span><span class="vb">False</span><span class="cpp">false</span></span></span><span class="nu"><span class="keyword">false</span> (<span class="keyword">False</span> in Visual Basic)</span> zurück, wenn <paramref name="item" /> nicht in der ursprünglichen <see cref="T:System.Collections.Generic.ICollection`1"/> gefunden wurde.
        /// </returns>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (Contains(item))
            {
                TValue trash;
                return dic.TryRemove(item.Key, out trash);
            }
            return false;
        }

        /// <summary>Gibt einen Enumerator zurück, der die Auflistung durchläuft.</summary>
        /// <returns>Ein Enumerator, der zum Durchlaufen der Auflistung verwendet werden kann.</returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return dic.GetEnumerator();
        }

        /// <summary>Gibt einen Enumerator zurück, der eine Auflistung durchläuft.</summary>
        /// <returns>Ein <see cref="T:System.Collections.IEnumerator"/>-Objekt, das zum Durchlaufen der Auflistung verwendet werden kann.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return dic.GetEnumerator();
        }

        /// <summary>Fügt dem <see cref="T:System.Collections.IDictionary"/>-Objekt ein Element mit dem angegebenen Schlüssel und Wert hinzu.</summary>
        /// <param name="key">Das <see cref="T:System.Object"/>, das als Schlüssel für das hinzuzufügende Element verwendet werden soll.</param>
        /// <param name="value">Das <see cref="T:System.Object"/>, das als Wert für das hinzuzufügende Element verwendet werden soll.</param>
        public void Add(object key, object value)
        {
            if (key is TKey && value is TValue)
            {
                Add(new KeyValuePair<TKey, TValue>((TKey)key, (TValue)value));
            }
        }

        /// <summary>Fügt der <see cref="T:System.Collections.Generic.IDictionary`2"/>-Schnittstelle ein Element mit dem angegebenen Schlüssel und Wert hinzu.</summary>
        /// <param name="key">Das Objekt, das als Schlüssel des hinzuzufügenden Elements verwendet werden soll.</param>
        /// <param name="value">Das Objekt, das als Wert des hinzuzufügenden Elements verwendet werden soll.</param>
        public void Add(TKey key, TValue value)
        {
            Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        /// <summary>Ermittelt, ob das <see cref="T:System.Collections.IDictionary"/>-Objekt ein Element mit dem angegebenen Schlüssel enthält.</summary>
        /// <param name="key">Der im <see cref="T:System.Collections.IDictionary"/>-Objekt zu suchende Schlüssel.</param>
        /// <returns>
        ///   <span class="keyword">
        ///     <span class="languageSpecificText">
        ///       <span class="cs">true</span>
        ///       <span class="vb">True</span>
        ///       <span class="cpp">true</span>
        ///     </span>
        ///   </span>
        ///   <span class="nu">
        ///     <span class="keyword">true</span> (<span class="keyword">True</span> in Visual Basic)</span>, wenn das <see cref="T:System.Collections.IDictionary"/> ein Element mit dem Schlüssel enthält, andernfalls <span class="keyword"><span class="languageSpecificText"><span class="cs">false</span><span class="vb">False</span><span class="cpp">false</span></span></span><span class="nu"><span class="keyword">false</span> (<span class="keyword">False</span> in Visual Basic)</span>.
        /// </returns>
        public bool Contains(object key)
        {
            return ((IDictionary)dic).Contains(key);
        }

        /// <summary>Gibt ein <see cref="T:System.Collections.IDictionaryEnumerator"/>-Objekt für das <see cref="T:System.Collections.IDictionary"/>-Objekt zurück.</summary>
        /// <returns>Ein <see cref="T:System.Collections.IDictionaryEnumerator"/>-Objekt für das <see cref="T:System.Collections.IDictionary"/>-Objekt.</returns>
        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IDictionary)dic).GetEnumerator();
        }

        /// <summary>Ruft einen Wert ab, der angibt, ob das <see cref="T:System.Collections.IDictionary"/>-Objekt eine feste Größe hat.</summary>
        public bool IsFixedSize
        {
            get { return ((IDictionary)dic).IsFixedSize; }
        }

        /// <summary>Ruft eine <see cref="T:System.Collections.Generic.ICollection`1"/> ab, die die Schlüssel des <see cref="T:System.Collections.Generic.IDictionary`2"/> enthält.</summary>
        ICollection IDictionary.Keys
        {
            get
            {
                //TODO: sort
                return ((IDictionary)dic).Keys;
            }
        }

        /// <summary>Entfernt das Element mit dem angegebenen Schlüssel aus dem <see cref="T:System.Collections.IDictionary"/>-Objekt.</summary>
        /// <param name="key">Der Schlüssel des zu entfernenden Elements.</param>
        public void Remove(object key)
        {
            if (key is TKey) { Remove((TKey)key); }
        }

        /// <summary>Ruft eine <see cref="T:System.Collections.Generic.ICollection`1"/> ab, die die Werte im <see cref="T:System.Collections.Generic.IDictionary`2"/> enthält.</summary>
        ICollection IDictionary.Values
        {
            get
            {
                //TODO: sort
                return ((IDictionary)dic).Values;
            }
        }

        /// <summary>Gets or sets the <see cref="System.Object"/> with the specified key.</summary>
        /// <param name="key">The key.</param>
        /// <value>The <see cref="System.Object"/>.</value>
        /// <returns>the value related to the given key</returns>
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

        /// <summary>
        /// Kopiert die Elemente der <see cref="T:System.Collections.ICollection"/> in ein <see cref="T:System.Array"/>, beginnend bei einem bestimmten <see cref="T:System.Array"/>-Index.
        /// </summary>
        /// <param name="array">
        /// Das eindimensionale <see cref="T:System.Array"/>, das das Ziel der aus der <see cref="T:System.Collections.ICollection"/> kopierten Elemente ist.
        /// Für das <see cref="T:System.Array"/> muss eine nullbasierte Indizierung verwendet werden.
        /// </param>
        /// <param name="index">Der nullbasierte Index im <paramref name="array" />, bei dem der Kopiervorgang beginnt.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void CopyTo(Array array, int index)
        {
            //TODO:
            throw new NotImplementedException();
        }

        /// <summary>Ruft einen Wert ab, der angibt, ob der Zugriff auf die <see cref="T:System.Collections.ICollection"/> synchronisiert (threadsicher) ist.</summary>
        public bool IsSynchronized
        {
            get
            {
                //TODO
                return false;
            }
        }

        /// <summary>Ruft ein Objekt ab, mit dem der Zugriff auf <see cref="T:System.Collections.ICollection"/> synchronisiert werden kann.</summary>
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

        /// <summary>Performs a case-sensitive comparison of two objects of the same type and returns a value indicating whether one is less than, equal to, or greater than the other.</summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// A signed integer that indicates the relative values of
        /// <paramref name="x" />
        /// and
        /// <paramref name="y" />
        /// , as shown in the following table.
        /// Value
        /// Meaning
        /// Less than zero
        /// <paramref name="x" /> is less than <paramref name="y" />.
        /// Zero
        /// <paramref name="x" /> equals <paramref name="y" />.
        /// Greater than zero
        /// <paramref name="x" /> is greater than <paramref name="y" />.
        /// </returns>
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
	/// <remarks> </remarks>
    class BrailleIOViewRangeComparer : Comparer<KeyValuePair<String, BrailleIOViewRange>>,
        IComparer<KeyValuePair<String, object>>
    {

        /*
         * return:
         *      < 0 |   x < y
         *      = 0 |   x == x
         *      > 0 |   x > y
         * */


        /// <summary>
        /// Performs a case-sensitive comparison of two objects of the same type and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// A signed integer that indicates the relative values of 
        /// <paramref name="x" />
        /// and 
        /// <paramref name="y" />
        /// , as shown in the following table.
        /// Value
        /// Meaning
        /// Less than zero
        /// <paramref name="x" /> is less than <paramref name="y" />.
        /// Zero
        /// <paramref name="x" /> equals <paramref name="y" />.
        /// Greater than zero
        /// <paramref name="x" /> is greater than <paramref name="y" />.
        /// </returns>
        public override int Compare(KeyValuePair<String, BrailleIOViewRange> x, KeyValuePair<String, BrailleIOViewRange> y)
        {
            int zx = 0;
            int zy = 0;

            if (x.Value is BrailleIOViewRange) zx = ((BrailleIOViewRange)x.Value).GetZIndex();
            if (y.Value is BrailleIOViewRange) zy = ((BrailleIOViewRange)y.Value).GetZIndex();

            return zx - zy;
        }

        /// <summary>Performs a case-sensitive comparison of two objects of the same type and returns a value indicating whether one is less than, equal to, or greater than the other.</summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// A signed integer that indicates the relative values of
        /// <paramref name="x" />
        /// and
        /// <paramref name="y" />
        /// , as shown in the following table.
        /// Value
        /// Meaning
        /// Less than zero
        /// <paramref name="x" /> is less than <paramref name="y" />.
        /// Zero
        /// <paramref name="x" /> equals <paramref name="y" />.
        /// Greater than zero
        /// <paramref name="x" /> is greater than <paramref name="y" />.
        /// </returns>
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
