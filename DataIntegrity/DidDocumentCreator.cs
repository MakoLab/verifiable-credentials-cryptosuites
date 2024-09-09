using Cryptosuite.Core.ControllerDocuments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataIntegrity
{
    internal class DidDocumentCreator
    {
        internal ControllerDocument CreateControllerDocument(Uri uri)
        {
            if (uri.Scheme != "did")
                throw new ArgumentException("URI must be a DID");
            var method = uri.LocalPath.Split(":").First();
            throw new NotImplementedException();
        }
    }
}
