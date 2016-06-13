
namespace Gestures.Geometrie.Vertex
{

    /// <summary>
    /// Vertex
    /// </summary>
    public interface IVertex
    {
        /// <summary>
        /// Gets or sets the <see cref="System.Double"/> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="System.Double"/>.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        double this[int index] { get; set; }
        //int Dimension { get; }
        /// <summary>
        /// Gets or sets the number.
        /// </summary>
        /// <value>
        /// The number.
        /// </value>
        int Num { get; set; }
    }

    /// <summary>
    /// Class for vertex operation functions
    /// </summary>
    public static class Vertexoperators
    {
        /// <summary>
        /// Subtracts two vertexes.
        /// </summary>
        /// <typeparam name="Vert1">The type of the vert1.</typeparam>
        /// <typeparam name="Vert2">The type of the vert2.</typeparam>
        /// <param name="vertex1">The vertex1.</param>
        /// <param name="vertex2">The vertex2.</param>
        /// <returns>The result of the substraction</returns>
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
        /// <summary>
        /// Calculates the dot product.
        /// </summary>
        /// <typeparam name="Vert1">The type of the vert1.</typeparam>
        /// <typeparam name="Vert2">The type of the vert2.</typeparam>
        /// <param name="vertex1">The vertex1.</param>
        /// <param name="vertex2">The vertex2.</param>
        /// <returns>The dot product of two vertexes.</returns>
        public static double DotProduct<Vert1, Vert2>(this Vert1 vertex1, Vert2 vertex2)
            where Vert1 : IVertex
            where Vert2 : IVertex
        {
            return vertex1[0] * vertex2[0] + vertex1[1] * vertex2[1];
        }
        /// <summary>
        /// Adds the specified vertex2 to the vertex1.
        /// </summary>
        /// <typeparam name="Vert1">The type of the vert1.</typeparam>
        /// <typeparam name="Vert2">The type of the vert2.</typeparam>
        /// <param name="vertex1">The vertex1.</param>
        /// <param name="vertex2">The vertex2.</param>
        /// <returns>The result of the addition.</returns>
        public static Vert1 Add<Vert1, Vert2>(this Vert1 vertex1, Vert2 vertex2)
            where Vert1 : struct, IVertex
            where Vert2 : IVertex
        {
            var result = vertex1;
            result[0] = vertex1[0] + vertex2[0];
            result[1] = vertex1[1] + vertex2[1];
            return result;
        }

        /// <summary>
        /// Calculates the Euclidean norm.
        /// </summary>
        /// <typeparam name="Vert">The type of the vert.</typeparam>
        /// <param name="vertex">The vertex.</param>
        /// <returns></returns>
        public static double EuclideanNorm<Vert>(this Vert vertex) where Vert : IVertex
        {
            return EuclideanNorm(vertex);
        }
        /// <summary>
        /// Calculates the Euclidean distance.
        /// </summary>
        /// <typeparam name="Vert1">The type of the vert1.</typeparam>
        /// <typeparam name="Vert2">The type of the vert2.</typeparam>
        /// <param name="vertex1">The vertex1.</param>
        /// <param name="vertex2">The vertex2.</param>
        /// <returns></returns>
        public static double EuclideanDistance<Vert1, Vert2>(this Vert1 vertex1, Vert2 vertex2)
            where Vert1 : struct, IVertex
            where Vert2 : IVertex
        {
            return vertex1.Subtract(vertex2).EuclideanNorm();
        }

        /// <summary>
        /// Divides a vertex by a double.
        /// </summary>
        /// <typeparam name="Vert1">The type of the vert1.</typeparam>
        /// <param name="vertex">The vertex.</param>
        /// <param name="v">The denominator.</param>
        /// <returns>The divided Vertex</returns>
        public static Vert1 Divide<Vert1>(this Vert1 vertex, double v)
            where Vert1 : struct, IVertex
        {
            var result = vertex;
            result[0] = vertex[0] / v;
            result[1] = vertex[1] / v;
            return result;
        }
    }


    /// <summary>
    /// A Vertex object.
    /// </summary>
    /// <seealso cref="Gestures.Geometrie.Vertex.IVertex" />
    [System.Xml.Serialization.XmlRoot("Vertex")]
    public struct Vertex : IVertex
    {
        //public fixed double Coords[2];
        double coord1;
        double coord2;
        private int _num;

        /// <summary>
        /// Initializes a new instance of the <see cref="Vertex"/> struct.
        /// </summary>
        /// <param name="coords1">The coords1.</param>
        /// <param name="coords2">The coords2.</param>
        public Vertex(double coords1, double coords2)
        {
            coord1 = coords1;
            coord2 = coords2;
            _num = 0;
        }

        #region IVertex Members

        /// <summary>
        /// Gets or sets the <see cref="System.Double"/> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="System.Double"/>.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets or sets the numeric value.
        /// </summary>
        /// <value>
        /// The numeric value.
        /// </value>
        [System.Xml.Serialization.XmlAttribute(AttributeName = "Number")]
        public int Num { get { return _num; } set { _num = value; } }

        #endregion

        /// <summary>
        /// Gets a value indicating whether this instance is zero.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is zero; otherwise, <c>false</c>.
        /// </value>
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

