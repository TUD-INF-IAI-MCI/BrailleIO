
using System;
using System.Collections.Generic;

namespace Gestures.Geometrie.FibonacciHeap
{
    /// <summary>
    /// </summary>
    /// <author>Dr. rer. nat. Michael Schmidt - Techniche Universität Dresden 2014.</author>
    public class FibonacciHeap
    {
        private class FibNode
        {
            public FibNode(int key, int rank, int mark)
            {
                Father = null;
                Son = null;
                Last = this;
                Next = this;
                Rank = rank;
                Mark = mark;
                Key = key;
            }

            public FibNode Father { get; set; }
            public FibNode Son { get; set; }
            public FibNode Last { get; set; }
            public FibNode Next { get; set; }
            public int Rank { get; set; }
            public int Mark { get; set; }
            public int Key { get; set; }
        }



        FibNode[] nodeList; //references every node in heap
        double[] nodeValues; //tores the discrimination-criterion of the nodes
        int maxRank; //fheaps maximum rank(number of sons of a node) is smaller than the number of nodes in the tsp
        int count = 0; //number of nodes within fibonacci heap

        FibNode root = null;

        public void Initialize(int maxSize)
        {
            count = 0;
            root = null;
            nodeList = new FibNode[maxSize];
            nodeValues = new double[maxSize];
            for (int i = 0; i < maxSize; i++)
            {
                nodeValues[i] = double.MaxValue;
            }
            maxRank = GetMaxRank(maxSize);
        }

        public int Minimum { get { return root != null ? root.Key : -1; } }

        public int Count { get { return count; } }


        private int GetMaxRank(int size)
        {
            return (int)(Math.Log(size, 2) * 1.5) + 1; //maximum node-rank is 1.44... *log2(n)
        }

        private FibNode MeltFibHeap(FibNode fheap1, FibNode fheap2)
        {
            if (fheap1 == null)
            { return fheap2; }
            if (fheap2 == null)
            { return fheap1; }

            FibNode h1next, h2last;
            h1next = fheap1.Next;
            h2last = fheap2.Last;
            fheap1.Next = fheap2;
            fheap2.Last = fheap1;

            h1next.Last = h2last;
            h2last.Next = h1next;

            if (nodeValues[fheap1.Key] > nodeValues[fheap2.Key])
                return fheap2;
            else
                return fheap1;
        }

        public bool InsertKey(int key, double value)
        {
            if (nodeList[key] == null)
            {
                FibNode fheap2 = new FibNode(key, 0, 0);
                nodeValues[key] = value;
                nodeList[key] = fheap2;
                root = MeltFibHeap(root, fheap2);
                count++;
                return true;
            }
            return false;
        }

        private FibNode MeltHeapOrderedTree(FibNode fheap)
        {
            FibNode[] ranklist = new FibNode[maxRank];
            FibNode temp, tempnext, minptr, rltemp, tnext;
            int minkey;
            minkey = fheap.Key;
            minptr = fheap;
            temp = fheap;
            tempnext = temp.Next;

            do
            {
                temp = tempnext;
                tempnext = tempnext.Next;

                if (nodeValues[temp.Key] < nodeValues[minkey])
                {
                    minptr = temp;
                    minkey = temp.Key;
                }

                temp.Father = null;			//necessary because of possibly DeleteMin operation
                while (ranklist[temp.Rank] != null)		//melding the two trees, root with higher key becomes son of the other one
                {
                    rltemp = ranklist[temp.Rank];
                    if (nodeValues[temp.Key] < nodeValues[rltemp.Key])
                    {
                        //extract tree with smaller key from rootlist and...
                        rltemp.Last.Next = rltemp.Next;
                        rltemp.Next.Last = rltemp.Last;

                        //...insert this tree into sonlist of the other one

                        if (temp.Son != null)
                        {
                            tnext = temp.Son.Next;
                            temp.Son.Next = rltemp;
                            rltemp.Last = temp.Son;
                            rltemp.Next = tnext;
                            tnext.Last = rltemp;
                            temp.Son = temp.Son.Next; //added node becomes son
                        }
                        else
                        {
                            rltemp.Last = rltemp;
                            rltemp.Next = rltemp;
                            temp.Son = rltemp;
                        }

                        temp.Rank++;		//increment rank, because of new additional son
                        temp.Son.Mark = 0;	//setting node unmarked
                        temp.Son.Father = temp; //every node have to know his father

                        ranklist[(temp.Rank) - 1] = null; //delete entry in ranklist					
                    }
                    else
                    {

                        //extract tree with smaller key from rootlist and...
                        temp.Last.Next = temp.Next;
                        temp.Next.Last = temp.Last;
                        //...insert this tree into sonlist of the other one
                        if (rltemp.Son != null)
                        {
                            tnext = rltemp.Son.Next;
                            rltemp.Son.Next = temp;
                            temp.Last = rltemp.Son;
                            temp.Next = tnext;
                            tnext.Last = temp;

                            rltemp.Son = rltemp.Son.Next; //added node becomes son

                        }
                        else
                        {
                            temp.Last = temp;
                            temp.Next = temp;
                            rltemp.Son = temp;

                        }

                        rltemp.Rank++;		//increment rank, because of new additional son
                        rltemp.Son.Mark = 0;	//setting node unmarked					
                        rltemp.Son.Father = rltemp; //every node have to know his father
                        ranklist[temp.Rank] = null; //delete entry in ranklist
                        temp = rltemp;
                    }
                }
                ranklist[temp.Rank] = temp;	//entry in ranklist with pointer to root- node with index rank

            }
            while (tempnext.Key != fheap.Key); //test,vorher fheap.Last.Key


            if (fheap != temp)
            {
                temp = fheap;

                if (nodeValues[temp.Key] < nodeValues[minkey])
                {
                    minptr = temp;
                    minkey = temp.Key;
                }

                temp.Father = null;			//nessesary because of possibly DeleteMin operation

                while (ranklist[temp.Rank] != null)		//melding the two trees, root with higher key becomes son of the other one
                {
                    rltemp = ranklist[temp.Rank];
                    if (nodeValues[temp.Key] < nodeValues[rltemp.Key])
                    {
                        //extract tree with smaller key from rootlist and...
                        rltemp.Last.Next = rltemp.Next;
                        rltemp.Next.Last = rltemp.Last;

                        //...insert this tree into sonlist of the other one

                        if (temp.Son != null)
                        {
                            tnext = temp.Son.Next;
                            temp.Son.Next = rltemp;
                            rltemp.Last = temp.Son;
                            rltemp.Next = tnext;
                            tnext.Last = rltemp;

                            temp.Son = temp.Son.Next; //added node becomes son
                        }
                        else
                        {
                            rltemp.Last = rltemp;
                            rltemp.Next = rltemp;
                            temp.Son = rltemp;
                        }

                        temp.Rank++;		//increment rank, because of new additional son
                        temp.Son.Mark = 0;	//setting node unmarked
                        temp.Son.Father = temp; //every node have to know his father

                        ranklist[(temp.Rank) - 1] = null; //delete entry in ranklist	

                    }
                    else
                    {
                        //extract tree with smaller key from rootlist and...
                        temp.Last.Next = temp.Next;
                        temp.Next.Last = temp.Last;

                        //...insert this tree into sonlist of the other one
                        if (rltemp.Son != null)
                        {
                            tnext = rltemp.Son.Next;

                            rltemp.Son.Next = temp;
                            temp.Last = rltemp.Son;
                            temp.Next = tnext;
                            tnext.Last = temp;

                            rltemp.Son = rltemp.Son.Next; //added node becomes son

                        }
                        else
                        {
                            temp.Last = temp;
                            temp.Next = temp;
                            rltemp.Son = temp;

                        }

                        rltemp.Rank++;		//increment rank, because of new additional son
                        rltemp.Son.Mark = 0;	//setting node unmarked					
                        rltemp.Son.Father = rltemp; //every node have to know his father
                        ranklist[temp.Rank] = null; //delete entry in ranklist

                        temp = rltemp;

                    }


                }
                ranklist[temp.Rank] = temp;	//entry in ranklist with pointer to root- node with index rank
            }

            while (minptr.Father != null)
            {	//using fheaps for tsp possibly equal distances causes faults where minptr is not on top
                minptr = minptr.Father;
            }

            return minptr;
        }

