using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using Core.Domain.Financeiro.NotaFiscal;
using Dapper;
using DataTransferObject.Domain.Financeiro.NF.NotaFiscalRubrica;

namespace Colaboracao.Infra.Repositories.Financeiro.NotaFiscal;

public class NotaFiscalRubricaRepository(IDBConnection dapperConnection) : INotaFiscalRubricaRepository
{
    private const string DEFAULT_SQL = @"
            SELECT
                tnfr.id AS Id,
                tnfr.tb_nota_fiscal_id AS NotaFiscalId,
                tnfr.tb_rubrica_colaborador_id AS RubricaColaboradorId,
                tnfr.valor AS Valor,
                tr.descricao AS RubricaDescricao,
                tr.calculo_tipo AS Tipo,
                tr.codigo_rubrica as CodigoRubrica,
                 CASE 
                            WHEN tr.rubrica_tipo = 'Provento' THEN 'Crédito'
                            WHEN tr.rubrica_tipo = 'Desconto' THEN 'Débito'
                            ELSE ''
                        END AS Natureza
            FROM tb_nota_fiscal_rubrica tnfr
            INNER JOIN tb_rubrica_colaborador trc ON trc.id = tnfr.tb_rubrica_colaborador_id
            INNER JOIN tb_rubrica tr ON tr.id = trc.tb_rubrica_id
    ";

    public async Task InserirNotaFiscalRubricaAsync(Guid notaFiscalId, string rubricaColaboradorId, decimal valor, int orgId)
    {
        var connection = dapperConnection.GetConnection();
        
        var query = @"
            INSERT INTO tb_nota_fiscal_rubrica
            (
                id,
                tb_nota_fiscal_id,
                tb_rubrica_colaborador_id,
                valor,
                tb_org_id
            )
            VALUES
            (
                @Id,
                @NotaFiscalId,
                @RubricaColaboradorId,
                @Valor,
                @OrgId
            )
        ";
        
        var uuid = Guid.NewGuid();
        var parameters = new
        {
            Id =  uuid,
            NotaFiscalId = notaFiscalId,
            RubricaColaboradorId = rubricaColaboradorId,
            Valor = valor,
            OrgId = orgId
        };
        
        await connection.ExecuteAsync(query, parameters);
    }
    
    public async Task AtualizarNotaFiscalRubricaAsync(string rubricaColaboradorId, decimal valor)
    {
        var connection = dapperConnection.GetConnection();
        
        var query = @"
            UPDATE tb_nota_fiscal_rubrica
                SET valor = @Valor
            WHERE 
                tb_rubrica_colaborador_id = @RubricaColaboradorId
        ";
        
        var parameters = new
        {
            RubricaColaboradorId = rubricaColaboradorId,
            Valor = valor
        };
        
        await connection.ExecuteAsync(query, parameters);
    }

    public async Task DeletarNotaFiscalRubricaAsync(string rubricaColaboradorId)
    {
        var connection = dapperConnection.GetConnection();
        
        var query = @"
            DELETE FROM tb_nota_fiscal_rubrica
            WHERE tb_rubrica_colaborador_id = @RubricaColaboradorId;
        ";

        var parameters = new
        {
            RubricaColaboradorId = rubricaColaboradorId
        };

        await connection.ExecuteAsync(query, parameters);
    }

    public async Task<IEnumerable<NotaFiscalRubricaResult>> ListarRubricasPorNotaFiscalId(Guid notaFiscalId)
    {
        var connection = dapperConnection.GetConnection();
        
        var query = DEFAULT_SQL;
        query += @"
            WHERE tnfr.tb_nota_fiscal_id = @Id
        ";
        
        var parameters = new
        {
            Id =  notaFiscalId,
        };
        
        return await connection.QueryAsync<NotaFiscalRubricaResult>(query, parameters);
    }

    public async Task<IEnumerable<NotaFiscalRubricaResult>> ListarRubricasPorListaDeNotaFiscalId(List<Guid> notasFiscaisIds)
    {
        var connection = dapperConnection.GetConnection();
        
        var query = DEFAULT_SQL;
        query += @"
            WHERE tnfr.tb_nota_fiscal_id IN @Ids
        ";
        
        var parameters = new
        {
            Ids =  notasFiscaisIds
        };
        
        return await connection.QueryAsync<NotaFiscalRubricaResult>(query, parameters);
    }
}