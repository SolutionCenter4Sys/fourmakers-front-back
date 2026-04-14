using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Empresa
{
    public class ListaUsuarioEmpresaResult : StatusResult
    {
        [JsonPropertyName("usuarios_empresa")]
        public List<UsuarioEmpresaDTO> UsuariosEmpresa { get; set; }
    }
}