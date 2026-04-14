using DataTransferObject.Domain.Colaborador;
using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.FeedAtualizacoes
{
    public class FeedDTO
    {
        [JsonPropertyName("colaborador")]
        public ColaboradorDTO Colaborador { get; set; }

        [JsonPropertyName("tipo")]
        public TipoFeed Tipo { get; set; }

        [JsonPropertyName("data")]
        public DateTime Data { get; set; }

        [JsonPropertyName("tipoValor")]
        public Object TipoValor { get; set; }

        [JsonPropertyName("curtidas")]
        public int Curtidas { get; set; }

        [JsonPropertyName("curtido")]
        public bool Curtido { get; set; }
    }

    public enum TipoFeed
    {
        COMPETENCIA = 1,
        FORMACAO,
        DOMINIO,
        METODOLOGIA,
        MODELO,
        INTERESSE,
        HOBBIE,
        NOTICIA
    }
}