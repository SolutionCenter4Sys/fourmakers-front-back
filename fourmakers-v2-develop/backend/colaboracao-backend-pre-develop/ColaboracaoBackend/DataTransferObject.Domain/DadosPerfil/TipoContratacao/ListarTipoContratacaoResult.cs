using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.DadosPerfil.TipoContratacao
{
    public class ListarTipoContratacaoResult : StatusResult
    {
        [JsonPropertyName("tipoContratacao")]
        public List<ListarTipoContratacaoDTO> ListaTipoContratacao { get; set; }
    }
}