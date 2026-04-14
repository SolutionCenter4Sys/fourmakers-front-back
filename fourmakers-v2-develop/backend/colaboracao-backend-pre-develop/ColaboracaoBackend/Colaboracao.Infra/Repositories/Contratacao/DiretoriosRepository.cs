using Colaboracao.Core.Impl;
using Colaboracao.Core.Interfaces;
using Core.Domain.Contratacao;
using Dapper;
using DataTransferObject.Domain.Contratacao;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Contratacao
{
    public class DiretoriosRepository : IDiretoriosRepository
    {
        private readonly IDBConnection _dapperConnection;

        public DiretoriosRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task<IEnumerable<DiretorioDTO>> ListarDiretoriosAsync()
        {
            using var connection = new MySqlConnection(DBConnection.DBGColbConnectionString);

            try
            {
                await connection.OpenAsync();

                var query = @"
                    SELECT 
                        id as Id,
                        descricao as Descricao
                    FROM tb_diretorios_srs_template 
                    WHERE ativo = 1
                    ORDER BY descricao";

                var diretorios = await connection.QueryAsync<DiretorioDTO>(query);
                return diretorios;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        public async Task<DiretorioDTO> ObterDiretorioPorIdAsync(Guid id)
        {
            using var connection = new MySqlConnection(DBConnection.DBGColbConnectionString);

            try
            {
                await connection.OpenAsync();

                var query = @"
                    SELECT 
                        id as Id,
                        descricao as Descricao
                    FROM tb_diretorios_srs_template 
                    WHERE id = @Id AND ativo = 1";

                var diretorio = await connection.QueryFirstOrDefaultAsync<DiretorioDTO>(query, new { Id = id.ToString() });
                return diretorio;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        public async Task<bool> ExisteDiretorioAsync(Guid id)
        {
            using var connection = new MySqlConnection(DBConnection.DBGColbConnectionString);

            try
            {
                await connection.OpenAsync();

                var query = @"
                    SELECT COUNT(1) 
                    FROM tb_diretorios_srs_template 
                    WHERE id = @Id AND ativo = 1";

                var count = await connection.QuerySingleAsync<int>(query, new { Id = id.ToString() });
                return count > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }
    }
}
