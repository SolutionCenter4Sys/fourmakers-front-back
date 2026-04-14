using DataTransferObject.Domain.Colaborador;
using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.FeedAtualizacoes
{
    public class FeedV2DTO
    {
        [JsonPropertyName("colaborador")]
        public ColaboradorDTO Colaborador { get; set; }

        [JsonPropertyName("tipo")]
        public TipoFeed Tipo { get; set; }

        [JsonPropertyName("tipoScreen")]
        public string TipoScreen { get; set; }

        [JsonPropertyName("data")]
        public DateTime Data { get; set; }

        [JsonPropertyName("tipoValor")]
        public Object TipoValor { get; set; }

        [JsonPropertyName("curtidas")]
        public int Curtidas { get; set; }

        [JsonPropertyName("curtido")]
        public bool Curtido { get; set; }
    }
}