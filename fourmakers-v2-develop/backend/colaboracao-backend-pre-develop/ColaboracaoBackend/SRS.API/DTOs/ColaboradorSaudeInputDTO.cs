using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SRS.API.DTOs
{
    /// <summary>
    /// DTO de entrada para ColaboradorSaude que aceita grupoDeRiscoCovid como booleano
    /// </summary>
    public class ColaboradorSaudeInputDTO
    {
        [JsonPropertyName("pcd")]
        public DataTransferObject.Domain.Colaborador.EnumPCD? PCD { get; set; }

        [JsonPropertyName("grupoDeRiscoCovid")]
        [JsonConverter(typeof(BooleanToSByteConverter))]
        public sbyte GrupoDeRiscoCovid { get; set; }

        [JsonPropertyName("condicaoDeSaudeRelevante")]
        public string CondicaoDeSaudeRelevante { get; set; }
    }

    /// <summary>
    /// Conversor JSON customizado para converter booleano para sbyte (0 ou 1)
    /// </summary>
    public class BooleanToSByteConverter : JsonConverter<sbyte>
    {
        public override sbyte Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.True)
            {
                return 1;
            }
            else if (reader.TokenType == JsonTokenType.False)
            {
                return 0;
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                if (reader.TryGetInt32(out int intValue))
                {
                    if (intValue == 0 || intValue == 1)
                    {
                        return (sbyte)intValue;
                    }
                    throw new JsonException($"O valor '{intValue}' não é válido para grupoDeRiscoCovid. Deve ser um booleano (true/false) ou 0/1.");
                }
                else if (reader.TryGetInt64(out long longValue))
                {
                    if (longValue == 0 || longValue == 1)
                    {
                        return (sbyte)longValue;
                    }
                    throw new JsonException($"O valor '{longValue}' não é válido para grupoDeRiscoCovid. Deve ser um booleano (true/false) ou 0/1.");
                }
                throw new JsonException($"O valor numérico não é válido para grupoDeRiscoCovid. Deve ser 0 ou 1.");
            }
            else
            {
                throw new JsonException($"Tipo de token '{reader.TokenType}' não é válido para grupoDeRiscoCovid. Esperado: booleano (true/false) ou número (0/1).");
            }
        }

        public override void Write(Utf8JsonWriter writer, sbyte value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }
}

