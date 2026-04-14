using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Apontamento.FolhaPonto
{
    public class ItemLoteDTO
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("dataCriacao")]
        public DateTime DataCriacao { get; set; }

        [JsonPropertyName("dataFinalizacao")]
        public DateTime? DataFinalizacao { get; set; }

        [JsonPropertyName("filePath")]
        public string FilePath { get; set; }
        [JsonIgnore]
        public string Retorno { get; set; }

        [JsonIgnore]
        public string LoteId { get; set; }

        [JsonPropertyName("sucesso")]
        public bool Sucesso { get; set; }
    }
} 