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
    public class GruposEmailsRepository : IGruposEmailsRepository
    {
        private readonly IDBConnection _dapperConnection;

        public GruposEmailsRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task<IEnumerable<GrupoEmailDTO>> ListarGruposEmailsAsync()
        {
            using var connection = new MySqlConnection(DBConnection.DBGColbConnectionString);

            try
            {
                await connection.OpenAsync();

                var query = @"
                    SELECT 
                        id as Id,
                        descricao as Descricao
                    FROM tb_grupos_emails_template 
                    WHERE ativo = 1
                    ORDER BY descricao";

                var gruposEmails = await connection.QueryAsync<GrupoEmailDTO>(query);
                return gruposEmails;
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

        public async Task<GrupoEmailDTO> ObterGrupoEmailPorIdAsync(Guid id)
        {
            using var connection = new MySqlConnection(DBConnection.DBGColbConnectionString);

            try
            {
                await connection.OpenAsync();

                var query = @"
                    SELECT 
                        id as Id,
                        descricao as Descricao
                    FROM tb_grupos_emails_template 
                    WHERE id = @Id AND ativo = 1";

                var grupoEmail = await connection.QueryFirstOrDefaultAsync<GrupoEmailDTO>(query, new { Id = id.ToString() });
                return grupoEmail;
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

        public async Task<bool> ExisteGrupoEmailAsync(Guid id)
        {
            using var connection = new MySqlConnection(DBConnection.DBGColbConnectionString);

            try
            {
                await connection.OpenAsync();

                var query = @"
                    SELECT COUNT(1) 
                    FROM tb_grupos_emails_template 
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
