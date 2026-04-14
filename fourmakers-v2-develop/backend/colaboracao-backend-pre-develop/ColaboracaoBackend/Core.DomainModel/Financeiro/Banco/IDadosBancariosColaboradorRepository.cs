using System.Threading.Tasks;
using DataTransferObject.Domain.Financeiro.Banco.DadosBancariosColaborador;

namespace Core.Domain.Financeiro.Banco;

public interface IDadosBancariosColaboradorRepository
{
    Task<DadosBancariosColaboradorResult> BuscarDadosBancariosPorColaboradorIdAsync(string id, int orgId);
    Task<DadosBancariosColaboradorResult> CriarDadosBancariosAsync(DadosBancariosColaboradorBase input, string codigoInternoColaborador, int orgId);
    Task<DadosBancariosColaboradorResult> EditarDadosBancariosAsync(DadosBancariosColaboradorBase input, string codigoInternoColaborador, int orgId);
}