using DataTransferObject.Domain.Base;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Projeto
{
    public class ProjetoResult : StatusResult
    {
        [JsonPropertyName("projeto")]
        public ProjetoDTO projeto { get; set; }
    }
}