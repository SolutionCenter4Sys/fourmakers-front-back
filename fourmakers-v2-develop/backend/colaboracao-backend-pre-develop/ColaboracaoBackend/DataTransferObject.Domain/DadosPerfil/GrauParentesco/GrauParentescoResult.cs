using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.DadosPerfil.GrauParentesco
{
    public class GrauParentescoResult : StatusResult
    {
        [JsonPropertyName("grauParentesco")]
        public List<GrauParentescoDTO> GrauParentesco { get; set; }
    }
}