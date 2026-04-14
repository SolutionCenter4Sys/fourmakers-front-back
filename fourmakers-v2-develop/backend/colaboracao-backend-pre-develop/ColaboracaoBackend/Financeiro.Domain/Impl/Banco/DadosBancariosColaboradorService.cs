using Colaboracao.Helper.Enum;
using Core.Domain.Financeiro.Banco;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.Banco.DadosBancariosColaborador;
using Financeiro.Domain.Interfaces.Banco;
using Logs.Infra.Attributes;

namespace Financeiro.Domain.Impl.Banco;

[LogDomainClass]
public class DadosBancariosColaboradorService(IDadosBancariosColaboradorRepository dadosBancariosColaboradorRepository, IDadosBancariosColaboradorValidadorService dadosBancariosColaboradorValidadorService) : IDadosBancariosColaboradorService
{
    public async Task<ApiGenericResult<DadosBancariosColaboradorResult?>> BuscarDadosBancariosPorColaboradorIdAsync(string codigoInternoColaborador, int orgId)
    {
        var apiResult = new ApiGenericResult<DadosBancariosColaboradorResult?>();
        try
        {
            var result = await dadosBancariosColaboradorRepository.BuscarDadosBancariosPorColaboradorIdAsync(codigoInternoColaborador, orgId);
            apiResult.Retorno = result;
        }
        catch (Exception e)
        {
            ExceptionUtil.GerenciarRetornoExcecao(e, CRUDEnum.Read, "Dados Bancários Colaborador");
        }
        
        return apiResult;
    }
    
    public async Task<ApiGenericResult<DadosBancariosColaboradorResult>> CriarDadosBancariosAsync(DadosBancariosColaboradorBase input,string codigoInternoColaborador, int orgId)
    {
        var apiResult = new ApiGenericResult<DadosBancariosColaboradorResult>();
        var erros = await dadosBancariosColaboradorValidadorService.ValidarEntrada(input, CRUDEnum.Create);
        if (erros.Any())
        {
            apiResult.Mensagem = "Erros de validação encontrados nos dados bancários do colaborador.";
            apiResult.Erros = erros;
            return apiResult;
        }
        try
        {
            var result = await dadosBancariosColaboradorRepository.CriarDadosBancariosAsync(input, codigoInternoColaborador, orgId);
            apiResult.Retorno = result;
        }
        catch (Exception e)
        {
            ExceptionUtil.GerenciarRetornoExcecao(e, CRUDEnum.Create, "Dados Bancários Colaborador");
        }
        
        return apiResult;
    }
    
    public async Task<ApiGenericResult<DadosBancariosColaboradorResult>> EditarDadosBancariosAsync(DadosBancariosColaboradorBase input, string codigoInternoColaborador, int orgId)
    {
        var apiResult = new ApiGenericResult<DadosBancariosColaboradorResult>();
        var erros = await  dadosBancariosColaboradorValidadorService.ValidarEntrada(input, CRUDEnum.Update);
        if (erros.Any())
        {
            apiResult.Mensagem = "Erros de validação encontrados nos dados bancários do colaborador.";
            apiResult.Erros = erros;
            return apiResult;
        }
        try
        {
            var result = await dadosBancariosColaboradorRepository.EditarDadosBancariosAsync(input, codigoInternoColaborador, orgId);
            apiResult.Retorno = result;
        }
        catch (Exception e)
        {
            ExceptionUtil.GerenciarRetornoExcecao(e, CRUDEnum.Update, "Dados Bancários Colaborador");
        }
        
        return apiResult;
    }
}