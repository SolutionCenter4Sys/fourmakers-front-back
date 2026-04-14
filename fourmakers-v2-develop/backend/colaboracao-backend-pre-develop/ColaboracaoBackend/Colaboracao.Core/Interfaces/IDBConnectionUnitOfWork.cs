namespace Colaboracao.Core.Interfaces
{
    public interface IDBConnectionUnitOfWork
    {
        void BeginTransaction();
        void Commit();
        void Rollback();
        void SafeRollback();
    }
}