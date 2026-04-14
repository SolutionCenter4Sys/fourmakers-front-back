using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Competencia.DashboardMinhaJornada;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Competencia.DashboardMinhaJornada
{
    public interface IDashboardMinhaJornadaRepository
    {
        Task<BigNumbersDashboardMinhaJornada> BigNumbers(int orgIdUsuarioLogado);
        Task<TopDezDashboardMinhaJornada> TopDez(int orgIdUsuarioLogado);
        Task<List<LogDetalhadoDashboardMinhaJornada>> ListarSkillsLog(int limit, int cursor, int orgIdUsuarioLogado);
        Task<List<dynamic>> BuscaRelatorioLogDetalhado(DateTime dataInicio, DateTime dataFim, int orgIdUsuarioLogado);
    }
}
