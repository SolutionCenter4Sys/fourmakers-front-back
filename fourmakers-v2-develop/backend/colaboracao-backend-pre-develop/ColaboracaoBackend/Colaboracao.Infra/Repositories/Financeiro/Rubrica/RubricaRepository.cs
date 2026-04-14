using Colaboracao.Core.Interfaces;
using Core.Domain.Financeiro.Rubrica;
using Dapper;
using DataTransferObject.Domain.Financeiro.Rubrica.Rubrica;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Financeiro.Financeiro.Rubrica
{
    public class RubricaRepository : IRubricaRepository
    {
        private readonly IDBConnection _dapperConnection;

        public RubricaRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        private string SELECT_DEFAULT => @"
            SELECT 
                tr.id AS Id,
                tr.tb_org_id AS OrgId,
                tr.descricao AS Descricao,
                tr.rubrica_tipo AS RubricaTipo,
                tr.calculo_tipo AS CalculoTipo,
                tr.codigo_rubrica AS CodigoRubrica,
                tr.refletir_contabil AS RefletirContabil,
                tr.ativo AS Ativo,
                tr.data_criacao AS DataCriacao,
                tr.data_alteracao AS DataAlteracao,
                tr.codigo_interno_colaborador_criacao AS CodigoInternoColaboradorCriacao,
                tr.codigo_interno_colaborador_alteracao AS CodigoInternoColaboradorAlteracao
            FROM 
                tb_rubrica tr
            LEFT JOIN tb_rubrica_template trt ON trt.id = tr.tb_rubrica_template_id
            WHERE 
                tr.ativo = 1";

        public async Task<IEnumerable<RubricaResult>> ListarRubricasAsync(int orgId, bool somenteComTemplates = false)
        {
            var connection = _dapperConnection.GetConnection();
            var query = SELECT_DEFAULT + " AND tr.tb_org_id = @OrgId";
            if (somenteComTemplates)
            {
                query += " AND trt.id IS NOT NULL";
            }

            var result = await connection.QueryAsync<RubricaResult>(query, new { OrgId = orgId });
            return result;
        }

        public async Task<RubricaResult> ObterRubricaPorIdAsync(Guid id)
        {
            var connection = _dapperConnection.GetConnection();

            var query = SELECT_DEFAULT + " AND tr.id = @Id";
            return await connection.QueryFirstOrDefaultAsync<RubricaResult>(query, new { Id = id });
        }

        public async Task<RubricaResult> ObterRubricaPorCodigoAsync(string codigo, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = SELECT_DEFAULT + " AND tr.codigo_rubrica = @CodigoRubrica AND tr.tb_org_id = @OrgId";
            return await connection.QueryFirstOrDefaultAsync<RubricaResult>(query, new
            {
                CodigoRubrica = codigo,
                OrgId = orgId
            });
        }

        public async Task<RubricaResult> ObterRubricaPorDescricaoAsync(string descricao, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = SELECT_DEFAULT + " AND tr.descricao = @Descricao AND tr.tb_org_id = @OrgId";
            return await connection.QueryFirstOrDefaultAsync<RubricaResult>(query, new
            {
                Descricao = descricao,
                OrgId = orgId
            });
        }

        public async Task<RubricaResult> InserirRubricaAsync(RubricaInput input)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                        INSERT INTO tb_rubrica 
                            (id, tb_org_id, descricao, rubrica_tipo, calculo_tipo, codigo_rubrica, refletir_contabil, ativo, codigo_interno_colaborador_criacao, codigo_interno_colaborador_alteracao)
                        VALUES 
                            (@Id, @OrgId, @Descricao, @RubricaTipo, @CalculoTipo, @CodigoRubrica, @RefletirContabil, @Ativo, @CodigoInternoColaboradorAlteracao, @CodigoInternoColaboradorAlteracao)";

            var parameters = new
            {
                input.Id,
                input.OrgId,
                input.Descricao,
                input.RubricaTipo,
                input.CalculoTipo,
                input.CodigoRubrica,
                input.RefletirContabil,
                input.Ativo,
                input.CodigoInternoColaboradorAlteracao
            };

            int rowsAffected = await connection.ExecuteAsync(query, parameters);
            if (rowsAffected > 0)
            {
                return await ObterRubricaPorIdAsync(input.Id);
            }

            return null;
        }

        public async Task<RubricaResult> AtualizarRubricaAsync(RubricaInput input)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                        UPDATE 
                            tb_rubrica 
                        SET 
                            descricao = @Descricao,
                            rubrica_tipo = @RubricaTipo,
                            calculo_tipo = @CalculoTipo,
                            codigo_rubrica = @CodigoRubrica,
                            refletir_contabil = @RefletirContabil,
                            codigo_interno_colaborador_alteracao = @CodigoInternoColaboradorAlteracao
                        WHERE
                            id = @Id and tb_org_id = @OrgId";

            var parameters = new
            {
                input.Id,
                input.OrgId,
                input.Descricao,
                input.CodigoRubrica,
                input.RubricaTipo,
                input.CalculoTipo,
                input.RefletirContabil,
                input.CodigoInternoColaboradorAlteracao
            };

            int rowsAffected = await connection.ExecuteAsync(query, parameters);
            if (rowsAffected > 0)
            {
                return await ObterRubricaPorIdAsync(input.Id);
            }

            return null;
        }

        public async Task<bool> DeletarRubricaAsync(Guid id)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"UPDATE 
                             tb_rubrica
                          SET
                             ativo = @Ativo
                          WHERE
                             id = @Id";

            int rowsAffected = await connection.ExecuteAsync(query, new { Id = id, Ativo = false });
            return rowsAffected > 0;
        }
    }
}