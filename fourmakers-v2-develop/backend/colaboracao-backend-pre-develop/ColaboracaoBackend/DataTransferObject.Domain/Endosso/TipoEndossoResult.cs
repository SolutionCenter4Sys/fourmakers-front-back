using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Endosso
{
    public class TipoEndossoResult : StatusResult
    {
        [JsonPropertyName("tiposEndosso")]
        public List<TipoEndossoDTO> TiposEndosso { get; set; }
    }
}