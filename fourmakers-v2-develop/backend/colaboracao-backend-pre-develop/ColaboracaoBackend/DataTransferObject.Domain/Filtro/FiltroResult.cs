using DataTransferObject.Domain.Base;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Filtro
{
    public class FiltroResult : StatusResult
    {
        public FiltroResult()
        {
            FiltroDTO = new FiltroDTO();
        }

        [JsonPropertyName("filtroDTO")]
        public FiltroDTO FiltroDTO { get; set; }

        [JsonPropertyName("colaborador")]
        public string Colaborador { get; set; }
    }
}