using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace RDFDatasetCanonicalization.Tests
{
    public class RDFCTestData : TheoryData<string, string, string>
    {
        private List<string> _testCases = new();
        private List<string> _expectedResults = new();

        public RDFCTestData()
        {
            // Read manifest.jsonld
            var manifest = JObject.Parse(File.ReadAllText("manifest.jsonld"));
            var entries = manifest["entries"];
            foreach (var entry in entries!)
            {
                var name = entry["name"]?.ToString();
                var isPositive = entry.Value<string>("type") != "rdfc:RDFC10NegativeEvalTest";
                if (isPositive)
                {
                    var input = Path.Combine("TestCases", entry["action"].ToString().Remove(0, 7));
                    var expected = Path.Combine("Results", entry["result"].ToString().Remove(0, 7));
                    if (name != null && input != null && expected != null)
                    {
                        Add(name, File.ReadAllText(input), File.ReadAllText(expected));
                    }
                }
            }
        }
    }
}
