using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.DadosPerfil
{
    public class EstadoCivilResult : StatusResult
    {
        [JsonPropertyName("EstadoCivil")]
        public List<EstadoCivilDTO> EstadoCivil { get; set; }
    }
}