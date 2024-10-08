﻿using Cryptosuite.Core;
using JsonLdExtensions;
using Newtonsoft.Json.Linq;
using VDS.RDF.JsonLd;

namespace JsonLdSignatures
{
    internal class ExtendedDocumentLoader : IDocumentLoader
    {
        private readonly IDocumentLoader _documentLoader;
        private readonly Dictionary<string, JToken> _contexts = new()
        {
            { Contexts.SecurityContextV1Url, Contexts.SecurityContextV1 },
            { Contexts.SecurityContextV2Url, Contexts.SecurityContextV2 }
        };

        internal ExtendedDocumentLoader(IDocumentLoader documentLoader)
        {
            _documentLoader = documentLoader;
        }

        public RemoteDocument LoadDocument(Uri url)
        {
            return LoadDocument(url, new JsonLdLoaderOptions());
        }

        public RemoteDocument LoadDocument(Uri url, JsonLdLoaderOptions options)
        {
            if (_contexts.ContainsKey(url.OriginalString))
            {
                return new RemoteDocument()
                {
                    ContextUrl = null,
                    DocumentUrl = url,
                    Document = _contexts[url.OriginalString],
                };
            }
            return _documentLoader.LoadDocument(url, options);
        }
    }

    internal class StrictDocumentLoader : IDocumentLoader
    {
        public RemoteDocument LoadDocument(Uri url)
        {
            throw new ArgumentException($"{url} not found");
        }

        public RemoteDocument LoadDocument(Uri url, JsonLdLoaderOptions options)
        {
            throw new ArgumentException($"{url} not found");
        }
    }
}
