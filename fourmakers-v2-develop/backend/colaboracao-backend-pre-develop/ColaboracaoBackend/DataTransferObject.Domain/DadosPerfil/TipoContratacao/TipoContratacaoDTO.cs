using DataTransferObject.Domain.CCH;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.DadosPerfil.TipoContratacao
{
    public class TipoContratacaoDTO
    {
        [JsonPropertyName("recurso")]
        public RecursoCCH Recurso { get; set; }

        [JsonPropertyName("tempoNaEmpresa")]
        public string TempoNaEmpresa { get; set; }
    }
}