using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Foursys
{
    public class RelatorioUsuarioAcessoResult : StatusResult
    {
        [JsonPropertyName("usuarios")]
        public List<UsuarioAcessoDTO> Usuarios { get; set; }
    }
}