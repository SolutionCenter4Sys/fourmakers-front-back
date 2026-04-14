using DataTransferObject.Domain.Certificado;
using DataTransferObject.Domain.Endosso;
using DataTransferObject.Domain.Nivel;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Formacao
{
    public class FormacaoColaboradorDTO
    {
        public FormacaoColaboradorDTO()
        {
            Formacao = new FormacaoDTO();
            Endosso = new StatusEndossoDTO();
            Nivel = new NivelDTO();
            EndossoConcedido = new List<EndossoColaboradorDTO>();
            Certificado = new CertificadoDTO();
        }

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonIgnore]
        public long IdFormacao { get; set; }

        [JsonIgnore]
        public long? IdNivel { get; set; }

        [JsonIgnore]
        public string ColaboradorCpf { get; set; }

        [JsonIgnore]
        public long? IdCertificado { get; set; }

        [JsonPropertyName("formacao")]
        public FormacaoDTO Formacao { get; set; }

        [JsonPropertyName("data")]
        public DateTime Data { get; set; }

        [JsonPropertyName("certificado")]
        public CertificadoDTO Certificado { get; set; }

        [JsonPropertyName("endosso")]
        public StatusEndossoDTO Endosso { get; set; }

        [JsonPropertyName("nivel")]
        public NivelDTO Nivel { get; set; }

        [JsonPropertyName("endossoConcedido")]
        public List<EndossoColaboradorDTO> EndossoConcedido { get; set; }

        [JsonPropertyName("emissor")]
        public string Emissor { get; set; }

        [JsonPropertyName("data_conclusao")]
        public DateTime? DataConclusao { get; set; }
    }
}