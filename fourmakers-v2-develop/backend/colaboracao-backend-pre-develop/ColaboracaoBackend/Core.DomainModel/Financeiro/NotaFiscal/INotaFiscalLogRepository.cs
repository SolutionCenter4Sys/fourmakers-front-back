using System;
using System.Threading.Tasks;
using DataTransferObject.Domain.Financeiro.NotaFiscal;

namespace Core.Domain.Financeiro.NotaFiscal;

public interface INotaFiscalLogRepository
{
    Task InserirLogAsync(NotaFiscalLogInput notaFiscalLogInput);
}