        /// <summary>
        /// Implements the operator /.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <param name="v">The v.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Vertex operator /(Vertex vertex, double v)
        {
            //return new Vertex(vertex[0]/v,vertex[1]/v);
            return vertex.Divide(v);
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="vertex1">The vertex1.</param>
        /// <param name="vertex2">The vertex2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Vertex operator +(Vertex vertex1, Vertex vertex2)
        {
            return vertex1.Add(vertex2);
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="vertex1">The vertex1.</param>
        /// <param name="vertex2">The vertex2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Vertex operator -(Vertex vertex1, Vertex vertex2)
        {
            //return new Vertex(vertex1[0] - vertex2[0], vertex1[1] - vertex2[1]);
            return vertex1.Subtract(vertex2);
        }

        //dot product
        /// <summary>
        /// Implements the operator *.
        /// </summary>
        /// <param name="vertex1">The vertex1.</param>
        /// <param name="vertex2">The vertex2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static double operator *(Vertex vertex1, Vertex vertex2)
        {
            return vertex1.DotProduct(vertex2);
        }
        /// <summary>
        /// Performs an explicit conversion from <see cref="Vertex"/> to <see cref="Sample"/>.
        /// </summary>
        /// <param name="v">The v.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator Sample(Vertex v)
        {
            return new Sample(System.DateTime.Now, v);
        }
        #endregion
    }


    /// <summary>
    /// A Touch sample vertex.
    /// </summary>
    /// <seealso cref="Gestures.Geometrie.Vertex.IVertex" />
    public struct Sample : IVertex
    {
        //public fixed double Coords[2];
        private double _y;
        /// <summary>
        /// Gets the y coordinate.
        /// </summary>
        /// <value>
        /// The y coordinate.
        /// </value>
        public double Y { get { return _y; } private set { _y = value; } }
        private double _x;
        /// <summary>
        /// Gets the x coordinate.
        /// </summary>
        /// <value>
        /// The x coordinate.
        /// </value>
        public double X { get {return _x;} private set { _x = value; }}

        /// <summary>
        /// Initializes a new instance of the <see cref="Sample"/> struct.
        /// </summary>
        /// <param name="timeStamp">The time stamp of the sample.</param>
        /// <param name="coord1">The vertical coordinate (y).</param>
        /// <param name="coord2">The horizontal coordinate (x).</param>
        public Sample(System.DateTime timeStamp, double coord1, double coord2)
        {          
            _TimeStamp = timeStamp;
            _num = 0;
            _y = coord1;
            _x = coord2;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sample"/> struct.
        /// </summary>
        /// <param name="timeStamp">The time stamp of the sample.</param>
        /// <param name="vertex">The vertex.</param>
        public Sample(System.DateTime timeStamp, IVertex vertex)
        {
            _num = 0;
            _TimeStamp = timeStamp;
            _y = vertex[0];
            _x = vertex[1];
        }

        private System.DateTime _TimeStamp;
        /// <summary>
        /// Gets or sets the time stamp.
        /// </summary>
        /// <value>
        /// The time stamp.
        /// </value>
        public System.DateTime TimeStamp { get { return _TimeStamp; } set { _TimeStamp = value; } }
        private int _num;
        /// <summary>
        /// Gets or sets the numeric value.
        /// </summary>
        /// <value>
        /// The numeric value.
        /// </value>
        public int Num { get { return _num; } set { _num = value; } }
        /// <summary>
        /// Gets or sets the <see cref="System.Double"/> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="System.Double"/>.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
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
        /// <summary>
        /// Sets the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        public void Set(int index, double value)
        {
            this[index] = value;
        }
        /// <summary>
        /// Implements the operator /.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <param name="v">The v.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Sample operator /(Sample vertex, double v)
        {
            return vertex.Divide(v); ;
        }
        /// <summary>
        /// Performs an explicit conversion from <see cref="Sample"/> to <see cref="Vertex"/>.
        /// </summary>
        /// <param name="v">The v.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator Vertex(Sample v)
        {
            return new Vertex(v[0], v[1]);
        }
    }

    /// <summary>
    /// Defines a structure for distance calculation
    /// </summary>
    /// <typeparam name="Vert1">The type of the ert1.</typeparam>
    /// <typeparam name="Vert2">The type of the ert2.</typeparam>
    /// <param name="a">a.</param>
    /// <param name="b">The b.</param>
    /// <returns></returns>
    public delegate double DistanceDelegate<Vert1, Vert2>(Vert1 a, Vert2 b)
        where Vert1 : struct, IVertex
        where Vert2 : struct, IVertex;

    /// <summary>
    /// Class for different distance calculation.
    /// </summary>
    public static class MetricDistances
    {
        /// <summary>
        /// Calculates a Manhatten distance.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>The Manhatten distance.</returns>
        public static double ManhattenDistance(IVertex a, IVertex b)
        {
            double result = 0;
            for (int i = 0; i < 2; i++)
            {
                result += System.Math.Abs(a[i] - b[i]);
            }
            return result;
        }

        /// <summary>
        /// Calculates a squared Euclidean distance.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>the squared Euclidean distance</returns>
        public static double SquaredEuclideanDistance(IVertex a, IVertex b)
        {
            double result = 0;
            for (int i = 0; i < 2; i++)
            {
                result += (a[i] - b[i]) * (a[i] - b[i]);
            }
            return result;
        }
        /// <summary>
        /// Calculates the Euclidean distance.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>The Euclidean distance</returns>
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