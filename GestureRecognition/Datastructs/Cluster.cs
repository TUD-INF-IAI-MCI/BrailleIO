using System.Collections.Generic;
using Gestures.Geometrie.Vertex;

namespace GestureRecognition
{
    /// <summary>
    /// Class for a complex cluster.
    /// </summary>
		/// <remarks> </remarks>
    /// <author>Dr. rer. nat. Michael Schmidt - Techniche Universität Dresden 2014.</author>
    public class Cluster
    {
        private int num;

        private IDictionary<int, int> clusterSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="Cluster"/> class.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="num">The numeric value.</param>
        public Cluster(int num)
            : base()
        {
            clusterSet = new Dictionary<int, int>();
            this.num = num;
        }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
		/// <remarks> </remarks>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get { return num; } set { num = value; } }
        /// <summary>
        /// Gets or sets the mean value.
        /// </summary>
		/// <remarks> </remarks>
        /// <value>
        /// The mean.
        /// </value>
        public Vertex Mean { get; set; }

        /// <summary>
        /// Gets the cluster set.
        /// </summary>
		/// <remarks> </remarks>
        /// <value>
        /// The cluster set.
        /// </value>
        public IDictionary<int, int> ClusterSet
        { get { return clusterSet; } }


        /// <summary>
        /// Adds the specified element to the cluster set.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="element">The id of the element to add.</param>
        public void Add(int element)
        {
            if (!clusterSet.ContainsKey(element))
            {
                clusterSet.Add(element, element);
            }
        }

        /// <summary>
        /// Removes the specified element from the cluster set.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="element">The id of the element to remove.</param>
        public void Remove(int element)
        {
            if (clusterSet.ContainsKey(element))
            {
                clusterSet.Remove(element);
            }
        }

        /// <summary>
        /// Merges the specified target into the cluster set.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="target">The target that should be merged into the cluster set.</param>
        public void Merge(Cluster target)
        {
            foreach (int element in target.ClusterSet.Values)
            {
                Add(element);
            }
        }

        /// <summary>
        /// Differences the specified target. Removes all elements of the target from the cluster set.
        /// </summary>
		/// <remarks> </remarks>
        /// <param name="target">The target that should be removed from the cluster set.</param>
        public void Diff(Cluster target)
        {
            foreach (int element in target.ClusterSet.Values)
            {
                Remove(element);
            }
        }

    }
}
