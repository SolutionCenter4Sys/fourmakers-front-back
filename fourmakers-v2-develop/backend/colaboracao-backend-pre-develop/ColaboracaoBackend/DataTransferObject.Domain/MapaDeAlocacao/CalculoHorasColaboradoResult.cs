using DataTransferObject.Domain.Base;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.MapaDeAlocacao
{
    public class CalculoHorasColaboradoResult : StatusResult
    {
        [JsonPropertyName("totalHorasColaborador")]
        public long TotalHorasColaborador { get; set; }
    }
}