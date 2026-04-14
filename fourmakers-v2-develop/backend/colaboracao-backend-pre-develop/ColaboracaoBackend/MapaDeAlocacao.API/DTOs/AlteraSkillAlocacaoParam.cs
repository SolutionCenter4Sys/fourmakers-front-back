using DataTransferObject.Domain;
using System.Text.Json.Serialization;

namespace MapaDeAlocacao.API.DTOs
{
    public class AlteraSkillAlocacaoParam
    {
        [JsonPropertyName("periodoAlocacaoId")]
        public long PeriodoAlocacaoId { get; set; }
        [JsonPropertyName("skill")]
        public ItemSkillPerfilAlocacaoDTO Skill { get; set; }
    }
}