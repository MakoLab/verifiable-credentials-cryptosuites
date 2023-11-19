using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Writing.Formatting;

namespace JsonLdExtensions.Canonicalization
{
    public static class TriplestoreExtensions
    {
        private static HashAlgorithm HashAlgorithm { get; set; } = SHA256.Create();

        public static NormalizedTriplestore Normalize(this ITripleStore ts, JsonLdNormalizerOptions? options = null)
        {
            if (ts is null)
            {
                throw new ArgumentNullException(nameof(ts));
            }
            options ??= new JsonLdNormalizerOptions();
            if (options.HashAlgorithm != HashAlgorithmType.Sha256)
            {
                HashAlgorithm = options.HashAlgorithm switch
                {
                    HashAlgorithmType.Sha384 => SHA384.Create(),
                    HashAlgorithmType.Sha512 => SHA512.Create(),
                    _ => throw new ArgumentException($"Invalid hash algorithm {nameof(options.HashAlgorithm)}")
                };
            }

            // 1) Create the canonicalization state.
            var state = new CanonicalizationState();
            // 2) For every quad in input dataset:
            foreach (var q in ts.GetQuads())
            {
                // 2.1) For each blank node that occurs in the quad, add a reference to the quad using the blank node
                // identifier in the blank node to quads map, creating a new entry if necessary.
                if (q.Object is IBlankNode bn)
                {
                    state.AddQuadToBlankNode(bn.InternalID, q);
                }
                if (q.Subject is IBlankNode bn2)
                {
                    state.AddQuadToBlankNode(bn2.InternalID, q);
                }
                if (q.Graph.Name is IBlankNode bn3)
                {
                    state.AddQuadToBlankNode(bn3.InternalID, q);
                }
            }
            // 3) Create a list of non-normalized blank node identifiers non-normalized identifiers and populate it using the
            // keys from the blank node to quads map.
            // It's "state.BlankNodeToQuads.Keys"
            // 4) Initialize simple, a boolean flag, to true.
            var simple = true;
            // 5) While simple is true, issue canonical identifiers for blank nodes:
            while (simple)
            {
                // 5.1) Set simple to false.
                simple = false;
                // 5.2) Clear hash to blank nodes map.
                state.HashToBlankNodeIdentifier.Clear();
                // 5.3) For each blank node identifier identifier in non-normalized identifiers:
                foreach (var k in state.BlankNodeToQuads.Keys)
                {
                    // 5.3.1) Create a hash, hash, according to the Hash First Degree Quads algorithm.
                    var hash = HashFirstDegreeQuads(k, state);
                    // 5.3.2) Add hash and identifier to hash to blank nodes map, creating a new entry if necessary.
                    if (!state.HashToBlankNodeIdentifier.ContainsKey(hash))
                    {
                        state.HashToBlankNodeIdentifier.Add(hash, new List<string>());
                    }
                    state.HashToBlankNodeIdentifier[hash].Add(k);
                }
                // 5.4) For each hash to identifier mapping in hash to blank nodes map, lexicographically-sorted by hash:
                foreach (var kv in state.HashToBlankNodeIdentifier.OrderBy(kv => kv.Key))
                {
                    // 5.4.1) If the length of identifier list is greater than 1, continue to the next mapping.
                    if (kv.Value.Count > 1)
                    {
                        continue;
                    }
                    // 5.4.2) Use the Issue Identifier algorithm, passing canonical issuer and the single blank node
                    // identifier in identifier list, identifier, to issue a canonical replacement identifier for identifier.
                    var identifier = state.IssueIdentifier(kv.Value[0]);
                    // 5.4.3) Remove identifier from non-normalized identifiers.
                    state.BlankNodeToQuads.Remove(kv.Value[0]);
                    // 5.4.4) Remove hash from the hash to blank nodes map.
                    state.HashToBlankNodeIdentifier.Remove(kv.Key);
                }
            }
            // 6) For each hash to identifier list mapping in hash to blank nodes map, lexicographically-sorted by hash:
            foreach (var kv in state.HashToBlankNodeIdentifier.OrderBy(kv => kv.Key))
            {
                // 6.1) Create hash path list where each item will be a result of running the Hash N-Degree Quads
                // algorithm.
                var hashPathList = new List<(string Hash, IdentifierIssuer Issuer)>();
                // 6.2) For each blank node identifier identifier in identifier list:
                foreach (var identifier in kv.Value)
                {
                    // 6.2.1) If a canonical identifier has already been issued for identifier, continue to the next
                    // identifier.
                    if (state.IsIssued(identifier))
                    {
                        continue;
                    }
                    // 6.2.2) Create temporary issuer, an identifier issuer initialized with the prefix _:b.
                    var issuer = new IdentifierIssuer(Constants.TemporaryIssuerPrefix);
                    // 6.2.3) Use the Issue Identifier algorithm, passing temporary issuer and identifier, to issue a new
                    // temporary blank node identifier for identifier.
                    var tempBNodeIdentifier = issuer.IssueIdentifier(identifier);
                    // 6.2.4) Run the Hash N-Degree Quads algorithm, passing temporary issuer, and append the result
                    // to the hash path list.
                    hashPathList.Add(HashNDegreeQuads(tempBNodeIdentifier, state, issuer));
                }
                // 6.3) For each result in the hash path list, lexicographically-sorted by the hash in result:
                foreach (var result in hashPathList.OrderBy(r => r.Hash))
                {
                    // 6.3.1) For each blank node identifier, existing identifier, that was issued a temporary identifier by
                    // identifier issuer in result, issue a canonical identifier, in the same order, using the Issue Identifier
                    // algorithm, passing canonical issuer and existing identifier.
                    foreach (var existingIdentifier in result.Issuer.GetExistingIdentifiers())
                    {
                        state.IssueIdentifier(existingIdentifier);
                    }
                }
            }
            var normalizedDataset = new TripleStore();
            // 7) For each quad, quad, in input dataset:
            foreach (var g in ts.Graphs)
            {
                // 7.1) Create a copy, quad copy, of quad and replace any existing blank node identifiers using the
                // canonical identifiers previously issued by canonical issuer.
                if (g.Name is BlankNode gn)
                {
                    gn = new BlankNode(state.GetIdentifier(gn.InternalID));
                }
                var graphCopy = new Graph(g.Name);
                foreach (var t in g.Triples)
                {
                    var tripleCopy = CreateTripleCopy(t, state);
                    graphCopy.Assert(tripleCopy);
                }
                // 7.2) Add quad, which is now in sorted canonical form, to the normalized dataset.
                normalizedDataset.Add(graphCopy);
            }
            // 8) Return the normalized dataset.
            return new NormalizedTriplestore(normalizedDataset);
        }

