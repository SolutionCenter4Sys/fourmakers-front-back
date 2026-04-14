using DataTransferObject.Domain.Base;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Projeto
{
    public class AddColaboradorProjetoResult : StatusResult
    {
        [JsonPropertyName("colaboradorProjeto")]
        public ColaboradorProjetoParam ColaboradorProjeto { get; set; }
    }
}