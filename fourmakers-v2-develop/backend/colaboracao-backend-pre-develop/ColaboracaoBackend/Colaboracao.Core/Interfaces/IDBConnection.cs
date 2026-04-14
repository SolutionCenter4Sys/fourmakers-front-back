using MySql.Data.MySqlClient;
using System.Data;

namespace Colaboracao.Core.Interfaces
{
    public interface IDBConnection
    {
        MySqlConnection GetConnection();
        IDbTransaction GetTransaction();
        void SetTransaction(IDbTransaction _transaction);

        void NewConnection();
    }
}