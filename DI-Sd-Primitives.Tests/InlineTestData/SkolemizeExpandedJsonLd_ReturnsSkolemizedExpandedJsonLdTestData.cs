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

    public class SkolemizeCompactJsonLd_ReturnsSkolemizedDocumentsTestData : TheoryData<string, string, string>
    {
        public SkolemizeCompactJsonLd_ReturnsSkolemizedDocumentsTestData()
        {
            Add(
                """
                {
                  "@context": {
                    "ical": "http://www.w3.org/2002/12/cal/ical#",
                    "xsd": "http://www.w3.org/2001/XMLSchema#",
                    "ical:dtstart": {
                      "@type": "xsd:dateTime"
                    }
                  },
                  "ical:summary": "Lady Gaga Concert",
                  "ical:location": "New Orleans Arena, New Orleans, Louisiana, USA",
                  "ical:dtstart": "2011-04-09T20:00:00Z"
                }
                """, """
                [
                  {
                    "http://www.w3.org/2002/12/cal/ical#dtstart": [
                      {
                        "@type": "http://www.w3.org/2001/XMLSchema#dateTime",
                        "@value": "2011-04-09T20:00:00Z"
                      }
                    ],
                    "http://www.w3.org/2002/12/cal/ical#location": [
                      {
                        "@value": "New Orleans Arena, New Orleans, Louisiana, USA"
                      }
                    ],
                    "http://www.w3.org/2002/12/cal/ical#summary": [
                      {
                        "@value": "Lady Gaga Concert"
                      }
                    ]
                  }
                ]
                """, """
                {
                  "@context": {
                    "ical": "http://www.w3.org/2002/12/cal/ical#",
                    "xsd": "http://www.w3.org/2001/XMLSchema#",
                    "ical:dtstart": {
                      "@type": "xsd:dateTime"
                    }
                  },
                  "ical:summary": "Lady Gaga Concert",
                  "ical:location": "New Orleans Arena, New Orleans, Louisiana, USA",
                  "ical:dtstart": "2011-04-09T20:00:00Z"
                }
                """
                );
        }
    }
}
