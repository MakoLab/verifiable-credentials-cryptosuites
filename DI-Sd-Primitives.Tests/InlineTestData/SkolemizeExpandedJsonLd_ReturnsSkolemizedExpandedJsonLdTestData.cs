using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI_Sd_Primitives.Tests.InlineTestData
{
    public class SkolemizeExpandedJsonLd_ReturnsSkolemizedExpandedJsonLdTestData : TheoryData<string, string>
    {
        public SkolemizeExpandedJsonLd_ReturnsSkolemizedExpandedJsonLdTestData()
        {
            Add(
                """
                [
                  {
                    "@id": "_:subject",
                    "http://example.org/predicate": [
                      {
                        "@id": "_:object"
                      }
                    ]
                  }
                ]
                """, """
                [
                  {
                    "@id": "urn:example_subject",
                    "http://example.org/predicate": [
                      {
                        "@id": "urn:example_object"
                      }
                    ]
                  }
                ]
                """
                );

        }
    }
}
