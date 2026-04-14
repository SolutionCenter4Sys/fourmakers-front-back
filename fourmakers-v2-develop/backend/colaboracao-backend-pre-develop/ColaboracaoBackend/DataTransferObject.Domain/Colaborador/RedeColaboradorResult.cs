using DataTransferObject.Domain.Base;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Colaborador
{
    public class RedeColaboradorResult : StatusResult
    {
        [JsonPropertyName("redeColaborador")]
        public RedeColaboradorDTO RedeColaborador { get; set; }
    }
}