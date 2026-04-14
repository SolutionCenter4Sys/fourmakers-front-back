using Colaboracao.Core.Interfaces;
using Colaboracao.Helper.Util;
using Core.Domain.Financeiro.IntegracaoContabil;
using DataTransferObject.Domain.Base;
using Financeiro.Domain.Interfaces.IntegracaoContabil;
using Financeiro.Domain.Interfaces.Reembolso.Solicitacao;
using Microsoft.AspNetCore.Mvc;

using Logs.Infra.Attributes;

namespace Financeiro.Domain.Impl.IntegracaoContabil
{
    [LogDomainClass]
    public class IntegracaoContabilService : IIntegracaoContabilService
    {
        public readonly static string DESCRICAO_ENTIDADE = "Integração Contábil";
        
        private readonly IIntegracaoContabilRepository _integracaoContabilRepository;
        private readonly IDBConnectionUnitOfWork _dbConnectionUnitOfWork;

        public IntegracaoContabilService(
            IIntegracaoContabilRepository integracaoContabilRepository,
            IDBConnectionUnitOfWork dbConnectionUnitOfWork,
            ISolicitacaoReembolsoService solicitacaoReembolsoService
            )
        {
            _integracaoContabilRepository = integracaoContabilRepository;
            _dbConnectionUnitOfWork = dbConnectionUnitOfWork;
        }

        public async Task<ApiGenericResult<FileContentResult>> GerarRemessaContabilMensalFolhaPontoERubricaAsync(string cnpj, string competencia, string cpfUsuario, int orgId)
        {
            var result = new ApiGenericResult<FileContentResult>();

            try
            {
                _dbConnectionUnitOfWork.BeginTransaction();

                var competenciaNormalizada = competencia.Replace("-", "/");

                var vigenciaId = await _integracaoContabilRepository.BuscarOuCriarVigenciaAsync(competenciaNormalizada);

                var remessa = await _integracaoContabilRepository.GerarRemessaContabilMensalFolhaPontoERubrica(cnpj, competenciaNormalizada, orgId);

                if (!remessa.Dados.Any())
                {
                    result.Sucesso = false;
                    result.Mensagem = "Nenhum dado encontrado para os parâmetros informados.";
                    _dbConnectionUnitOfWork.SafeRollback();
                    return result;
                }

                var idsRegistros = new
                {
                    folhaponto = remessa.Ids.FolhaPonto,
                    rubrica = remessa.Ids.Rubrica
                };

                _dbConnectionUnitOfWork.Commit();

                var dadosOrdenados = remessa.Dados.OrderBy(c => c.NomeColaborador).ToList();

                var fileBytes = ExcelFileUtil.CreateExcelFile(dadosOrdenados);
                var competenciaFormatada = dadosOrdenados.FirstOrDefault()?.Competencia ?? "00/0000";
                var fileName = $"RemessaContabil_{cnpj}_{competenciaFormatada.Replace("/", "")}.xlsx";

                result.Sucesso = true;
                result.Mensagem = "Remessa contábil gerada e registrada com sucesso.";
                result.Retorno = new FileContentResult(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = fileName
                };

                return result;
            }
            catch (Exception ex)
            {
                _dbConnectionUnitOfWork.SafeRollback();
                result.Sucesso = false;
                result.Mensagem = $"Erro ao gerar remessa contábil: {ex.Message}";
                return result;
            }
        }
    }
}