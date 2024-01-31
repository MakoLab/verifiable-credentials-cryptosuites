using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonLdExtensions.Canonicalization;
using VDS.RDF;

namespace RDFDatasetCanonicalization.Tests
{
    public class CanonicalizationTests
    {
        [Theory]
        [ClassData(typeof(RDFCTestData))]
        public void TestPositiveCases(string name, string input, string expected)
        {
            var dataset = new TripleStore();
            dataset.LoadFromString(input);
            var actual = dataset.Normalize();
            Assert.Equal(expected, actual.Serialize());
        }
    }
}
