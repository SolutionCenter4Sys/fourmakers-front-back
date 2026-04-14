using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Projeto
{
    public class AddColaboradorProjetoParam
    {
        [JsonPropertyName("colaboradorProjeto")]
        public ColaboradorProjetoParam ColaboradorProjeto { get; set; }
    }
}