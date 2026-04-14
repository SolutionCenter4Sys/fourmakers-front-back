using Colaboracao.Core.Interfaces;
using Core.Domain.Colaborador.Colaborador;
using Dapper;
using DataTransferObject.Domain.Colaborador.DepartamentoOrg;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Colaborador.Colaborador.Colaborador
{
    public class DepartamentoOrgRepository : IDepartamentoOrgRepository
    {
        private readonly IDBConnection _dapperConnection;

        public DepartamentoOrgRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        private string SELECT_DEFAULT => @"
            SELECT
                id AS Id,
                tb_org_id AS OrgId,
                ativo AS Ativo,
                codigo_interno_colaborador_criacao AS CodigoInternoColaboradorCriacao,
                departamento AS Departamento,
                cod_departamento AS CodDepartamento,
                data_criacao AS DataCriacao
            FROM
                tb_departamento_org
            WHERE
                ativo = 1";

        public async Task<IEnumerable<DepartamentoOrgResult>> ListarDepartamentoOrgsAsync(int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            var query = SELECT_DEFAULT + " AND tb_org_id = @OrgId";

            var result = await connection.QueryAsync<DepartamentoOrgResult>(query, new { OrgId = orgId });
            return result;
        }

        public async Task<DepartamentoOrgResult> ObterDepartamentoOrgPorIdAsync(Guid id)
        {
            var connection = _dapperConnection.GetConnection();
            var query = SELECT_DEFAULT + " AND id = @Id";
            return await connection.QueryFirstOrDefaultAsync<DepartamentoOrgResult>(query, new { Id = id });
        }

        public async Task<DepartamentoOrgResult> ObterDepartamentoOrgPorCodigoAsync(string codigo, int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            var query = SELECT_DEFAULT + " AND cod_departamento = @Codigo AND tb_org_id = @OrgId";
            return await connection.QueryFirstOrDefaultAsync<DepartamentoOrgResult>(query, new
            {
                Codigo = codigo,
                OrgId = orgId
            });
        }

        public async Task<DepartamentoOrgResult> InserirDepartamentoOrgAsync(DepartamentoOrgInput input)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                        INSERT INTO tb_departamento_org
                            (id, tb_org_id, ativo, codigo_interno_colaborador_criacao, departamento, cod_departamento)
                        VALUES
                            (@Id, @OrgId, @Ativo, @CodigoInternoColaboradorAlteracao, @Departamento, @CodDepartamento)";

            var parameters = new
            {
                input.Id,
                input.OrgId,
                input.Ativo,
                CodigoInternoColaboradorAlteracao = input.CodigoInternoColaboradorAlteracao,
                input.Departamento,
                input.CodDepartamento
            };

            int rowsAffected = await connection.ExecuteAsync(query, parameters);
            if (rowsAffected > 0)
            {
                return await ObterDepartamentoOrgPorIdAsync(input.Id);
            }

            return null;
        }

        public async Task<DepartamentoOrgResult> AtualizarDepartamentoOrgAsync(DepartamentoOrgInput input)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                        UPDATE
                            tb_departamento_org
                        SET
                            departamento = @Departamento,
                            codigo_interno_colaborador_alteracao = @CodigoInternoColaboradorAlteracao
                        WHERE
                            id = @Id AND tb_org_id = @OrgId";

            var parameters = new
            {
                input.Id,
                input.OrgId,
                input.Departamento,
                CodigoInternoColaboradorAlteracao = input.CodigoInternoColaboradorAlteracao
            };

            int rowsAffected = await connection.ExecuteAsync(query, parameters);
            if (rowsAffected > 0)
            {
                return await ObterDepartamentoOrgPorIdAsync(input.Id);
            }

            return null;
        }

        public async Task<bool> DeletarDepartamentoOrgAsync(Guid id)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"UPDATE
                             tb_departamento_org
                          SET
                             ativo = @Ativo
                          WHERE
                             id = @Id";

            int rowsAffected = await connection.ExecuteAsync(query, new { Id = id, Ativo = false });
            return rowsAffected > 0;
        }
    }
}