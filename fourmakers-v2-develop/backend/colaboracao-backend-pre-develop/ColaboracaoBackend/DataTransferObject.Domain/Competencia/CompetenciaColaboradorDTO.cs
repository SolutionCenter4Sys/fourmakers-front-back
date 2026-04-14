using DataTransferObject.Domain.Certificado;
using DataTransferObject.Domain.Endosso;
using DataTransferObject.Domain.Nivel;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Competencia
{
    public class CompetenciaColaboradorDTO
    {
        public CompetenciaColaboradorDTO()
        {
            Competencia = new CompetenciaDTO();
            Endosso = new StatusEndossoDTO();
            Nivel = new NivelDTO();
            EndossoConcedido = new List<EndossoColaboradorDTO>();
            Certificados = new List<CertificadoDTO>();
        }

        public long Id { get; set; }

        [JsonIgnore]
        public long IdCompetencia { get; set; }

        [JsonIgnore]
        public long? IdNivel { get; set; }

        [JsonIgnore]
        public string ColaboradorCpf { get; set; }

        [JsonIgnore]
        public List<long> ListaIdCertificado { get; set; }

        public CompetenciaDTO Competencia { get; set; }

        public DateTime Data { get; set; }

        public List<CertificadoDTO> Certificados { get; set; }

        public StatusEndossoDTO Endosso { get; set; }

        public NivelDTO? Nivel { get; set; }
        public List<EndossoColaboradorDTO> EndossoConcedido { get; set; }
    }
}