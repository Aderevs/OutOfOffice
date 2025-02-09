using System.Text.Json.Serialization;
using System.Text.Json;
using System.ComponentModel;
using System.Reflection;
using OutOfOffice.Attributes;

namespace OutOfOffice
{
    public class EnumNameConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
    {
        public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException($"Unexpected token type '{reader.TokenType}' when parsing enum.");
            }

            string enumString = reader.GetString();
            if (Enum.TryParse<TEnum>(enumString, true, out TEnum value))
            {
                return value;
            }

            throw new JsonException($"Unable to parse enum '{enumString}'.");
        }

        public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
        {
            var field = value.GetType().GetField(value.ToString());
            var translatedValue = field?.GetCustomAttribute<DisplayJsonAttribute>().GetValue();
            writer.WriteStringValue(translatedValue);
        }
    }
}
