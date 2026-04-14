using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Cargo
{
    public class BuscaCargoResult : StatusResult
    {
        public BuscaCargoResult()
        {
            Cargos = new List<CargoDTO>();
        }

        [JsonPropertyName("cargos")]
        public List<CargoDTO> Cargos { get; set; }

        [JsonPropertyName("filteredResultCount")]
        public int FilteredResultCount { get; set; }

        [JsonPropertyName("totalResultCount")]
        public int TotalResultCount { get; set; }
    }
}