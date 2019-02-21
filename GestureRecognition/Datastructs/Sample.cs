
namespace Gestures.Geometrie.Vertex
{

    /// <summary>
    /// </summary>
		/// <remarks> </remarks>
    /// <author>Dr. rer. nat. Michael Schmidt - Techniche Universität Dresden 2014.</author>
    /// <seealso cref="Gestures.Geometrie.Vertex.Vertex" />
    public class Sample : Vertex
    {
        public Sample()
            : this(System.DateTime.Now, 0, 0)
        {
        }

        public Sample(System.DateTime timeStamp, params double[] coords)
            : base(coords)
        {
            this.TimeStamp = timeStamp;
        }

        public Sample(System.DateTime timeStamp, IVertex vertex)
            : base(new double[vertex.Dimension])
        {
            this.Coords = new double[vertex.Dimension];
            for (int i = 0; i < vertex.Dimension; i++)
            {
                this.Coords[i] = vertex[i];
            }
            this.TimeStamp = timeStamp;
        }


        public System.DateTime TimeStamp { get; set; }
    }
}