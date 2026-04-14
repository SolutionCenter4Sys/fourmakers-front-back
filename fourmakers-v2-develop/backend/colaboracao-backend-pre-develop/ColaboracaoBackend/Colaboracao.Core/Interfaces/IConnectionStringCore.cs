using MySql.Data.MySqlClient;

namespace Colaboracao.Core.Interfaces
{
    public interface IConnectionStringCore
    {
        MySqlConnection CreateMySqlConnection();
    }
}