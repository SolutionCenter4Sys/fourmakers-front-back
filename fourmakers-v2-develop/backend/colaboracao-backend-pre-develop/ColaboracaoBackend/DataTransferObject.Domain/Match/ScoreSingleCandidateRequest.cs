using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Match
{
    using System.Text.Json.Serialization;

    public class ScoreSingleCandidateRequest
    {
        [JsonPropertyName("codigo_interno_colaborador")]
        public string CodigoInternoColaborador { get; set; }

        [JsonPropertyName("hard_skills")]
        public List<SkillItem> HardSkills { get; set; }

        [JsonPropertyName("soft_skills")]
        public List<SkillItem> SoftSkills { get; set; }

        [JsonPropertyName("metodologias")]
        public List<SkillItem> Metodologias { get; set; }

        [JsonPropertyName("dominios_negocio")]
        public List<SkillItem> DominiosNegocio { get; set; }

        [JsonPropertyName("idiomas")]
        public List<SkillItem> Idiomas { get; set; }

        [JsonPropertyName("disponibilidades")]
        public List<DisponibilidadeItem> Disponibilidades { get; set; }

        [JsonPropertyName("peso_hard_skills")]
        public double PesoHardSkills { get; set; }

        [JsonPropertyName("peso_soft_skills")]
        public double PesoSoftSkills { get; set; }

        [JsonPropertyName("peso_metodologias")]
        public double PesoMetodologias { get; set; }

        [JsonPropertyName("peso_dominios_negocio")]
        public double PesoDominiosNegocio { get; set; }

        [JsonPropertyName("peso_idiomas")]
        public double PesoIdiomas { get; set; }

        [JsonPropertyName("peso_disponibilidades")]
        public double PesoDisponibilidades { get; set; }

        [JsonPropertyName("visible_to_org_ids")]
        public List<int> VisibleToOrgIds { get; set; }

        [JsonPropertyName("numero_de_candidatos")]
        public int NumeroDeCandidatos { get; set; }
    }

    public class SkillItem
    {
        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("nivel")]
        public string Nivel { get; set; }

        [JsonPropertyName("obrigatoriedade")]
        public string Obrigatoriedade { get; set; }
    }

    public class SkillItemPerfilId  : SkillItem
    {
        public int PerfilTipoId { get; set; }
        public string TipoPerfil { get; set; }
    }


    public class DisponibilidadeItem
    {
        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("obrigatoriedade")]
        public string Obrigatoriedade { get; set; }
    }

}