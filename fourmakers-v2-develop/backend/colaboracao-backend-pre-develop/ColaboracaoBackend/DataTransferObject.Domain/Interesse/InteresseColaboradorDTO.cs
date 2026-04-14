using DataTransferObject.Domain.Base;
using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Interesse
{
    public class InteresseColaboradorDTO
    {
        [JsonIgnore]
        public string ColaboradorCpf { get; set; }

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("interesse")]
        public ItemPerfilDTO Interesse { get; set; }

        [JsonPropertyName("data")]
        public DateTime Data { get; set; }
    }
}