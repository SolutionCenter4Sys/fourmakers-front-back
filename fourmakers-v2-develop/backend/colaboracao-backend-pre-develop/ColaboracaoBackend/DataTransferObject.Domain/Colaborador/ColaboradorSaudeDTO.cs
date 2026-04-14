using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Colaborador
{
    public class ColaboradorSaudeDTO
    {
        [JsonPropertyName("pcd")]
        public EnumPCD PCD { get; set; }

        private int? _enumPCDValue;

        [JsonPropertyName("enum_pcd")]
        public int? EnumPCD
        {
            get
            {
                return (int)PCD;
            }
            set
            {
                _enumPCDValue = value;
                // Se o front enviar o número do enum, converter para o enum
                if (value.HasValue && Enum.IsDefined(typeof(EnumPCD), value.Value))
                {
                    PCD = (EnumPCD)value.Value;
                }
            }
        }

        [JsonPropertyName("grupoDeRiscoCovid")]
        public sbyte GrupoDeRiscoCovid { get; set; }

        [JsonPropertyName("condicaoDeSaudeRelevante")]
        public string CondicaoDeSaudeRelevante { get; set; }
    }
}