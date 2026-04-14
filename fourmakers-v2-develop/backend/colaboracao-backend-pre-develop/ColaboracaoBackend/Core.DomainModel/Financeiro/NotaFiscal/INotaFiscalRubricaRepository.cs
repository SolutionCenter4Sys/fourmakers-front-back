using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.Financeiro.NF.NotaFiscalRubrica;

namespace Core.Domain.Financeiro.NotaFiscal;

public interface INotaFiscalRubricaRepository
{
    Task InserirNotaFiscalRubricaAsync(Guid notaFiscalId, string rubricaColaboradorId, decimal valor, int orgId);
    Task<IEnumerable<NotaFiscalRubricaResult>> ListarRubricasPorNotaFiscalId(Guid notaFiscalId);
    Task<IEnumerable<NotaFiscalRubricaResult>> ListarRubricasPorListaDeNotaFiscalId(List<Guid> notasFiscaisIds);
    Task AtualizarNotaFiscalRubricaAsync(string rubricaColaboradorId, decimal valor);
    Task DeletarNotaFiscalRubricaAsync(string rubricaColaboradorId);
}