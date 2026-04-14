using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.Banco.DadosBancariosColaborador;

namespace Financeiro.Domain.Interfaces.Banco;

public interface IDadosBancariosColaboradorService
{
    Task<ApiGenericResult<DadosBancariosColaboradorResult?>> BuscarDadosBancariosPorColaboradorIdAsync(string codigoInternoColaborador, int orgId);
    Task<ApiGenericResult<DadosBancariosColaboradorResult>> CriarDadosBancariosAsync(DadosBancariosColaboradorBase input,string codigoInternoColaborador, int orgId);
    Task<ApiGenericResult<DadosBancariosColaboradorResult>> EditarDadosBancariosAsync(DadosBancariosColaboradorBase input, string codigoInternoColaborador, int orgId);
}