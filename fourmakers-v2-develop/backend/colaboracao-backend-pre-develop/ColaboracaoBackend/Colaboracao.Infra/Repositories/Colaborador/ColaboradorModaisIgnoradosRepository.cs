using Colaboracao.Core.Impl;
using Core.Domain.Colaborador;
using Dapper;
using DataTransferObject.Domain.Colaborador;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Colaborador
{
    public class ColaboradorModaisIgnoradosRepository : IColaboradorModaisIgnoradosRepository
    {
        public async Task<IEnumerable<ColaboradorModaisIgnoradosDTO>> ListarPorColaboradorOrgAsync(string codigoInternoColaborador, int tbOrgId)
        {
            using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT id AS Id, codigo_interno_colaborador AS CodigoInternoColaborador, tb_org_id AS TbOrgId, tag AS Tag, data_criacao AS DataCriacao
                FROM tb_colaborador_modais_ignorados
                WHERE codigo_interno_colaborador = @CodigoInternoColaborador AND tb_org_id = @TbOrgId
                ORDER BY data_criacao DESC";

            return await connection.QueryAsync<ColaboradorModaisIgnoradosDTO>(query, new { CodigoInternoColaborador = codigoInternoColaborador, TbOrgId = tbOrgId });
        }

        public async Task<bool> InserirAsync(string codigoInternoColaborador, int tbOrgId, string tag)
        {
            using var connection = new MySql.Data.MySqlClient.MySqlConnection(DBConnection.DBGColbConnectionString);
            await connection.OpenAsync();

            var sql = @"
                INSERT IGNORE INTO tb_colaborador_modais_ignorados (codigo_interno_colaborador, tb_org_id, tag)
                VALUES (@CodigoInternoColaborador, @TbOrgId, @Tag)";

            try
            {
                var rows = await connection.ExecuteAsync(sql, new { CodigoInternoColaborador = codigoInternoColaborador, TbOrgId = tbOrgId, Tag = tag });
                return rows > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