        public static IEnumerable<Quad> GetQuads(this ITripleStore ts)
        {
            foreach (var g in ts.Graphs)
            {
                foreach (var t in g.Triples)
                {
                    yield return new Quad(g, t);
                }
            }
        }

        // 4.6 Hash First Degree Quads
        private static string HashFirstDegreeQuads(string referenceBNodeIdentfier, CanonicalizationState state)
        {
            // 1) Initialize nquads to an empty list. It will be used to store quads in N-Quads format.
            var nquads = new List<string>();
            // 2) Get the list of quads quads associated with the reference blank node identifier in the blank node to quads
            // map.
            var quads = state.GetQuadsForBlankNode(referenceBNodeIdentfier);
            var formatter = new NQuadsFormatter();
            // 3) For each quad quad in quads:
            foreach (var quad in quads)
            {
                // 3.1) Serialize the quad in N-Quads format with the following special rule:
                // 3.1.1) If any component in quad is an blank node, then serialize it using a special identifier as
                // follows:
                // 3.1.1.1) If the blank node's existing blank node identifier matches the reference blank node
                // identifier then use the blank node identifier _:a, otherwise, use the blank node identifier _:z.
                var s = RenameBNodes(referenceBNodeIdentfier, quad.Subject);
                var p = RenameBNodes(referenceBNodeIdentfier, quad.Predicate);
                var o = RenameBNodes(referenceBNodeIdentfier, quad.Object);
                var g = (IRefNode)RenameBNodes(referenceBNodeIdentfier, quad.Graph.Name);
                nquads.Add(formatter.Format(new Triple(s, p, o), g));
            }
            // 4) Sort nquads in lexicographical order.
            nquads.Sort();
            // 5) Return the hash that results from passing the sorted, joined nquads through the hash algorithm.
            return HashString(string.Join("", nquads));
        }

