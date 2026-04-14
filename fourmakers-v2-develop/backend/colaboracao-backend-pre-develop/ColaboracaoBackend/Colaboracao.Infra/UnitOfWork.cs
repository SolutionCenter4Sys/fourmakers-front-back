using Colaboracao.Core;
using Colaboracao.Infra.Context;
using Core.Domain;
using DataTransferObject.Domain.Log;
using System;

namespace Colaboracao.Infra
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ColaboradorContext _colaboradorContext;
        private readonly ILogCore _log;

        public UnitOfWork(ILogCore log, ColaboradorContext colaboradorContext)
        {
            this._colaboradorContext = colaboradorContext;
            _log = log;
        }

        public ITransaction BeginTransaction()
        {
            try
            {
                _log.Log("Iniciando bloco de transação.", LevelsEnum.Trace);
                return new TransactionDisposable(_log, _colaboradorContext.Database.BeginTransaction());
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}