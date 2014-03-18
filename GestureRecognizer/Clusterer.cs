using System.Collections.Generic;
using Gestures.Geometrie.KdTree;
using Gestures.Geometrie.Vertex;
using Gestures.Recognition.Preprocessing;
using Gestures.Recognition.GestureData;

namespace GestureRecognition
{
    /// <summary>
    /// Clusters sensor data made available to a braille display representation of type <typeparamref name="BrailleDisplay"/>.
    /// </summary>
    public class Clusterer
    {
        #region  private fields
        IDictionary<int, Cluster> currentClusterSet;
        IDictionary<int, Cluster> previousClusterSet;
        
        List<int> moduleRegions;
        List<int> equalRegions;

        int[] tempArray = new int[200];
        int[] old;

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Clusterer"/> class.
        /// </summary>
        public Clusterer(int size)
        {
            currentClusterSet = new Dictionary<int, Cluster>();
            previousClusterSet = new Dictionary<int,Cluster>();

            moduleRegions = new List<int>();
            equalRegions = new List<int>();


            for (int j = 0; j < size; j++)
            {
                equalRegions.Add(j);
                moduleRegions.Add(j);
            }
        }
        #endregion

        #region Segmentation

        #region Blob Exctraction
        private List<int> BlobExctraction(double[,] sensorMatrix, int connectivity, double threshold)
        {
            int regionCounter = 0;
            bool increment;
            for (int j = 0; j < sensorMatrix.Length; j++)
            {
                equalRegions[j] = j;
                moduleRegions[j] = j;
            }
            int i = 0;

            for (int row = 0; row < sensorMatrix.GetLength(0); row++)
            {
                for (int col = 0; col < sensorMatrix.GetLength(1); col++)
                {
                    double v = sensorMatrix[row, col];
                    increment = true;
                    int tempX,tempY;

                    tempX = row - 1; tempY = col - 1;
                    if ((row>0&&col>0) && CheckCriterion(v, sensorMatrix[row - 1, col - 1], threshold))//northwest                    
                    {
                        increment = false;
                        moduleRegions[i] = moduleRegions[tempX * sensorMatrix.GetLength(1) + tempY];
                    }
                    tempX = row - 1; tempY = col;
                    if ((row > 0) && CheckCriterion(v, sensorMatrix[row - 1, col], threshold))//north
                    {
                        if (!increment) { equalRegions[moduleRegions[tempX * sensorMatrix.GetLength(1) + tempY]] = equalRegions[moduleRegions[i]]; }
                        else
                        {
                            increment = false;
                            moduleRegions[i] = moduleRegions[tempX * sensorMatrix.GetLength(1) + tempY];
                        }
                    }
                    tempX = row - 1; tempY = col + 1;
                    if ((row > 0 && col < sensorMatrix.GetLength(1)-1) && CheckCriterion(v, sensorMatrix[row - 1, col + 1], threshold))//northeast
                    {
                        if (!increment)
                        { equalRegions[moduleRegions[tempX * sensorMatrix.GetLength(1) + tempY]] = equalRegions[moduleRegions[i]]; }
                        else
                        {
                            increment = false;
                            moduleRegions[i] = moduleRegions[tempX * sensorMatrix.GetLength(1) + tempY];
                        }
                    }
                    tempX = row; tempY = col - 1;
                    if (col>0 && CheckCriterion(v, sensorMatrix[row, col - 1], threshold))//west
                    {
                        if (!increment)
                        { equalRegions[moduleRegions[tempX * sensorMatrix.GetLength(1) + tempY]] = equalRegions[moduleRegions[i]]; }
                        else
                        {
                            increment = false;
                            moduleRegions[i] = moduleRegions[tempX * sensorMatrix.GetLength(1) + tempY];
                        }
                    }
                    if (increment)
                    {
                        moduleRegions[i] = regionCounter;
                        regionCounter++;
                    }
                    i++;
                }
            }
            for (int j = 0; j < i; j++)
            {
                equalRegions[j] = equalRegions[equalRegions[j]];
            }
            for (int j = 0; j < sensorMatrix.Length; j++)
            {
                moduleRegions[j] = equalRegions[moduleRegions[j]];
            }
            return moduleRegions;
        }
        private bool CheckCriterion(double m1, double m2, double threshold)
        {
            if (m2 == null) { return false; }
            return (!((m1 > threshold) ^ (m2 > threshold)));
        }
        #endregion

        /// <summary>
        /// Clusters the sensor data in the braille display representation regarding to some specified threshold.
        /// </summary>
        /// <param name="threshold">The threshold.</param>
        /// <returns></returns>
        public IDictionary<int, Cluster> Cluster(double[,] sensorMatrix, double threshold)
        {
            List<int> preSegmentedList = BlobExctraction(sensorMatrix, 8, threshold);

            currentClusterSet = new Dictionary<int, Cluster>();
            int clusterCount = 0;
            if(tempArray.Length < preSegmentedList.Count) tempArray = new int[preSegmentedList.Count]; 
            for (int i = 0; i < preSegmentedList.Count; i++)
            {
                tempArray[i] = i;
            }

            for (int i = 0; i < preSegmentedList.Count; i++)
            {
                if (sensorMatrix[i/sensorMatrix.GetLength(1), i % sensorMatrix.GetLength(1)] > threshold)
                {
                    if (!currentClusterSet.ContainsKey(tempArray[preSegmentedList[i]]))
                    {
                        tempArray[preSegmentedList[i]] = clusterCount;
                        currentClusterSet.Add(clusterCount, new Cluster(clusterCount));
                        clusterCount++;
                    }
                    if (currentClusterSet.Count > 0)
                    {
                        (currentClusterSet[tempArray[preSegmentedList[i]]]).Add(i);

                    }
                }
            }
            //for (int i = 0; i < currentClusterSet.Count; i++)
            //{
            //    foreach (int j in (currentClusterSet[i]).ClusterSet.Values)
            //    {
            //        sensorMatrix[j / sensorMatrix.GetLength(1), j % sensorMatrix.GetLength(0)] = (i + 1) * (100 / (currentClusterSet.Count + 1));
            //    }
            //}
            AddClusterMeans(sensorMatrix,currentClusterSet);
            return currentClusterSet;
        }

        private Vertex GetClusterMean(double[,] sensorMatrix, Cluster c)
        {
            Vertex mean = new Vertex(0, 0);
            int meanNum = 0;
            foreach (int i in c.ClusterSet.Values)
            {
                mean[0] += (double)(i / sensorMatrix.GetLength(1));
                mean[1] += (double)(i % sensorMatrix.GetLength(1));
                ++meanNum;
            }
            mean[0] /= meanNum;
            mean[1] /= meanNum;
            mean.Num = meanNum;
            return mean;
        }

        private void AddClusterMeans(double[,] sensorMatrix, IDictionary<int, Cluster> clusterList)
        {
            foreach (Cluster c in clusterList.Values)
            {
                c.Mean = GetClusterMean(sensorMatrix,c);
            }
        }

        ///// <summary>
        ///// Merges data within two clusters into one.
        ///// </summary>
        ///// <param name="target">The target.</param>
        ///// <param name="source">The source.</param>
        //public void MergeClusters(Cluster target, Cluster source)
        //{
        //    target.Merge(source);
        //    target.Mean = GetClusterMean(target);
        //}

        #endregion
    }
}