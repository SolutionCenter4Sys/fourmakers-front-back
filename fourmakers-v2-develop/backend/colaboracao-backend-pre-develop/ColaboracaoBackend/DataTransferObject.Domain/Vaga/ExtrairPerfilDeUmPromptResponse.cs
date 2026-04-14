using System.Collections.Generic;

namespace DataTransferObject.Domain.Vaga
{
    public class ExtrairPerfilDeUmPromptResponse
    {
        public PerfilExtraido perfil_extraido { get; set; }
        public SkillsPropostas skills_propostas { get; set; }
        public ValidacaoInformacoes validacao_informacoes { get; set; }
        public MetadadosConsulta metadados_consulta { get; set; }
    }

    public class PerfilExtraido
    {
        public string codGestorExterno { get; set; }
        public string nomePerfil { get; set; }
        public decimal custoPerfil { get; set; }
        public decimal ratecardPerfil { get; set; }
        public string informacoesRelevantes { get; set; }
        public string permanenciaId { get; set; }
        public string modeloTrabalhoId { get; set; }
        public string modeloTrabalhoDescricao { get; set; }
        public string profissionalLocalidadeId { get; set; }
        public string cidade { get; set; }
        public string estado { get; set; }
        public int hibridoDias { get; set; }
        public string cep { get; set; }
        public string origem { get; set; }
        public List<GestorExternoPerfilSkill> gestorExternoPerfilSkills { get; set; }
    }

    public class SkillsPropostas
    {
        public List<GestorExternoPerfilSkill> gestorExternoPerfilSkills { get; set; }
    }

    public class GestorExternoPerfilSkill
    {
        public ItemPerfil itemPerfil { get; set; }
        public Skill skill { get; set; }
        public Nivel nivel { get; set; }
        public bool relevante { get; set; }
    }

    public class ValidacaoInformacoes
    {
        public InformacoesEncontradas informacoes_encontradas { get; set; }
        public ResumoInformacoes resumo_informacoes { get; set; }
        public List<string> informacoes_faltantes { get; set; }
        public string mensagem_usuario { get; set; }
        public decimal completude_percentual { get; set; }
    }

    public class InformacoesEncontradas
    {
        public bool nome_perfil { get; set; }
        public bool custo_perfil { get; set; }
        public bool ratecard_perfil { get; set; }
        public bool informacoes_relevantes { get; set; }
        public bool permanencia { get; set; }
        public bool modelo_trabalho { get; set; }
        public bool localidade { get; set; }
        public bool cidade { get; set; }
        public bool estado { get; set; }
        public bool hibrido_dias { get; set; }
        public bool cep { get; set; }
    }

    public class ResumoInformacoes
    {
        public string nome_perfil { get; set; }
        public string custo_perfil { get; set; }
        public string ratecard_perfil { get; set; }
        public string informacoes_relevantes { get; set; }
        public string permanencia { get; set; }
        public string modelo_trabalho { get; set; }
        public string localidade { get; set; }
        public string cidade { get; set; }
        public string estado { get; set; }
        public string hibrido_dias { get; set; }
        public string cep { get; set; }
    }

    public class MetadadosConsulta
    {
        public decimal tempo_processamento { get; set; }
        public int numero_tokens { get; set; }
        public string modelo_usado { get; set; }
        public string provedor { get; set; }
    }
}
