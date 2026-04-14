using Colaboracao.Core;
using Core.Domain;
using DataTransferObject.Domain.Log;
using Microsoft.EntityFrameworkCore.Storage;

namespace Colaboracao.Infra
{
    public class TransactionDisposable : ITransaction
    {
        private readonly ILogCore _log;
        private readonly IDbContextTransaction _transaction;

        public TransactionDisposable(ILogCore log, IDbContextTransaction transaction)
        {
            _log = log;
            _transaction = transaction;
        }

        public void Commit()
        {
            _log.Log("Finalizando bloco de transação.", LevelsEnum.Trace);
            _transaction.Commit();
        }

        public void Dispose()
        {
            _log.Log("Liberando transação da memória.", LevelsEnum.Trace);
            _transaction.Dispose();
        }

        public void Rollback()
        {
            _log.Log("Rollback do bloco de transação.", LevelsEnum.Trace);
            _transaction.Rollback();
        }
    }
}