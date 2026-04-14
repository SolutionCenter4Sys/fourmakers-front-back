using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Colaborador
{
    public class EstatisticasResult : StatusResult
    {
        public EstatisticasResult()
        {
            Estatistica = new List<EstatisticasProcDTO>();
        }

        [JsonPropertyName("estatistica")]
        public List<EstatisticasProcDTO> Estatistica { get; set; }
    }
}