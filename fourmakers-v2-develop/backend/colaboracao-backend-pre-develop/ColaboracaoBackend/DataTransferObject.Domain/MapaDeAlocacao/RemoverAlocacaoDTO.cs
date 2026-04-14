using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.MapaDeAlocacao
{
    public class RemoverAlocacaoDTO
    {
        [JsonPropertyName("idColaboradorAlocado")]
        public int idColaboradorAlocado { get; set; }
    }
}