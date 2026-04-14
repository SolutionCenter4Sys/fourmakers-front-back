using DataTransferObject.Domain.CCH;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.DadosPerfil.TipoContratacao
{
    public class TipoContratacaoResponse
    {
        [JsonPropertyName("colaboradores")]
        public List<ColaboradorCCH> Colaboradores { get; set; }

        [JsonPropertyName("recurso")]
        public RecursoCCH Recurso { get; set; }
    }
}