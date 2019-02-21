using BrailleIO.Structs;
using System;
using System.Collections.Generic;

namespace Gestures.Recognition.GestureData
{
    /// <summary>
    /// Touch contact on the surface, i.e. closed area or cluster of sensor values.
    /// </summary>
		/// <remarks> </remarks>
    /// <author>Dr. rer. nat. Michael Schmidt - Techniche Universität Dresden 2014.</author>
    public class _Touch
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Touch"/> class.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="id">The identifier.</param>
        /// <param name="x">The horizontal x position of the touch.</param>
        /// <param name="y">The vertical y position of the touch.</param>
        /// <param name="cx">The horizontal diameter of the touch.</param>
        /// <param name="cy">The vertical diameter of the touch.</param>
        /// <param name="intense">The intense of the sensory data.</param>
        public _Touch(int id, double x, double y, double cx, double cy, double intense)
        {
            this.ID = id;
            this.X = x;
            this.Y = y;
            this.DimX = cx;
            this.DimY = cy;
            this.Intense = intense;
        }
        /// <summary>
        /// The identifier
        /// </summary>
		/// <remarks> </remarks>
        public int ID;
        /// <summary>
        /// horizontal position
        /// </summary>
		/// <remarks> </remarks>
        public double X;
        /// <summary>
        /// vertical position
        /// </summary>
		/// <remarks> </remarks>
        public double Y;
        /// <summary>
        /// horizontal diameter
        /// </summary>
		/// <remarks> </remarks>
        public double DimX;
        /// <summary>
        /// vertical diameter
        /// </summary>
		/// <remarks> </remarks>
        public double DimY;
        /// <summary>
        /// The sensory intense data value
        /// </summary>
		/// <remarks> </remarks>
        public double Intense;
    }

    /// <summary>
    /// A sample of the sensor data, i.e. all current touches during one sampling step.
    /// </summary>
		/// <remarks> </remarks>
    /// <author>Dr. rer. nat. Michael Schmidt - Techniche Universität Dresden 2014.</author>
    public class Frame : IEnumerable<Touch>
    {
        IList<Touch> touches = new List<Touch>();
        IDictionary<int, Touch> dict = new Dictionary<int, Touch>();
        /// <summary>
        /// Initializes a new instance of the <see cref="Frame"/> class.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="timeStamp">The time stamp.</param>
        public Frame(DateTime timeStamp)
        {
            TimeStamp = timeStamp;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Frame"/> class.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="timeStamp">The time stamp.</param>
        /// <param name="touches">The list of touches inside this frame.</param>
        public Frame(DateTime timeStamp, params Touch[] touches)
        {
            TimeStamp = timeStamp;
            foreach (Touch t in touches)
            {
                AddTouch(t);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Frame"/> class.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="timeStamp">The time stamp.</param>
        /// <param name="touches">The matrix of touch-sensory data values.</param>
        public Frame(DateTime timeStamp, double[,] touches)
        {
            TimeStamp = timeStamp;
            if (touches != null && touches.GetLength(0) > 0 && touches.GetLength(1) > 0)
            {
                int id = 0;
                for (int x = 0; x < touches.GetLength(0); x++)
                {
                    for (int y = 0; y < touches.GetLength(1); y++)
                    {
                        if (touches[x, y] > 0)
                        {
                            AddTouch(new Touch(id, x, y, 1, 1, touches[x, y]));
                            id++;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Frame"/> class.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="sampleSet">The matrix of touch-sensory data values.</param>
        public Frame(double[,] sampleSet) : this(sampleSet, DateTime.Now){}

        /// <summary>
        /// Initializes a new instance of the <see cref="Frame"/> class.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="sampleSet">The matrix of touch-sensory data values.</param>
        /// <param name="timestamp">The time stamp.</param>
        public Frame(double[,] sampleSet, DateTime timestamp)
        {
            TimeStamp = timestamp;
            if (sampleSet != null)
            {
                var clusterer = new GestureRecognition.Clusterer(sampleSet.Length);
                var cluster = clusterer.Cluster(sampleSet, 4);

                foreach (var c in cluster.Values)
                {
                    //get blob extension in x and y axis
                    double cX = 1, cY = 1;
                    int count = 0;
                    double val = 0;
                    foreach (var m in c.ClusterSet.Values)
                    {
                        int x = m / sampleSet.GetLength(1);
                        int y = m % sampleSet.GetLength(1);
                        try
                        {
                            val += sampleSet[x, y];
                            count++;
                        }
                        catch { }
                        if (Math.Abs(x - c.Mean[0]) >= cX) cX = Math.Abs(x - c.Mean[0]);
                        if (Math.Abs(y - c.Mean[1]) >= cY) cY = Math.Abs(y - c.Mean[1]);
                    }
                    Touch t = new Touch(c.Id, c.Mean[0], c.Mean[1], cX, cY, val / count);
                    AddTouch(t);
                }
            }
        }

        /// <summary>
        /// Adds a touch to the frame.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="touch">The touch.</param>
        public void AddTouch(Touch touch)
        {
            if (touch != null)
            {                
                while (dict.ContainsKey(touch.ID))
                {
                    touch.ID++;
                }
                touches.Add(touch);
                dict.Add(touch.ID, touch); 
            }
        }

        /// <summary>
        /// Gets the count of touches inside this frame.
        /// </summary>
		/// <remarks> </remarks>
        /// <value>
        /// The count of touches.
        /// </value>
        public int Count { get { return touches.Count; } }
        /// <summary>
        /// Gets the <see cref="Touch"/> at the specified index.
        /// </summary>
		/// <remarks> </remarks>
        /// <value>
        /// The <see cref="Touch"/>.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public Touch this[int index] { get { return touches[index]; } }
        /// <summary>
        /// Gets or sets the time stamp.
        /// </summary>
		/// <remarks> </remarks>
        /// <value>
        /// The time stamp.
        /// </value>
        public DateTime TimeStamp { get; set; }
        /// <summary>
        /// Gets the specific touch.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="id">The identifier.</param>
        /// <returns>the requested touch or null</returns>
        public Touch GetTouch(int id)
        {
            Touch touch = null;
            if (dict.TryGetValue(id, out touch))
            {
                return touch;
            }
            return null;
        }

        #region IEnumerable<Touch> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
		/// <remarks> </remarks>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
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