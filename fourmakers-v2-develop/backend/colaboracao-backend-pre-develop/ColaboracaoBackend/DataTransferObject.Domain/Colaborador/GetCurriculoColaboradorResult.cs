using DataTransferObject.Domain.Base;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Colaborador
{
    public class GetCurriculoColaboradorResult : StatusResult
    {
        [JsonPropertyName("curriculo")]
        public string file { get; set; }
    }
}