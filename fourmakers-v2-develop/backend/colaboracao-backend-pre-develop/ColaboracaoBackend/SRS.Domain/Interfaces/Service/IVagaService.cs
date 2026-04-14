using Colaboracao.Helper.Enum;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfil;
using DataTransferObject.Domain.SRS;
using DataTransferObject.Domain.SRS.Candidate;
using DataTransferObject.Domain.SRS.Vagas;
using DataTransferObject.Domain.Vaga;
using DataTransferObject.Domain.VagasSRS;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SRS.Domain.Interfaces.Service
{
    public interface IVagaService
    {
        Task<ApiGenericResult<CriarVagasSRSParam>> EditarCadastrarVaga(VagaFourmakersDTO vagaDTO, CRUDEnum cRUDEnum);
        //Task<bool> CriarOrdemDeTrabalhoInterna(List<VagaDTO> jobOrderDTO, string token);
        Task<List<VagaDTO>> BuscarVagaSRS(string token);
        Task<IEnumerable<VagaOrquestracaoDTO>> ListarVagaOrquestracao(string email);
        Task<ActionResult<IEnumerable<CanditatoVagaSrsDTO>>> ListarCandidatosPorVaga(long? idVaga);
        Task<IEnumerable<TotalizadoresVagaDTO>> TotalizadoresVaga(long? idVaga);

        // Novos métodos sem token
        Task<ApiGenericResult<List<DropDownItemDTO>>> ListarSolicitantes();
        Task<ApiGenericResult<List<DropDownItemDTO>>> ListarAprovadores();
        Task<ApiGenericResult<List<DropDownItemDTO>>> ListarTermometroVagas();
        Task<ApiGenericResult<List<DropDownItemDTO>>> ListarStackPrincipal();
        Task<ApiGenericResult<List<DropDownItemDTO>>> ListarConfiguracaoMaquina(string idContaCrm, int hardskillId);
        Task<ApiGenericResult<List<DropDownItemDTO>>> ListarTipoVaga();
        Task<ApiGenericResult<List<DropDownItemDTO>>> ListarDuracaoContrato();
        Task<ApiGenericResult<List<CargoDropdownItemDTO>>> ListarCargos();
        Task<ApiGenericResult<List<DropDownItemDTO>>> ListarTipoContratacao();
        Task<ApiGenericResult<List<DropDownItemDTO>>> ListarCargaHoraria();
        Task<ApiGenericResult<List<DropDownItemDTO>>> ListarLocalTrabalho();
        Task<ApiGenericResult<List<DropDownItemDTO>>> ListarUnidadesSRS();
        Task<ApiGenericResult<List<JobOrderSemanticaDTO>>> ListarVagasParaSemanticaComSkill(int cursor, int limite);
        Task<ApiGenericResult<List<JobOrderSemanticaListagemDTO>>> ListarVagasSemantica(int cursor, int limite);
        Task<IEnumerable<ListarVagasCadastradasFourmakersResult>> ListarVagasCadastradas(int limite, int cursor, int orgId, string codCliente = null, string gestorExternoPerfilId = null, string busca = null);
        Task<ApiGenericResult<List<DropDownItemDTO>>> ListarClientesComVagasVigentes(int orgId);
        Task<ApiGenericResult<List<DropDownItemDTO>>> ListarGestorExternoPerfilComVagasVigentesPorCliente(string codigoCliente, int orgId);
        Task<ApiGenericResult<VagaFourmakersDTO>> ObterVagaPorIdEGestorExternoPerfilId(int vagaId, Guid? gestorExternoPerfilId);
        Task<ApiGenericResult> RecomendarCandidatosPorEmail(RecomendarCandidatosParam request);
        Task<ApiGenericResult<VagaRecrutamentoDTO>> ObterVagaRecrutamentoPorCodigo(int codigo);
        Task<ApiGenericResult<VagaRecrutamentoDTO>> ObterVagaRecrutamentoPorId(string vagaId);
        Task<ApiGenericResult<VagaRecrutamentoDTO>> AtualizarVagaRecrutamentoPorId(AtualizarVagaRecrutamentoDTO vaga, string codColaborador);
        Task<ApiGenericResult<VagaRecrutamentoDTO>> InserirVagaRecrutamento(InserirVagaRecrutamentoDTO vaga, string codColaborador, bool criadaAutomaticamente = false);
        Task<ApiGenericResult<VagaRecrutamentoDTO>> AtualizarVagaRecrutamento(VagaVindaDeGestorExternoPerfil vaga, string codColaborador, bool edicaoViaTela = false);
        Task<ApiGenericResult<bool>> CancelarVagaRecrutamento(int codigo, string codColaborador);
        Task<ApiGenericResult<IEnumerable<VagaRecrutamentoDTO>>> ListarVagasRecrutamento(int limite, int cursor, string busca = null, List<int> status = null, int? orgId = null, string dataInicio = null, string dataFim = null);
        Task<Guid?> CriarVagaAutomaticamenteAPartirDePerfilDeAtuacao(GestorExternoPerfilInput gestorExternoPerfilInput, string cpfUsuarioLogado);
        Task<ActionResult<ApiGenericResult<List<ListarVagasEmBancoDeTalentosResult>>>> ListarVagasPipeline(int cursor, int limite, string busca, string cliente, string dataInicio, string dataFim, int? orgId);
        Task<ApiGenericResult<VagaRecrutamentoDTO>> ObterVagaPorIdEGestorExternoPerfilIdRecrutamento(Guid idPerfil);
        Task<ApiGenericResult<IEnumerable<VagaRecrutamentoDTO>>> ObterTodasVagasPorIdEGestorExternoPerfilIdRecrutamento(Guid idPerfil);
        Task AtualizarVagaCriadaAutomaticamenteRecrutamento(VagaRecrutamentoDTO vagaRecrutamento, GestorExternoPerfilInput gestorExternoPerfilInput, string cpfRequest);
        Task CandidatarSe(CandidatarSeRecrutamentoParam candidatarSeRecrutamentoParam);
        Task<ApiGenericResult<IEnumerable<OpcaoContatoDTO>>> ListarOpcoesContato();
        Task<IEnumerable<ListarCandidatosInscritosResult>> ListarCandidatosInscritos(string vagaId, string busca, string dataInicio, string dataFim, int cursor, int limite, string codigoInternoColaboradorLogado, bool? qualificados, int? diasUltimaAlteracao, string? localizacaoCidade, string? localizacaoEstado);
        Task<IEnumerable<ListarCandidaturasPorCodCandidatoResult>> ListarCandidaturasPorCodCandidato(string codColaborador);
        Task<IEnumerable<StatusVagaRecrutamentoDTO>> ListarStatusVagaRecrutamento(int? orgIdUsuarioLogado);
        Task<ApiGenericResult<bool>> AtualizarOrdemStatusVagaRecrutamento(AtualizarOrdemStatusVagaRecrutamentoParam param, string codColaborador);
        Task<IEnumerable<VagaRecrutamentoDTO>> ListarVagasRecrutamentoPorParent(string vagaIdParent, int orgId);
        Task<IEnumerable<VagaRecrutamentoDTO>> ListarVagasRecrutamentoPorParentEmAndamento(string vagaIdParent, int orgId);
        Task<VagaAnonymousDTO> ObterVagaRecrutamentoPorCodigoAnonymous(int codigo);
        Task GravarPerdaVaga(int codigoVaga, Guid? idMotivoPerda, string comentario, string codColaborador, int orgIdColaborador);
        Task MudarStatusVaga(MudarStatusVagaRecrutamentoParam mudarStatusVagaRecrutamentoParam, string cpf, int orgIdColaborador);
        Task <(string, string)> MudarStatusCandidatura(string codigoCandidatura, int codigoStatus, string cpf, string comentario = null);
        Task<IEnumerable<StatusCandidaturaRecrutamentoDTO>> ListarStatusCandidaturaRecrutamento();
        Task<IEnumerable<MotivoDescandidatarDTO>> ListarMotivosDescandidatura();
        Task DescandidatarSe(string idCandidatura, string idMotivoDescandidatura);
        Task<ApiGenericResult<IEnumerable<MotivoPerdaVagaDTO>>> ListarMotivosPerdaVaga();
        Task CandidatarOutraPessoa(CandidatarOutraPessoaRecrutamentoParam candidatarOutraPessoaRecrutamentoParam);
        Task<IEnumerable<ListarCandidatosAderentesResult>> ListarCandidatosAderentes(ListarCandidatosAderentesParam param);
        Task<DataTransferObject.Domain.Vaga.CountCandidatosInscritosResult> CountCandidatosInscritos(string vagaId);
        Task<ObterTotaisInscritosResult> ObterTotaisInscritos(string vagaId);

        Task<List<DataTransferObject.Domain.Match.CandidatosMatchResponse>> RankCandidates(DataTransferObject.Domain.Match.CandidatosMatchRequest request);
        Task<IEnumerable<DisponibilidadeEntrevistaDTO>> ListarDisponibilidadesEntrevista();
        Task<ListarCandidaturasPorCodCandidatoResult> ObterUltimaCandidaturaPorCodColaborador(string codColaborador);
        Task InserirInformacoesComplementaresVagaRecrutamento(InserirInformacoesComplementaresVagaRecrutamentoParam param, string cpf);
        Task<IEnumerable<TipoVagaDTO>> ListarTiposVaga();
        Task<IEnumerable<NivelVagaDTO>> ListarNiveisVaga();
        Task<IEnumerable<TipoContratacaoDTO>> ListarTiposContratacao();
        Task<IEnumerable<UnidadeDTO>> ListarUnidades();
        Task<int> CountVagasRecrutamento(int? orgId);
        Task<ContadorVagasPorStatusResultDTO> CountVagasRecrutamentoPorStatus(int? orgId, List<int> status = null, string dataInicio = null, string dataFim = null);
        Task<int> CountInscritoVagas(int orgId, string codColaborador);
        Task<IEnumerable<TipoEmpregoLinkedin>> ListarTiposEmpregosLinkedin();
        Task<IEnumerable<NivelExperienciaLinkedin>> ListarNiveisExperienciaLinkedin();
        Task<ApiGenericResult<Microsoft.AspNetCore.Mvc.FileContentResult>> RelatorioVagas(int orgId, DateTime dataInicio, DateTime dataFim);
        Task<ApiGenericResult<string>> AdicionarRecrutadorVaga(string vagaId, string codInternoColaboradorRecrutador);
        Task<ApiGenericResult<FileContentResult>> RelatorioProdutividade(int orgId, DateTime dataInicio, DateTime dataFim);
        Task<ApiGenericResult<FileContentResult>> RelatorioVagasCandidaturas(DateTime dataInicio, DateTime dataFim, int orgIdUsuarioLogado);
        Task<TemplateDescricaoVagaDTO> BuscarTemplateDescricaoVaga(int orgId);
        Task AdicionarTemplateDescricaoVaga(int orgId, string textoIntroducao, string textoFinalizacao);
        Task AtualizarTemplateDescricaoVaga(int orgId, string textoIntroducao, string textoFinalizacao);
        Task ExcluirTemplateDescricaoVaga(int orgId);
        Task<ApiGenericResult<IEnumerable<HistoricoStatusVagaItemDTO>>> ListarHistoricoStatusVaga(string vagaId);
    }
}