using DataTransferObject.Domain.Base;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Projeto
{
    public class ColaboradorProjetoGetByIdResult : StatusResult
    {
        [JsonPropertyName("colaboradorProjeto")]
        public ColaboradorProjetoDTO ColaboradorProjeto { get; set; }
    }
}