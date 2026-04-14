using Colaboracao.Core.Interfaces;
using Core.Domain.Usuario;
using Dapper;
using System;
using System.Linq;

namespace Colaboracao.Infra.Repositories.Usuario
{
    public class TokenSistemaRepository : ITokenSistemaRepository
    {
        private readonly IConnectionStringCore _connectionString;

        public TokenSistemaRepository(IConnectionStringCore connectionString)
        {
            _connectionString = connectionString;
        }
        public bool ValidaTokenSistema(string token, int orgId)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();

                    string sql = @"SELECT 1 FROM tb_token_sistema WHERE token = @Token and tb_org_id = @OrgId;";

                    var result = _connection.Query<int>(sql, new { Token = token, OrgId = orgId });

                    return result.Any();
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    _connection.Close();
                }
            }
        }
    }
}