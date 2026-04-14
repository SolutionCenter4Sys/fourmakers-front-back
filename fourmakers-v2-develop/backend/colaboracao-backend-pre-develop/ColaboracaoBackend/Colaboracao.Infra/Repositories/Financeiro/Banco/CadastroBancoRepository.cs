using Colaboracao.Core.Interfaces;
using Core.Domain.Financeiro.Banco;
using Dapper;
using DataTransferObject.Domain.Financeiro.Banco.CadastroBanco;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Financeiro.Financeiro.Banco
{
    public class CadastroBancoRepository : ICadastroBancoRepository
    {
        private readonly IDBConnection _dapperConnection;

        public CadastroBancoRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        private string SELECT_DEFAULT => @"
            SELECT 
                nome AS Nome,
                descricao AS Descricao,
                codigo_banco AS CodigoBanco,
                ativo AS Ativo
            FROM 
                tb_banco
            WHERE 
                ativo = 1";

        public async Task<IEnumerable<CadastroBancoResult>> ListarCadastroBancosAsync()
        {
            var connection = _dapperConnection.GetConnection();
            var query = SELECT_DEFAULT;

            var result = await connection.QueryAsync<CadastroBancoResult>(query);
            return result;
        }

        public async Task<CadastroBancoResult> ObterCadastroBancoPorCodigoAsync(string codigo)
        {
            var connection = _dapperConnection.GetConnection();

            var query = SELECT_DEFAULT + " AND codigo_banco = @CodigoBanco";
            var result = await connection.QueryFirstOrDefaultAsync<CadastroBancoResult>(query, new
            {
                CodigoBanco = codigo
            });

            return result;
        }

        public async Task<CadastroBancoResult> InserirCadastroBancoAsync(CadastroBancoInput input)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                        INSERT INTO tb_banco 
                            (nome, descricao, codigo_banco, ativo)
                        VALUES 
                            (@Nome, @Descricao, @CodigoBanco, @Ativo)";

            var parameters = new
            {
                input.Nome,
                input.Descricao,
                input.CodigoBanco,
                input.Ativo
            };

            int rowsAffected = await connection.ExecuteAsync(query, parameters);

            if (rowsAffected > 0)
            {
                var result = await ObterCadastroBancoPorCodigoAsync(input.CodigoBanco);
                return result;
            }

            return null;
        }

        public async Task<CadastroBancoResult> AtualizarCadastroBancoAsync(CadastroBancoInput input)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                        UPDATE
                            tb_banco
                        SET
                            nome = @Nome,
                            descricao = @Descricao
                        WHERE
                            codigo_banco = @CodigoBanco";

            var parameters = new
            {
                input.Nome,
                input.Descricao,
                input.CodigoBanco
            };

            int rowsAffected = await connection.ExecuteAsync(query, parameters);
            if (rowsAffected > 0)
            {
                return await ObterCadastroBancoPorCodigoAsync(input.CodigoBanco);
            }

            return null;
        }

        public async Task<bool> DeletarCadastroBancoAsync(string codigoBanco)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"UPDATE 
                             tb_banco
                          SET
                             ativo = @Ativo
                          WHERE
                             codigo_banco = @CodigoBanco";

            int rowsAffected = await connection.ExecuteAsync(query, new { CodigoBanco = codigoBanco, Ativo = false });
            return rowsAffected > 0;
        }
    }
}