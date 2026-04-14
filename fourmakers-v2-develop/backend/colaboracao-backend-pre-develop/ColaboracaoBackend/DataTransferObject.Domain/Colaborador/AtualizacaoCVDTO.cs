using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Colaborador
{
    public class AtualizacaoCVDTO
    {
        [JsonPropertyName("origem")]
        public string Origem { get; set; }

        [JsonPropertyName("data")]
        public DateTime Data { get; set; }
    }
}