        // 4.7 Hash Related Blank Node
        private static string HashRelatedBlankNode(string relatedBNodeIdentifier, Quad quad, IdentifierIssuer issuer, string position, CanonicalizationState state)
        {
            // 1) Set the identifier to use for related, preferring first the canonical identifier for related if issued, second the
            // identifier issued by issuer if issued, and last, if necessary, the result of the Hash First Degree Quads
            // algorithm, passing related.
            string? identifier;
            identifier = state.GetIdentifier(relatedBNodeIdentifier);
            identifier ??= issuer.GetIdentifier(relatedBNodeIdentifier);
            identifier ??= HashFirstDegreeQuads(relatedBNodeIdentifier, state);
            // 2) Initialize a string input to the value of position.
            var input = new StringBuilder(position);
            // 3) If position is not g, append <, the value of the predicate in quad, and > to input.
            if (position != "g")
            {
                input.Append($"<{quad.Predicate}>");
            }
            // 4) Append identifier to input.
            input.Append(identifier);
            // 5) Return the hash that results from passing input through the hash algorithm.
            return HashString(input.ToString());
        }

        // 4.8 Hash N-Degree Quads
        private static (string Hash, IdentifierIssuer Issuer) HashNDegreeQuads(string referenceBNodeIdentfier, CanonicalizationState state, IdentifierIssuer issuer)
        {
            // 1) Create a hash to related blank nodes map for storing hashes that identify related blank nodes.
            var hashToRelated = new Dictionary<string, IList<string>>();
            // 2) Get a reference, quads, to the list of quads in the blank node to quads map for the key identifier.
            var quads = state.GetQuadsForBlankNode(referenceBNodeIdentfier);
            // 3) For each quad, in quads:
            foreach (var quad in quads)
            {
                // 3.1) For each component in quad, where component is the subject, object, or graph name, and it is a
                // blank node that is not identified by identifier:
                var bnodes = GetBNodeIdentifiers(quad);
                foreach (var node in bnodes)
                {
                    if (node.Value != referenceBNodeIdentfier)
                    {
                        // 3.1.1) Set hash to the result of the Hash Related Blank Node algorithm, passing the blank node identifier for
                        // component as related, quad, path identifier issuer as issuer, and either s, o, or g as position,
                        // depending on whether component is a subject, object, graph name, respectively.
                        var hash = HashRelatedBlankNode(node.Value, quad, issuer, node.Key, state);
                        // 3.1.2) Add a mapping of hash to the blank node identifier for component to hash to related blank
                        // nodes map, adding an entry as necessary.
                        if (!hashToRelated.ContainsKey(hash))
                        {
                            hashToRelated.Add(hash, new List<string>());
                        }
                        hashToRelated[hash].Add(node.Value);
                    }
                }
            }
            // 4) Create an empty string, data to hash.
            var dataToHash = new StringBuilder();
            // 5) For each related hash to blank node list mapping in hash to related blank nodes map, sorted
            // lexicographically by related hash:
            foreach (var kv in hashToRelated.OrderBy(kv => kv.Key))
            {
                // 5.1) Append the related hash to the data to hash.
                dataToHash.Append(kv.Key);
                // 5.2) Create a string chosen path.
                var chosenPath = "";
                // 5.3) Create an unset chosen issuer variable.
                IdentifierIssuer? chosenIssuer = null;
                // 5.4) For each permutation of blank node list:
                foreach (var permutation in GeneratePermutationIndices(kv.Value.Count))
                {
                    // 5.4.1) Create a copy of issuer, issuer copy.
                    var issuerCopy = new IdentifierIssuer(issuer);
                    // 5.4.2) Create a string path.
                    var path = "";
                    // 5.4.3) Create a recursion list, to store blank node identifiers that must be recursively processed by
                    // this algorithm.
                    var recursionList = new List<string>();
                    // 5.4.4) For each related in permutation:
                    foreach (var i in permutation)
                    {
                        var related = kv.Value[i];
                        // 5.4.4.1) If a canonical identifier has been issued for related, append it to path.
                        var identifier = state.GetIdentifier(related);
                        if (identifier != null)
                        {
                            path += identifier;
                        }
                        // 5.4.4.2) Otherwise:
                        else
                        {
                            if (!issuerCopy.IsIssued(related))
                            {
                                // 5.4.4.2.1) If issuer copy has not issued an identifier for related, append related to
                                // recursion list.
                                recursionList.Add(related);
                                // 5.4.4.2.2) Use the Issue Identifier algorithm, passing issuer copy and related and append
                                // the result to path.
                                path += issuerCopy.IssueIdentifier(related);
                            }
                        }
                        // 5.4.4.3) If chosen path is not empty and the length of path is greater than or equal to the length
                        // of chosen path and path is lexicographically greater than chosen path, then skip to the next
                        // permutation.
                        if (chosenPath != "" && path.Length >= chosenPath.Length && string.CompareOrdinal(path, chosenPath) > 0)
                        {
                            break;
                        }
                    }
                    // 5.4.5) For each related in recursion list:
                    foreach (var related in recursionList)
                    {
                        // 5.4.5.1) Set result to the result of recursively executing the Hash N-Degree Quads algorithm,
                        // passing related for identifier and issuer copy for path identifier issuer.
                        var result = HashNDegreeQuads(related, state, issuerCopy);
                        // 5.4.5.2) Use the Issue Identifier algorithm, passing issuer copy and related and append the
                        // result to path.
                        path += issuerCopy.IssueIdentifier(related);
                        // 5.4.5.3) Append <, the hash in result, and > to path.
                        path += $"<{result.Hash}>";
                        // 5.4.5.4) Set issuer copy to the identifier issuer in result.
                        issuerCopy = result.Issuer;
                        // 5.4.5.5) If chosen path is not empty and the length of path is greater than or equal to the length
                        // of chosen path and path is lexicographically greater than chosen path, then skip to the next
                        // permutation.
                        if (chosenPath != "" && path.Length >= chosenPath.Length && string.CompareOrdinal(path, chosenPath) > 0)
                        {
                            break;
                        }
                    }
                    // 5.4.6) If chosen path is empty or path is lexicographically less than chosen path, set chosen path to
                    // path and chosen issuer to issuer copy.
                    if (chosenPath == "" || string.CompareOrdinal(path, chosenPath) < 0)
                    {
                        chosenPath = path;
                        chosenIssuer = issuerCopy;
                    }
                }
                // 5.5) Append chosen path to data to hash.
                dataToHash.Append(chosenPath);
                // 5.6) Replace issuer, by reference, with chosen issuer.
                issuer = chosenIssuer ?? throw new NullReferenceException("chosenIssuer is null");
            }
            // 6) Return issuer, and the hash that results from passing data to hash through the hash algorithm.
            return (HashString(dataToHash.ToString()), issuer);
        }

