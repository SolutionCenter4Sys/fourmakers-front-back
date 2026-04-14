using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Competencia.DashboardMinhaJornada;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Competencia.Domain.Interfaces.Services.DashboardMinhaJornada
{
    public interface IDashboardMinhaJornadaService
    {
        Task<ApiGenericResult<BigNumbersDashboardMinhaJornada>> BigNumbers(int orgIdUsuarioLogado);
        Task<ApiGenericResult<TopDezDashboardMinhaJornada>> TopDez(int orgIdUsuarioLogado);
        Task<ApiGenericResult<List<LogDetalhadoDashboardMinhaJornada>>> LogDetalhado(int limit, int cursor, int orgIdUsuarioLogado);
        Task<ApiGenericResult<FileContentResult>> RelatorioLogDetalhado(DateTime dataInicio, DateTime dataFim, int orgIdUsuarioLogado);
    }
}