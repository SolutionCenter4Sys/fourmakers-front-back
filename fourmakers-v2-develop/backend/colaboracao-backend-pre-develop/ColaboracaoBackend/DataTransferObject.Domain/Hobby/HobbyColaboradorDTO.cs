using DataTransferObject.Domain.Base;
using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Hobby
{
    public class HobbyColaboradorDTO
    {
        [JsonIgnore]
        public string ColaboradorCpf { get; set; }

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("hobbie")]
        public ItemPerfilDTO Hobbie { get; set; }

        [JsonPropertyName("data")]
        public DateTime Data { get; set; }
    }
}