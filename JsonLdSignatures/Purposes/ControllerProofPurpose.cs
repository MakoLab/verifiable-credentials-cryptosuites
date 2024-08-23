using Cryptosuite.Core;
using Cryptosuite.Core.ControllerDocuments;
using Cryptosuite.Core.Util;
using FluentResults;
using JsonLdExtensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VDS.RDF.JsonLd;

namespace JsonLdSignatures.Purposes
{
    public class ControllerProofPurpose : ProofPurpose
    {
        private readonly string[] DidVrTerms =
        [
            "assertionMethod",
            "authentication",
            "capabilityInvocation",
            "capabilityDelegation",
            "keyAgreement",
            "verificationMethod"
        ];
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
                var mustFrame = !(_termDefinedByDIDContext && contextString == Contexts.DidContextV1Url);
                if (mustFrame)
                {
                    var framingDocument = new JObject
                    {
                        { "@context", Contexts.SecurityContextUrl },
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
