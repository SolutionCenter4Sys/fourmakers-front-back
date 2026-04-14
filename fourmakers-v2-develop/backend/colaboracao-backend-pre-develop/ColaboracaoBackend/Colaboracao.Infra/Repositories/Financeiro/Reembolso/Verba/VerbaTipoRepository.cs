using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using Core.Domain.Reembolso.Verba;
using Dapper;
using DataTransferObject.Domain.Financeiro.Reembolso.Verba;

namespace Colaboracao.Infra.Repositories.Financeiro.Reembolso.Verba;

public class VerbaTipoRepository(IDBConnection dapperConnection) : IVerbaTipoRepository
{
    private const string DEFAULT_SQL = @"
        SELECT
            tvt.id AS Id,
            tvt.descricao AS Descricao,
            tvt.label AS Label,
            tvt.operacao AS Operacao,
            tvt.tb_verba_tipo_custo_id AS TipoCodigo,
            tvtc.descricao_acao AS DescricaoAcao
        FROM tb_verba_tipo tvt
        INNER JOIN tb_verba_tipo_custo tvtc
            ON tvtc.id = tb_verba_tipo_custo_id
    ";
    
    public async Task<List<VerbaTipoDTO>> ListarAsync(int orgId)
    {
        var connection = dapperConnection.GetConnection();

        var query = DEFAULT_SQL;
        query += @"
            WHERE tvt.tb_org_id = @OrgId;
        ";

        var parametros = new
        {
            OrgId = orgId
        };

        var result = await connection.QueryAsync<VerbaTipoDTO>(query, parametros);
        return result.ToList();
    }

    public async Task<VerbaTipoDTO> BuscarPorIdAsync(int id)
    {
        var connection = dapperConnection.GetConnection();

        var query = DEFAULT_SQL;
        
        query += @"
            WHERE tvt.id = @Id
        ";

        var parametro = new
        {
            Id = id
        };

        var result = await connection.QueryFirstOrDefaultAsync<VerbaTipoDTO>(query, parametro);
        return result;
    }
}