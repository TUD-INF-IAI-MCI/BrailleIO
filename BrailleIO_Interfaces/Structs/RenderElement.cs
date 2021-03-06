﻿using System;
using System.Collections.Generic;

namespace BrailleIO.Renderer.Structs
{
    /// <summary>
    /// Element that represent a rendered Element an can be used for collision testings, building tree structures and other stuff
    /// </summary>
    public struct RenderElement
    {
        #region  Members

        #region Private

        /// <summary>
        /// List of sub elements if the element consists of sub parts 
        /// e.g. if the element is slitted over several lines.
        /// </summary>
        private LinkedList<RenderElement> Subparts;

        private string _displayName;

        private int _x;
        private int _y;
        private int _width;
        private int _height;

        /// <summary>
        /// The value of the object that was rendered
        /// </summary>
        private Object Value;

        /// <summary>
        /// Parent. Must be an Object to set it <c>null</c>
        /// </summary>
        private Object Parent;

        #endregion

        #region Public Fields

        /// <summary>
        /// Gets or sets the optional display name. 
        /// Will returned by calling the toString() method. 
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        public string DisplayName { get { return _displayName; } set { _displayName = value; _isNotEmpty = true; } }

        /// <summary>
        /// Horizontal start position of this element's bonding box 
        /// </summary>
		/// <value>The horizontal position.</value>
        public int X { get { return _x; } set { _x = value; _isNotEmpty = true; } }

        /// <summary>Vertical start position of this element's bonding box</summary>
        /// <value>The vertical position.</value>
        public int Y { get { return _y; } set { _y = value; _isNotEmpty = true; } }

        /// <summary>Width of the element's bonding box</summary>
        /// <value>The width.</value>
        public int Width { get { return _width; } set { _width = Math.Max(0, value); _isNotEmpty = true; } }

        /// <summary>Height of the element's bonding box</summary>
        /// <value>The height.</value>
        public int Height { get { return _height; } set { _height = Math.Max(0, value); _isNotEmpty = true; } }

