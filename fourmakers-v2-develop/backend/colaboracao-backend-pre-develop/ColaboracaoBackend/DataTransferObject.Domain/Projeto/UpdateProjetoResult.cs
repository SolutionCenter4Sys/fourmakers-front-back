using DataTransferObject.Domain.Base;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Projeto
{
    public class UpdateProjetoResult : StatusResult
    {
        [JsonPropertyName("projeto")]
        public UpdateProjetoDTO projeto { get; set; }
    }
}