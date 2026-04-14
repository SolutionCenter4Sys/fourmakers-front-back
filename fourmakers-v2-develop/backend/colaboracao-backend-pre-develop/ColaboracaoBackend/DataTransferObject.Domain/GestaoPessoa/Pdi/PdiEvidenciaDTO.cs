using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.GestaoPessoa.Pdi
{
    public class PdiEvidenciaDTO
    {
        public Guid Id { get; set; }
        public Guid PdiId { get; set; }
        public string DocName { get; set; }
        public string DocPath { get; set; }
        public string DocMime { get; set; }
        public long? DocSize { get; set; }
        public string Tipo { get; set; }
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("link")]
        public string Link { get; set; }
    }
}
