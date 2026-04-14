using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Dominio
{
    public class ListaDominioColaboradorResult : StatusResult
    {
        public ListaDominioColaboradorResult()
        {
            Dominio = new List<DominioColaboradorDTO>();
        }

        [JsonPropertyName("dominio")]
        public List<DominioColaboradorDTO> Dominio { get; set; }
    }
}