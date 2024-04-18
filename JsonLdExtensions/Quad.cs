using VDS.RDF;
using VDS.RDF.Writing.Formatting;

namespace JsonLdExtensions
{
    public class Quad
    {
        private readonly IGraph _graph;
        private readonly Triple _triple;

        public IGraph Graph => _graph;
        public INode Subject => _triple.Subject;
        public INode Predicate => _triple.Predicate;
        public INode Object => _triple.Object;
        public Triple Triple => _triple;

        /// <summary>
        /// Gets the components of the quad.
        /// </summary>
        /// <remarks>
        /// Components are the Graph, Subject, Predicate, and Object of the quad.
        /// </remarks>
        public IEnumerable<INode> Components
        {
            get
            {
                yield return _graph.Name;
                yield return _triple.Subject;
                yield return _triple.Predicate;
                yield return _triple.Object;
            }
        }

        public Quad(IGraph graph, Triple triple)
        {
            _graph = graph;
            _triple = triple;
            _graph.Assert(_triple);
        }

        public Quad(INode graphName, INode subject, INode predicate, INode @object)
        {
            _graph = new Graph(graphName as IRefNode);
            _triple = new Triple(subject, predicate, @object);
            _graph.Assert(_triple);
        }

        //Deep copy constructor
        public Quad(Quad quad)
        {
            _graph = new Graph(quad.Graph.Name);
            var s = CopyNode(quad.Subject);
            var p = CopyNode(quad.Predicate);
            var o = CopyNode(quad.Object);
            _triple = new Triple(s, p, o);
            _graph.Assert(_triple);
        }

        /// <summary>
        /// Converts the quad to N-Quads format.
        /// </summary>
        /// <returns>Formatted string.</returns>
        /// <remarks>Uses the default <see cref="NQuadsFormatter"/>.</remarks>
        public string ToNQuad()
        {
            return new NQuadsFormatter().Format(_triple, _graph.Name);
        }

        /// <summary>
        /// Converts the quad to N-Quads format.
        /// </summary>
        /// <param name="formatter">Formatter to use for serialization.</param>
        /// <returns>Formatted string.</returns>
        public string ToNQuad(NQuadsFormatter formatter)
        {
            return formatter.Format(_triple, _graph.Name);
        }

        private static INode CopyNode(INode node)
        {
            return node switch
            {
                IUriNode uriNode => new UriNode(uriNode.Uri),
                IBlankNode blankNode => new BlankNode(blankNode.InternalID),
                ILiteralNode literalNode => literalNode.Language == string.Empty
                    ? new LiteralNode(literalNode.Value, literalNode.DataType)
                    : new LiteralNode(literalNode.Value, literalNode.Language),
                _ => throw new Exception("Unknown node type")
            };
        }
    }
}
