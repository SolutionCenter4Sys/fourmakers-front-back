using System;
using System.Collections.Generic;
using DataTransferObject.Domain.Labs.MatchSemantico;
using DataTransferObject.Domain.Match;
using DataTransferObject.Domain.Vaga;

namespace DataTransferObject.Domain.BancoDeTalentos
{
    public class PromptMatchResult
    {
        public List<PessoasPromptMatch> Colaboradores { get; set; }
        public ExtrairPerfilDeUmPromptResponse Prompt { get; set; }
        public Guid? IdLogRankCandidatesIds { get; set; }

        public PromptMatchResult()
        {
            Colaboradores = new List<PessoasPromptMatch>();
            Prompt = new ExtrairPerfilDeUmPromptResponse();
        }
    }

    public class PessoasPromptMatch
    {
        public string Nome { get; set; }
        public string CodigoInternoColaborador { get; set; }
        public bool PossuiCandidatura { get; set; }
        public List<CandidaturaComTituloDTO> Candidaturas { get; set; }
        public double Match { get; set; }
        public CandidatosMatchResponse RetornoMatch { get; set; }
        public string Origem { get; set; }
        public List<OrganizacaoCandidatoDTO> Organizacoes { get; set; }
    }

    // Novas classes para retorno com formato snake_case
    public class PromptMatchResultSnakeCase
    {
        public List<PessoasPromptMatchSnakeCase> colaboradores { get; set; }
        public ExtrairPerfilDeUmPromptResponse prompt { get; set; }
        public Guid? IdLogRankCandidatesIds { get; set; }

        public PromptMatchResultSnakeCase()
        {
            colaboradores = new List<PessoasPromptMatchSnakeCase>();
            prompt = new ExtrairPerfilDeUmPromptResponse();
        }
    }

    public class PessoasPromptMatchSnakeCase
    {
        public string origem;

        public string nome { get; set; }
        public string codigo_interno_colaborador { get; set; }
        public bool possui_candidatura { get; set; }
        public List<CandidaturaComTituloDTOSnakeCase> candidaturas { get; set; }
        public double match { get; set; }
        public CandidatosMatchResponseSnakeCase retornoMatch { get; set; }
        public List<OrganizacaoCandidatoDTO> organizacoes { get; set; }
    }

    public class CandidaturaComTituloDTOSnakeCase
    {
        public string id_candidatura { get; set; }
        public string titulo_vaga { get; set; }
        public string codigo_cliente { get; set; }
        public string nome_cliente { get; set; }
        public string codigo_gestor { get; set; }
        public string nome_gestor { get; set; }
    }

    public class CandidatosMatchResponseSnakeCase
    {
        public string codigo_interno_colaborador { get; set; }
        public string nome { get; set; }
        public List<int> orgs { get; set; }
        public double match { get; set; }
        public double score_candidato { get; set; }
        public double score_vaga { get; set; }
        public DetalhamentoCalculoSnakeCase detalhamento_calculo { get; set; }
        public ComparativoPorSkillSnakeCase comparativo_por_skill { get; set; }
    }

    public class DetalhamentoCalculoSnakeCase
    {
        public CategoriaScoreSnakeCase hard_skills { get; set; }
        public CategoriaScoreSnakeCase soft_skills { get; set; }
        public CategoriaScoreSnakeCase metodologias { get; set; }
        public CategoriaScoreSnakeCase dominios_negocio { get; set; }
        public CategoriaScoreSnakeCase idiomas { get; set; }
        public CategoriaScoreSnakeCase disponibilidades { get; set; }
    }

    public class CategoriaScoreSnakeCase
    {
        public double score_bruto_categoria { get; set; }
        public double score_bruto_obrigatorio { get; set; }
        public double score_bruto_desejavel { get; set; }
    }

    public class ComparativoPorSkillSnakeCase
    {
        public List<SkillComparativaSnakeCase> hard_skills { get; set; }
        public List<SkillComparativaSnakeCase> soft_skills { get; set; }
        public List<SkillComparativaSnakeCase> metodologias { get; set; }
        public List<SkillComparativaSnakeCase> dominios_negocio { get; set; }
        public List<SkillComparativaSnakeCase> idiomas { get; set; }
        public List<SkillComparativaSnakeCase> disponibilidades { get; set; }
    }

    public class SkillComparativaSnakeCase
    {
        public string skill_requisitada { get; set; }
        public string nivel_requerido { get; set; }
        public string obrigatoriedade { get; set; }
        public string skill_do_candidato { get; set; }
        public string nivel_do_candidato { get; set; }
        public double pontuacao_da_skill { get; set; }
    }
}