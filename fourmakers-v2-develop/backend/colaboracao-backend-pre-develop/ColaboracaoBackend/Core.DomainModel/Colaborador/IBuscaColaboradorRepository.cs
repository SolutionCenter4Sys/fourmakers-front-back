using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Diretoria;
using DataTransferObject.Domain.LG.Holerite;
using DataTransferObject.Domain.Org;
using DataTransferObject.Domain.Vaga;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Colaborador
{
    public interface IBuscaColaboradorRepository
    {
        ColaboradorDTO GetColaborador(string cpf, int orgId);
        List<SimpleColaboradorDTO> GetColaboradoresPorDiretoria(string cpf, string nome, string diretoria, int cursor, int limite, string codGestor, int orgId);
        bool ValidaAcesso(string cpf, int orgId, string token);
        ColaboradorOrgDTO GetColaboradorOrg(string cpf, int orgId);
        List<ColaboradorOrgDTO> GetSubordinadosColaboradorOrg(string cpf, int orgId);

        /// <summary>
        /// Códigos internos do gestor e de todos os subordinados em todos os níveis (tb_colaborador_hierarquia), na org.
        /// </summary>
        HashSet<string> ListarCodigosInternosHierarquiaTimeIncluindoGestor(string codigoInternoGestor, int orgId);

        /// <summary>
        /// Indica se o colaborador está na hierarquia abaixo do gestor (percorre tb_colaborador_hierarquia do alvo até o superior do gestor).
        /// Útil para quem é time do subordinado do gestor (vários níveis).
        /// </summary>
        bool ColaboradorEstaNaHierarquiaSubordinadaDoGestorNaOrg(string codigoInternoGestor, string codigoInternoColaborador, int orgId);

        ColaboradorOrgHierarquiaDTO BuscaColaboradorOrgHierarquia(string codColaboradorExterno, int orgId);
        List<HoleriteSimplesDTO> GetAllHolerite(string cpf);
        List<ColaboradoresOrgDTO> ListaColaboradoresOrg(int orgId, int page, int pageSize, bool fourtalents, string nomeOuEmail = "", string codExterno = "", List<string>? restricaoDiretorias = null);
        int? GetEmailOrg(string email);
        string MontarUrlSSO(int orgId);
        string BuscaColaboradorPorCodigoExterno(string codigoColaborador, int orgId);
        Task<List<dynamic>> RelatorioColaboradoresSkills(int orgId, bool ComHardskills);
        List<KeyValuePair<string, List<int?>>> GetAllColaboradoresCodigoInternoComOrgs();
        List<string> BuscaColaborador(string coluna, string busca, int candidato, string cpf);
        void AtualizaInfoLinkedin(string codigoInternoColaborador, string urlLinkedin);
        Task<List<AniversariantesSemanaColaboradorDTO>> GetColaboradoresAniversariantesDaSemana(int orgId, string? codDiretoria, int? proximosDiasQuantidade, bool ocultarEmail);
        Task<List<ColaboradorCvDadosDTO>> ListarDadosDoColaboradorPorIds(List<string> cpfs);
        Task<List<AnalistaResponsavelDTO>> BuscarAnalistasResponsaveis(int orgId);
        Task<bool> ExisteColaboradorComEsteEmail(string email);
        Task<bool> ExisteColaboradorComEsteLinkedin(string perfilIN);
        Task<bool> QualificarColaborador(string cpf, string cpfUsuarioLogado);
        Task<bool> DesqualificarColaborador(string cpf, string cpfUsuarioLogado);
        Task<ColaboradorBasicoDTO> GetColaboradorBasicoPorCodigo(string codigoInternoColaborador);
        Task<List<ColaboradorQualificadoDTO>> GetColaboradoresQualificadosPorColaboradorQualificador(string codigoInternoColaborador);
        Task<List<OrganizacaoCandidatoDTO>> GetOrganizacoesColaborador(string codigoInternoColaborador);
        Task<string> GetCodigoInternoColaboradorByLinkedin(string perfilIN);
        void InserirDadosIA(ColaboradorIADTO colaboradorIA);
        Task<string> BuscaNomeColaboradorPorEmail(string email);
        Task<IEnumerable<OrigemColaboradorDTO>> ListarOrigensColaborador();
        Task AtualizaInfoLinkedinAsync(string codigoInternoColaborador, string urlLinkedin);

        /// <summary>Diretorias distintas com colaboradores ativos na org (combos / filtros).</summary>
        Task<List<DiretoriaColaboradorDTO>> ListarDiretoriasDistintasPorOrgAsync(int orgId);
    }
}