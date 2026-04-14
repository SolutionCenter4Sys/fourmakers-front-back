using DataTransferObject.Domain.Apontamento;
using DataTransferObject.Domain.Apontamento.ColaboradorEApontamento;
using DataTransferObject.Domain.BI;
using DataTransferObject.Domain.Colaborador;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Apontamento
{
    public interface IApontamentoRepository
    {
        Task<List<ProjetoAtividadeDTO>> ListarProjetosAtivosPorColaborador(string cpf, int orgId);
        bool DeletarApontamento(string colaborador_apontamento_id);
        Task<ColaboradorApontamentoDTO> GetApontamentoById(string colaboradorApontamentoId, string idioma);
        Task<List<ColaboradorApontamentoDTO>> GetApontamentosColaborador(string colaboradorCpf, int orgId, string projetoId = null, string atividadeId = null, DateTime? dataRegistro = null, List<DateTime> periodo = null, int? mes = null, int? ano = null);
        Task<ColaboradorApontamentoDTO> InserirApontamento(ColaboradorApontamentoDTO colaboradorApontamentoDTO, string idioma, string cpfRequest);
        Task<ColaboradorApontamentoDTO> EditarApontamento(ColaboradorApontamentoDTO colaboradorApontamentoDTO, string idioma, string cpfRequest);
        Task<List<ApontamentoMensalDTO>> ListarApontamentosMensaisPorVigenciaColaborador(int mes, int ano, string cpf, string codColaborador, int orgId, string cpfGerente, bool soProjetosDesteGerente = false);
        Task<List<AprovadorApontamentoMensalDTO>> ListarAprovadoresApontamentosMensaisColaborador(int mes, int ano, string cpf, string codProjeto, int orgId);
        Task<List<ApontamentoColaboradorDTO>> ListarApontamentosPorVigenciaColaborador(int mes, int ano, string cpf, string codColaborador, int orgId, string cpfGerente, bool soProjetosDesteGerente = false);
        Task<List<StatusApontamentoResult>> ListarStatus(string idioma);
        Task<List<VigenciaDTO>> ListarVigencia();
        Task<bool> InserirApontamentoLog(ColaboradorApontamentoLogDTO colaboradorApontamentoLogDTO);
        Task<bool> InserirLogsAprovacaoOuReprovacaoApontamentosGerenteDeProjeto(List<string> ids, string justificativa, int codStatus, string cpfRequest);
        Task<List<ProjetoGerenteResult>> ListarProjetosGerenteDeProjetos(string cpf, int orgId);
        int AtualizarStatusApontamentoAprovadoOuReprovadoPorIds(List<string> ids, int codStatusApontamento, string justificativa, string cpfRequest);
        Task<List<ApontamentoReduzidoDTO>> GetIdsApontamentosPorCodStatusApontamento(List<string> ids, int statusId, int orgId);
        Task<IEnumerable<ProjetoAtividadeDTO>> ListarProjetosComAtividadesPorCPF(string cpf, int orgId, bool lancamentoParaOutroColaborador = false);
        Task<List<VigenciaSimplesDTO>> GetVigenciaColaborador(string colaboradorCpf, int orgId);
        Task<List<VigenciaSimplesDTO>> GetVigenciaApontamentoGerenteDeProjetos(string gerenteCpf, int orgId); 
        Task<List<(Guid id_apontamento, string cod_projeto, string cpf_gerente, int tb_org_id, string cod_gestor_hierarquico)>> GetCodProjetoECpfGerenteByIdsApontamentos(List<string> ids, string? cpfGestor = null);        
        Task<IEnumerable<ColaboradoresVinculadosGerenteDTO>> ListarColaboradoresVinculadosGerenteDeProjetoComApontamentos(int mes, int ano, string codProjeto, string cpfGerenteDeProjeto, int orgId, List<string>? diretorias);
        Task<IEnumerable<ColaboradoresVinculadosGerenteDTO>> ListarGerentesAdministrativosDosColaboradoresVinculadosGerenteProjeto(string cpf, int orgId, List<string>? diretorias);
        Task<ListaComTotalizadorHorasResult<ProjetoVisaoGerenteDTO>> ListarProjetosVisaoGerenteDeProjeto(string cpf, int org_id, string codProjeto, int mes, int ano, string cpfColaborador, int codStatusGrupo, string cpfGerenteAdm, List<string>? diretorias);
        List<FeriadoDTO> ListarFeriados(int orgId);
        Task<List<RelatorioApontamentoDTO>> ListarApontamentoRecenteRelatorioBI(int orgId);
        Task<ListaComTotalizadorHorasResult<ColaboradorEApontamentoResult>> ListarColaboradoresEApontamentosPorGestor(string nomeColaborador, string codigoGerente, string codigoStatus, int mesVigencia, int anoVigencia, int orgId, string codProjeto, string codColaboradorExternoAprovador, int cursor, int limite, List<string>? diretorias, bool considerarApenasAtivos = false);
        Task<List<GestoresApontamentoResult>> ListarGestoresApontamento(int orgId);
        Task<List<StatusApontamentoGrupoResult>> ListaStatusApontamentoGerenteProjeto(int orgId, string idioma);
        Task<ColaboradoresOrgDTO> GetColaboradorPorCpfEOrgId(string cpfColaborador, int orgId);
        Task<ProjetoDTO> GetProjetoPorCodProjetoOrgId(string codProjeto, int orgId);
        Task<List<ColaboradorRelatorioNaoApontadoDTO>> RelatorioColaboradoresQueNaoApontaram(int orgId, int mes, int ano);
        Task<bool> ExisteApontamentosAlteradosDuranteAExecucaoPorIds(List<string> ids, DateTime? dataColetaDeDados);
        Task<DateTime> BuscarHorasExataBancoDeDados();
        Task<bool> VerificaSeDataDeRegistroEhMaiorQueDataDoCadastro(string dataRegistro, string codigoColaborador, int orgId);
        Task<bool> VerificaSeEhGestorHierarquicoDeUmAprovador(int orgId, string codigoInternoColaborador, string codigoInternoGerente);
        Task<string> BuscarCpfPorApontamentoId(string apontamentoId);
        Task<List<VigenciaSimplesDTO>> ListarVigenciasMesEQuantidade(int quantidadeMes, DateTime dataAtual);
        Task<bool> VerificaSeColaboradorPodeApontarPorModeloDeTrabalho(int orgId, string codigoInternoColaborador);
        Task<List<ApontamentoExcedenteDTO>> ListarApontamentosExcedentesPorOrgId(int orgId, int mes, int ano, int horasExcedentes);
        Task<dynamic> RelatorioApontamento(int? mes, int? ano, int orgId, string cpfGerente, List<string>? diretorias);
        Task<dynamic> RelatorioApontamentoSimplificado(int? mes, int? ano, int orgId, List<string>? diretorias, bool considerarApenasAtivos = false);
        Task<IEnumerable<RelatorioApontamentoDTO>> ListarApontamentoRelatorioBIAsync(int orgId, string dataInicial);
    }
}