using System.Collections.Generic;
using Gestures.Geometrie.Vertex;

namespace GestureRecognition
{
    public class Cluster
    {
        private int num;

        private IDictionary<int, int> clusterSet;

        public Cluster(int num)
            : base()
        {
            clusterSet = new Dictionary<int, int>();
            this.num = num;
        }

        public int Id { get { return num; } set { num = value; } }
        public Vertex Mean { get; set; }

        public IDictionary<int, int> ClusterSet
        { get { return clusterSet; } }


        public void Add(int element)
        {
            if (!clusterSet.ContainsKey(element))
            {
                clusterSet.Add(element, element);
            }
        }

        public void Remove(int element)
        {
            if (clusterSet.ContainsKey(element))
            {
                clusterSet.Remove(element);
            }
        }

        public void Merge(Cluster target)
        {
            foreach (int element in target.ClusterSet.Values)
            {
                Add(element);
            }
        }

        public void Diff(Cluster target)
        {
            foreach (int element in target.ClusterSet.Values)
            {
                Remove(element);
            }
        }

    }
}
