using Newtonsoft.Json;
using OneOf;

namespace Cryptosuite.Core.Util
{
    public class OneOfConverter<T, U> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsAssignableFrom(typeof(OneOf<T, U>));
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var oneOf = (OneOf<T, U>)value;

            if (oneOf.IsT0)
            {
                serializer.Serialize(writer, oneOf.AsT0);
            }
            else
            {
                serializer.Serialize(writer, oneOf.AsT1);
            }
        }
    }
}
