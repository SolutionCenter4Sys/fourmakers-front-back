using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Colaborador
{
    public class PassaporteColaboradorResult : StatusResult
    {
        [JsonPropertyName("passaporteColaborador")]
        public IEnumerable<PassaporteColaboradorDTO> PassaportesColaborador { get; set; }
    }
}