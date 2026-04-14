using DataTransferObject.Domain.Base;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.MapaDeAlocacao
{
    public class CalculoHorasTotalResult : StatusResult
    {
        [JsonPropertyName("totalHoras")]
        public HorasTotaisDTO TotalHoras { get; set; }
    }
}