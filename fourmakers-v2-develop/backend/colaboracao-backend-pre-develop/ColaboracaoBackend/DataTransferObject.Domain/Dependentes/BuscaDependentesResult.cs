using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Dependentes
{
    public class BuscaDependentesResult : StatusResult
    {
        public BuscaDependentesResult()
        {
            Dependentes = new List<DependentesDTO>();
        }

        [JsonPropertyName("dependentes")]
        public List<DependentesDTO> Dependentes { get; set; }
        [JsonPropertyName("quantidadeDependentes")]
        public int QuantidadeDependentes { get; set; }
        [JsonPropertyName("quantidadeDependentesFilhos")]
        public int QuantidadeDependentesFilhos { get; set; }
    }
}