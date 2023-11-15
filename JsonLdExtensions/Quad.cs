using VDS.RDF;

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
