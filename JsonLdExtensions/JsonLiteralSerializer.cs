using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonLdExtensions
{
    /// <summary>
    /// Overrides some of the default Newtonsoft.Json JSON value formatting so that
    /// the output of the JSON-LD writer is better conforming to the JSON-LD 1.1 specification.
    /// </summary>
    internal class JsonLiteralSerializer
    {
        /// <summary>
        /// Return a string serialization of the provided token.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public string Serialize(JToken token)
        {
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            using (var writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.None;
                Serialize(writer, token);
            }

            return sb.ToString();
        }

        private static void Serialize(JsonWriter writer, JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    writer.WriteStartObject();
                    foreach (JProperty property in (token as JObject).Properties().OrderBy(p => p.Name, StringComparer.Ordinal))
                    {
                        writer.WritePropertyName(property.Name);
                        Serialize(writer, property.Value);
                    }
                    writer.WriteEndObject();
                    break;
                case JTokenType.Array:
                    writer.WriteStartArray();
                    foreach (JToken item in (token as JArray))
                    {
                        Serialize(writer, item);
                    }
                    writer.WriteEndArray();
                    break;
                case JTokenType.Float:

                    var doubleValue = token.Value<double>();
                    switch (doubleValue)
                    {
                        case double.NaN:
                            writer.WriteRawValue("NaN");
                            break;
                        case double.NegativeInfinity:
                            writer.WriteRawValue("-Infinity");
                            break;
                        case double.PositiveInfinity:
                            writer.WriteRawValue("Infinity");
                            break;
                        default:
                            {
                                var v = token.ToString(Formatting.None);
                                if (v.EndsWith(".0"))
                                {
                                    v = v[..^2];
                                }
                                writer.WriteRawValue(v);
                                break;
                            }
                    };
                    break;
                default:
                    writer.WriteRawValue(token.ToString(Formatting.None));
                    break;
            }
        }
    }
}
