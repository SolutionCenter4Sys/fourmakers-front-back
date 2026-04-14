using Colaboracao.Core.Interfaces;
using Core.Domain;
using Dapper;
using DataTransferObject.Domain;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra
{
    public class AcessoUsuarioRepository : IAcessoUsuarioRepository
    {
        private readonly IConnectionStringCore _connectionString;
        private readonly IDBConnection _dapperConnection;
        public AcessoUsuarioRepository(IConnectionStringCore connectionString, IDBConnection dapperConnection)
        {
            _connectionString = connectionString;
            _dapperConnection = dapperConnection;
        }

        public async Task CriaTokenAcesso(string token, string codInternoColab, DateTime validade, TipoTokenAcessoEnum tipoToken, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            string query = @"
                INSERT INTO tb_token_validacao (id, codigo_interno_colaborador, token, data_criacao, validade, tipo_validacao, confirmado, tb_org_id)
                VALUES (@Id, @CodInternoColab, @Token, @DataCriacao, @Validade, @TipoValidacao, 0, @OrgId);";

            var parametros = new
            {
                Id = Guid.NewGuid().ToString(),
                CodInternoColab = codInternoColab,
                Token = token,
                DataCriacao = DateTime.UtcNow,
                Validade = validade,
                TipoValidacao = tipoToken.ToString(),
                OrgId = orgId
            };
            await connection.ExecuteAsync(query, parametros);
        }

        public void FechaTokens(string codInternoColab)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();

                    string sql = @"UPDATE tb_token_validacao set  confirmado = 1 where codigo_interno_colaborador = @CodInternoColab;";

                    var result = _connection.Execute(sql, new
                    {
                        CodInternoColab = codInternoColab
                    });
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
        public TokenValidacaoAcessoDTO GetToken(string codInternoColab, TipoTokenAcessoEnum tipoToken, int orgId)
        {
            using (var _connection = _connectionString.CreateMySqlConnection())
            {
                try
                {
                    _connection.Open();

                    string sql = @"SELECT * FROM tb_token_validacao WHERE codigo_interno_colaborador = @CodInternoColab and confirmado = 0 and tipo_validacao = @TipoValidacao and tb_org_id = @OrgId";

                    var result = _connection.Query<dynamic>(sql, new
                    {
                        CodInternoColab = codInternoColab,
                        TipoValidacao = tipoToken.ToString(),
                        OrgId = orgId
                    }).ToList().Select(x => new TokenValidacaoAcessoDTO
                    {
                        Token = x.token,
                        Validade = x.validade,
                        Tipo = tipoToken
                    });

                    return result.LastOrDefault();
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