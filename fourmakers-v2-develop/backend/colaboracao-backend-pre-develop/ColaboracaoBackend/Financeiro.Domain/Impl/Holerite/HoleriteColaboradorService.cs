using ApiClient.Domain;
using ApiClient.Domain.Interfaces;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Colaboracao.Helper.Util;
using Core.Domain.Apontamento;
using Core.Domain.Financeiro.Holerite;
using DataTransferObject.Domain.Apontamento.FolhaPonto;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.Holerite;
using DataTransferObject.Domain.Util;
using Financeiro.Domain.Interfaces.Holerite;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

using Logs.Infra.Attributes;

namespace Financeiro.Domain.Impl.Holerite;

[LogDomainClass]
public class HoleriteColaboradorService(IHoleriteRepository holeriteRepository, IFolhaPontoRepository _folhaPontoRepository, ITokens tokens, IUploadFilesClient _uploadFilesClient) : IHoleriteColaboradorService
{
    public async Task<ApiGenericResult<IEnumerable<HoleriteColaboradorDTO>>> ListarHoleritesColaboradorPorAnoAsync(string codigoInternoColaborador, int ano, int orgId)
    {
        var apiResult = new ApiGenericResult<IEnumerable<HoleriteColaboradorDTO>>();
        try
        {
            var result =  await holeriteRepository.BuscarHoleritesPorColaboradorPorAnoAsync(codigoInternoColaborador, ano, orgId);
            foreach (var holerite in result)
            {
                await AjustarHoleriteComFolhaPonto(holerite, orgId);
            }
            apiResult.Retorno = result;
            return apiResult;
        }
        catch (Exception e)
        {
            ExceptionUtil.GerenciarRetornoExcecao(e, CRUDEnum.Read, "Holerites Colaborador");
        }
        
        return apiResult;
    }

    public async Task<ApiGenericResult<HoleriteColaboradorDTO>> AssinarHoleritePorLoteId(string tbItemLoteId, string tokenUsuarioLogado, string cpfLogado, int orgId)
    {
        var result = new ApiGenericResult<HoleriteColaboradorDTO>();
        try
        {
            var buscarItem = await holeriteRepository.BuscarHoleriteColaboradorPorItemLoteIdAsync(tbItemLoteId);
            if (buscarItem == null)
            {
                throw new ArgumentException("Erro ao tentar baixar documento, holerite com numero do lote invalido.");
            }

            if (cpfLogado != buscarItem.CodigoInternoColaborador)
            {
                throw new ArgumentException("Não é possivel assinar holerite de outro colaborador.");
            }

            
            if (buscarItem.Assinado)
            {
                result.Retorno = buscarItem;
            }
            else
            {
                var urlPdfFullPath = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA) +
                                 buscarItem.HoleritePdf;
                var replaceUrl = urlPdfFullPath.Replace("$1", tokens.Base64(tokenUsuarioLogado));

                var httpClient = new HttpClient();
                var pdfBytes = await httpClient.GetByteArrayAsync(replaceUrl);

                await holeriteRepository.AssinarHoleriteColaborador(tbItemLoteId);
                
                var buscarHoleriteAtualizado = await holeriteRepository.BuscarHoleriteColaboradorPorItemLoteIdAsync(tbItemLoteId);
                    
                buscarItem.ObjetoHolerite = JsonConvert.DeserializeObject<HoleriteWrapperDTO>(buscarItem.ObjetoHoleriteString);

                var dataVisualizacao = buscarHoleriteAtualizado.AssinadoEm?.ToString("dd/MM/yyyy");

                var listaDeAssinaturas = new List<PdfSignatureLine>()
                {
                    new PdfSignatureLine()
                    {
                        Text = buscarItem.ObjetoHolerite.Holerite.Funcionario.Nome,
                        PosX = 170,
                        PosY = 15,
                        FontSize = 10
                    },

                    new PdfSignatureLine()
                    {
                        Text = $"Data Visualização: {dataVisualizacao}",
                        PosX = 170 - 42,
                        PosY = 15 - 12,
                        FontSize = 8
                    },
                    new PdfSignatureLine()
                    {
                        Text = buscarItem.ObjetoHolerite.Holerite.Funcionario.Nome,
                        PosX = 170,
                        PosY = 435,
                        FontSize = 10
                    },

                    new PdfSignatureLine()
                    {
                        Text = $"Data Visualização: {dataVisualizacao}",
                        PosX = 170 - 42,
                        PosY = 435 - 12,
                        FontSize = 8
                    }
                };
                
                var addSignature = PdfUtil.AddSignature(pdfBytes, listaDeAssinaturas);

                await _uploadFilesClient.UploadFile(buscarItem.HoleritePdf, addSignature);
                
                await AjustarHoleriteComFolhaPonto(buscarHoleriteAtualizado, orgId);
                result.Retorno = buscarHoleriteAtualizado;
            }
        }
        catch (Exception e)
        {
            ExceptionUtil.GerenciarRetornoExcecao(e, CRUDEnum.Create, "Assinatura Holerite Colaborador");
        }
        
        return result;
    }

    private async Task AjustarHoleriteComFolhaPonto(HoleriteColaboradorDTO holerite, int orgId)
    {
        holerite.ObjetoHolerite = JsonConvert.DeserializeObject<HoleriteWrapperDTO>(holerite.ObjetoHoleriteString);
        holerite.HoleritePdf = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA) + holerite.HoleritePdf;
        var getFolhaPonto =
            await _folhaPontoRepository.BuscarFolhaPontoColaboradorPorLoteIdAsync(orgId, holerite.Competencia, holerite.CodigoInternoColaborador);

        if (getFolhaPonto != null)
        {
            getFolhaPonto.ObjetoFolhaPonto = JsonConvert.DeserializeObject<RelatorioPontoRootDTO>(getFolhaPonto.ObjetoFolhaPontoString);
            if (getFolhaPonto.ObjetoFolhaPonto != null)
            {
                var diasTrabalhados = getFolhaPonto.ObjetoFolhaPonto.RelatorioPonto.Resumo.DiasTrabalhados;
                var saldoAdicional = getFolhaPonto.ObjetoFolhaPonto.RelatorioPonto.Totais.SaldoAtual;
                    
                holerite.TotalHorasExtras = saldoAdicional;
                holerite.DiasTrabalhados = int.TryParse(diasTrabalhados, out var dias) ? dias : 0;
            }
        }
    }
}