        public int DeleteMin()
        {
            FibNode fheap = root;
            if (fheap == null)
                return -1;

            //store predecessor, successor and child of minimum element in fheap
            FibNode rlnext = fheap.Next;
            FibNode rllast = fheap.Last;
            FibNode son = fheap.Son;

            //delete entry of min- element in list of pointers 
            nodeList[fheap.Key] = null;
            count--;

            if (rllast == fheap)			//rootlist with only one element..
            {
                if (son == null)		//...and no sons
                {
                    root = null;
                }
                else		//...or with sons
                {
                    root = MeltHeapOrderedTree(son);
                }
                return fheap.Key;
            }
            else						//rootlist with more than one element
            {
                if (son != null)
                {
                    rllast.Next = son;
                    son.Last.Next = rlnext;
                    rlnext.Last = son.Last;
                    son.Last = rllast;

                }
                else
                {
                    rllast.Next = rlnext;
                    rlnext.Last = rllast;
                    son = rllast;
                }
            }

            if (son.Next != son)
            {
                root = MeltHeapOrderedTree(son);
            }
            else
            {
                root = son;
            }

            return fheap.Key;
        }

        public void DecreaseKey(int key, double newval)
        {
            FibNode fheap = root;
            FibNode p = nodeList[key];		//node to decrease value 
            FibNode pfather = p.Father;

            if (newval > nodeValues[key])
            {
                if (p == fheap)
                {
                    DeleteMin();
                    InsertKey(key, newval);
                }
                else
                {
                    DeleteNode(key);
                    InsertKey(key, newval);
                }
                fheap = root;
            }
            else
                if (p.Father != null)
                {
                    nodeValues[key] = newval;
                    //cutting p from father 
                    p.Father.Rank--;
                    if (p.Next != p)
                    {
                        p.Father.Son = p.Next;
                        p.Next.Last = p.Last;
                        p.Last.Next = p.Next;
                        p.Last = p;
                        p.Next = p;
                        p.Father = null;
                    }
                    else
                    {
                        p.Father.Son = null;
                        p.Father = null;
                    }
                    root = MeltFibHeap(fheap, p);

                    if (pfather.Mark == 0)
                        pfather.Mark = 1;
                    else
                    {
                        DecreaseKey(pfather.Key, nodeValues[pfather.Key]);
                    }
                    fheap = root;
                }
                else
                {
                    nodeValues[key] = newval;
                    if (newval < nodeValues[fheap.Key])
                    {
                        fheap = nodeList[key];
                        nodeValues[key] = newval;

                        root = fheap;
                    }
                }
            root = fheap;
        }

        public int DeleteNode(int key)
        {
            DecreaseKey(key, -double.MaxValue);
            return DeleteMin();
        }

        public void ShowFHeap()
        {
            showFHeap(root);
        }
        private void showFHeap(FibNode root)
        {
            FibNode t = root;
            if (root != null)
                do
                {
                    Console.Write("%d ", t.Key);
                    if (t.Son != null)
                    {
                        Console.Write("(");
                        showFHeap(t.Son);
                        Console.Write(")");
                    }
                    t = t.Next;
                }
                while (t != root);
        }
    }
}