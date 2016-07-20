
using System.Collections.Generic;
using Gestures.Geometrie.Vertex;

namespace Gestures.Geometrie.KdTree
{
    /// <summary>
    /// Implementation for a KD-Tree structure
    /// </summary>
    public class KdTree
    {
        #region private fields

        #region Collections
        System.Collections.Generic.IList<Vertex.Vertex> vertexes;
        System.Collections.Generic.List<int> perm;
        System.Collections.Generic.List<KdNode> bucketptr;
        System.Collections.Generic.List<double> radii;
        System.Collections.Generic.List<bool> included;

        System.Collections.Generic.Dictionary<int, int> idNumberAssignment = 
            new System.Collections.Generic.Dictionary<int, int>();
        #endregion

        #region Building
        KdNode root;
        int dimension, cutoff, bndslevel;
        #endregion

        #region Distances
        DistanceDelegate<Vertex.Vertex, Vertex.Vertex>
            euclideanDistance = delegate(Vertex.Vertex a, Vertex.Vertex b) { return MetricDistances.EuclideanDistance(a,b);};
        DistanceDelegate<Vertex.Vertex, Vertex.Vertex>
            sqEuclideanDistance = delegate(Vertex.Vertex a, Vertex.Vertex b) { return MetricDistances.SquaredEuclideanDistance(a, b); };
        #endregion

        #region Searching
        int nntarget, nnptnum;
        double nndist, nndist2;
        #endregion

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="KdTree"/> class.
        /// </summary>
        /// <param name="elements">The elements of the tree.</param>
        /// <param name="dimension">The dimension of the tree.</param>
        /// <param name="cutoff">The cutoff.</param>
        /// <param name="bndslevel">The bndslevel.</param>
        public KdTree(IList<Vertex.Vertex> elements, int dimension, int cutoff, int bndslevel)
        {
            this.vertexes = elements;
            this.perm = new List<int>(vertexes.Count);
            this.bucketptr = new List<KdNode>(vertexes.Count);
            this.radii = new List<double>(vertexes.Count);
            this.included = new List<bool>(vertexes.Count);
            this.dimension = elements.Count > 0 ? 2 : 0;
            this.cutoff = cutoff;
            this.bndslevel = bndslevel;
            this.Populate();
        }

        #region Building

        private void Populate()
        {
            for (int i = 0; i < vertexes.Count; ++i)
            {
                perm.Add(i);
                bucketptr.Add(null);
                radii.Add(0.0);
                included.Add(true);
                idNumberAssignment.Add(vertexes[i].Num,i);
            }
            List<double> bounds = new List<double>(2 * dimension);
            for (int i = 0; i < dimension; i++) { bounds.Add(double.MinValue); bounds.Add(double.MaxValue); }
            root = Build(0, vertexes.Count - 1, null, 0, bounds);
        }        

        private KdNode Build(int l, int u, KdNode father, int depth, List<double> bounds)
        {
            KdNode p = new KdNode(depth);
            p.Father = father;
            p.Bound = bounds;
            #region for displaying only
            p.LoPt = l;
            p.HiPt = u;
            #endregion
            if (u - l + 1 <= cutoff)
            {
                p.Bucket = true;
                //indices of perm
                p.LoPt = l;
                p.HiPt = u;
                for (int i = l; i <= u; i++)
                {
                    bucketptr[perm[i]] = p;

                }
            }
            else
            {
                p.Bucket = false;
                p.Empty = false;
                p.CutDim = FindMaxSpread(l, u);
                int m = Select(l, u, (l + u) / 2, p.CutDim);
                p.CutVal = (vertexes[perm[m]][p.CutDim] + vertexes[perm[m + 1]][p.CutDim]) / 2;

                List<double> bound1 = new List<double>(2 * dimension);
                List<double> bound2 = new List<double>(2 * dimension);
                for (int i = 0; i < bounds.Count; i++)
                {
                    bound1.Add(bounds[i]);
                    bound2.Add(bounds[i]);
                }
                bound1[2 * p.CutDim + 1] = p.CutVal;
                p.LoSon = Build(l, m, p, depth + 1, bound1);
                bound1[2 * p.CutDim] = p.CutVal;
                p.HiSon = Build(m + 1, u, p, depth + 1, bound2);
            }
            return p;
        }

