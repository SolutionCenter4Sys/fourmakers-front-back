using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Match
{
    public class CandidatosMatchResponse
    {
        [JsonPropertyName("codigoInternoColaborador")]
        public string CodigoInternoColaborador { get; set; }

        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("orgs")]
        public List<int> Orgs { get; set; }

        [JsonPropertyName("match")]
        public double Match { get; set; }

        [JsonPropertyName("score_candidato")]
        public double ScoreCandidato { get; set; }

        [JsonPropertyName("score_vaga")]
        public double ScoreVaga { get; set; }

        [JsonPropertyName("detalhamento_calculo")]
        public DetalhamentoCalculo DetalhamentoCalculo { get; set; }

        [JsonPropertyName("comparativo_por_skill")]
        public ComparativoPorSkill ComparativoPorSkill { get; set; }

        [JsonPropertyName("origem")]
        public string Origem { get; set; }

        [JsonPropertyName("comunidade")]
        public string Comunidade { get; set; }
    }

    public class CandidatoComPerfilResponse
    {
        public string? TipoId { get; set; }
        public string? Perfil { get; set; }
        public CandidatosMatchResponse MatchResponse { get; set; }

    }
    public class DetalhamentoCalculo
    {
        [JsonPropertyName("hard_skills")]
        public CategoriaScore HardSkills { get; set; }

        [JsonPropertyName("soft_skills")]
        public CategoriaScore SoftSkills { get; set; }

        [JsonPropertyName("metodologias")]
        public CategoriaScore Metodologias { get; set; }

        [JsonPropertyName("dominios_negocio")]
        public CategoriaScore DominiosNegocio { get; set; }

        [JsonPropertyName("idiomas")]
        public CategoriaScore Idiomas { get; set; }

        [JsonPropertyName("disponibilidades")]
        public CategoriaScore Disponibilidades { get; set; }
    }

    public class CategoriaScore
    {
        [JsonPropertyName("score_bruto_categoria")]
        public double ScoreBrutoCategoria { get; set; }

        [JsonPropertyName("score_bruto_obrigatorio")]
        public double ScoreBrutoObrigatorio { get; set; }

        [JsonPropertyName("score_bruto_desejavel")]
        public double ScoreBrutoDesejavel { get; set; }
    }

    public class ComparativoPorSkill
    {
        [JsonPropertyName("hard_skills")]
        public List<SkillComparativa> HardSkills { get; set; }

        [JsonPropertyName("soft_skills")]
        public List<SkillComparativa> SoftSkills { get; set; }

        [JsonPropertyName("metodologias")]
        public List<SkillComparativa> Metodologias { get; set; }

        [JsonPropertyName("dominios_negocio")]
        public List<SkillComparativa> DominiosNegocio { get; set; }

        [JsonPropertyName("idiomas")]
        public List<SkillComparativa> Idiomas { get; set; }

        [JsonPropertyName("disponibilidades")]
        public List<SkillComparativa> Disponibilidades { get; set; }
    }

    public class SkillComparativa
    {
        [JsonPropertyName("skill_requisitada")]
        public string SkillRequisitada { get; set; }

        [JsonPropertyName("nivel_requerido")]
        public string NivelRequerido { get; set; }

        [JsonPropertyName("obrigatoriedade")]
        public string Obrigatoriedade { get; set; }

        [JsonPropertyName("skill_do_candidato")]
        public string SkillDoCandidato { get; set; }

        [JsonPropertyName("nivel_do_candidato")]
        public string NivelDoCandidato { get; set; }

        [JsonPropertyName("pontuacao_da_skill")]
        public double PontuacaoDaSkill { get; set; }
    }
}
