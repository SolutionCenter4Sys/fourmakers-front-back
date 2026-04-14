using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS
{
    public class InscricoesCandidatoResult : StatusResult
    {
        public InscricoesCandidatoResult()
        {
            Data = new List<VagasInscritoDTO>();
        }

        [JsonPropertyName("data")]
        public List<VagasInscritoDTO> Data { get; set; }
    }
}