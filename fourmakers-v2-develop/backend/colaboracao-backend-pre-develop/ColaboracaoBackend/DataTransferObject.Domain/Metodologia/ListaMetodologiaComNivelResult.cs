using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Nivel;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Metodologia
{
    public class ListaMetodologiaComNivelResult : StatusResult
    {
        [JsonPropertyName("retorno")]
        public List<MetodologiaDTO> Retorno { get; set; }

        public List<NivelDTO> Nivel { get; set; }
    }
}