        private static Triple CreateTripleCopy(Triple triple, CanonicalizationState state)
        {
            var s = triple.Subject;
            var p = triple.Predicate;
            var o = triple.Object;
            if (s is IBlankNode sn)
            {
                s = new BlankNode(state.GetIdentifier(sn.InternalID));
            }
            if (triple.Object is IBlankNode on)
            {
                o = new BlankNode(state.GetIdentifier(on.InternalID));
            }
            return new Triple(s, p, o);
        }

        private static string HashString(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = HashAlgorithm.ComputeHash(bytes);
            return Convert.ToHexString(hash).ToLower();
        }

        private static INode RenameBNodes(string referenceBNodeIdentfier, INode node)
        {
            if (node is IBlankNode bn)
            {
                if (bn.InternalID == referenceBNodeIdentfier)
                {
                    node = new BlankNode(Constants.MatchingBNodeIdentifier);
                }
                else
                {
                    node = new BlankNode(Constants.NonMatchingBNodeIdentifier);
                }
            }
            return node;
        }

        private static IEnumerable<KeyValuePair<string, string>> GetBNodeIdentifiers(Quad quad)
        {
            if (quad.Subject is IBlankNode bn)
            {
                yield return KeyValuePair.Create("s", bn.InternalID);
            }
            if (quad.Object is IBlankNode bn2)
            {
                yield return KeyValuePair.Create("o", bn2.InternalID);
            }
            if (quad.Graph is IBlankNode bn3)
            {
                yield return KeyValuePair.Create("g", bn3.InternalID);
            }
        }

        /// <summary>
        /// Generates all permutations of the indices of an array of length n.
        /// </summary>
        /// <remarks>Implemetation of Heap's algorithm.</remarks>
        /// <param name="n">Length of the array</param>
        /// <returns>IEnumerable of permutations.</returns>
        private static IEnumerable<int[]> GeneratePermutationIndices(int n)
        {
            var array = new int[n];
            var c = new int[n];
            for (int j = 0; j < n; j++)
            {
                array[j] = j;
                c[j] = 0;
            }
            yield return array;
            var i = 0;
            while (i < n)
            {
                if (c[i] < i)
                {
                    if ((i & 1) == 0)
                    {
                        (array[0], array[i]) = (array[i], array[0]);
                    }
                    else
                    {
                        (array[c[i]], array[i]) = (array[i], array[c[i]]);
                    }
                    yield return array;
                    c[i]++;
                    i = 0;
                }
                else
                {
                    c[i] = 0;
                    i++;
                }
            }
        }
    }
}
