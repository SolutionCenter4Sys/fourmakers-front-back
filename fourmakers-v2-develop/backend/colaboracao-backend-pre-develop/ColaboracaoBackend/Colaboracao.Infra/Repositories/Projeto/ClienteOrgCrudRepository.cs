using Colaboracao.Core.Interfaces;
using Core.DomainModel.Projeto;
using Dapper;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.Projeto.ClienteOrgCrud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Projeto
{
    public class ClienteOrgCrudRepository : IClienteOrgCrudRepository
    {
        private readonly IDBConnection _dapperConnection;

        public ClienteOrgCrudRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        private string SELECT_DEFAULT => @" SELECT
                                                id AS Id,
                                                codigo_cliente AS CodigoCliente,
                                                nome_cliente AS NomeCliente,
                                                tb_org_id AS OrgId,
                                                ativo AS Ativo,
                                                data_criacao AS DataCriacao,
                                                data_alteracao AS DataAlteracao
                                            FROM
                                                tb_cliente_org
                                            WHERE
                                                ativo = 1";

        public async Task<IEnumerable<ClienteOrgCrudResult>> ListarClienteOrgCrudsAsync(int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = SELECT_DEFAULT + " AND tb_org_id = @OrgId";

            var result = await connection.QueryAsync<ClienteOrgCrudResult>(query, new { OrgId = orgId });

            return result;
        }

        public async Task<List<string>> ListarCodigosDosProjetosAssociadosAoClienteAsync(string codCliente, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"SELECT
                            cod_projeto
                          FROM
                            tb_projeto_org
                          WHERE
                            cod_cliente = @CodCliente
                            AND tb_org_id = @OrgId";

            var result = await connection.QueryAsync<string>(query, new { CodCliente = codCliente, OrgId = orgId });

            return result.ToList();
        }

        public async Task<ClienteOrgCrudResult> ObterClienteOrgCrudPorIdAsync(Guid id, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = SELECT_DEFAULT + " AND id = @Id AND tb_org_id = @OrgId";
            return await connection.QueryFirstOrDefaultAsync<ClienteOrgCrudResult>(query, new { Id = id, OrgId = orgId });
        }

        public async Task<ClienteOrgCrudResult> ObterClienteOrgCrudPorCodigoAsync(string codigo, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = SELECT_DEFAULT + " AND codigo_cliente = @Codigo AND tb_org_id = @OrgId";
            return await connection.QueryFirstOrDefaultAsync<ClienteOrgCrudResult>(query, new { Codigo = codigo, OrgId = orgId });
        }

        public async Task<ClienteOrgCrudResult> ObterClienteOrgCrudPorNomeAsync(string nome, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = SELECT_DEFAULT + " AND nome_cliente = @Nome AND tb_org_id = @OrgId";
            return await connection.QueryFirstOrDefaultAsync<ClienteOrgCrudResult>(query, new { Nome = nome, OrgId = orgId });
        }

        public async Task<ClienteOrgCrudResult> InserirClienteOrgCrudAsync(ClienteOrgCrudInput input, TipoCadastroClienteEnum tipoCadastroEnum)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @" INSERT INTO tb_cliente_org
                               (id, codigo_cliente, nome_cliente, tb_org_id, ativo, codigo_interno_colaborador_criacao, codigo_interno_colaborador_alteracao, tipo_cadastro)
                           VALUES
                               (@Id, @CodigoCliente, @NomeCliente, @OrgId, @Ativo, @CodigoInternoColaboradorAlteracao, @CodigoInternoColaboradorAlteracao, @TipoCadastro)";

            var parameters = new
            {
                input.Id,
                input.CodigoCliente,
                input.NomeCliente,
                input.OrgId,
                input.Ativo,
                input.CodigoInternoColaboradorAlteracao,
                TipoCadastro = tipoCadastroEnum.ToString()
            };

            int rowsAffected = await connection.ExecuteAsync(query, parameters);
            if (rowsAffected > 0)
            {
                return await ObterClienteOrgCrudPorIdAsync(input.Id, input.OrgId);
            }

            return null;
        }

        public async Task<ClienteOrgCrudResult> AtualizarClienteOrgCrudAsync(ClienteOrgCrudInput input, TipoCadastroClienteEnum tipoCadastroEnum)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                            UPDATE
                                tb_cliente_org
                            SET
                                codigo_cliente = @CodigoCliente,
                                nome_cliente = @NomeCliente,
                                tb_org_id = @OrgId,
                                ativo = @Ativo,
                                codigo_interno_colaborador_alteracao = @CodigoInternoColaboradorAlteracao,
                                tipo_cadastro = @TipoCadastro
                            WHERE
                                id = @Id";

            var parameters = new
            {
                input.Id,
                input.CodigoCliente,
                input.NomeCliente,
                input.OrgId,
                input.Ativo,
                input.CodigoInternoColaboradorAlteracao,
                TipoCadastro = tipoCadastroEnum.ToString()
            };

            int rowsAffected = await connection.ExecuteAsync(query, parameters);
            if (rowsAffected > 0)
            {
                return await ObterClienteOrgCrudPorIdAsync(input.Id, input.OrgId);
            }

            return null;
        }

        public async Task<bool> DeletarClienteOrgCrudAsync(Guid id, string codigoInternoColaborador, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @" UPDATE
                               tb_cliente_org
                           SET
                               ativo = false,
                               codigo_interno_colaborador_alteracao = @CodigoInternoColaboradorAlteracao
                           WHERE
                               id = @Id
                               AND tb_org_id = @OrgId";

            int rowsAffected = await connection.ExecuteAsync(query,
                                                             new
                                                             {
                                                                 Id = id,
                                                                 CodigoInternoColaboradorAlteracao = codigoInternoColaborador,
                                                                 OrgId = orgId
                                                             }
                                                             );

            return rowsAffected > 0;
        }
    }
}