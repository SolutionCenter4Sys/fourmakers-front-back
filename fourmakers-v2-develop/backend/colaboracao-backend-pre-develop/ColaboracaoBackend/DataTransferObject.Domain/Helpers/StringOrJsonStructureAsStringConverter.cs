using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DataTransferObject.Domain.Helpers
{
    /// <summary>
    /// Newtonsoft: aceita string, array ou objeto no JSON e normaliza para <see cref="string"/>.
    /// Útil quando a API externa envia o mesmo campo ora como texto, ora como coleção.
    /// </summary>
    public sealed class StringOrJsonStructureAsStringConverter : JsonConverter<string>
    {
        public override string ReadJson(
            JsonReader reader,
            Type objectType,
            string existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            if (reader.TokenType == JsonToken.String)
                return (string)reader.Value;

            var token = JToken.Load(reader);
            return token?.ToString(Formatting.None);
        }

        public override void WriteJson(JsonWriter writer, string value, JsonSerializer serializer)
        {
            if (value == null)
                writer.WriteNull();
            else
                writer.WriteValue(value);
        }
    }
}
