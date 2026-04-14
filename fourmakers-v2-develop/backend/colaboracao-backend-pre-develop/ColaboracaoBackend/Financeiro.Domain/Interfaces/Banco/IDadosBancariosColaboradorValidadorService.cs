using Colaboracao.Helper.Enum;
using DataTransferObject.Domain.Financeiro.Banco.DadosBancariosColaborador;

namespace Financeiro.Domain.Interfaces.Banco;

public interface IDadosBancariosColaboradorValidadorService
{
     Task<List<string>> ValidarEntrada(DadosBancariosColaboradorBase input, CRUDEnum crudEnum);
}