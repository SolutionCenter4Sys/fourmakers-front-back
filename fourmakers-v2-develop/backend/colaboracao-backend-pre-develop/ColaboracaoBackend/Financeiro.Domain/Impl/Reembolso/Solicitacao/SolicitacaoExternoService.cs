using ApiClient.Domain;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Colaboracao.Helper.Util;
using Core.Domain.Financeiro.IntegracaoContabil;
using Core.Domain.Reembolso.Solicitacao;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.IntegracaoContabil;
using DataTransferObject.Domain.Util.Enum;
using Financeiro.Domain.Interfaces.Reembolso.Solicitacao;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Financeiro.Domain.Services.Reembolso.Solicitacao;

[LogDomainClass]
public class SolicitacaoExternoService(ISolicitacaoReembolsoService _solicitacaoReembolsoService, 
                                       IIntegracaoContabilRepository _integracaoContabilRepository,
                                       ITokenSistemaService _tokenSistemaService,
                                       IDBConnectionUnitOfWork _unitOfWork,
                                       ISolicitacaoReembolsoRepository _solicitacaoReembolsoRepository) : ISolicitacaoExternoService
{

    public async Task<ApiGenericResult<List<RemessaContabilRegistroReembolsoDTO>>> ProcessarReembolsoAsync(string tokenSistema, bool atualizarStatusParaPago, string cnpj, string codigoColaboradorExternoAprovador)
    {
        var result = new ApiGenericResult<List<RemessaContabilRegistroReembolsoDTO>>();

        try
        {
            _unitOfWork.BeginTransaction();

            int orgId = _tokenSistemaService.GetOrgTokenSistemaWithValidatingOrgs(tokenSistema, EnumORG.NUMEN_4, EnumORG.ROYAL_9);

            string codigoInternoColaboradorAprovador = "", mensagemErro = "";

            if (atualizarStatusParaPago)
            {
                if (string.IsNullOrWhiteSpace(codigoColaboradorExternoAprovador))
                    mensagemErro = "Código do colaborador externo aprovador é obrigatório quando atualizarStatusParaPago = true.";
                else
                {
                    codigoInternoColaboradorAprovador = await _solicitacaoReembolsoRepository.BuscarCodigoInternoColaboradorPorExterno(codigoColaboradorExternoAprovador, orgId);

                    if (string.IsNullOrWhiteSpace(codigoInternoColaboradorAprovador))
                        mensagemErro = "Código do colaborador externo aprovador não encontrado para esta organização.";
                }
            }

            if (!string.IsNullOrEmpty(mensagemErro))
            {
                result.Sucesso = false;
                result.Mensagem = mensagemErro;
                return result;
            }

            var remessa = await _integracaoContabilRepository.GerarRemessaContabilMensalReembolso(cnpj, null, orgId);
            
            foreach (var item in remessa.DadosReembolso)
            {
                List<string> documentos = JsonConvert.DeserializeObject<List<string>>(item.DocumentosJson);
                item.Documentos = documentos;
            }

            if (!remessa.DadosReembolso.Any())
            {
                result.Sucesso = false;
                result.Mensagem = "Nenhum dado encontrado para os parâmetros informados.";
                _unitOfWork.SafeRollback();
                return result;
            }

            var idsRegistros = new
            {
                reembolso = remessa.Ids.Reembolso,
            };


            if (atualizarStatusParaPago)
            {
                await _solicitacaoReembolsoService.GerarPagamentoDeSolicitacoesPorIds(idsRegistros.reembolso.Select(x => x.ToIntOuZero()).ToList(), orgId, codigoInternoColaboradorAprovador);

                if(remessa.DadosReembolso.First().DataAprovacao == null)
                {
                    result.Sucesso = false;
                    result.Mensagem = "Não foi possível atualizar o status para pago. Erro ao consultar data de aprovação.";
                    _unitOfWork.SafeRollback();
                    return result;
                }

                var competenciaNormalizada = remessa.DadosReembolso.First().DataAprovacao.Value.ToString("MM/yyyy");
                var vigenciaId = await _integracaoContabilRepository.BuscarOuCriarVigenciaAsync(competenciaNormalizada);

                await _integracaoContabilRepository.GravarLogRemessaContabilAsync(vigenciaId, cnpj, orgId, idsRegistros, codigoInternoColaboradorAprovador);
            }

            _unitOfWork.Commit();

            var dadosOrdenados = remessa.DadosReembolso.OrderBy(c => c.NomeColaborador).ToList();

            result.Sucesso = true;
            result.Mensagem = "Processamento reembolso efetuado com sucesso.";
            result.Retorno = dadosOrdenados;

            return result;
        }
        catch (Exception ex)
        {
            _unitOfWork.SafeRollback();
            result.Sucesso = false;
            result.Mensagem = $"Erro ao gerar remessa contábil: {ex.Message}";
            return result;
        }
    }

}