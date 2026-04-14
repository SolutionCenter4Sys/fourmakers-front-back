using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Projeto
{
    public class StatusProjetosDTO
    {
        [JsonPropertyName("nomeStatusProjeto")]
        public string NomeStatusProjeto { get; set; }
        [JsonPropertyName("codigoStatusProjeto")]
        public long CodigoStatusProjeto { get; set; }
    }
}