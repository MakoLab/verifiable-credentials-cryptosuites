using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF.JsonLd;

namespace JsonLdExtensions
{
    public interface IDocumentLoader
    {
        public RemoteDocument LoadDocument(Uri url);
        public RemoteDocument LoadDocument(Uri url, JsonLdLoaderOptions options);
    }
}
