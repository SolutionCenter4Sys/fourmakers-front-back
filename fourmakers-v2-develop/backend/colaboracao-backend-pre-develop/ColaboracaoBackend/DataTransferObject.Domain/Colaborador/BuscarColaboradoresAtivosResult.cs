using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Colaborador
{
    public class BuscarColaboradoresAtivosResult : StatusResult
    {
        [JsonPropertyName("colaboradores")]
        public List<ColaboradorAtivoDTO> Colaboradores { get; set; }

        [JsonPropertyName("filteredResultCount")]
        public int FilteredResultCount { get; set; }

        [JsonPropertyName("totalResultCount")]
        public int TotalResultCount { get; set; }
    }
}