using Colaboracao.Core.Interfaces;
using Core.Domain.Financeiro.IntegracaoBancaria;
using Dapper;
using DataTransferObject.Domain.Financeiro.IntegracaoBancaria.CnabOrg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataTransferObject.Domain.Financeiro.IntegracaoBancaria.RemessaBancaria;

namespace Colaboracao.Infra.Repositories.Financeiro.Financeiro.IntegracaoBancaria
{
    public class CnabOrgRepository : ICnabOrgRepository
    {
        private readonly IDBConnection _dapperConnection;

        public CnabOrgRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        private string SELECT_DEFAULT => @"
            SELECT
                id AS Id,
                tb_org_id AS TbOrgId,
                cod_diretoria AS CodDiretoria,
                codigo_banco AS CodigoBanco,
                agencia AS Agencia,
                agencia_dv AS AgenciaDv,
                conta AS Conta,
                conta_dv AS ContaDV,
                codigo_convenio AS CodigoConvenio,
                cnpj_empresa AS CnpjEmpresa,
                forma_pagamento AS FormaPagamento,
                nome_empresa AS NomeEmpresa,
                nome_banco AS NomeBanco,
                endereco_empresa AS EnderecoEmpresa,
                numero_local AS NumeroLocal,
                complemento_endereco AS ComplementoEndereco,
                cidade AS Cidade,
                cep AS Cep,
                estado AS Estado,
                data_criacao AS DataCriacao,
                data_atualizacao AS DataAtualizacao
            FROM
                tb_cnab_org";

        public async Task<IEnumerable<CnabOrgResult>> ListarCnabOrgsAsync(int orgId)
        {
            var connection = _dapperConnection.GetConnection();
            var query = SELECT_DEFAULT + " WHERE tb_org_id = @TbOrgId";

            var result = await connection.QueryAsync<CnabOrgResult>(query, new { TbOrgId = orgId });
            return result;
        }

        public async Task<CnabOrgResult> ObterCnabOrgPorIdAsync(Guid id)
        {
            var connection = _dapperConnection.GetConnection();

            var query = SELECT_DEFAULT + " WHERE id = @Id";
            return await connection.QueryFirstOrDefaultAsync<CnabOrgResult>(query, new { Id = id });
        }

        public async Task<CnabOrgResult> ObterCnabOrgPorCodigoAsync(string codigo, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = SELECT_DEFAULT + " WHERE codigo_banco = @CodigoBanco AND tb_org_id = @TbOrgId";
            return await connection.QueryFirstOrDefaultAsync<CnabOrgResult>(query, new
            {
                CodigoBanco = codigo,
                TbOrgId = orgId
            });
        }

        public async Task<CnabOrgResult> ObterCnabOrgPorChaveUnicaAsync(int orgId, string diretoria, string formaPagamento)
        {
            var connection = _dapperConnection.GetConnection();

            var query = SELECT_DEFAULT + @" WHERE tb_org_id = @TbOrgId
                AND (cod_diretoria = @CodDiretoria OR (cod_diretoria IS NULL AND @CodDiretoria IS NULL))
                AND forma_pagamento = @FormaPagamento";
            return await connection.QueryFirstOrDefaultAsync<CnabOrgResult>(query, new
            {
                TbOrgId = orgId,
                CodDiretoria = string.IsNullOrWhiteSpace(diretoria) ? null : diretoria,
                FormaPagamento = formaPagamento
            });
        }

