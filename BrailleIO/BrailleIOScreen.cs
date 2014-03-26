using System;
using System.Collections.Specialized;
using BrailleIO.Interface;

namespace BrailleIO
{
    public class BrailleIOScreen : AbstractViewBoxModelBase, IViewable
    {
        #region Members
        private OrderedDictionary view_ranges = new OrderedDictionary();
        private bool is_visible = true;
        public string Name { get; private set; }
        #endregion

        public BrailleIOScreen() { }
        public BrailleIOScreen(String name) { Name = name; }

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
                if (!this.view_ranges.Contains(name)) this.view_ranges.Add(name, _view_range);
                else
                {
                    if (!this.view_ranges[name].Equals(_view_range)) { 
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
            return this.view_ranges;
        }

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
            //foreach (BrailleIOViewRange r in this.view_ranges.Values)
            //    r.SetVisibility(visible);
        }

        public bool IsVisible() { return this.is_visible; }

    }
}
