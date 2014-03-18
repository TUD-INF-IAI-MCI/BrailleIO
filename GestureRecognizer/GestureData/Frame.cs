﻿using System;
using System.Collections.Generic;

namespace Gestures.Recognition.GestureData
{
    /// <summary>
    /// Touch contact on the surface, i.e. closed area or cluster of sensor values.
    /// </summary>
    public class Touch
    {
        public Touch(int id, double x, double y, double cx, double cy, double intense)
        {
            this.id = id;
            this.x = x;
            this.y = y;
            this.cx = cx;
            this.cy = cy;
            this.intense = intense;
        }
        public int id;
        public double x;
        public double y;
        public double cx;
        public double cy;
        public double intense;
    }

    /// <summary>
    /// A sample of the sensor data, i.e. all current touches during one sampling step.
    /// </summary>
    public class Frame : IEnumerable<Touch>
    {
        IList<Touch> touches = new List<Touch>();
        IDictionary<int, Touch> dict = new Dictionary<int, Touch>();
        public Frame(DateTime timeStamp, params Touch[] touches)
        {
            TimeStamp = timeStamp;
            foreach(Touch t in touches)
            {
                AddTouch(t);
            }
        }

        public void AddTouch(Touch touch)
        {
            touches.Add(touch);
            dict.Add(touch.id,touch);
        }

        public int Count { get { return touches.Count; } }
        public Touch this[int index] { get { return touches[index]; } }
        public DateTime TimeStamp { get; set; }
        public Touch GetTouch(int id) {
            Touch touch = null;
            if (dict.TryGetValue(id, out touch))
            {
                return touch;
            }
            return null; 
        }

        #region IEnumerable<Touch> Members

        public IEnumerator<Touch> GetEnumerator()
        {
            return touches.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return touches.GetEnumerator();
        }

        #endregion
    }

}