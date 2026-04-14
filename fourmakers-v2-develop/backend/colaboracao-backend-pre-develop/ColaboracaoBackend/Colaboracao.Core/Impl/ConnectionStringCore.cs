using ApiClient.Domain;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using MySql.Data.MySqlClient;

namespace Colaboracao.Core.Impl
{
    public class ConnectionStringCore : IConnectionStringCore
    {
        private readonly string _connectionString;
        public ConnectionStringCore()
        {
            this._connectionString = DBGColbConnectionString;
        }

        public MySqlConnection CreateMySqlConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        public static string DBGColbConnectionString => string.Format(
              @"server={0};database={1};uid={2};pwd={3};ConvertZeroDateTime=True",
              VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.DB_HOSTNAME),
              VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.GCOLB_DATABASE),
              VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.DB_USERNAME),
              VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.DB_PASSWORD)
            );
    }
}