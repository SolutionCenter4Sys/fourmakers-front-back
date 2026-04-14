using DataTransferObject.Domain.MapaDeAlocacao;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.MapaAlocacao
{
    public interface IExtracaoAlocacaoRepository
    {
        Task<List<dynamic>> ExportarAlocacoesTodasExcel(int orgId);
        Task<List<dynamic>> ExportarAlocacoesPorPesquisaExcel(string pesquisa, int? periodoAlocacaoId, int orgId, string codigoUnidade = null, string codigoDepartamento = null, string codigoGestorAdm = null, string codigoColabOuTbd = null, TipoProfissionalEnum filtroTipoProfissional = 0, string codigoGestorProjeto = null, string listaCodigoClientes = null, bool apenasProjetosPrioritarios = false, string listaCodigoProjetos = null, string codigoStatusProjeto = null, string qtdGerenteProjetoPrioridade = null, bool incluirInativos = false, string habilidades = null, string perfis = null);
    }
}