        private int FindMaxSpread(int l, int u)
        {
            int i;
            double min = vertexes[perm[l]][0], max = min, temp;
            for (i = l + 1; i <= u; i++)
            {
                if (vertexes[perm[i]][0] < min)
                    min = vertexes[perm[i]][0];
                if (vertexes[perm[i]][0] > max)
                    max = vertexes[perm[i]][0];
            }
            temp = max - min;
            min = vertexes[perm[l]][1]; max = min;
            for (i = l + 1; i <= u; i++)
            {
                if (vertexes[perm[i]][1] < min)
                    min = vertexes[perm[i]][1];
                if (vertexes[perm[i]][1] > max)
                    max = vertexes[perm[i]][1];
            }
            if (max - min < temp)
                return 0;
            else return 1;
        }

        private int Select(int l, int u, int m, int cutdim)
        {
            //TODO: create to vertexcomparer in advance
            perm.Sort(l, u - l + 1, new VertexCoordComparer(vertexes, cutdim));

            int j = 0, i = 0;
            while (m + i + 1 < u && vertexes[perm[m]][cutdim] == vertexes[perm[m + i + 1]][cutdim])
            {
                ++i;
            }
            while (m - j - 2 > l && vertexes[perm[m]][cutdim] == vertexes[perm[m - j - 1]][cutdim])
            {
                ++j;
            }
            if (i > j) m = m - j - 1; else m = m + i;
            return m;
        }

        private int Partition(IList<int> perm, double b, int dim)
        {
            int i = 0, j = perm.Count - 1;
            do
            {
                do { j--; } while (vertexes[perm[j]][dim] > b);
                do { i++; } while (vertexes[perm[i]][dim] < b);
                if (i < j)
                {
                    int temp = perm[i];
                    perm[i] = perm[j];
                    perm[j] = temp;
                }
            }
            while (i < j);
            return j;
        }

        //returns index of median vertex
        int Find_ith_element(List<int> perm, int dim, int l, int u, int i)
        {
            // SortRange(moduleRegions, u-moduleRegions+1, new VertexCoordComparer(vertexes, dim));

            return perm[i];
            //   IComparer<int> comparer = new VertexCoordComparer(vertexes, dim);
            //   int eleCount = u - moduleRegions + 1;
            //   if (eleCount < 6)
            //   {
            //       SortRange(moduleRegions,u,comparer);
            //       return perm[yOffset];
            //   }
            //   int pinMatrix = (eleCount) / 5, yOffset, t1=moduleRegions, t2=t1+4;
            //   IList<int> tempArray = new List<int>(pinMatrix + 1);
            //   for(yOffset=0;yOffset<pinMatrix;yOffset++)
            //   {
            //       //SortRange(t1,t2,comparer);
            //       tempArray[yOffset]=perm[t1+2];
            //       t1=t1+5;t2=t1+4;        
            //   }
            //   if(t1<=u)
            //   {
            //      //     qst_perm(perm,coordmat,dim,t1,u);
            //       tempArray[yOffset]=perm[t1+(u-t1+1)/2];
            //       pinMatrix++;

            //   }
            ////   else if (t1 == u) { tempArray[yOffset] = perm[t1]; }
            //  // else { pinMatrix--; }

            //   pinMatrix = find_ith_element(tempArray, dim, 0, pinMatrix, pinMatrix / 2);
            //   pinMatrix = partition(perm,vertexes[pinMatrix][dim], dim);

            //   if(yOffset<=pinMatrix) return find_ith_element(perm,dim,moduleRegions,pinMatrix,yOffset);
            //   //else return find_ith_element(perm,coordmat,dim,pinMatrix+1,u,yOffset-pinMatrix);
            //   return 0;
        }

