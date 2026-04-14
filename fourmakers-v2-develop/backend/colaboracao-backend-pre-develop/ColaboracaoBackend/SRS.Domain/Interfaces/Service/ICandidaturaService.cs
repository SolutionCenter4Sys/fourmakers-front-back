using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Candidato;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.SRS;
using DataTransferObject.Domain.SRS.Vagas;
using DataTransferObject.Domain.Vaga;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SRS.Domain.Interfaces.Service
{
    public interface ICandidaturaService
    {
        Task<ApiGenericResult<CandidaturaArquivosDTO>> InserirArquivo(CandidaturaArquivosParams param, string codigoInternoColaboradorLogado);
        Task<ApiGenericResult<bool>> DeletarArquivo(string arquivoPath);
        Task AtualizarStatusCandidatura(int vagaId, int candidatoId, int statusId, string descricao, OrigemVagaEnum origem);
        Task<List<CandidaturaStatusDTO>> ListarStatusCandidatura();
        Task<List<VagaCandidatoGestaoDTO>> ListarCandidatosDeVagas(string codColaborador, string pesquisa, string candidato, List<int> status, string titulo, int orgId);
        Task<string> ReprovarCandidatura(string cpf, ReprovarCandidaturaParam param);
        Task<IEnumerable<CandidaturaLogDTO>> ListarLogsCandidaturaPorColaborador(string codColaborador);
        Task<IEnumerable<CandidaturaAgrupadaDTO>> ListarLogsCandidaturaAgrupadosPorColaborador(string codColaborador, string busca, string idCandidatura = null);
        Task AtualizarCandidatura(AtualizarCandidaturaParam param, string codigoInternoColaborador);
        Task<List<LogPretensaoModeloCandidaturaDTO>> ListarLogPretensaoModeloPorCandidatura(Guid idCandidatura, string codigoUsuarioLogado, int limit, int cursor);
        Task<List<LogPretensaoModeloCandidaturaDTO>> ListarLogPretensaoModeloPorOrganizacao(Guid idCandidatura, int orgId, int limit, int cursor, string codigoUsuarioLogado);
        Task<CandidaturaRecrutamentoDTO> ObterCandidaturaPorId(string idCandidatura);
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
        Task<string> DeclinarCandidato(DeclinarCandidatoParam param, string codColaborador);
        Task<List<MeusTalentos>> ListarMeusTalentos(string codigoUsuarioLogado);
        Task AtualizarRecrutadorResponsavel(string idCandidatura, string codigoRecrutadorResponsavel, string codigoUsuarioLogado);
        Task<CandidatoTemplateEmailResponseDTO> CandidatoTemplateEmailInserir(CandidatoTemplateEmailParamDTO param);
        Task<CandidatoTemplateEmailResponseDTO> CandidatoTemplateEmailAtualizar(CandidatoTemplateEmailParamDTO param);
        Task<bool> CandidatoTemplateEmailDeletar(int orgId);
        Task<CandidatoTemplateEmailResponseDTO> CandidatoTemplateEmailListarPorId(int orgId);
        Task<ApiGenericResult<DashboardBigNumbers>> dashboardMetricasRecrutamento(DashboardBigNumbersParam param);
        Task<ApiGenericResult<List<DashboardVagasEmFocoResponse>>> dashboardMetricasVagasEmFoco(DashboardBigNumbersParam param);
        Task<ApiGenericResult<DashboardFunilVagasResponse>> dashboardMetricasFunilDeVagas(DashboardBigNumbersParam param);
        Task<ApiGenericResult<List<DashboardPerdidasMotivoResponse>>> dashboardMetricasVagasPerdidasMotivo(DashboardBigNumbersParam param);
        Task<ApiGenericResult<List<DashboardAquisicaoCandidatos>>> dashboardMetricasAquisicaoCandidatos(DashboardBigNumbersParam param);
        Task<ApiGenericResult<List<DashboardAquisicaoCandidatosResponse>>> dashboardNovosCandidatosPorOrigem(DashboardNovosCandidatosParam param);
        Task<ApiGenericResult<List<RecrutadorListagemResponse>>> RecrutadorListagem(string nomeRecrutador, int cursor, int limite);
        Task<IEnumerable<ListarCandidaturasPorCodCandidatoResult>> BuscarCandidaturasColaborador(string codColaborador);
    }
}