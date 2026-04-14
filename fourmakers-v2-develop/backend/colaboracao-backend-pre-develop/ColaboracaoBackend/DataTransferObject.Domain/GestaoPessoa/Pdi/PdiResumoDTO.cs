using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace DataTransferObject.Domain.GestaoPessoa.Pdi
{
    /// <summary>
    /// PDI completo (meus PDIs e detalhe).
    /// </summary>
    public class PdiResumoDTO
    {
        public Guid Id { get; set; }
        public string ColaboradorId { get; set; }

        [JsonProperty("nome_colaborador")]
        [JsonPropertyName("nome_colaborador")]
        public string NomeColaborador { get; set; }

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
    }
}
