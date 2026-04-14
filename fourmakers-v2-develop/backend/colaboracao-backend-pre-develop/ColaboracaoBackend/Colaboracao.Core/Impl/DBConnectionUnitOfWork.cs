using Colaboracao.Core.Interfaces;
using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace Colaboracao.Core.Impl
{
    public sealed class DBConnectionUnitOfWork : IDBConnectionUnitOfWork
    {
        private readonly IDBConnection _dbConnection;

        public DBConnectionUnitOfWork(IDBConnection dapperConnection)
        {
            _dbConnection = dapperConnection;
        }

        public void BeginTransaction()
        {
            if (_dbConnection.GetTransaction() != null)
            {
                throw new InvalidOperationException("Uma transação já foi iniciada. Não é possível iniciar uma nova transação sem finalizar a anterior (commit ou rollback).");
            }

            _dbConnection.SetTransaction(_dbConnection.GetConnection().BeginTransaction());
        }

        public void Commit()
        {
            var transaction = _dbConnection.GetTransaction();

            ValidateTransaction(transaction, _dbConnection.GetConnection());

            try
            {
                transaction.Commit();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao tentar realizar o commit da transação.", ex);
            }
            finally
            {
                Dispose();
            }
        }

        public void Rollback()
        {
            var transaction = _dbConnection.GetTransaction();

            ValidateTransaction(transaction, _dbConnection.GetConnection());

            try
            {
                transaction.Rollback();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao tentar realizar o rollback da transação.", ex);
            }
            finally
            {
                Dispose();
            }
        }

        public void SafeRollback()
        {
            var transaction = _dbConnection.GetTransaction();

            if (transaction is null)
            {
                return;
            }

            Rollback();
        }

        private static void ValidateTransaction(IDbTransaction transaction, MySqlConnection connection)
        {
            if (transaction is null)
            {
                throw new InvalidOperationException("Não é possível realizar o comando. A transação não foi iniciada. Utilize o método 'BeginTransaction' para iniciar uma transação.");
            }

            if (connection is not null && connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Falha ao tentar confirmar a transação. A \"connection\" foi fechada inesperadamente, possivelmente devido 'using', 'Dispose' ou 'Close' em um momento anterior. Verifique o ciclo de vida da conexão e garanta que ela esteja aberta durante a operação.");
            }
        }

        public void Dispose()
        {
            _dbConnection.GetTransaction()?.Dispose();
            _dbConnection.SetTransaction(null); // Limpa a transação registrada
        }
    }
}