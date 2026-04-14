using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Labs
{
    /// <summary>
    /// Métricas de uso do Match Semântico vs RankCandidatesIds para a organização.
    /// </summary>
    public class MatchSemanticoMetricasDto
    {
        /// <summary>Total de gerações registradas em tb_labs_log_match_semantico.</summary>
        [JsonPropertyName("quantidadeMatchsGerados")]
        public int QuantidadeMatchsGerados { get; set; }

        /// <summary>Preferência pelo RankCandidatesIds (método antigo), entre os feedbacks (0–100).</summary>
        [JsonPropertyName("porcentagemPreferenciaMetodoAntigo")]
        public decimal PorcentagemPreferenciaMetodoAntigo { get; set; }

        /// <summary>Preferência pelo Match Semântico (método novo), entre os feedbacks (0–100).</summary>
        [JsonPropertyName("porcentagemPreferenciaMetodoNovo")]
        public decimal PorcentagemPreferenciaMetodoNovo { get; set; }

        /// <summary>Feedbacks em que o usuário preferiu o método antigo (RankCandidatesIds).</summary>
        [JsonPropertyName("quantidadeFeedbacksMetodoAntigo")]
        public int QuantidadeFeedbacksMetodoAntigo { get; set; }

        /// <summary>Feedbacks em que o usuário preferiu o método novo (Match Semântico).</summary>
        [JsonPropertyName("quantidadeFeedbacksMetodoNovo")]
        public int QuantidadeFeedbacksMetodoNovo { get; set; }
    }
}
