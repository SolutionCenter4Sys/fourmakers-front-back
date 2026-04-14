using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Empresa
{
    public class ListaEmpresaResult : StatusResult
    {
        [JsonPropertyName("empresas")]
        public List<EmpresaDTO> Empresas { get; set; }
    }
}