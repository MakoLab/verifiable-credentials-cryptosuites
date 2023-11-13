using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF.JsonLd;

namespace JsonLdExtensions
{
    public static class RemoteDocumentExtensions
    {
        public static JToken GetDocument(this RemoteDocument remoteDocument)
        {
            if (remoteDocument.Document is JToken token)
            {
                return token;
            }
            return JToken.Parse(remoteDocument.Document.ToString() ?? "{}");
        }
    }
}
