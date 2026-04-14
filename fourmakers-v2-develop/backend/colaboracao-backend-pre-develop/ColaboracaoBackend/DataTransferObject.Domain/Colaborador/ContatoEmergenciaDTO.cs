using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Colaborador
{
    public class ContatoEmergenciaDTO
    {
        [JsonPropertyName("contact_order")]
        public string Ordem { get; set; } //Remover após refactor
        [JsonPropertyName("name")]
        public string Nome { get; set; }

        [JsonPropertyName("degree_kinship")]
        public string GrauParentesco { get; set; }

        [JsonPropertyName("phone")]
        public string Telefone { get; set; }
    }
}