        #region Sorting

        private void SortRange(int l, int u, IComparer<int> comparer)
        {
            perm.Sort(l, u - l + 1, comparer);
        }

        private class VertexCoordComparer : IComparer<int>
        {
            System.Collections.Generic.IList<Vertex.Vertex> vertexes;
            int dimension;
            public VertexCoordComparer(System.Collections.Generic.IList<Vertex.Vertex> vertexes, int dimension)
            {
                this.vertexes = vertexes;
                this.dimension = dimension;
            }
            public int Compare(int a, int b)
            {
                if (a == b) return 0;
                //{
                //    if (vertexes[a][(dimension + 1) % 2] == vertexes[b][(dimension + 1) % 2])
                //    { return 0; }
                //    return vertexes[a][(dimension + 1) % 2] < vertexes[b][(dimension + 1) % 2] ? -1 : 1;
                //}
                return vertexes[a][dimension] < vertexes[b][dimension] ? -1 : 1;

            }
        }

        #endregion

        #endregion

        #region Searching

        /// <summary>
        /// Try to find the nearest neighbor in the tree top down.
        /// </summary>
        /// <typeparam name="Vertex">The type of the vertex.</typeparam>
        /// <param name="target">The target to start search.</param>
        /// <returns>The nearest neighbor.</returns>
        public int TopDownNearestNeighbour<Vertex>(Vertex target) where Vertex : struct, IVertex
        {
            nndist = double.MaxValue;
            nnptnum = -1;
            RecursiveTopDownNearestNeighbour(root, target);

            return nnptnum == -1 ? nnptnum : vertexes[nnptnum].Num;
        }

        private void RecursiveTopDownNearestNeighbour<Vert>(KdNode node, Vert target) where Vert : struct, IVertex
        {
            double thisDist, thisX, val;
            if (node.Bucket)
            {
                for (int i = node.LoPt; i <= node.HiPt; i++)
                {
                    thisDist = euclideanDistance(vertexes[perm[i]], new Vertex.Vertex(target[0], target[1]));
                    if (thisDist < nndist)
                    {
                        nndist = thisDist;
                        nnptnum = perm[i];
                    }
                }
            }
            else
            {
                val = node.CutVal;
                thisX = target[node.CutDim];
                if (thisX < val)
                {
                    RecursiveTopDownNearestNeighbour(node.LoSon, target);
                    if (thisX + nndist > val)
                    {
                        RecursiveTopDownNearestNeighbour(node.HiSon, target);
                    }
                }
                else
                {
                    RecursiveTopDownNearestNeighbour(node.HiSon, target);
                    if (thisX - nndist < val)
                    {
                        RecursiveTopDownNearestNeighbour(node.LoSon, target);
                    }
                }
            }
        }

        private int TopDownNearestNeighbour(int j)
        {
            nntarget = j;
            nndist = double.MaxValue;
            RecursiveNearestNeighbour(root);
            return nnptnum;
        }

        private void RecursiveNearestNeighbour(KdNode node)
        {
            double thisDist, thisX, val;
            if (node.Bucket)
            {
                for (int i = node.LoPt; i <= node.HiPt; i++)
                {
                    //exclude target from search
                 //   if (perm[i] == nntarget) { continue; }
                    thisDist = euclideanDistance(vertexes[perm[i]], vertexes[nntarget]);
                    if (thisDist < nndist)
                    {
                        nndist = thisDist;
                        nnptnum = perm[i];
                    }
                }
            }
            else
            {
                val = node.CutVal;
                thisX = vertexes[nntarget][node.CutDim];
                if (thisX < val)
                {
                    RecursiveNearestNeighbour(node.LoSon);
                    if (thisX + nndist > val)
                    {
                        RecursiveNearestNeighbour(node.HiSon);
                    }
                }
                else
                {
                    RecursiveNearestNeighbour(node.HiSon);
                    if (thisX - nndist < val)
                    {
                        RecursiveNearestNeighbour(node.LoSon);
                    }
                }
            }
        }

