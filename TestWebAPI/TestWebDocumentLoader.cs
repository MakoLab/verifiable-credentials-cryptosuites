using ECDsa_2019_Cryptosuite.Tests;
using Newtonsoft.Json.Linq;

namespace TestWebAPI
{
    public class TestWebDocumentLoader : SecurityDocumentLoader.SecurityDocumentLoader
    {
        public TestWebDocumentLoader()
        {
            var mockData = new ECDsa_2019_Cryptosuite.Tests.MockData();
            AddStatic(mockData.EcdsaMultikeyKeyPair.Id!, JObject.FromObject(mockData.EcdsaMultikeyKeyPair));
            AddStatic(mockData.ControllerDocEcdsaMultikey.Id!, JObject.FromObject(mockData.ControllerDocEcdsaMultikey));
        }
    }
}
