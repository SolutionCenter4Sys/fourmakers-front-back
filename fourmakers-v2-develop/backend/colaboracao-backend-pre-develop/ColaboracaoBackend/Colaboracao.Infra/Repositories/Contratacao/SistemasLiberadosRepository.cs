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
    public class SistemasLiberadosRepository : ISistemasLiberadosRepository
    {
        private readonly IDBConnection _dapperConnection;

        public SistemasLiberadosRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task<IEnumerable<SistemaLiberadoDTO>> ListarSistemasLiberadosAsync()
        {
            using var connection = new MySqlConnection(DBConnection.DBGColbConnectionString);

            try
            {
                await connection.OpenAsync();

                var query = @"
                    SELECT 
                        id as Id,
                        descricao as Descricao
                    FROM tb_sistemas_liberados_srs_template 
                    WHERE ativo = 1
                    ORDER BY descricao";

                var sistemas = await connection.QueryAsync<SistemaLiberadoDTO>(query);
                return sistemas;
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

        public async Task<SistemaLiberadoDTO> ObterSistemaLiberadoPorIdAsync(Guid id)
        {
            using var connection = new MySqlConnection(DBConnection.DBGColbConnectionString);

            try
            {
                await connection.OpenAsync();

                var query = @"
                    SELECT 
                        id as Id,
                        descricao as Descricao
                    FROM tb_sistemas_liberados_srs_template 
                    WHERE id = @Id AND ativo = 1";

                var sistema = await connection.QueryFirstOrDefaultAsync<SistemaLiberadoDTO>(query, new { Id = id.ToString() });
                return sistema;
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

        public async Task<bool> ExisteSistemaLiberadoAsync(Guid id)
        {
            using var connection = new MySqlConnection(DBConnection.DBGColbConnectionString);

            try
            {
                await connection.OpenAsync();

                var query = @"
                    SELECT COUNT(1) 
                    FROM tb_sistemas_liberados_srs_template 
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
