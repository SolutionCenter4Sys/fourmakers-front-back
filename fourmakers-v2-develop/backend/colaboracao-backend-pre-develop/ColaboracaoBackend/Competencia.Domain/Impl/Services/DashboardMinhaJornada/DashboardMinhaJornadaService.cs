using Colaboracao.Helper.Util;
using Competencia.Domain.Interfaces.Services.DashboardMinhaJornada;
using Core.Domain.Competencia.DashboardMinhaJornada;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Competencia.DashboardMinhaJornada;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Competencia.Domain.Impl.Services.DashboardMinhaJornada
{
    [LogDomainClass]
    public class DashboardMinhaJornadaService : IDashboardMinhaJornadaService
    {
        private readonly IDashboardMinhaJornadaRepository _dashboardMinhaJornadaRepository;

        public DashboardMinhaJornadaService(IDashboardMinhaJornadaRepository dashboardMinhaJornadaRepository)
        {
            _dashboardMinhaJornadaRepository = dashboardMinhaJornadaRepository;
        }

        public async Task<ApiGenericResult<BigNumbersDashboardMinhaJornada>> BigNumbers(int orgIdUsuarioLogado)
        {
            var ret = new ApiGenericResult<BigNumbersDashboardMinhaJornada>();

            ret.Retorno = await _dashboardMinhaJornadaRepository.BigNumbers(orgIdUsuarioLogado);
            ret.Mensagem = "Big numbers retornados com sucesso.";

            return ret;
        }

        public async Task<ApiGenericResult<TopDezDashboardMinhaJornada>> TopDez(int orgIdUsuarioLogado)
        {
            var ret = new ApiGenericResult<TopDezDashboardMinhaJornada>();

            ret.Retorno = await _dashboardMinhaJornadaRepository.TopDez(orgIdUsuarioLogado);
            ret.Mensagem = "Top dez retornados com sucesso.";

            return ret;
        }

        public async Task<ApiGenericResult<List<LogDetalhadoDashboardMinhaJornada>>> LogDetalhado(int limit, int cursor, int orgIdUsuarioLogado)
        {
            var ret = new ApiGenericResult<List<LogDetalhadoDashboardMinhaJornada>>();

            ret.Retorno = await _dashboardMinhaJornadaRepository.ListarSkillsLog(limit, cursor, orgIdUsuarioLogado);
            ret.Mensagem = "Logs detalhados retornados com sucesso.";

            return ret;
        }

        public async Task<ApiGenericResult<FileContentResult>> RelatorioLogDetalhado(DateTime dataInicio, DateTime dataFim, int orgIdUsuarioLogado)
        {
            var ret = new ApiGenericResult<FileContentResult>();
            try
            {

                var RelatorioLogDetalhadoResult = await _dashboardMinhaJornadaRepository.BuscaRelatorioLogDetalhado(dataInicio, dataFim, orgIdUsuarioLogado);

                if (!RelatorioLogDetalhadoResult.Any())
                {
                    ret.Mensagem = "Não existem registros para gerar o arquivo.";
                    ret.Sucesso = false;
                    return ret;
                }

                var fileBytes = ExcelFileUtil.CreateExcelFile(RelatorioLogDetalhadoResult);
                var fileName = "Relatorio_Log_Detalhado_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".xlsx";

                ret.Retorno = new FileContentResult(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = fileName
                };
                ret.Sucesso = true;
            }
            catch (Exception ex)
            {
                ret.Sucesso = false;
                ret.Mensagem = ex.Message;
            }

            return ret;
        }
    }
}
