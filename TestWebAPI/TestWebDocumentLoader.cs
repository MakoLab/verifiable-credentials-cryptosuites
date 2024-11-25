using DataIntegrity;

namespace TestWebAPI
{
    public class TestWebDocumentLoader : SecurityDocumentLoader.SecurityDocumentLoader
    {
        public TestWebDocumentLoader(IDidDocumentCreator didDocumentCreator) : base(didDocumentCreator)
        {
        }
    }
}
