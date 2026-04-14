using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS
{
    public class DetalheResult : StatusResult
    {
        public DetalheResult()
        {
            Data = new List<MotivosDTO>();
            Detalhe = new List<DetalheDTO>();
        }

        [JsonPropertyName("data")]
        public List<MotivosDTO> Data { get; set; }

        [JsonPropertyName("detalhe")]
        public List<DetalheDTO> Detalhe { get; set; }
    }
}