using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Candidato
{
    public class ListaCandidatoResult : StatusResult
    {
        [JsonPropertyName("colaboradores")]
        public List<ColaboradorDTO> Colaboradores { get; set; }

        [JsonPropertyName("filteredResultCount")]
        public int FilteredResultCount { get; set; }

        [JsonPropertyName("totalResultCount")]
        public int TotalResultCount { get; set; }
    }
}