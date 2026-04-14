using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.BI
{
    public class ListaDeCpfColaboradoresECandidatosResult : StatusResult
    {
        public ListaDeCpfColaboradoresECandidatosResult()
        {
            Colaboradores = new List<RetornoColaboradorBIDTO>();
        }

        [JsonPropertyName("colaboradores")]
        public List<RetornoColaboradorBIDTO> Colaboradores { get; set; }
    }
}