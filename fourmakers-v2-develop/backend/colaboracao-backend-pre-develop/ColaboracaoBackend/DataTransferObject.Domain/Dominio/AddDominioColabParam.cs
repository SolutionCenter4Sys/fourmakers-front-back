using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Dominio
{
    public class AddDominioColabParam
    {
        [JsonPropertyName("id")]
        public long DominioId { get; set; }
        public string Descricao { get; set; }

        [JsonPropertyName("nivelId")]
        public long? NivelId { get; set; }

        [JsonPropertyName("cpf")]
        public string Cpf { get; set; }

        [JsonPropertyName("gestorExternoPerfil")]
        public string GestorExternoPerfil { get; set; }
    }
}