        private void RecursiveSetRad(KdNode node, int ptnum, double rad, bool delete)
        {
            if (node.Bucket)
            {
                if (delete)
                {
                    node.IntersectingBalls.Remove(node.IntersectingBalls.Find(ptnum));
                }
                else
                {
                    node.IntersectingBalls.AddLast(ptnum);
                }
            }
            else
            {
                double diff = vertexes[ptnum][node.CutDim] - node.CutVal;
                if (diff < 0.0)
                {
                    RecursiveSetRad(node.LoSon, ptnum, rad, delete);
                    if (rad >= -diff) { RecursiveSetRad(node.HiSon, ptnum, rad, delete); }
                }
                else
                {
                    RecursiveSetRad(node.HiSon, ptnum, rad, delete);
                    if (rad >= diff) { RecursiveSetRad(node.LoSon, ptnum, rad, delete); }

                }
            }
        }

        #region Buttom-Up

        /// <summary>
        /// Sets the radius for bottom up search.
        /// </summary>
        /// <param name="nntarget">The nntarget.</param>
        /// <param name="newRad">The new RAD.</param>
        public void SetRad(int nntarget, double newRad)
        {
            SetRad(nntarget, radii[nntarget], true);
            SetRad(nntarget, newRad, false);
        }

        private void SetRad(int target, double rad, bool delete)
        {
            nndist2 = rad * rad;
            KdNode node = bucketptr[target];
            RecursiveSetRad(node, target, rad, delete);
            while (true)
            {
                KdNode lastNode = node;
                node = node.Father;
                if (node == null) { break; }
                double diff = vertexes[target][node.CutDim] - node.CutVal;
                if (nndist2 >= diff * diff)
                {
                    if (lastNode == node.LoSon)
                    {
                        RecursiveSetRad(node.HiSon, target, rad, delete);
                    }
                    else
                    {
                        RecursiveSetRad(node.LoSon, target, rad, delete);
                    }
                }
                if (node.Depth % this.bndslevel == 0 && BallInBounds(node.Bound, target, nndist2))
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Try to get the Nearest neighbor in bottom up search.
        /// </summary>
        /// <param name="j">The start position.</param>
        /// <returns>the nearest neighbor in bottom up search.</returns>
        public int NearestNeighbour(int j)
        {
            nntarget = j;
            nndist2 = double.MaxValue;
            nndist = double.MaxValue;
            nnptnum = -1;
            KdNode node = bucketptr[nntarget];
            RecursiveNearestNeighbour(node);
            while (true)
            {
                KdNode lastNode = node;
                node = node.Father;
                if (node == null) { break; }
                double diff = vertexes[nntarget][node.CutDim] - node.CutVal;
                if (nndist2 >= diff * diff)
                {
                    if (lastNode == node.LoSon)
                    {
                        RecursiveNearestNeighbour(node.HiSon);
                    }
                    else
                    {
                        RecursiveNearestNeighbour(node.LoSon);
                    }
                }
                if (node.Depth % this.bndslevel == 0 && BallInBounds(node.Bound, nntarget, nndist2))
                {
                    break;
                }

            }
            return nnptnum == -1 ? nnptnum : vertexes[nnptnum].Num;
        }

        //bounds contains pairwise dimension-ordered coordinates
        //bounds[0], bounds[1] = x1,x2
        //bounds[2], bounds[3] = y1,y2
        private bool BallInBounds(List<double> bounds, int pointIndex, double dist)
        {
            for (int i = 0; i < bounds.Count / 2; i++)
            {
                if (!(bounds[i] <= vertexes[pointIndex][i] - dist && bounds[i + 1] >= vertexes[pointIndex][i] + dist))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Try to find the nearest neighbor in bottom up search but with a fixes search radius..
        /// </summary>
        /// <param name="ptnum">The start point.</param>
        /// <param name="rad">The maximum radius.</param>
        /// <returns>Th nearest neighbor in bottom up search with fixed search radius.</returns>
        public List<int> FixedRadiusNearestNeighbour(int ptnum, double rad)
        {
            List<int> results = new List<int>();
            nntarget = ptnum;
            nndist = rad;
            nndist2 = rad * rad;
            KdNode p = bucketptr[nntarget];
            RecursiveFixedRadiusNN(p, results);
            while (true)
            {
                KdNode lastp = p;
                p = p.Father;
                if (p == null) { break; }
                double diff = vertexes[nntarget][p.CutDim] - p.CutVal;
                if (lastp == p.LoSon)
                {
                    if (nndist >= -diff)
                    {
                        RecursiveFixedRadiusNN(p.HiSon, results);
                    }
                }
                else
                {
                    if (nndist >= diff)
                    {
                        RecursiveFixedRadiusNN(p.LoSon, results);
                    }
                }
                if (p.Depth % bndslevel == 0 && BallInBounds(p.Bound, nntarget, nndist))
                { break; }
            }
            for (int i = 0; i < results.Count; i++)
            {
                results[i] = vertexes[results[i]].Num;
            }
            return results;
        }

        private void RecursiveFixedRadiusNN(KdNode node, List<int> results)
        {
            if (node.Empty) { return; }
            if (node.Bucket)
            {
                for (int i = node.LoPt; i <= node.HiPt; i++)
                {
                    if (euclideanDistance(vertexes[perm[i]], vertexes[nntarget]) * euclideanDistance(vertexes[perm[i]], vertexes[nntarget]) <= nndist2)
                    {
                        results.Add(perm[i]);
                    }
                }
            }
            else
            {
                double diff = vertexes[nntarget][node.CutDim] - node.CutVal;
                if (diff < 0.0)
                {
                    RecursiveFixedRadiusNN(node.LoSon, results);
                    if (nndist >= -diff)
                    {
                        RecursiveFixedRadiusNN(node.HiSon, results);
                    }
                }
                else
                {
                    RecursiveFixedRadiusNN(node.HiSon, results);
                    if (nndist >= diff)
                    {
                        RecursiveFixedRadiusNN(node.LoSon, results);
                    }
                }
            }
        }

        /// <summary>
        /// Performs a Ball search
        /// </summary>
        /// <param name="j">The start point.</param>
        /// <returns></returns>
        public List<int> BallSearch(int j)
        {
            List<int> results = new List<int>();
            KdNode node = bucketptr[j];
            foreach (int i in node.IntersectingBalls)
            {
                if (euclideanDistance(vertexes[i], vertexes[j]) <= radii[i])
                {
                    results.Add(i);
                }
            }
            for (int i = 0; i < results.Count; i++)
            {
                results[i] = vertexes[results[i]].Num;
            }
            return results;
        }


        #endregion
        
        #endregion

        #region Un-/Deleting

        /// <summary>
        /// Deletes the specified point from the tree.
        /// </summary>
        /// <param name="pointNum">The point number to delete.</param>
        public void Delete(int pointNum)
        {
            pointNum = idNumberAssignment[pointNum];

            included[pointNum] = false;
            KdNode node = bucketptr[pointNum];
            int j = node.LoPt;
            while (perm[j] != pointNum)
            { j++; }
            if (j <= node.HiPt)
            {
                //swap yOffset and hipt
                #region
                int temp = perm[j];
                perm[j] = perm[node.HiPt];
                perm[node.HiPt] = temp;
                #endregion
                node.HiPt--;
                if (node.LoPt > node.HiPt)
                {
                    node.Empty = true;
                    while ((node = node.Father) != null && node.LoSon.Empty && node.HiSon.Empty)
                    {
                        node.Empty = true;
                    }
                }
            }
        }

        /// <summary>
        /// Undo a delete operation.
        /// </summary>
        /// <param name="pointNum">The point number.</param>
        public void Undelete(int pointNum)
        {
            pointNum = idNumberAssignment[pointNum];
            included[pointNum] = true;
            KdNode node = bucketptr[pointNum];
            int j = node.LoPt;
            while (perm[j] != pointNum)
            { j++; }
            if (j > node.HiPt)
            {
                node.HiPt++;
                //swap yOffset and hipt
                #region
                int temp = perm[j];
                perm[j] = perm[node.HiPt];
                perm[node.HiPt] = temp;
                #endregion
                if (node.Empty)
                {
                    node.Empty = false;
                    while ((node = node.Father) != null && node.Empty)
                    {
                        node.Empty = false;
                    }
                }

            }
        }

        #endregion

        #region Informative

        /// <summary>
        /// Check if the tree contains a specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns><c>true</c> if the index is included; otherwise <c>false</c>.</returns>
        public bool Included(int index)
        {
            return included[idNumberAssignment[index]];
        }
        /// <summary>
        /// Gets the amount of vertexes of this tree.
        /// </summary>
        /// <value>
        /// The count of vertexes.
        /// </value>
        public int Count { get { return vertexes.Count; } }
        /// <summary>
        /// Gets the dimension of this tree.
        /// </summary>
        /// <value>
        /// The dimension.
        /// </value>
        public int Dimension { get { return dimension; } }
        /// <summary>
        /// Gets the cut off value of this tree.
        /// </summary>
        /// <value>
        /// The cut off value.
        /// </value>
        public int CutOffValue { get { return cutoff; } }
        /// <summary>
        /// Gets the level for a bounds check.
        /// </summary>
        /// <value>
        /// The level for a bounds check.
        /// </value>
        public int Level4BoundsCheck { get { return bndslevel; } }
        /// <summary>
        /// Gets the <see cref="Vertex.Vertex"/> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="Vertex.Vertex"/>.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public Vertex.Vertex this[int index]
        {
            get { return vertexes[idNumberAssignment[index]]; }
        }

        #endregion

        #region Drawing
        /*
        public void Representation(System.Drawing.Graphics g, int x, int y, int width, int height, int scale, System.Windows.Forms.PictureBox pb, System.Drawing.Bitmap b)
        {
            Representation(root, g, x, y, width, height, scale, pb, b);
        }
        private void Representation(KdNode node, System.Drawing.Graphics g, int x, int y, int width, int height, int scale, System.Windows.Forms.PictureBox pb, System.Drawing.Bitmap b)
        {
            if (node.Bucket)
            {
                //for (int yOffset = node.LoPt; yOffset <= node.HiPt; yOffset++)
                //{
                //    if (col == 0)
                //    {
                //        g.DrawLine(new System.Drawing.Pen(System.Drawing.Brushes.Red,4), (float)vertexes[perm[yOffset]][0] * scale, (float)vertexes[perm[yOffset]][1] * scale, (float)vertexes[perm[yOffset]][0] * scale + 1, (float)vertexes[perm[yOffset]][1] * scale + 1);
                //    }
                //    else
                //    {
                //        g.DrawLine(new System.Drawing.Pen(System.Drawing.Brushes.Blue,4), (float)vertexes[perm[yOffset]][0] * scale, (float)vertexes[perm[yOffset]][1] * scale, (float)vertexes[perm[yOffset]][0] * scale + 1, (float)vertexes[perm[yOffset]][1] * scale + 1);
                //    }
                //}
                //col = (col == 0 ? 1 : 0);
            }
            else
            {
                pb.Image = b;
                int row = node.LoPt;
                while (vertexes[perm[row]][node.CutDim] <= node.CutVal)
                {
                    g.DrawLine(new System.Drawing.Pen(System.Drawing.Brushes.Red, 4), (float)vertexes[perm[row]][0] * scale, (float)vertexes[perm[row]][1] * scale, (float)vertexes[perm[row]][0] * scale + 1, (float)vertexes[perm[row]][1] * scale + 1);
                    row++;
                }
                while (row <= node.HiPt)
                {
                    g.DrawLine(new System.Drawing.Pen(System.Drawing.Brushes.Blue, 4), (float)vertexes[perm[row]][0] * scale, (float)vertexes[perm[row]][1] * scale, (float)vertexes[perm[row]][0] * scale + 1, (float)vertexes[perm[row]][1] * scale + 1);
                    row++;
                }


                if (node.CutDim == 0) //vertical cut
                {
                    g.DrawLine(new System.Drawing.Pen(System.Drawing.Brushes.Black, 2), (float)node.CutVal * scale, y * scale,
                        (float)node.CutVal * scale, (y + height) * scale);

                    Representation(node.LoSon, g, x, y, (int)(node.CutVal - x), height, scale, pb, b);
                    Representation(node.HiSon, g, (int)node.CutVal, y, (int)(x + width - node.CutVal), height, scale, pb, b);
                }
                else //horizontal cut
                {
                    g.DrawLine(new System.Drawing.Pen(System.Drawing.Brushes.Black, 2), x * scale, (float)node.CutVal * scale,
                         (x + width) * scale, (float)node.CutVal * scale);

                    Representation(node.LoSon, g, x, y, width, (int)(node.CutVal - y), scale, pb, b);
                    Representation(node.HiSon, g, x, (int)node.CutVal, width, (int)(y + height - node.CutVal), scale, pb, b);

                }
            }
        }
        */
        #endregion
    }

    /// <summary>
    /// Class of a node inside a KD-tree
    /// </summary>
    public class KdNode
    {
        int depth;
        LinkedList<int> intersectingBalls;
        /// <summary>
        /// Initializes a new instance of the <see cref="KdNode"/> class.
        /// </summary>
        /// <param name="depth">The depth.</param>
        public KdNode(int depth)
        {
            this.depth = depth;
            intersectingBalls = new LinkedList<int>();

        }

        /// <summary>
        /// Gets the cut off dimension of this node.
        /// </summary>
        /// <value>
        /// The cut dim.
        /// </value>
        public int CutDim { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="KdNode"/> is bucket.
        /// </summary>
        /// <value>
        ///   <c>true</c> if bucket; otherwise, <c>false</c>.
        /// </value>
        public bool Bucket { get; set; }
        /// <summary>
        /// Gets or sets the low point.
        /// </summary>
        /// <value>
        /// The low point.
        /// </value>
        public int LoPt { get; set; }
        /// <summary>
        /// Gets or sets the high point.
        /// </summary>
        /// <value>
        /// The high point.
        /// </value>
        public int HiPt { get; set; }
        /// <summary>
        /// Gets the cut off value of this node.
        /// </summary>
        /// <value>
        /// The cut off value.
        /// </value>
        public double CutVal { get; set; }
        /// <summary>
        /// Gets or sets the lower son.
        /// </summary>
        /// <value>
        /// The lower son.
        /// </value>
        public KdNode LoSon { get; set; }
        /// <summary>
        /// Gets or sets the higher son.
        /// </summary>
        /// <value>
        /// The higher son.
        /// </value>
        public KdNode HiSon { get; set; }
        /// <summary>
        /// Gets or sets the father of this node.
        /// </summary>
        /// <value>
        /// The father.
        /// </value>
        public KdNode Father { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="KdNode"/> is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if empty; otherwise, <c>false</c>.
        /// </value>
        public bool Empty { get; set; }
        /// <summary>
        /// Gets the depth.
        /// </summary>
        /// <value>
        /// The depth.
        /// </value>
        public int Depth { get { return depth; } }
        /// <summary>
        /// Gets or sets the bounds.
        /// </summary>
        /// <value>
        /// The bounds.
        /// </value>
        public List<double> Bound { get; set; }
        /// <summary>
        /// Gets the intersecting balls.
        /// </summary>
        /// <value>
        /// The intersecting balls.
        /// </value>
        public LinkedList<int> IntersectingBalls { get { return intersectingBalls; } }

    }
  
}