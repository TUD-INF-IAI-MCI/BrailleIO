
namespace Gestures.Geometrie.Vertex
{
    public interface IVertex
    {
        double this[int index] { get; set; }
        double X { get; set; }
        double Y { get; set; }
        int Dimension { get; }
        int Num { get; }
    }

    /// <summary>
    /// </summary>
		/// <remarks> </remarks>
    /// <author>Dr. rer. nat. Michael Schmidt - Techniche Universität Dresden 2014.</author>
    /// <seealso cref="Gestures.Geometrie.Vertex.IVertex" />
    [System.Xml.Serialization.XmlRoot("Vertex")]
    public class Vertex : IVertex
    {
        private double[] coords;
        private int dimension;

        public Vertex()
            : this(0, 0)
        {
        }        

        public Vertex(params double[] coords)
        {
            Coords = coords;
            Num = 0;
        }

        public double X
        {
            get
            {
                if (Coords.Length > 0) return Coords[0];
                else return 0.0;
            }

            set
            {
                if (Coords.Length > 0) Coords[0] = value;
                else Coords = new double[] { value };
            }
        }

        public double Y
        {
            get
            {
                if (Coords.Length > 1) return Coords[1];
                else return 0.0;
            }

            set
            {
                if (Coords.Length > 1) Coords[1] = value;
                else Coords = new double[] { X, value };
            }
        }


        #region IVertex Members

        [System.Xml.Serialization.XmlIgnore]
        public double this[int index]
        {
            get { return (index > -1 && index < dimension) ? coords[index] : 0; }
            set { if (index > -1 && index < dimension) { coords[index] = value; } }
        }
        [System.Xml.Serialization.XmlAttribute(AttributeName = "Dimension")]
        public int Dimension { get { return dimension; } set { } }
        [System.Xml.Serialization.XmlAttribute(AttributeName = "Number")]
        public int Num { get; set; }

        [System.Xml.Serialization.XmlArray]
        public double[] Coords
        {
            get { return coords; }
            set
            {
                coords = value;
                dimension = coords.Length;
            }
        }

        #endregion

        [System.Xml.Serialization.XmlIgnore]
        public bool IsZero
        {
            get
            {
                for (int i = 0; i < Dimension; i++)
                {
                    if (this[i] != 0.0) { return false; };
                }
                return true;
            }
        }

        #region static methods

        public static Vertex operator /(Vertex vertex, double v)
        {
            double[] result = new double[vertex.Dimension];
            for (int i = 0; i < vertex.Dimension; ++i)
            {
                result[i] = vertex[i] / v;
            }
            return new Vertex(result);
        }

        public static Vertex operator +(Vertex vertex1, Vertex vertex2)
        {
            double[] result = new double[vertex1.Dimension];
            for (int i = 0; i < vertex1.Dimension; ++i)
            {
                result[i] = vertex1[i] + vertex2[i];
            }
            return new Vertex(result);
        }

        public static Vertex operator -(Vertex vertex1, Vertex vertex2)
        {
            double[] result = new double[vertex1.Dimension];
            for (int i = 0; i < vertex1.Dimension; ++i)
            {
                result[i] = vertex1[i] - vertex2[i];
            }
            return new Vertex(result);
        }

        //dot product
        public static double operator *(Vertex vertex1, Vertex vertex2)
        {
            double result = 0.0;
            for (int i = 0; i < vertex1.Dimension; ++i)
            {
                result += vertex1[i] * vertex2[i];
            }
            return result;
        }

        public static double EuclideanDistance(IVertex vertex1, IVertex vertex2)
        {
            double[] result = new double[vertex1.Dimension];
            for (int i = 0; i < vertex1.Dimension; ++i)
            {
                result[i] = vertex1[i] - vertex2[i];
            }
            return EuclideanNorm(new Vertex(result));
        }
        public static double EuclideanNorm(IVertex vertex)
        {
            double result = 0;
            for (int i = 0; i < vertex.Dimension; i++)
            {
                result += vertex[i] * vertex[i];
            }
            return System.Math.Sqrt(result);
        }

        public static double EuclideanNorm(Vertex vertex)
        {
            return EuclideanNorm((IVertex)vertex);
        }
        public static double EuclideanDistance(Vertex vertex1, Vertex vertex2)            
        {
            return EuclideanDistance((IVertex)vertex1, (IVertex)vertex2);
        }

        #endregion

        #region overrides

        public override string ToString()
        {
            return this.GetType().ToString() + " x:" + this.X.ToString("#,###.##") + " y:" + this.Y.ToString("#,###.##");
        }

        #endregion
    }




    public delegate double DistanceDelegate(IVertex a, IVertex b);

    /// <summary>
    /// </summary>
		/// <remarks> </remarks>
    /// <author>Dr. rer. nat. Michael Schmidt - Techniche Universität Dresden 2014.</author>
    public static class MetricDistances
    {
        public static double ManhattenDistance(IVertex a, IVertex b)
        {
            double result = 0;
            for (int i = 0; i < a.Dimension; i++)
            {
                result += System.Math.Abs(a[i] - b[i]);
            }
            return result;
        }

        public static double SquaredEuclideanDistance(IVertex a, IVertex b)
        {
            double result = 0;
            for (int i = 0; i < a.Dimension; i++)
            {
                result += (a[i] - b[i]) * (a[i] - b[i]);
            }
            return result;
        }

        public static double EuclideanDistance(IVertex a, IVertex b)
        {
            double result = 0;
            for (int i = 0; i < a.Dimension; i++)
            {
                result += (a[i] - b[i]) * (a[i] - b[i]);
            }
            return System.Math.Sqrt(result);
        }

        public static double WeightedSquaredEuclideanDistance(IVertex a, IVertex b, double[,] weights)
        {
            double result = 0;
            for (int i = 0; i < a.Dimension; i++)
            {
                result += (a[i] - b[i]) * (a[i] - b[i]) * weights[i, i];
            }
            return result;
        }
    }
}