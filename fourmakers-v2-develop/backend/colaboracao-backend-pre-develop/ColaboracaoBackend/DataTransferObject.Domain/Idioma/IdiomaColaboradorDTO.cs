using DataTransferObject.Domain.Endosso;
using DataTransferObject.Domain.Nivel;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Idioma
{
    public class IdiomaColaboradorDTO
    {
        public IdiomaColaboradorDTO()
        {
            Idioma = new IdiomaDTO();
            Endosso = new StatusEndossoDTO();
            Nivel = new NivelDTO();
            EndossoConcedido = new List<EndossoColaboradorDTO>();
        }

        public long Id { get; set; }

        [JsonIgnore]
        public int IdIdioma { get; set; }

        [JsonIgnore]
        public long? IdNivel { get; set; }

        [JsonIgnore]
        public string ColaboradorCpf { get; set; }

        public IdiomaDTO Idioma { get; set; }
        public DateTime Data { get; set; }
        public StatusEndossoDTO Endosso { get; set; }
        public NivelDTO Nivel { get; set; }
        public List<EndossoColaboradorDTO> EndossoConcedido { get; set; }
    }
}