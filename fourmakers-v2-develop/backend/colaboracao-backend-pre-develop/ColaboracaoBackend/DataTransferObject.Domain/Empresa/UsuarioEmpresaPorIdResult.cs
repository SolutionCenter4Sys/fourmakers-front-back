using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Empresa
{
    public class UsuarioEmpresaPorIdResult : StatusResult
    {
        [JsonPropertyName("usuariosEmpresa")]
        public List<UsuarioEmpresaPorIdDTO> UsuariosEmpresa { get; set; }
    }
}