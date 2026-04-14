using DataTransferObject.Domain.Colaborador;
using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Endosso
{
    public class EndossoColaboradorDTO
    {
        [JsonIgnore]
        public string ColaboradorCpf { get; set; }

        [JsonIgnore]
        public int? TipoEndossoId { get; set; }

        [JsonPropertyName("dataEndosso")]
        public DateTime DataEndosso { get; set; }

        [JsonPropertyName("colaborador")]
        public ColaboradorDTO Colaborador { get; set; }

        [JsonPropertyName("tipoEndosso")]
        public TipoEndossoDTO TipoEndosso { get; set; }
    }
}