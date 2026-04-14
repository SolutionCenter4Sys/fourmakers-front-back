using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Colaborador
{
    public class TotalizadoresResult : StatusResult
    {
        public TotalizadoresResult()
        {
            Totalizadores = new List<TotalizadoresDTO>();
        }

        [JsonPropertyName("totalizadores")]
        public List<TotalizadoresDTO> Totalizadores { get; set; }
    }
}