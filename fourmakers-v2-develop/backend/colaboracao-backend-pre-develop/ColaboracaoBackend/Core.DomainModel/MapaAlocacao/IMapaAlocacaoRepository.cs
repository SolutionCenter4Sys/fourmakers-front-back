using DataTransferObject.Domain.MapaDeAlocacao;
using DataTransferObject.Domain.MapaDeAlocacao.CalculoMensal;
using DataTransferObject.Domain.MapaDeAlocacao.CargaMapaAlocacao;
using DataTransferObject.Domain.MapaDeAlocacao.ConsultaAlocacao;
using DataTransferObject.Domain.MapaDeAlocacao.DetalharColaborador;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.MapaAlocacao
{
    public interface IMapaAlocacaoRepository
    {
        string BuscaNomeProjeto(string codigoProjeto, int orgId);
        Task<List<BuscarCargaMapaAlocacaoDTO>> BuscarCargaPeriodoAlocacaoFoursysBI();
        Task<CadastroMapaAlocacaoDTO> CadastroMapaAlocacao(CadastroMapaAlocacaoDTO cadastroMapaAlocacaoDTO, int orgId, bool isTbd = false, bool isColaborador = true);
        Task CadastrarAssociacaoAutomaticaNoProjeto(string codigoColaborador, string codigoProjeto, int orgId);
        List<ColaboradorAlocadoDTO> ColaboradoresAlocadosNoProjeto(string projetoId, int orgId);
        List<ColaboradorAlocadoDTO> ColaboradoresAlocadosNoProjetoFiltradoPorData(string projetoId, int orgId, DateTime inicioProjeto, DateTime fimProjeto);
        Task<List<ListaConsultaDatasDTO>> ConsultaDatasPeriodoAlocacaoColabOuTbd(string codigoProjeto, string codigoColabOuTbd, long? periodoAlocacaoId, bool ehTbd, int orgId);
        Task<AlocacaoColabETbdDTO> EditarAlocacao(long periodoAlocacaoId, DateTime? dataInicio, DateTime? dataFim, bool? incluiFimDeSemana, double? quantidadeDeHoras, sbyte? prioritario, string observacao, string oportunidade, double? percentual, string codigoColaborador, string colaboradorCpf, int? codTbdAlocado, string codigoProjeto, bool? flagRetroalimentaCV);
        bool ExisteCodigoProjetoParaOrgId(string codigoProjeto, int orgId);
        Task<List<RecursoMapaDTO>> FiltroMapaAlocacao(GetMapaAlocacaoInputDTO mapaAlocacaoInputDTO, int orgId);
        Task<List<CalculoMensalDTO>> GetCalculosMensaisAsync(int orgId, string codigoColaborador = null, int? codTbdAlocado = null);
        List<HorasMensalDTO> GetHorasTotalProjeto(List<HorasMensalDTO> mesesBase, string codProjeto, IEnumerable<FeriadoAlocacaoDTO> feriados, int org_id);
        Task<int> GetQuantidadeColaboradoresAsync(int orgId);
        Task<List<PeriodoAlocadoDTO>> GetPeriodoAlocacaoColaboradorComFiltro(string cpf, int? tbdId, int mes, int ano, string codigoProjeto, int orgId, bool filtrarPorMesAtualEFuturas = false);
        CadastroMapaAlocacaoDTO GetColaboradorPeriodoAlocacaoById(long id);
        List<CadastroMapaAlocacaoDTO> GetColaboradorPeriodoAlocacaoByIds(long[] ids);
        Task<int> ObterQuantidadeAlocacoesColaboradorNumProjeto(string cpfColaborador, string codProjeto, int orgId);
        List<CadastroMapaAlocacaoDTO> GetColaboradorPeriodoAlocacaoTodos();
        Task<IEnumerable<AlocacaoColabETbdDTO>> ListarAlocacoesColaboradoresETbds(string pesquisa, int? periodoAlocacaoId, int orgId, List<string>? codigoUnidade, string codigoDepartamento, string codigoGestorAdm, List<string>? listaCodigoColabOuTbd, TipoProfissionalEnum filtroTipoProfissional, string codigoGestorProjeto, List<string>? listaCodigoClientes, bool apenasProjetosPrioritarios, List<string>? listaCodigoProjetos, string codigoStatusProjeto, string qtdGerenteProjetoPrioridade, bool incluirInativos, string habilidades, string perfis);
        Task<IEnumerable<ColaboradorETbdDTO>> ListarColaboradoresETbds(int orgId, List<string>? diretorias, string codigoGestor, string codigoDepartamento, TipoProfissionalEnum filtroTipoProfissional);
        public List<ColaboradorCchDTO> ListarColaboradoresOrg(string busca, int cursor, int limite, int orgId, List<string>? restricaoDiretorias = null);
        public List<ProjetosCchDTO> ListarProjetosOrg(string busca, int cursor, int limite, int orgId);
        Task PersistirCalculosMensaisAsync(List<CalculoMensalDTO> calculosMensal);
        Task RemoverAlocacaoPorIds(long[] idsPeriodoAlocacao);
        Task RemoverCalculosMensaisAsync(string codigoColaborador, int? codTbdAlocado, int orgId);
        bool ValidaAcesso(string cpf, int orgId);
        Task<List<RelatorioAprovadoresDTO>> ListarProjetosEAprovadores(int orgId);
        Task<(List<dynamic> alocacoes, List<dynamic> perfis)> ListarAlocacoesColaboradoresETbdsDynamicAsync(string pesquisa, int? periodoAlocacaoId, int orgId, List<string> codigoUnidade = null, string codigoDepartamento = null, string codigoGestorAdm = null, List<string> listaCodigoColabOuTbd = null, TipoProfissionalEnum filtroTipoProfissional = 0, string codigoGestorProjeto = null, List<string> listaCodigoClientes = null, bool apenasProjetosPrioritarios = false, List<string> listaCodigoProjetos = null, string codigoStatusProjeto = null, string qtdGerenteProjetoPrioridade = null, bool incluirInativos = false, string habilidades = null, string perfis = null);
    }
}