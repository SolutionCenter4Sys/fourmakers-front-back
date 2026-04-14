using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Competencia;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Endosso
{
    public class PedidoEndossoDTO
    {
        [JsonPropertyName("tipoItemPerfil")]
        public EnumItemPerfilEndosso TipoItemPerfil { get; set; }

        [JsonPropertyName("idEndosso")]
        public long IdEndosso { get; set; }

        [JsonPropertyName("itemPerfilInfo")]
        public CompetenciaDTO ItemPerfilInfo { get; set; }

        [JsonPropertyName("respostas")]
        public List<TipoEndossoDTO> Respostas { get; set; }

        [JsonPropertyName("colaborador")]
        public ColaboradorDTO Colaborador { get; set; }

        [JsonPropertyName("cpfSolicitante")]
        public string CpfSolicitante { get; set; }
    }
}