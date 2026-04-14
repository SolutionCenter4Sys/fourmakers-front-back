using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.MapaDeAlocacao
{
    public class RemoverPeriodoAlocado
    {
        [JsonPropertyName("idPeriodoAlocacao")]
        public long idPeriodoAlocacao { get; set; }
    }
}