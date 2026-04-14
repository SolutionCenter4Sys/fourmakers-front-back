using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.MapaDeAlocacao
{
    public class GestorDTO
    {
        [JsonPropertyName("codigoProfissional")]
        public string CodigoProfissional { get; set; }
        [JsonPropertyName("nome")]
        public string NomeGestor { get; set; }
    }
}