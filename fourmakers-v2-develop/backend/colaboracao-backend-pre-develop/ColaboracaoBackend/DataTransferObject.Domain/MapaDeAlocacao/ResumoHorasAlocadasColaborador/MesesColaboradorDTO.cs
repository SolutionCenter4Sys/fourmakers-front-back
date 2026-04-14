using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.MapaDeAlocacao
{
    public class MesesColaboradorDTO
    {
        [JsonPropertyName("nomeMesAtual")]
        public string NomeMesAtual { get; set; }
    }
}