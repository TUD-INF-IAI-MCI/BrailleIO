using System;
using System.Collections.Specialized;
using BrailleIO.Interface;

namespace BrailleIO
{
    public class BrailleIOScreen : AbstractViewBoxModelBase, IViewable
    {
        #region Members
        private OrderedDictionary view_ranges = new OrderedDictionary();
        private bool is_visible = false;
        private bool has_border = false;
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
        public void addViewRange(String name, BrailleIOViewRange _view_range)
        {
            if (_view_range != null)
            {
                _view_range.Name = name;
                this.view_ranges.Add(name, _view_range);
            }
        }

        /// <summary>
        /// remove ViewRange
        /// </summary>
        /// <param name="name">
        /// name of ViewRange
        /// </param>
        public void removeViewRange(String name)
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
        public void renameViewRange(String from, String to)
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
        public OrderedDictionary getViewRanges()
        {
            return this.view_ranges;
        }

        public BrailleIOViewRange getViewRange(String name)
        {
            if (hasViewRange(name))
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
        public bool hasViewRange(String name)
        {
            return this.view_ranges.Contains(name);
        }

        /// <summary>
        /// has Border?
        /// </summary>
        /// <returns>
        /// bool has border
        /// </returns>
        public bool hasBorder()
        {
            return this.has_border;
        }

        /// <summary>
        /// has any ViewRanges?
        /// </summary>
        /// <returns></returns>
        public bool isEmpty()
        {
            return (this.view_ranges.Count > 0) ? false : true;
        }

        /// <summary>
        /// amount of ViewRanges
        /// </summary>
        /// <returns>
        /// int count
        /// </returns>
        public int count()
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

    }
}
