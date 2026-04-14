using DataTransferObject.Domain;
using DataTransferObject.Domain.Candidato;
using DataTransferObject.Domain.SRS;
using DataTransferObject.Domain.Usuario;
using DataTransferObject.Domain.Vaga;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.DomainModel
{
    public interface ICandidaturaRepository
    {
        Task<CandidaturaArquivosDTO> InserirArquivo(CandidaturaArquivosParams param, string linkUrl, string codigoInternoColaboradorLogado);
        Task<bool> DeletarArquivo(string arquivoPath);
        Task InserirLogAlteracaoStatusCandidatura(int vagaId, int candidatoId, int statusId);
        Task<CandidaturaStatusDTO> BuscarStatusCandidaturaPorId(int id);
        Task<CandidaturaStatusDTO> InserirTipoStatusCandidatura(int statusId, string descricao, string origem);
        Task AlterarStatusCandidatura(int vagaId, int candidatoId, int statusId);
        Task<List<CandidaturaStatusDTO>> ListarStatusCandidatura();
        Task<List<VagaCandidatoGestaoDTO>> ListarCandidatosDeVagas(string pesquisa, string candidato, List<int> status, string titulo, int orgId);
        Task<int?> ExisteStatusCandidatura(int vagaId, int candidatoId);
        Task CandidatarSeRecrutamento(string cpf, string vagaId, int orgId, int status, List<string> opcoesContatoCodigos, decimal? pretencaoSalarial = null, string modeloTrabalhoId = null, string disponibilidadeEntrevistaId = null, int? quantidadeDiasPresencial = null);
        Task<IEnumerable<ListarCandidatosInscritosResult>> ListarCandidatosInscritos(string vagaId, string busca, string dataInicio, string dataFim, int cursor, int limite, string codigoInternoColaboradorLogado, bool? qualificados, int? diasUltimaAlteracao, string? localizacaoCidade, string? localizacaoEstado);
        Task<IEnumerable<ListarCandidaturasPorCodCandidatoResult>> ListarCandidaturasPorCodCandidato(string codColaborador);
        Task<bool> EstaCandidaturaExiste(string codigoColaborador, string idVaga);
        Task<CandidaturaRecrutamentoDTO> ObterCandidaturaPorId(string codigoCandidatura);
        Task<IEnumerable<int>> ListarIdsStatusCandidaturaRecrutamento();
        Task <string>MudarStatusCandidatura(string idCandidatura, int codigoStatus, string codColaborador, string comentario = null);
        Task<IEnumerable<StatusCandidaturaRecrutamentoDTO>> ListarStatusCandidaturaRecrutamento();
        Task<IEnumerable<MotivoDescandidatarDTO>> ListarMotivosDescandidatura();
        Task DescandidatarSe(string idCandidatura, string idMotivoDescandidatura);
        Task<bool> EstaCandidaturaExiste(string idCandidatura);
        Task CandidatarOutraPessoa(string codigoColaborador, string vagaId, int orgId, int status, string codUsuarioLogado, decimal? pretencaoSalarial = null, string modeloTrabalhoId = null, string disponibilidadeEntrevistaId = null, int? quantidadeDiasPresencial = null);
        Task<IEnumerable<CandidaturaLogDTO>> ListarLogsCandidaturaPorColaborador(string codColaborador);
        Task<IEnumerable<CandidaturaAgrupadaDTO>> ListarLogsCandidaturaAgrupadosPorColaborador(string codColaborador, string busca);
        Task<IEnumerable<DisponibilidadeEntrevistaDTO>> ListarDisponibilidadesEntrevista();
        Task<bool> EstaModeloTrabalhoExiste(string id);
        Task<bool> EstaDisponibilidadeEntrevistaExiste(string id);
        Task<IEnumerable<QuantidadeCandidatosPorEstagioDTO>> ObterQuantidadeCandidatosPorEstagio(string vagaId);
        Task<Dictionary<string, List<QuantidadeCandidatosPorEstagioDTO>>> ObterQuantidadeCandidatosPorEstagioMultiplasVagas(IEnumerable<string> vagaIds);
        Task AtualizarCandidatura(string idCandidatura, decimal? pretencaoSalarial, string modeloTrabalhoId, string disponibilidadeEntrevistaId, int? quantidadeDiasPresencial, string codigoInternoColaborador, OrigemAlteracaoPretensaoModeloLogEnum origemAlteracao = OrigemAlteracaoPretensaoModeloLogEnum.MovimentacaoKanban);
        Task<List<LogPretensaoModeloCandidaturaDTO>> ListarLogPretensaoModeloPorCandidatura(Guid idCandidatura, int limit, int cursor);
        Task<List<LogPretensaoModeloCandidaturaDTO>> ListarLogPretensaoModeloPorOrganizacao(int orgId, int limit, int cursor, string codigoInternoCandidato);
        Task<UltimaPretensaoModeloColaboradorSnapshotDTO> ObterUltimaPretensaoModeloInformadaColaboradorAsync(string codigoInternoColaborador);
        Task GravarLogCandidatura(string idCandidatura, int statusCandidatura, string codigoInternoColaborador, object objeto, System.Data.IDbTransaction transaction = null);
        Task<DataTransferObject.Domain.Vaga.CountCandidatosInscritosResult> CountCandidatosInscritos(string vagaId);
        Task<int> CountInscritoVagas(int orgId, string codColaborador);
        Task<PdfTemplateDTO> GetPdfTemplateHeaderPdfAdmissao(string idCandidatura, string codInternoCandidato, string idVaga);
        Task<ProfissionalDTO> GetProfissionalPdfAdmissao(string idCandidatura, string codInternoCandidato, string idVaga);
        Task<VagaAdmissaoDTO> GetVagaAdmissaoPdfAdmissao(string idCandidatura, string codInternoCandidato, string idVaga);
        Task<BeneficiosDTO> GetBeneficiosPdfAdmissao(string idCandidatura, string codInternoCandidato, string idVaga);
        Task<ChecklistInstalacaoDTO> GetChecklistInstalacaoPdfAdmissao(string idCandidatura, string codInternoCandidato, string idVaga);
        Task<AcessosUsuarioDTO> GetAcessosUsuarioPdfAdmissao(string idCandidatura, string codInternoCandidato, string idVaga);
        Task<PdfTemplateDTO> GetPdfTemplateDeAdmissaoPdf(string idCandidatura, string codInternoCandidato, string idVaga);
        Task<EquipamentosFoursysDTO> GetEquipamentoFourSysAdmissaoPdf(string idCandidatura, string codInternoCandidato, string idVaga);
        Task<PdfTemplateDTO> CreatePdfTemplateDeAdmissao(CreatePdfTemplateParameters param);
        Task<List<AcessoSistemaDTO>> GetSistemasLiberadosTemplatePdf();
        Task<List<AcessoDiretorioRedeDTO>> GetAcessosPastaRedeTemplatePdf();
        Task<List<MotivoDeclinioDTO>> ListarMotivosDeclinio();
        Task<List<MotivoReprovacaoDTO>> ListarMotivosReprovacao();
        Task<MotivoDeclinioDTO> ObterMotivoDeclinioPorId(string id);
        Task<MotivoReprovacaoDTO> ObterMotivoReprovacaoPorId(string id);
        Task DeclinarCandidato(string idCandidatura, string idMotivoDeclinio);
        Task ReprovarCandidato(string candidaturaId, string idMotivoReprovacao);
        Task<IEnumerable<TemplateDestinatarioEmail>> BuscaDestinatariosTemplateCandidatos(int orgId, bool anexo);
        Task ExcluirCandidatura(string codColaboradorContratado, string id, string codColaboradorLogado);
        Task<CandidaturaRecrutamentoDTO> ObterCandidaturaPorIdVagaColaborador(string codColaboradorContratado, string idVaga);
        Task<List<MeusTalentos>> ListarMeusTalentos(string codigoUsuarioLogado);
        Task AtualizarRecrutadorResponsavel(string idCandidatura, string? codigoRecrutadorResponsavel, string codigoInternoColaborador);
        Task<UsuarioColaboradorDTO> BuscarDadosColaboradorEmail(string codInterno, int orgId);
        Task<UsuarioColaboradorDTO> BuscarDescricaoOrg(int orgId);
        Task<TemplateOrgEmailResponseDTO> BuscarDadosTemplateEmail(int orgId);
        Task<CandidatoTemplateEmailResponseDTO> CandidatoTemplateEmailInserir(CandidatoTemplateEmailParamDTO param);
        Task<CandidatoTemplateEmailResponseDTO> CandidatoTemplateEmailAtualizar(CandidatoTemplateEmailParamDTO param);
        Task<bool> CandidatoTemplateEmailDeletar(int orgId);
        Task<CandidatoTemplateEmailResponseDTO> CandidatoTemplateEmailListarPorId(int orgId);
        Task<DashboardBigNumbers> dashboardMetricasRecrutamento(DashboardBigNumbersParam param, int orgId);
        Task<List<DashboardVagasEmFocoResponse>> dashboardMetricasVagasEmFoco(DashboardBigNumbersParam param, int orgId);
        Task<DashboardFunilVagasResponse> dashboardMetricasFunilDeVagas(DashboardBigNumbersParam param, int orgId);
        Task<List<DashboardPerdidasMotivoResponse>> dashboardMetricasVagasPerdidasMotivo(DashboardBigNumbersParam param, int orgId);
        Task<List<DashboardAquisicaoCandidatos>> dashboardMetricasAquisicaoCandidatos(DashboardBigNumbersParam param, int orgId);
        Task<List<DashboardNovosCandidatosPorOrigemItem>> dashboardNovosCandidatosPorOrigem(DashboardNovosCandidatosParam param, int orgId);
        Task<List<RecrutadorListagemResponse>> RecrutadorListagem(string nomeRecrutador, int cursor, int limite, int orgId);

    }
}