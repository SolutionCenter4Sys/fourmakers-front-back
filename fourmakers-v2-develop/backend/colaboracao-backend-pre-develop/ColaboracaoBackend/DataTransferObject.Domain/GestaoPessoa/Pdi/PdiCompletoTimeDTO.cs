using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.GestaoPessoa.Pdi
{
    /// <summary>
    /// PDI completo para visão do líder (pdis-time).
    /// </summary>
    public class PdiCompletoTimeDTO
    {
        public Guid Id { get; set; }
        public string ColaboradorId { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string Status { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public string CodigoInternoColaboradorCriacao { get; set; }
        public string CodigoInternoColaboradorAlteracao { get; set; }
        public double Progress { get; set; }
        public DateTime? DeadLine { get; set; }
        public List<PdiSkillDTO> Skills { get; set; } = new List<PdiSkillDTO>();
        public List<PdiActionPlanDTO> ActionPlans { get; set; } = new List<PdiActionPlanDTO>();
        public List<PdiEvidenciaResumoDTO> Evidencias { get; set; } = new List<PdiEvidenciaResumoDTO>();
    }

    public class PdiEvidenciaResumoDTO
    {
        public Guid Id { get; set; }
        public string DocName { get; set; }
        public string DocPath { get; set; }
        public string Tipo { get; set; }

        [JsonPropertyName("link")]
        public string Link { get; set; }
    }
}
