using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cryptosuite.Core.Util
{
    public class SingleArrayConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsAssignableFrom(typeof(IEnumerable<T>));
        }

        public override IEnumerable<T>? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);

            if (token.Type == JTokenType.Array)
            {
                return token.ToObject<List<T>>();
            }
            var t = token.ToObject<T>();
            var list = new List<T>();
            if (t is not null)
            {
                list.Add(t);
            }
            return list;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var list = ((IEnumerable<T>)value).ToList();

            if (list.Count == 1)
            {
                serializer.Serialize(writer, list[0]);
            }
            else
            {
                serializer.Serialize(writer, list);
            }
        }
    }
}
