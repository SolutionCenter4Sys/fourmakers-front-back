using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Core.Domain.Reembolso.Verba;
using Dapper;
using DataTransferObject.Domain.Financeiro.Reembolso.Verba;

namespace Colaboracao.Infra.Repositories.Financeiro.Reembolso.Verba;

public class VerbaRepository(IDBConnection dapperConnection) : IVerbaRepository
{
    private const string DEFAULT_SQL = @"
            SELECT 
                tv.id AS Id,
                tv.categoria AS Categoria,
                tv.tipo_custo AS TipoCusto,
                tvt.descricao AS TipoCustoDescricao,
                tv.unidade AS Unidade,
                tv.valor AS Valor,
                tv.custo_cliente AS CustoCliente,
                tv.ativo AS Ativo,
                tvt.tb_verba_tipo_custo_id AS TipoCodigo,
                tvt.exigir_comprovante AS ExigirComprovante
            FROM tb_verba tv
            INNER JOIN tb_verba_tipo tvt ON tvt.id = tipo_custo
        ";
    
    public async Task<VerbaDTO> InserirAsync(string categoria, int tipoCusto, string unidade, decimal valor, bool custoCliente, int orgId, bool ativo, string codigoInternoColaborador)
    {
        var connection = dapperConnection.GetConnection();
        var query = @"
            INSERT INTO tb_verba(
                  categoria,
                  tipo_custo,
                  unidade,
                  valor,
                  custo_cliente,
                  tb_org_id,
                  ativo,
                  codigo_interno_colaborador_criacao,
                  codigo_interno_colaborador_alteracao    
            )
            VALUES(
                @Categoria,
                @TipoCusto,
                @Uniadde,
                @Valor,
                @CustoCliente,
                @OrgId,
                @Ativo,
                @CodigoInternoColaboradorCriacao,
                @CodigoInternoColaboradorAlteracao
            );
            SELECT LAST_INSERT_ID();
        ";

        var parametros = new
        {
            Categoria = categoria,
            TipoCusto = tipoCusto,
            Uniadde = unidade,
            Valor = valor,
            CustoCliente = custoCliente,
            OrgId = orgId,
            Ativo = ativo,
            CodigoInternoColaboradorCriacao = codigoInternoColaborador,
            CodigoInternoColaboradorAlteracao = codigoInternoColaborador
        };

        var result = await connection.ExecuteScalarAsync<int>(query, parametros);
        return await ObterPorIdAsync(result);
    }
    
    public async Task<VerbaDTO> EditarAsync(int id, string categoria, int tipoCusto, string unidade, decimal valor, bool custoCliente, bool ativo, string codigoInternoColaborador)
    {
        var connection = dapperConnection.GetConnection();
        var query = @"
            UPDATE tb_verba
                  SET categoria = @Categoria,
                  tipo_custo = @TipoCusto,
                  unidade = @Uniadde,
                  valor = @Valor,
                  custo_cliente = @CustoCliente,
                  ativo = @Ativo,
                  codigo_interno_colaborador_alteracao = @CodigoInternoColaboradorAlteracao
            WHERE id = @Id;
        ";

        var parametros = new
        {
            Categoria = categoria,
            TipoCusto = tipoCusto,
            Uniadde = unidade,
            Valor = valor,
            CustoCliente = custoCliente,
            Ativo = ativo,
            CodigoInternoColaboradorAlteracao = codigoInternoColaborador,
            Id = id
        };

        await connection.ExecuteAsync(query, parametros);
        return await ObterPorIdAsync(id);
    }

    public async Task<List<VerbaDTO>> ListarAsync(int orgId)
    {
        var connection = dapperConnection.GetConnection();
        var query = DEFAULT_SQL;
        query += @"
            WHERE 
                tv.tb_org_id = @OrgId;
        ";

        var parametros = new
        {
            OrgId = orgId
        };

        var result = await connection.QueryAsync<VerbaDTO>(query, parametros);
        return result.ToList();
    }
    
    public async Task<List<VerbaSimplificadoDTO>> ListarSimplificadoAsync(int orgId)
    {
        var connection = dapperConnection.GetConnection();
        var query = DEFAULT_SQL;
        query += @"
            WHERE 
                tv.tb_org_id = @OrgId
                AND tv.ativo = 1;
        ";

        var parametros = new
        {
            OrgId = orgId
        };

        var result = await connection.QueryAsync<VerbaSimplificadoDTO>(query, parametros);
        return result.ToList();
    }
    
    public async Task<decimal?> ObterExcecaoDaVerbaPorColaboradorProjetoECliente(int verbaId, int orgId, string codigoCliente, string codigoProjeto, string codigoColaborador)
    {
        var connection = dapperConnection.GetConnection();
        var query = @"
            SELECT 
                valor_customizado
            FROM vw_verba_personalizada_prioritaria
            WHERE tb_verba_id = @VerbaId
              AND tb_org_id = @OrgId
              AND (
                    codigo_interno_colaborador IS NULL 
                    OR codigo_interno_colaborador = @CodigoColaborador
                  )
              AND (
  	            projeto_id is null
  	            OR projeto_id = @CodigoProjeto
              )
              AND (
  	            cliente_id is null
  	            OR cliente_id = @CodigoCliente
              )
            ORDER BY 
              prioridade ASC
             LIMIT 1;
        ";

        var parametros = new
        {
            OrgId = orgId,
            VerbaId = verbaId,
            CodigoCliente = codigoCliente.ToNullSeTextoNull(),
            CodigoProjeto = codigoProjeto.ToNullSeTextoNull(),
            CodigoColaborador = codigoColaborador
        };

        var result = await connection.QueryFirstOrDefaultAsync<decimal?>(query, parametros);
        return result;
    }

    public async Task<VerbaDTO> ObterPorIdAsync(int id)
    {
        var connection = dapperConnection.GetConnection();
        var query = DEFAULT_SQL;
        query += @"
            WHERE 
                tv.id = @Id;
        ";

        var parametros = new
        {
            Id = id
        };

        var result = await connection.QueryFirstOrDefaultAsync<VerbaDTO>(query, parametros);
        return result;
    }
}