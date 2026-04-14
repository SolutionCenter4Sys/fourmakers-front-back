using DataTransferObject.Domain.Base;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Colaborador
{
    public class NomeColaboradorResult : StatusResult
    {
        public NomeColaboradorResult()
        {
            Colaborador = new SimpleColaboradorDTO();
        }

        [JsonPropertyName("colaborador")]
        public SimpleColaboradorDTO Colaborador { get; set; }
    }
}