        /// <summary>
        /// Can be used to specify the type of this element, e.g. for the different sub elements.
        /// </summary>
        public Object Type;

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderElement"/> struct.
        /// </summary>
        /// <param name="x">The horizontal start position of the elemnt's bounding box.</param>
        /// <param name="y">The vertical start position of the elemnt's bounding box.</param>
        /// <param name="width">The width of the elemnt's bounding box.</param>
        /// <param name="height">The height of the elemnt's bounding box.</param>
        /// <param name="value">The value of the object.</param>
        /// <param name="subparts">Subparts if available (if elemt is splitted into several parts).</param>
        /// <param name="parent">The parent of this element. This element will NOT be added as child to the parent automatically!</param>
        private RenderElement(int x, int y, int width, int height, Object value, IList<RenderElement> subparts, Object parent)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;
            Value = value;
            Subparts = subparts != null ? new LinkedList<RenderElement>(subparts) : new LinkedList<RenderElement>();
            Parent = parent;
            Type = null;
            _displayName = null;
            _isNotEmpty = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderElement"/> struct.
        /// </summary>
        /// <param name="x">The horizontal start position of the elemnt's bounding box.</param>
        /// <param name="y">The vertical start position of the elemnt's bounding box.</param>
        /// <param name="width">The width of the elemnt's bounding box.</param>
        /// <param name="height">The height of the elemnt's bounding box.</param>
        /// <param name="value">The value of the object.</param>
        /// <param name="subparts">Subparts if available (if elemt is splitted into several parts).</param>
        /// <param name="parent">The parent of this element.</param>
        public RenderElement(int x, int y, int width, int height, Object value, IList<RenderElement> subparts, RenderElement parent)
            : this(x, y, width, height, value, subparts, parent as Object) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderElement"/> struct.
        /// </summary>
        /// <param name="x">The horizontal start position of the elemnt's bounding box.</param>
        /// <param name="y">The vertical start position of the elemnt's bounding box.</param>
        /// <param name="width">The width of the elemnt's bounding box.</param>
        /// <param name="height">The height of the elemnt's bounding box.</param>
        /// <param name="value">The value of the object.</param>
        /// <param name="subparts">Subparts if available (if elemt is splitted into several parts).</param>
        public RenderElement(int x, int y, int width, int height, Object value, IList<RenderElement> subparts)
            : this(x, y, width, height, value, subparts, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderElement"/> struct.
        /// </summary>
        /// <param name="x">The horizontal start position of the elemnt's bounding box.</param>
        /// <param name="y">The vertical start position of the elemnt's bounding box.</param>
        /// <param name="width">The width of the elemnt's bounding box.</param>
        /// <param name="height">The height of the elemnt's bounding box.</param>
        /// <param name="value">The value of the object.</param>
        /// <param name="parent">The parent of this element.</param>
        public RenderElement(int x, int y, int width, int height, Object value, RenderElement parent)
            : this(x, y, width, height, value, null, parent as Object) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderElement"/> struct.
        /// </summary>
        /// <param name="x">The horizontal start position of the elemnt's bounding box.</param>
        /// <param name="y">The vertical start position of the elemnt's bounding box.</param>
        /// <param name="width">The width of the elemnt's bounding box.</param>
        /// <param name="height">The height of the elemnt's bounding box.</param>
        /// <param name="value">The value of the object.</param>
        public RenderElement(int x, int y, int width, int height, Object value)
            : this(x, y, width, height, value, null, null) { }


        #endregion

        #region Collition Tests

        /// <summary>
        /// Check if the element contains a requested point
        /// </summary>
        /// <param name="x">horizontal point position</param>
        /// <param name="y">vertical point position</param>
        /// <returns><c>true</c> if the requested point is contained in the bounding box or in one bounding boxes of the subparts</returns>
        public bool ContainsPoint(int x, int y)
        {
            if (Subparts != null && Subparts.Count > 0)
            {
                foreach (var subPart in Subparts)
                {
                    if (subPart.ContainsPoint(x, y))
                        return true;
                }
            }

            return (x >= X && x <= (X + Width)) && (y >= Y && y <= (Y + Height));
        }

        /// <summary>
        /// Determines if this elements bounding box is completely inside the given region.
        /// </summary>
        /// <param name="left">Left border of the region to test (X).</param>
        /// <param name="right">Right border of the region to test (X + width).</param>
        /// <param name="top">Top border of the region to test (Y).</param>
        /// <param name="bottom">Bottom border of the region to test (Y + heigh).</param>
        /// <returns><c>true</c> if the element is completely inside the tested region, otherwise <c>false</c></returns>
        public bool IsCompletelyInArea(int left, int right, int top, int bottom)
        {
            return X >= left
                && (X + Width - 1) <= right
                && Y >= top
                && (Y + Height - 1) <= bottom;
        }

        /// <summary>
        /// Determines if this elements bounding box is at least partly inside the given region.
        /// </summary>
        /// <param name="left">Left border of the region to test (X).</param>
        /// <param name="right">Right border of the region to test (X + width).</param>
        /// <param name="top">Top border of the region to test (Y).</param>
        /// <param name="bottom">Bottom border of the region to test (Y + heigh).</param>
        /// <returns><c>true</c> if the element is at least partly inside the tested region, otherwise <c>false</c></returns>
        public bool IsInArea(int left, int right, int top, int bottom)
        {
            int Bottom = Y + Height - 1;
            int Right = X + Width - 1;

            if (Height > 0 && Width > 0)
            {
                bool vert_ok = false;
                // check vertical
                if (X < left)
                {
                    if (Right >= left) vert_ok = true;
                }
                else
                {
                    if (X <= Right) vert_ok = true;
                }

                // check horizontal
                if (vert_ok)
                {
                    if (Y < top)
                    {
                        if (Bottom >= top) return true;
                    }
                    else
                    {
                        if (Y <= bottom) return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Get the subparts at a specific point.
        /// </summary>
        /// <param name="x">The horizontal position of the point.</param>
        /// <param name="y">The vertical position of the point.</param>
        /// <returns>a list of sub elements containing this point.</returns>
        public List<RenderElement> GetSubPartsAtPoint(int x, int y)
        {
            List<RenderElement> result = new List<RenderElement>();

            if (Subparts != null && Subparts.Count > 0)
            {
                foreach (RenderElement subPart in Subparts)
                {
                    if (subPart.ContainsPoint(x, y))
                    {
                        result.Add(subPart);
                    }
                }
            }

            return result;
        }

        #endregion

        #region SubPart Handling

        /// <summary>
        /// Determine if this element has subparts registered.
        /// </summary>
        /// <returns><c>true</c> it some subparts are registered for this elment, otherwise <c>false</c>.</returns>
        public bool HasSubParts()
        {
            return Subparts.Count > 0;
        }

        /// <summary>
        /// Get a copy of the list of subparts.
        /// </summary>
        /// <returns>Copy of the subpart linked list.</returns>
        public LinkedList<RenderElement> GetSubParts()
        {
            return new LinkedList<RenderElement>(Subparts);
        }

        /// <summary>
        /// Adds a new subpart at the end of the list.
        /// </summary>
        /// <param name="subPart">The new Subpart to add.</param>
        public void AddSubPart(RenderElement subPart)
        {
            if (Subparts != null && Subparts.Contains(subPart)) return;

            if (Subparts == null) Subparts = new LinkedList<RenderElement>();
            Subparts.AddLast(subPart);
            subPart.SetParent(this);
            updateBoundingBox(subPart);
        }

        /// <summary>
        /// Adds a subpart after a specific one.
        /// </summary>
        /// <param name="subPart">The element to add.</param>
        /// <param name="previousSibling">An element to add this after. 
        /// If the element does not exist or is not inserted in the list, 
        /// the element will be added as last element.
        /// </param>
        public void AddSubPartAfter(RenderElement subPart, RenderElement previousSibling)
        {
            if (Subparts != null && Subparts.Contains(subPart)) return;

            if (Subparts == null) Subparts = new LinkedList<RenderElement>();
            if (Subparts.Contains(previousSibling))
            {
                Subparts.AddAfter(Subparts.Find(previousSibling), subPart);
                subPart.SetParent(this);
                updateBoundingBox(subPart);
            }
            else
            {
                AddSubPart(subPart);
            }
        }

        /// <summary>
        /// Adds a subpart before a specific one.
        /// </summary>
        /// <param name="subPart">The element to add.</param>
        /// <param name="nextSibling">An element to add after this. 
        /// If the element does not exist or is not inserted in the list, 
        /// the element will be added as first element.
        /// </param>
        public void AddSubPartBefore(RenderElement subPart, RenderElement nextSibling)
        {
            if (Subparts != null && Subparts.Contains(subPart)) return;

            if (Subparts == null) Subparts = new LinkedList<RenderElement>();
            if (Subparts.Contains(nextSibling))
            {
                Subparts.AddBefore(Subparts.Find(nextSibling), subPart);
            }
            else
            {
                Subparts.AddFirst(subPart);
            }
            subPart.SetParent(this);
            updateBoundingBox(subPart);
        }

        /// <summary>
        /// Removes a subpart from the list of subparts.
        /// </summary>
        /// <param name="subPart">the subpart to remove</param>
        /// <returns><c>true</c> if the list of suparts does not contain 
        /// the element anymore.</returns>
        public bool RemoveSubPart(RenderElement subPart)
        {
            if (Subparts != null && Subparts.Contains(subPart))
            {
                bool succ = Subparts.Remove(subPart);
                subPart.DeleteParent();
                if (succ)
                {
                    updateBoundingBox();
                }
                return succ;
            }

            return true;
        }

        /// <summary>
        /// Check if the SubPart List contains the requested Element
        /// </summary>
        /// <param name="needle">The element to search for.</param>
        /// <param name="subPart">The found linked Element as output parameter or <c>null</c></param>
        /// <returns><c>true</c> if the element is in the sub part list, otherwise <c>false</c></returns>
        public bool ContainsSubPart(RenderElement needle, out LinkedListNode<RenderElement> subPart)
        {
            subPart = null;
            if (Subparts != null && Subparts.Contains(needle))
            {
                subPart = Subparts.Find(needle);
            }
            return false;
        }

        #endregion

        #region BoundingBox Updating

        /// <summary>
        /// Update the bounding box of this element depending 
        /// on the bounding boxes of all subparts.
        /// </summary>
        private void updateBoundingBox()
        {
            if (Subparts != null && Subparts.Count > 0)
            {
                if (Subparts.Count > 1)
                {
                    var subL = Subparts.First;
                    if (subL != null)
                    {
                        updateBoundingBox(subL.Value);
                    }
                }
                else
                {
                    var subL = Subparts.First;
                    if (subL != null)
                    {
                        RenderElement sub = subL.Value;
                        X = sub.X;
                        Y = sub.Y;
                        Width = sub.Width;
                        Height = sub.Height;
                    }
                }
            }
        }

        /// <summary>
        /// Update the current bounding box of this
        /// element with the given element's bounding box
        /// </summary>
        /// <param name="newElement">the new element which bounding box
        /// should be included in this element's bounding box.</param>
        private void updateBoundingBox(RenderElement newElement)
        {
            // width --> calculate the right border position
            int oldRight = X + Width;
            int newRight = newElement.X + newElement.Width;
            Width += Math.Max(0, newRight - oldRight);

            // update position
            X = Math.Min(X, newElement.X);

            // height --> calculate the bottom border position
            int oldBottom = Y + Height;
            int newBottom = newElement.Y + newElement.Height;
            Height += Math.Max(0, newBottom - oldBottom);

            // update position
            Y = Math.Min(Y, newElement.Y);
        }

        /// <summary>
        /// Shifts the element in vertical and/or horizontal direction.
        /// </summary>
        /// <param name="x">The horizontal position change (x).</param>
        /// <param name="y">The vertical position change (y).</param>
        public void ShiftElement(int x, int y)
        {
            // set the x and y values
            X += x;
            Y += y;

            if (HasSubParts())
            {
                foreach (var item in Subparts)
                {
                    item.ShiftElement(x, y);
                }
            }
        }

        #endregion

        #region Value

        private bool _isNotEmpty;
        /// <summary>
        /// Determines whether this instance is empty.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEmpty()
        {
            return !_isNotEmpty; 
        }

        /// <summary>
        /// Gets the value object the rendering result is based on.
        /// </summary>
        /// <returns>An object that is transformed into the bool matrix by a renderer.</returns>
        public Object GetValue()
        {
            return Value;

            //TODO: combine the value out of the subparts?
        }

        #endregion

        #region Parent

        /// <summary>
        /// Determines whether this instance has a parent.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance has parent; otherwise, <c>false</c>.
        /// </returns>
        public bool HasParent()
        {
            return Parent != null && Parent is RenderElement;
        }

        /// <summary>
        /// Get the parent if one was set.
        /// </summary>
        /// <returns>The parent element as (a <see cref="RenderElement"/>) if one was set, otherwise <c>null</c></returns>
        public Object GetParent()
        {
            return HasParent() ? Parent : null;
        }

        /// <summary>
        /// Set the parent element for this element.
        /// This does not register this element to the parent's subpart list - you have to do this manually!
        /// This function is called when this element is added to another <see cref="RenderElement"/> subpart list.
        /// </summary>
        /// <param name="parent"></param>
        public void SetParent(RenderElement parent)
        {
            if (parent.Equals(this))
            {
                throw new ArgumentException("It is not allowed to set the element to be his own parent!", "parent");
            }
            Parent = parent;
        }

        /// <summary>
        /// Deletes the parent.
        /// Does not remove this element from the deleted parent's subpart list - you have to do this manually!
        /// This function is called when this element is removed from another <see cref="RenderElement"/> subpart list.
        /// </summary>
        public void DeleteParent()
        {
            Parent = null;
        }

        #endregion

        #region Override

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (!String.IsNullOrEmpty(DisplayName)) return DisplayName;
            return "RenderElement '" + Value.ToString() + "' (" + Type.ToString() + ") - BBox [X:" + X + ", Y:" + Y + ", Width:" + Width + ", Height:" + Height + "]";
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj is RenderElement
                && ((((RenderElement)obj).GetValue() != null && ((RenderElement)obj).GetValue().Equals(this.Value)) || this.Value == null)
                && ((RenderElement)obj).X == X
                && ((RenderElement)obj).Y == Y
                && ((RenderElement)obj).Width == Width
                && ((RenderElement)obj).Height == Height
                && ((RenderElement)obj).Type == Type;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

    }
}