using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Softskill
{
    public class SoftskillColaboradorResult : StatusResult
    {
        [JsonPropertyName("respostas")]
        public List<ItemPerfilResult> Respostas { get; set; }
    }
}