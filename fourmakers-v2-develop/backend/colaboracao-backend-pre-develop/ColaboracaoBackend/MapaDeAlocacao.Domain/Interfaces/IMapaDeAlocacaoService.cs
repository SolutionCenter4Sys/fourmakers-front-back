using DataTransferObject.Domain;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.MapaDeAlocacao;
using DataTransferObject.Domain.MapaDeAlocacao.CargaMapaAlocacao;
using DataTransferObject.Domain.MapaDeAlocacao.EditarAlocacao;
using DataTransferObject.Domain.MapaDeAlocacao.ExcluirAlocacao;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MapaDeAlocacao.Domain.Interfaces
{
    public interface IMapaDeAlocacaoService
    {
        Task<List<BuscarCargaMapaAlocacaoDTO>> BuscarCargaMapaLocacao();
        Task<CadastroMapaAlocacaoDTO> CadastroMapaAlocacao(CadastroMapaAlocacaoDTO cadastroMapaAlocacaoDTO, string cpfSolicitante, int orgId);
        Task<AlocacaoColabETbdDTO> EditarAlocacao(long periodoAlocacaoId, DateTime? dataInicio, DateTime? dataFim, bool? incluiFimDeSemana, double? quantidadeDeHoras, string cpfSolicitante, int orgId, sbyte? prioritario, string observacao, string oportunidade, double? percentual, bool? flagRetroalimentaCV);
        Task<DetalharColaboradorDTO> GetAlocacaoColaborador(string cpf, int? tbdId, string CodigoProjeto, int mes, int ano, string cpfSolicitante, int orgId);
        List<ColaboradorAlocadoDTO> GetColaboradoresAlocadosNoProjeto(string projetoId, string cpfSolicitante, int orgId);
        Task<GetMapaAlocacaoRecursoOutputDTO> GetMapaAlocacaoRecurso(GetMapaAlocacaoRecursoInputParam inputParam, string cpfSolicitante, int orgId);
        Task<GetMapaAlocacaoResumoOutputDTO> GetMapaAlocacaoResumo(GetMapaAlocacaoInputParam inputParam, string cpfSolicitante, int orgId);
        Task<DetalharColaboradorEditarDTO> GetPeriodoAlocacaoColaboradorVisaoEdicao(string cpf, int? tbdId, string CodigoProjeto, string mesParam, string anoParam, string cpfSolicitante, int orgId);
        Task<CargaMapaAlocacaoResult> InserirCargaMapaAlocacao(string cpf, int orgId);
        Task<IEnumerable<AlocacaoColabETbdDTO>> ListarAlocacoesColaboradoresETbds(ListarAlocacoesColabETbdInput dto, string cpfSolicitante, int orgId, string tokenUsuario, string cpfUsuarioLogado);
        Task<IEnumerable<ColaboradorETbdDTO>> ListarColaboradoresETbds(int orgId, string codigoDiretoria, string codigoGestor, string cpfSolicitante, string codigoDepartamento, TipoProfissionalEnum filtroTipoProfissional);
        List<NomeRecursoDTO> ListarColaboradoresGestor(string codGestor, string cpfSolicitante, int orgId);
        Task<List<ColaboradorCchDTO>> ListarColaboradoresOrgAsync(string busca, int cursor, int limite, int orgId, string cpfSolicitante);
        Task<List<GestorDTO>> ListarNomesGestores(string cpfSolicitante, int orgId, string? codDiretoria, string codigoDepartamento);
        Task<List<ProjetosColaboradorDTO>> ListarProjetosColaborador(string codColaborador, bool ehTbd, string cpfSolicitante, int orgId, string codigoGerenteProjeto, List<string> listaCodigoCliente, string status, FiltroProjetosPrioritariosEnum prioritarioFiltroEnum);
        List<ProjetosCchDTO> ListarProjetosOrg(string busca, int cursor, int limite, int orgId);
        Task<bool> RecalcularColaboradorNoPeriodoMensal(string codigoColaborador, bool ehTbd, string cpf, int orgId, bool forcarRecalculoHorasPrevistas);
        Task<StatusResult> RemoverAlocacao(long idPeriodoAlocacao, string cpfSolicitante, int orgId);
        Task<List<RemoverAlocacoesEmLoteResult>> RemoverAlocacoesEmLote(List<string> idsPeriodoAlocacao, string cpfSolicitante, int orgId);
        Task<IEnumerable<AlocacaoColabETbdDTO>> SubstituirDadosAlocacaoesPorPeriodo(SubstituirDadosAlocacaoesPorPeriodoParam param, string cpfSolicitante, int orgId);
        List<PerfilAlocacaoDTO> ListarPerfilAlocacao(string codProjeto, string codInternoColaborador, int orgId, bool ocultarSkills = false);
        Task<AlocacaoColabETbdDTO> RemoveSkillAlocacao(string codInternoColaborador, long periodoAlocacaoId, ItemSkillPerfilAlocacaoDTO skill, int orgId);
        Task<AlocacaoColabETbdDTO> AdicionaSkillAlocacao(string codInternoColaborador, long periodoAlocacaoId, ItemSkillPerfilAlocacaoDTO skill, int orgId);
        Task<List<SkillNivelDTO>> BuscarHabilidadesNaoDefinidasDoColaborador(string cpfColaborador, string token, int orgId);
        Task<AlocacaoColabETbdDTO> AlteraPerfilAlocacao(string codInternoColaborador, long periodoAlocacaoId, string perfilId, int orgId);
    }
}