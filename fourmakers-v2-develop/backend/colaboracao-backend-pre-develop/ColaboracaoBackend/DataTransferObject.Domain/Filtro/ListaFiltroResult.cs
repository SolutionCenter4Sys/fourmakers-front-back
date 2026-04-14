using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Filtro
{
    public class ListaFiltroResult : StatusResult
    {
        public ListaFiltroResult()
        {
            Filtro = new List<FiltroDTO>();
        }

        [JsonPropertyName("filtro")]
        public List<FiltroDTO> Filtro { get; set; }
    }
}