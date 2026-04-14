using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace DataTransferObject.Domain.GestaoPessoa.Pdi
{
    /// <summary>
    /// Item de PDI para listas de métricas (Ativos e Históricos).
    /// </summary>
    public class PdiMetricaItemDTO
    {
        public Guid PdiId { get; set; }
        /// <summary>Código interno do gestor do titular (via <c>vw_gestores_colaboradores_org</c>); nulo se não houver vínculo.</summary>
        public string GestorId { get; set; }
        /// <summary>Nome completo do gestor (<c>tb_colaborador.nome_completo</c>).</summary>
        [JsonProperty("nome_gestor")]
        [JsonPropertyName("nome_gestor")]
        public string NomeGestor { get; set; }
        public string ColaboradorId { get; set; }
        /// <summary>Nome completo do colaborador titular do PDI.</summary>
        [JsonProperty("nome_completo")]
        [JsonPropertyName("nome_completo")]
        public string NomeCompleto { get; set; }
        public string Titulo { get; set; }
        public string Status { get; set; }
        public double Progress { get; set; }
        /// <summary>Previsão de conclusão (maior deadline dos action plans do PDI).</summary>
        public DateTime? Previsao { get; set; }
    }
}
