using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Nivel;
using DataTransferObject.Domain.SRS;
using DataTransferObject.Domain.Vaga;
using DataTransferObject.Domain.Vaga.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Vaga
{
    public interface IVagaFourmakersRepository
    {
        List<VagaFourmakersSRSDTO> SalvarVagasSRS(List<VagaFourmakersSRSDTO> vagasSRSList);
        IEnumerable<VagaFourmakersSRSDTO> ObterTodasAsVagas();
        List<VagaFourmakersSRSDTO> AtualizarVagas(List<VagaFourmakersSRSDTO> vagasAtualizadas);
        Task<VagaFourmakersSRSDTO> ObterVagaPorId(int vagaId);
        Task<VagaFourmakersDTO> ObterVagaPorIdEPerfilId(int vagaId, string perfilId, int orgId);
        Task<string> InserirVagaGestorExternoPerfil(int orgId, string codigoInternoColaborador, int codigoVaga, string gestorExternoPerfilId);
        Task<string> InserirLogVaga(int orgId, string codigoInternoColaborador, int codigoVaga, string objetoVaga, AcaoLogVagaEnum acao);
        Task<IEnumerable<ListarVagasCadastradasFourmakersResult>> ListarVagasCadastradas(int limite, int cursor, int orgId, string codCliente = null, string gestorExternoPerfilId = null, string busca = null);
        Task<List<DropDownItemDTO>> ListarClientesComVagasVigentes(int orgId);
        Task<List<DropDownItemDTO>> ListarGestorExternoPerfilComVagasVigentesPorCliente(string codigoCliente, int orgId);
        Task<List<SkillNivelDTO>> GetSkillsPorIds(List<int> list);
        Task<List<NivelDTO>> GetTodosNiveis();
        Task<int> InserirRecomendacaoProfissionalAderencia(int tbOrgId, string codigoInternoColaboradorCriacao, int codigoVaga, string? cbjeto, int acao);
        Task<List<ListarVagasEmBancoDeTalentosResult>> ListarVagasPipelinePorOrg(DateTime dataInicio, DateTime dataFim, string cliente, int cursor, int limite, string busca, int? orgId);
        Task<List<ListarVagasEmBancoDeTalentosResult>> ListarVagasPipelinePorOrgs(DateTime dataInicio, DateTime dataFim, string cliente, int cursor, int limite, string busca, List<int> orgIds);
        Task<VagaRecrutamentoDTO> ObterVagaPorCodigo(int codigo);
        Task<VagaRecrutamentoDTO> InserirVaga(VagaRecrutamentoDTO vaga, string cpfUsuarioLogado);
        Task AtualizarVaga(VagaRecrutamentoDTO vaga, string codColaborador);
        Task CancelarVagaRecrutamento(string codigo, string codColaborador, int statusVagaCod);
        Task<IEnumerable<VagaRecrutamentoDTO>> ListarVagasRecrutamentoPorOrg(int limite, int cursor, string busca = null, string status = null, int? orgId = null, string dataInicio = null, string dataFim = null, List<string> clientesPermitidos = null);
        Task<VagaRecrutamentoDTO> ObterVagaPorIdEGestorExternoPerfilIdRecrutamento(Guid idPerfil);
        Task<IEnumerable<VagaRecrutamentoDTO>> ObterTodasVagasPorIdEGestorExternoPerfilIdRecrutamento(Guid idPerfil);
        Task<IEnumerable<OpcaoContatoDTO>> ListarOpcoesContato();
        Task<List<string>> ListarIdsDasOpcoesContato();
        Task<IEnumerable<StatusVagaRecrutamentoDTO>> ListarStatusVagaRecrutamento(int? orgIdUsuarioLogado);
        Task AtualizarOrdemStatusVagaRecrutamento(List<StatusOrdemItem> statusOrdem, string codColaborador);
        Task<IEnumerable<VagaRecrutamentoDTO>> ListarVagasRecrutamentoPorParent(string vagaIdParent, int orgId);
        Task<IEnumerable<VagaRecrutamentoDTO>> ListarVagasRecrutamentoPorParentEmAndamento(string vagaIdParent, int orgId);
        Task<VagaAnonymousDTO> ObterVagaRecrutamentoPorCodigoAnonymous(int codigo);
        Task MudarStatusVaga(int idVaga, int codigoStatus, string codColaborador);
        Task AtualizarMotivoPerdaVaga(int codigoVaga, Guid idMotivoPerda);
        Task<IEnumerable<int>> ListarIdsStatusVagaRecrutamento();
        Task<IEnumerable<VagaRecrutamentoDTO>> ListarVagasRecrutamento(int limite, int cursor, string busca, string status, string dataInicio, string dataFim, List<string> clientesPermitidos = null);
        Task<List<ListarVagasEmBancoDeTalentosResult>> ListarVagasPipeline(DateTime dtInicio, DateTime dtFim, string cliente, int cursor, int limite, string busca);
        Task IniciarSla(string idVaga);
        Task<string> ObterIdVagaPorCodigo(int codVaga);
        Task<bool> EstaVagaExiste(int codVaga);
        Task<bool> EstaVagaExistePorId(string idVaga);
        Task<VagaRecrutamentoDTO> ObterVagaRecrutamentoPorId(string vagaId);
        Task InserirInformacoesComplementaresVagaRecrutamento(InserirInformacoesComplementaresVagaRecrutamentoParam param, VagaRecrutamentoDTO vaga, string codColaboradorLogado);
        Task<IEnumerable<TipoVagaDTO>> ListarTiposVaga();
        Task<IEnumerable<TipoContratacaoDTO>> ListarTiposContratacao();
        Task<IEnumerable<UnidadeDTO>> ListarUnidades();
        Task<int> CountVagasRecrutamento();
        Task<int> CountVagasRecrutamentoPorOrg(int? orgId);
        
        Task<ContadorVagasPorStatusResultDTO> CountVagasRecrutamentoPorStatus(string status = null, string dataInicio = null, string dataFim = null, List<string> clientesPermitidos = null);
        Task<ContadorVagasPorStatusResultDTO> CountVagasRecrutamentoPorStatusPorOrg(int? orgId, string status = null, string dataInicio = null, string dataFim = null, List<string> clientesPermitidos = null);
        Task<IEnumerable<TipoEmpregoLinkedin>> ListarTiposEmpregosLinkedin();
        Task<IEnumerable<NivelExperienciaLinkedin>> ListarNiveisExperienciaLinkedin();
        Task<TipoEmpregoLinkedin> ObterTipoEmpregoLinkedinPorId(Guid id);
        Task<NivelExperienciaLinkedin> ObterNivelExperienciaLinkedinPorId(Guid id);
        Task<List<dynamic>> BuscaRelatorioVagas(int orgId, DateTime dataInicio, DateTime dataFim);
        Task<List<string>> ObterEmailsAnaliseGestorPorVagaId(string vagaId);
        Task<CandidatoQuePassouPorAnaliseGestorDTO> ObterCandidatosQuePassaramPorAnaliseGestor(string codigoCandidato, string idCandidatura);
        Task<bool> AdicionarRecrutadorVaga(string vagaId, string codInternoColaboradorRecrutador);
        Task CopiarCandidatosEInformacoesComplementares(string idVagaParent, string idVagaChild, string codColaborador, bool movidaAutomaticamente = false);
        Task<List<dynamic>> BuscaRelatorioProdutividade(int orgId, DateTime dataInicio, DateTime dataFim);
        Task<IEnumerable<MotivoPerdaVagaDTO>> ListarMotivosPerdaVaga();
        Task<List<dynamic>> RelatorioVagasCandidaturas(DateTime dataInicio, DateTime dataFim, int orgIdUsuarioLogado);
        Task<TemplateDescricaoVagaDTO> BuscarTemplateDescricaoVaga(int orgId);
        Task AdicionarTemplateDescricaoVaga(int orgId, string textoIntroducao, string textoFinalizacao);
        Task AtualizarTemplateDescricaoVaga(int orgId, string textoIntroducao, string textoFinalizacao);
        Task ExcluirTemplateDescricaoVaga(int orgId);
        Task<IEnumerable<HistoricoStatusVagaItemDTO>> ListarHistoricoStatusVaga(string vagaId);
        Task<IEnumerable<NivelVagaDTO>> ListarNiveisVaga();
    }
}