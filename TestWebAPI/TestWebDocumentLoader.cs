using DataIntegrity;
using SecurityTestDocumentLoader;

namespace TestWebAPI
{
    public class TestWebDocumentLoader : SecurityDocumentLoader
    {
        public TestWebDocumentLoader(IDidDocumentCreator didDocumentCreator) : base(didDocumentCreator)
        {
        }
    }
}