        public async Task<CnabOrgResult> InserirCnabOrgAsync(CnabOrgInput input)
        {
            var connection = _dapperConnection.GetConnection();
            var query = @"
                        INSERT INTO tb_cnab_org
                            (id, tb_org_id, cod_diretoria, codigo_banco, agencia, agencia_dv, conta, conta_dv, codigo_convenio, cnpj_empresa, forma_pagamento, nome_empresa, nome_banco, endereco_empresa, numero_local, complemento_endereco, cidade, cep, estado)
                        VALUES
                            (@Id, @TbOrgId, @CodDiretoria, @CodigoBanco, @Agencia, @AgenciaDv, @Conta, @ContaDV, @CodigoConvenio, @CnpjEmpresa, @FormaPagamento, @NomeEmpresa, @NomeBanco, @EnderecoEmpresa, @NumeroLocal, @ComplementoEndereco, @Cidade, @Cep, @Estado)";

            var parameters = new
            {
                input.Id,
                input.TbOrgId,
                CodDiretoria = string.IsNullOrWhiteSpace(input.CodDiretoria) ? null : input.CodDiretoria,
                input.CodigoBanco,
                input.Agencia,
                input.AgenciaDv,
                input.Conta,
                input.ContaDV,
                input.CodigoConvenio,
                input.CnpjEmpresa,
                input.FormaPagamento,
                input.NomeEmpresa,
                input.NomeBanco,
                input.EnderecoEmpresa,
                input.NumeroLocal,
                ComplementoEndereco = string.IsNullOrWhiteSpace(input.ComplementoEndereco) ? null : input.ComplementoEndereco,
                input.Cidade,
                input.Cep,
                input.Estado
            };

            int rowsAffected = await connection.ExecuteAsync(query, parameters);
            if (rowsAffected > 0)
            {
                return await ObterCnabOrgPorIdAsync(input.Id);
            }

            return null;
        }

        public async Task<CnabOrgResult> AtualizarCnabOrgAsync(CnabOrgInput input)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                        UPDATE
                            tb_cnab_org
                        SET
                            tb_org_id = @TbOrgId,
                            cod_diretoria = @CodDiretoria,
                            codigo_banco = @CodigoBanco,
                            agencia = @Agencia,
                            agencia_dv = @AgenciaDv,
                            conta = @Conta,
                            conta_dv = @ContaDV,
                            codigo_convenio = @CodigoConvenio,
                            cnpj_empresa = @CnpjEmpresa,
                            forma_pagamento = @FormaPagamento,
                            nome_empresa = @NomeEmpresa,
                            nome_banco = @NomeBanco,
                            endereco_empresa = @EnderecoEmpresa,
                            numero_local = @NumeroLocal,
                            complemento_endereco = @ComplementoEndereco,
                            cidade = @Cidade,
                            cep = @Cep,
                            estado = @Estado
                        WHERE
                            id = @Id";

            var parameters = new
            {
                input.Id,
                input.TbOrgId,
                CodDiretoria = string.IsNullOrWhiteSpace(input.CodDiretoria) ? null : input.CodDiretoria,
                input.CodigoBanco,
                input.Agencia,
                input.AgenciaDv,
                input.Conta,
                input.ContaDV,
                input.CodigoConvenio,
                input.CnpjEmpresa,
                input.FormaPagamento,
                input.NomeEmpresa,
                input.NomeBanco,
                input.EnderecoEmpresa,
                input.NumeroLocal,
                ComplementoEndereco = string.IsNullOrWhiteSpace(input.ComplementoEndereco) ? null : input.ComplementoEndereco,
                input.Cidade,
                input.Cep,
                input.Estado
            };

            int rowsAffected = await connection.ExecuteAsync(query, parameters);
            if (rowsAffected > 0)
            {
                return await ObterCnabOrgPorIdAsync(input.Id);
            }

            return null;
        }

        public async Task<bool> DeletarCnabOrgAsync(Guid id)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"DELETE FROM tb_cnab_org WHERE id = @Id";

            int rowsAffected = await connection.ExecuteAsync(query, new { Id = id });
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<DiretoriaResultDTO>> ListarDiretoriasAsync(int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var query = @"
                SELECT DISTINCT
                    cod_diretoria AS CodDiretoria,
                    CASE WHEN cod_diretoria IS NULL THEN 1 ELSE 0 END AS ConfiguracaoParaTodaOrg
                FROM tb_cnab_org
                WHERE tb_org_id = @OrgId
                ORDER BY
                    CASE WHEN cod_diretoria IS NULL THEN 0 ELSE 1 END,
                    cod_diretoria";

            var result = await connection.QueryAsync<DiretoriaResultDTO>(query, new { OrgId = orgId });
            return result;
        }
    }
}