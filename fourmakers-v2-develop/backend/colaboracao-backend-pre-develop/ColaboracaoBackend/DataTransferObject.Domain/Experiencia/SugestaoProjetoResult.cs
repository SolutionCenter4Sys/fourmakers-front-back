using DataTransferObject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Experiencia
{
    public class SugestaoProjetoResult : StatusResult
    {
        [JsonPropertyName("SugestaoProjetos")]
        public List<String> sugestaoProjetos { get; set; }
    }
}