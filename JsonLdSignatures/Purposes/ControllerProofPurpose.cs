using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cryptosuite.Core;
using JsonLdExtensions;
using Newtonsoft.Json.Linq;
using VDS.RDF.JsonLd;
using Newtonsoft.Json;
using Cryptosuite.Core.Util;

namespace JsonLdSignatures.Purposes
{
    public class ControllerProofPurpose : ProofPurpose
    {
        const string DidContextV1 = "https://www.w3.org/ns/did/v1";
        private readonly string[] DidVrTerms = new string[]
        {
            "assertionMethod",
            "authentication",
            "capabilityInvocation",
            "capabilityDelegation",
            "keyAgreement",
            "verificationMethod"
        };
        private readonly Controller? _controller;
        private readonly bool _termDefinedByDIDContext;
        private readonly string _term;

        public ControllerProofPurpose(string term, Controller? controller, DateTime? date, int maxTimestampDelta = int.MaxValue)
            : base(term, date, maxTimestampDelta)
        {
            _controller = controller;
            _termDefinedByDIDContext = DidVrTerms.Contains(term);
            _term = term;
        }

        public virtual Result<ValidationResult> Validate(Proof proof, VerificationMethod verificationMethod, IDocumentLoader documentLoader)
        {
            var result = base.Validate(proof);
            if (result.IsFailed) return result;
            var validationResult = result.Value;
            if (_controller is not null)
            {
                validationResult.Controller = JToken.FromObject(_controller);
            }
            else
            {
                var created = Uri.TryCreate(verificationMethod.Controller, new UriCreationOptions(), out var controllerId);
                if (!created)
                {
                    return Result.Fail("'controller' must be a string representing a URL.");
                }
                var document = (JToken)documentLoader.LoadDocument(controllerId!).Document;
                var context = document["@context"] is JArray contextArray ? contextArray[0] : document["@context"];
                var contextString = context?.ToString();
                var mustFrame = !(_termDefinedByDIDContext && contextString == DidContextV1);
                if (mustFrame)
                {
                    var framingDocument = new JObject
                    {
                        { "@context", SecurityConstants.SecurityContextUrl },
                        { "id", controllerId },
                        { _term, new JObject
                            {
                                { "@embed", "@never" },
                                { "id", verificationMethod.Id },
                            }
                        }
                    };
                    document = JsonLdProcessor.Frame(
                        document,
                        framingDocument,
                        new JsonLdProcessorOptions { CompactToRelative = false, DocumentLoader = documentLoader.LoadDocument });
                }
                validationResult.Controller = document;
            }
            var verificationMethods = (JArray)(validationResult.Controller![_term] ?? new JArray());
            var serializer = new JsonSerializer();
            serializer.Converters.Add(new ObjectIdToStringConverter());
            if (verificationMethods.Any(vm => vm.ToObject<string>(serializer) == verificationMethod.Id))
            {
                return Result.Ok(validationResult);
            }
            else
            {
                return Result.Fail($"The verification method '{verificationMethod.Id}' is not authorized by the controller for proof purpose '{_term}'.");
            }
        }
    }
}
