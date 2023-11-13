using VDS.RDF;

namespace JsonLdExtensions
{
    internal class Quad
    {
        internal IRefNode Graph { get; set; }
        internal Triple Triple { get; set; }

        internal Quad(IRefNode graph, Triple triple)
        {
            Graph = graph;
            Triple = triple;
        }
    }
}
