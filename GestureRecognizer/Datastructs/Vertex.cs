
namespace Gestures.Geometrie.Vertex
{

    public interface IVertex
    {
        double this[int index] { get; set; }
        //int Dimension { get; }
        int Num { get; set; }
    }

    public static class Vertexoperators
    {
        public static Vert1 Subtract<Vert1, Vert2>(this Vert1 vertex1, Vert2 vertex2)
            where Vert1 : struct, IVertex
            where Vert2 : IVertex
        {
            var result = vertex1;
            result[0] = vertex1[0] - vertex2[0];
            result[1] = vertex1[1] - vertex2[1];
            return result;
        }
        //dot product
        public static double DotProduct<Vert1, Vert2>(this Vert1 vertex1, Vert2 vertex2)
            where Vert1 : IVertex
            where Vert2 : IVertex
        {
            return vertex1[0] * vertex2[0] + vertex1[1] * vertex2[1];
        }
        public static Vert1 Add<Vert1, Vert2>(this Vert1 vertex1, Vert2 vertex2)
            where Vert1 : struct, IVertex
            where Vert2 : IVertex
        {
            var result = vertex1;
            result[0] = vertex1[0] + vertex2[0];
            result[1] = vertex1[1] + vertex2[1];
            return result;
        }

        public static double EuclideanNorm<Vert>(this Vert vertex) where Vert : IVertex
        {
            return EuclideanNorm(vertex);
        }
        public static double EuclideanDistance<Vert1, Vert2>(this Vert1 vertex1, Vert2 vertex2)
            where Vert1 : struct, IVertex
            where Vert2 : IVertex
        {
            return vertex1.Subtract(vertex2).EuclideanNorm();
        }

        public static Vert1 Divide<Vert1>(this Vert1 vertex, double v)
            where Vert1 : struct, IVertex
        {
            var result = vertex;
            result[0] = vertex[0] / v;
            result[1] = vertex[1] / v;
            return result;
        }
    }


    [System.Xml.Serialization.XmlRoot("Vertex")]
    public struct Vertex : IVertex
    {
        //public fixed double Coords[2];
        double coord1;
        double coord2;
        private int _num;

        public Vertex(double coords1, double coords2)
        {
            coord1 = coords1;
            coord2 = coords2;
            _num = 0;
        }

        #region IVertex Members

        [System.Xml.Serialization.XmlIgnore]
        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return coord1;
                    case 1: return coord2;
                    default: return .0;
                }
            }
            set
            {
                switch (index)
                {
                    case 0: coord1 = value; break;
                    case 1: coord2 = value; break;
                    default: break;
                }
            }
        }

        [System.Xml.Serialization.XmlAttribute(AttributeName = "Number")]
        public int Num { get { return _num; } set { _num = value; } }

        #endregion

        [System.Xml.Serialization.XmlIgnore]
        public bool IsZero
        {
            get
            {
                for (int i = 0; i < 2; i++)
                {
                    if (this[i] != 0.0) { return false; };
                }
                return true;
            }
        }

        #region static methods

        public static Vertex operator /(Vertex vertex, double v)
        {
            //return new Vertex(vertex[0]/v,vertex[1]/v);
            return vertex.Divide(v);
        }

        public static Vertex operator +(Vertex vertex1, Vertex vertex2)
        {
            return vertex1.Add(vertex2);
        }

        public static Vertex operator -(Vertex vertex1, Vertex vertex2)
        {
            //return new Vertex(vertex1[0] - vertex2[0], vertex1[1] - vertex2[1]);
            return vertex1.Subtract(vertex2);
        }

        //dot product
        public static double operator *(Vertex vertex1, Vertex vertex2)
        {
            return vertex1.DotProduct(vertex2);
        }
        public static explicit operator Sample(Vertex v)
        {
            return new Sample(System.DateTime.Now, v);
        }
        #endregion
    }


    public struct Sample : IVertex
    {
        //public fixed double Coords[2];
        private double _y;
        public double Y { get { return _y; } private set { _y = value; } }
        private double _x;
        public double X { get {return _x;} private set { _x = value; }}

        public Sample(System.DateTime timeStamp, double coord1, double coord2)
        {          
            _TimeStamp = timeStamp;
            _num = 0;
            _y = coord1;
            _x = coord2;
        }

        public Sample(System.DateTime timeStamp, IVertex vertex)
        {
            _num = 0;
            _TimeStamp = timeStamp;
            _y = vertex[0];
            _x = vertex[1];
        }

        private System.DateTime _TimeStamp;
        public System.DateTime TimeStamp { get { return _TimeStamp; } set { _TimeStamp = value; } }
        private int _num;
        public int Num { get { return _num; } set { _num = value; } }
        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return Y;
                    case 1: return X;
                    default: return .0;
                }
            }
            set
            {
                switch (index)
                {
                    case 0: Y = value; break;
                    case 1: X = value; break;
                    default: break;
                }
            }
        }
        public void Set(int index, double value)
        {
            this[index] = value;
        }
        public static Sample operator /(Sample vertex, double v)
        {
            return vertex.Divide(v); ;
        }
        public static explicit operator Vertex(Sample v)
        {
            return new Vertex(v[0], v[1]);
        }
    }

    public delegate double DistanceDelegate<Vert1, Vert2>(Vert1 a, Vert2 b)
        where Vert1 : struct, IVertex
        where Vert2 : struct, IVertex;

    public static class MetricDistances
    {
        public static double ManhattenDistance(IVertex a, IVertex b)
        {
            double result = 0;
            for (int i = 0; i < 2; i++)
            {
                result += System.Math.Abs(a[i] - b[i]);
            }
            return result;
        }

        public static double SquaredEuclideanDistance(IVertex a, IVertex b)
        {
            double result = 0;
            for (int i = 0; i < 2; i++)
            {
                result += (a[i] - b[i]) * (a[i] - b[i]);
            }
            return result;
        }
        public static double EuclideanDistance(IVertex a, IVertex b)
        {
            double result = 0;
            for (int i = 0; i < 2; i++)
            {
                result += (a[i] - b[i]) * (a[i] - b[i]);
            }
            return System.Math.Sqrt(result);
        }
    }
}