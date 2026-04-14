using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Diretoria
{
    public class BuscaDiretoriaResult : StatusResult
    {
        public BuscaDiretoriaResult()
        {
            Diretorias = new List<DiretoriaDTO>();
        }

        [JsonPropertyName("diretorias")]
        public List<DiretoriaDTO> Diretorias { get; set; }

        [JsonPropertyName("totalResultCount")]
        public int TotalResultCount { get; set; }
    }
}