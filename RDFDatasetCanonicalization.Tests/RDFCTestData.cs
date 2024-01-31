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
            var manifest = JObject.Parse(System.IO.File.ReadAllText("manifest.jsonld"));
            var entries = manifest["entries"];
            foreach (var entry in entries!)
            {
                var name = entry["name"]?.ToString();
                var isPositive = entry.Value<string>("type") != "rdfc:RDFC10NegativeEvalTest";
                if (isPositive)
                {
                    var input = Path.Combine("TestCases", entry["action"]?.ToString());
                    var expected = Path.Combine("Results", entry["result"]?.ToString());
                    Add(name, File.ReadAllText(input), File.ReadAllText(expected));
                }
            }
        }
    }
}
