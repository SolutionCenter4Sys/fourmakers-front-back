using DataTransferObject.Domain.Apontamento;
using DataTransferObject.Domain.Apontamento.ColaboradorEApontamento;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Usuario;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Apontamento.Domain.Interfaces.Service
{
    public interface IApontamentoService
    {
        Task<ApontamentosPorVigenciaDTO> ListarApontamentosPorVigencia(int mes, int ano, string cpfRequest, string codColaborador, int orgId, string cpfColaborador);
        Task<ApontamentosPorVigenciaDTO> ListarApontamentosVigenciaPorGerente(int mes, int ano, string cpf, string codColaborador, int orgId, string cpfGerente);
        Task<IEnumerable<ProjetoAtividadeDTO>> ListarProjetosComAtividadesPorColaborador(string cpfRequest, int orgId, string cpfColaborador);

        Task<bool> DeletarApontamentoColaborador(string colaboradorApontamentoId, string cpfRequest, int orgId, DateTime dataColetaDeDados, bool HoraZerada = false);
        Task<ColaboradorApontamentoDTO> ApontarHorasCarga(string cpfRequest, int orgId, string projetoId, string atividadeId, long horas, string dataRegistro, int diaQuebraSemana, bool deveSomarApontamentoDia, string cpfColaborador, string observacao);
        Task<ColaboradorApontamentoDTO> ApontarHoras(string cpfRequest, int orgId, string projetoId, string atividadeId, long horas, string dataRegistro, int diaQuebraSemana, bool deveSomarApontamentoDia, string cpfColaborador, string observacao);
        Task<ApontarHorasResult> ApontarHorasEmLote(string colaboradorCpf, int orgId, string projetoId, string atividadeId, long horas, string dataInicio, string dataFim, int diaQuebraSemana, bool deveSomarApontamentoDia, bool incluirSabado, bool incluirDomingo, bool incluirFeriado, string cpfColaborador, string observacao);
        int ObterNumeroSemanaDia(DateTime dia, int diaQuebraSemana, int orgId);
        int ObterNumeroDiaNaSemana(DateTime dia, int diaQuebraSemana, int orgId);
        Task<VigenciaDTO> ObterVigenciaDia(DateTime dia);
        TemplateSemanaVigenciaResult ObterTemplateSemanaVigencia(int mes, int ano, int diaQuebraSemana, int orgId);
        Task<List<StatusApontamentoGrupoDTO>> ObterListaStatusGrupo();
        Task<List<ProjetoGerenteResult>> ListarProjetosGerenteDeProjetos(string cpf, int orgId);
        Task<ListarVigenciaResult> ListarVigenciasProjetosGerente(string cpfGerente, int orgId);
        Task<ListarVigenciaResult> ListarVigenciasColaborador(string cpfRequest, int orgId, string cpfColaborador);
        Task<int> AprovarHorasGestorProjetoEmLote(AprovarHorasGestorProjetoEmLoteDTO aprovarHorasDTO, string cpf, int orgId, DateTime? dataColetaDeDados);
        Task<int> ReprovarHorasGestorProjetoEmLote(ReprovarHorasGestorProjetoEmLoteDTO reprovarHorasDTO, string cpf, int orgId);
        Task<IEnumerable<ColaboradoresVinculadosGerenteDTO>> ListarColaboradoresVinculadosGerenteDeProjetoComApontamentos(int mes, int ano, string codProjeto, string cpf, int orgId);
        Task<IEnumerable<ColaboradoresVinculadosGerenteDTO>> ListarGerentesAdministrativosDosColaboradoresVinculadosGerenteProjeto(string cpf, int orgId);
        Task<ListaComTotalizadorHorasResult<ProjetoVisaoGerenteDTO>> ListarProjetosVisaoGerenteDeProjeto(string codProjeto, int mes, int ano, string cpfColaborador, int statusMensal, string cpfRequest, int orgId, string cpfGerenteAdm);
        Task<ApiGenericResult<FileContentResult>> RelatorioApontamentos(int mes, int ano, string cpfRequest, int orgId);
        Task<ApiGenericResult<FileContentResult>> RelatorioApontamentosSimplificado(int mes, int ano, string cpfRequest, int orgId);
        Task<ListaComTotalizadorHorasResult<ColaboradorEApontamentoResult>> ListarColaboradoresEApontamentosPorGestor(string nomeColaborador, string codigoGerente, string codigoStatus, int mesVigencia, int anoVigencia, int orgId, string codProjeto, string codColaboradorExternoAprovador, int cursor, int limite, string cpfRequest);
        Task<List<GestoresApontamentoResult>> ListarGestoresApontamento(int orgId);
        Task<List<StatusApontamentoGrupoResult>> ListaStatusApontamentoGerenteProjeto(int orgId);
        Task<ApiGenericResult<AprovarProjetoVigenciaEmLoteResult>> AprovarProjetoVigenciaEmLote(List<AprovarProjetoVigenciaProjetoDTO> projetos, int mes, int ano, string justificativa, string cpfRequest, int orgId, DateTime dataColetaDeDados);
        Task<ApiGenericResult<FileContentResult>> RelatorioColaboradoresQueNaoApontaram(int orgId, string cpf, int mes, int ano);
        Task<ApiGenericResult<string>> EnviaEmailNotificacaoAprovadoresStatusPendentes(string nomeColaborador, string codigoGerente, string codigoStatus, int mesVigencia, int anoVigencia, string codProjeto, string codColaboradorExternoAprovador, int cursor, int limite, UsuarioLogadoDTO usuarioLogado);
        Task<ColaboradorApontamentoDTO> EditarApontamento(EditarApontamentoDTO editarApontamentoDTO, string cpfRquest, int orgId);
        
    }   
}