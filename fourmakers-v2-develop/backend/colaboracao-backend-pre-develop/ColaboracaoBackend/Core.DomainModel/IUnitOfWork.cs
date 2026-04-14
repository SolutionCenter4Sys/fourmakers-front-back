using System;

namespace Core.Domain
{
    public interface IUnitOfWork
    {
        ITransaction BeginTransaction();
    }
    public interface ITransaction : IDisposable
    {
        void Commit();
        void Rollback();
    }
}