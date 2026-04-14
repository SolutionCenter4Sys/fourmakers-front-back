using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.DadosPerfil
{
    public class DisponibilidadeResult : StatusResult
    {
        [JsonPropertyName("disponibilidade")]
        public List<DisponibilidadeDTO> Disponibilidade { get; set; }
    }
}