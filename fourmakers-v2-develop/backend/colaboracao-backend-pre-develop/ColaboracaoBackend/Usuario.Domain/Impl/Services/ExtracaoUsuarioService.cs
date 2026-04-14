using Colaboracao.Helper.Util;
using Core.Domain.Usuario.Permissao;
using Core.DomainModel.Usuario;
using DataTransferObject.Domain.Base;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Usuario.Domain.Interfaces.Services;

using Logs.Infra.Attributes;

namespace Usuario.Domain.Impl.Services
{
    [LogDomainClass]
    public class ExtracaoUsuarioService : IExtracaoUsuarioService
    {
        private readonly IUsuarioRelatorioRepository _extracaoUsuarioRepository;
        private readonly IFuncionalidadeSistemaRepository _funcionalidadeSistemaRepository;

        public ExtracaoUsuarioService(IUsuarioRelatorioRepository extracaoUsuarioRepository, IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository)
        {
            _extracaoUsuarioRepository = extracaoUsuarioRepository;
            _funcionalidadeSistemaRepository = funcionalidadeSistemaRepository;
        }

        public async Task<ApiGenericResult<FileContentResult>> RelatorioColaboradores(string cpf, int orgId)
        {
            var ret = new ApiGenericResult<FileContentResult>();
            try
            {
                var extracaoAlocacaoResult = await _extracaoUsuarioRepository.RelatorioColaboradores(orgId);

                if (!extracaoAlocacaoResult.Any())
                {
                    ret.Mensagem = "Não existem usuários para gerar o arquivo.";
                    ret.Sucesso = false;
                    return ret;
                }

                var fileBytes = ExcelFileUtil.CreateExcelFile(extracaoAlocacaoResult);
                var fileName = "Exportacao_Usuarios_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".xlsx";

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