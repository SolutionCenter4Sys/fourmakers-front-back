using Colaboracao.Helper.Util;
using Core.Domain.Projeto;
using DataTransferObject.Domain.Base;
using Microsoft.AspNetCore.Mvc;
using Projeto.Domain.Interfaces.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

using Logs.Infra.Attributes;

namespace Projeto.Domain.Impl.Services
{
    [LogDomainClass]
    public class ExtracaoProjetoService : IExtracaoProjetoService
    {
        private readonly IProjetoRelatorioRepository _extracaoProjetoRepository;

        public ExtracaoProjetoService(IProjetoRelatorioRepository extracaoProjetoRepository)
        {
            _extracaoProjetoRepository = extracaoProjetoRepository;
        }

        public async Task<ApiGenericResult<FileContentResult>> ExportarProjetosExcel(string cpf, int orgId)
        {
            var ret = new ApiGenericResult<FileContentResult>();
            try
            {
                var extracaoAlocacaoResult = await _extracaoProjetoRepository.ListarProjetosParaExportacao(orgId);

                if (!extracaoAlocacaoResult.Any())
                {
                    ret.Mensagem = "Não existem projetos para gerar o arquivo.";
                    ret.Sucesso = false;
                    return ret;
                }

                var fileBytes = ExcelFileUtil.CreateExcelFile(extracaoAlocacaoResult);
                var fileName = "Exportacao_Projetos_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".xlsx";

                ret.Retorno = new FileContentResult(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                ret.Retorno.FileDownloadName = fileName;
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