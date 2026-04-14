using DataTransferObject.Domain.Base;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Apontamento
{
    public class ApontamentosPorVigenciaResult : StatusResult
    {
        [JsonPropertyName("apontamentos_por_vigencia")]
        public ApontamentosPorVigenciaDTO ApontamentosPorVigencia { get; set; }
    }
}