using ApiClient.Domain;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace Colaboracao.Core.Impl
{
    public sealed class DBConnection : IDBConnection, IDisposable
    {
        private MySqlConnection _connection;
        private IDbTransaction _transaction { get; set; }

        public DBConnection()
        {
            _connection = new MySqlConnection(DBGColbConnectionString);
            _connection.Open();
        }

        public MySqlConnection GetConnection()
        {
            return _connection;
        }

        public void NewConnection()
        {
            _connection = new MySqlConnection(DBGColbConnectionString);
            _connection.Open();
        }

        public IDbTransaction GetTransaction()
        {
            return _transaction;
        }

        public void SetTransaction(IDbTransaction transaction)
        {
            _transaction = transaction;
        }

        public void Dispose()
        {
            _connection?.Dispose();
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