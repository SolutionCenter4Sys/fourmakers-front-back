using Colaboracao.Core.Interfaces;
using Core.DomainModel;
using Dapper;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories
{
    public class TokenFileRepository : ITokenFileRepository
    {
        private readonly IDBConnection _dapperConnection;

        public TokenFileRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task<bool> ValidarTokenArquivo(string token, string nomeArquivo)
        {
            var connection = _dapperConnection.GetConnection();
            
            var query = @"
                SELECT COUNT(*) 
                FROM tb_token_file 
                WHERE token = @Token AND nome_arquivo = @NomeArquivo";
            
            var count = await connection.QueryFirstOrDefaultAsync<int>(query, new { Token = token, NomeArquivo = nomeArquivo });
            return count > 0;
        }

        public async Task InserirTokenArquivo(string token, string nomeArquivo)
        {
            var connection = _dapperConnection.GetConnection();
            
            var query = @"
                INSERT INTO tb_token_file (token, nome_arquivo) 
                VALUES (@Token, @NomeArquivo)";
            
            await connection.ExecuteAsync(query, new { Token = token, NomeArquivo = nomeArquivo });
        }

        public async Task DeletarTokenArquivo(string nomeArquivo)
        {
            var connection = _dapperConnection.GetConnection();
            
            var query = @"
                DELETE FROM tb_token_file 
                WHERE nome_arquivo = @NomeArquivo";
            
            await connection.ExecuteAsync(query, new { NomeArquivo = nomeArquivo });
        }

        public async Task AtualizarNomeArquivo(string nomeAntigo, string nomeNovo)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                UPDATE tb_token_file 
                SET nome_arquivo = @NomeNovo 
                WHERE nome_arquivo = @NomeAntigo";

            await connection.ExecuteAsync(query, new { NomeAntigo = nomeAntigo, NomeNovo = nomeNovo });
        }
    }
}