using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Softskill
{
    public class ListaSoftskillColaboradorResult : StatusResult
    {
        public ListaSoftskillColaboradorResult()
        {
            Softskill = new List<SoftskillColaboradorDTO>();
        }

        [JsonPropertyName("retorno")]
        public List<SoftskillColaboradorDTO> Softskill { get; set; }
    }
}