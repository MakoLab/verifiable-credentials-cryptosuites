using Cryptosuite.Core.ControllerDocuments;

namespace DataIntegrity
{
    public interface IDidDocumentCreator
    {
        BaseDocument CreateControllerDocument(Uri uri);
    }
}