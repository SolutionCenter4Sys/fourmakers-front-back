using DataTransferObject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Experiencia
{
    public class SugestaoEmpresaResult : StatusResult
    {
        [JsonPropertyName("SugestaoEmpresas")]
        public List<String> sugestaoEmpresas { get; set; }
    }
}