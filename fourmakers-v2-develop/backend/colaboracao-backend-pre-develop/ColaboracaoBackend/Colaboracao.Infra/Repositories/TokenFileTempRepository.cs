using Colaboracao.Core.Interfaces;
using Core.DomainModel;
using Dapper;
using DataTransferObject.Domain.Arquivo.TokenFileTemp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories
{
    public class TokenFileTempRepository : ITokenFileTempRepository
    {
        private readonly IDBConnection _dapperConnection;

        public TokenFileTempRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task<IEnumerable<TokenFileTempDTO>> ListarAsync()
        {
            var connection = _dapperConnection.GetConnection();
            const string query = @"
                SELECT CONCAT(t.token, '') AS Token, t.nome_arquivo AS NomeArquivo, t.data_criacao AS DataCriacao
                FROM tb_token_files_temp t
                ORDER BY t.data_criacao DESC";
            return await connection.QueryAsync<TokenFileTempDTO>(query);
        }

        public async Task<IReadOnlyList<TokenFileTempDTO>> ListarCriadosAntesDeAsync(DateTime limiteUtc)
        {
            var connection = _dapperConnection.GetConnection();
            const string query = @"
                SELECT CONCAT(t.token, '') AS Token, t.nome_arquivo AS NomeArquivo, t.data_criacao AS DataCriacao
                FROM tb_token_files_temp t
                WHERE t.data_criacao < @Limite
                ORDER BY t.data_criacao ASC";
            var rows = await connection.QueryAsync<TokenFileTempDTO>(query, new { Limite = limiteUtc });
            return rows.ToList();
        }

        public async Task<TokenFileTempDTO> ObterPorTokenAsync(string token)
        {
            var connection = _dapperConnection.GetConnection();
            const string query = @"
                SELECT CONCAT(t.token, '') AS Token, t.nome_arquivo AS NomeArquivo, t.data_criacao AS DataCriacao
                FROM tb_token_files_temp t
                WHERE t.token = @Token";
            return await connection.QueryFirstOrDefaultAsync<TokenFileTempDTO>(query, new { Token = token });
        }

        public async Task InserirAsync(string token, string nomeArquivo)
        {
            var connection = _dapperConnection.GetConnection();
            const string sql = @"
                INSERT INTO tb_token_files_temp (token, nome_arquivo)
                VALUES (@Token, @NomeArquivo)";
            await connection.ExecuteAsync(sql, new { Token = token, NomeArquivo = nomeArquivo });
        }

        public async Task<int> AtualizarAsync(string token, string nomeArquivo)
        {
            var connection = _dapperConnection.GetConnection();
            const string sql = @"
                UPDATE tb_token_files_temp
                SET nome_arquivo = @NomeArquivo
                WHERE token = @Token";
            return await connection.ExecuteAsync(sql, new { Token = token, NomeArquivo = nomeArquivo });
        }

        public async Task<int> ExcluirAsync(string token)
        {
            var connection = _dapperConnection.GetConnection();
            const string sql = @"DELETE FROM tb_token_files_temp WHERE token = @Token";
            return await connection.ExecuteAsync(sql, new { Token = token });
        }

        public async Task<int> ExcluirPorTokensAsync(IEnumerable<string> tokens)
        {
            var list = tokens?
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Distinct()
                .ToList() ?? new List<string>();
            if (list.Count == 0)
                return 0;

            var connection = _dapperConnection.GetConnection();
            const string sql = @"DELETE FROM tb_token_files_temp WHERE token IN @Tokens";
            return await connection.ExecuteAsync(sql, new { Tokens = list });
        }
    }
}
