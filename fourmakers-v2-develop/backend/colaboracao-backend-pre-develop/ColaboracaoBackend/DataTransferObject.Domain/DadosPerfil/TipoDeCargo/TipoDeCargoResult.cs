using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.DadosPerfil
{
    public class TipoDeCargoResult : StatusResult
    {
        [JsonPropertyName("TipoDeCargo")]
        public List<TipoDeCargoDTO> TipoDeCargo { get; set; }
    }
}