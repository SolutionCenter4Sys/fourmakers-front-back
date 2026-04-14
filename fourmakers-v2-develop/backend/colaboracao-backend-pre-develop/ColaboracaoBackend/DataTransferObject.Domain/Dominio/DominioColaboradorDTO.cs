using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Endosso;
using DataTransferObject.Domain.Nivel;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Dominio
{
    public class DominioColaboradorDTO
    {
        public DominioColaboradorDTO()
        {
            Dominio = new ItemPerfilDTO();
            Endosso = new StatusEndossoDTO();
            Nivel = new NivelDTO();
            EndossoConcedido = new List<EndossoColaboradorDTO>();
        }

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonIgnore]
        public long IdDominio { get; set; }

        [JsonIgnore]
        public long? IdNivel { get; set; }

        [JsonIgnore]
        public string ColaboradorCpf { get; set; }

        [JsonPropertyName("dominio")]
        public ItemPerfilDTO Dominio { get; set; }

        [JsonPropertyName("data")]
        public DateTime Data { get; set; }

        [JsonPropertyName("endosso")]
        public StatusEndossoDTO Endosso { get; set; }

        [JsonPropertyName("nivel")]
        public NivelDTO Nivel { get; set; }

        [JsonPropertyName("endossoConcedido")]
        public List<EndossoColaboradorDTO> EndossoConcedido { get; set; }
    }
}