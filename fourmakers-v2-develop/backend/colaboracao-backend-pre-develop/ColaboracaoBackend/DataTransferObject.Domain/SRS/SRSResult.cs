using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS
{
    public class SRSResult : StatusResult
    {
        public SRSResult()
        {
            SrsDTO = new List<SRSDTO>();
            Data = new List<MotivosDTO>();
            Detalhe = new List<DetalheDTO>();
        }

        [JsonPropertyName("srsDTO")]
        public List<SRSDTO> SrsDTO { get; set; }

        [JsonPropertyName("data")]
        public List<MotivosDTO> Data { get; set; }

        [JsonPropertyName("detalhe")]
        public List<DetalheDTO> Detalhe { get; set; }
    }
}