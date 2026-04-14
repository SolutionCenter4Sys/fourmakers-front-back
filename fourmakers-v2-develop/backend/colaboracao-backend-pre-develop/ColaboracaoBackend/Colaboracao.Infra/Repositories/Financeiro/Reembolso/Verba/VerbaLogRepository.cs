using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using Core.Domain.Reembolso.Verba;
using Dapper;
using DataTransferObject.Domain.Financeiro.Reembolso.Verba;

namespace Colaboracao.Infra.Repositories.Financeiro.Reembolso.Verba;

public class VerbaLogRepository(IDBConnection dapperConnection) : IVerbaLogRepository
{
    public async Task<List<VerbaLogDTO>> ListarLogsAsync(int orgId)
    {
        var connection = dapperConnection.GetConnection();
        var query = @"
            SELECT
                tvl.regra AS Regra,
                tvl.acao AS Acao,
                tvl.valor_anterior AS ValorAnterior,
                tvl.novo_valor AS NovoValor,
                tc.nome_completo AS Colaborador,
                tvl.data_criacao AS DataCriacao
            FROM tb_verba_logs tvl
            LEFT JOIN 
                tb_colaborador tc ON tc.codigo_interno_colaborador = tvl.codigo_interno_colaborador
            WHERE 
                tvl.tb_org_id = @OrgId
            ORDER BY
                tvl.data_criacao DESC;
        ";
        var parametros = new
        {
            OrgId = orgId
        };

        var result = await connection.QueryAsync<VerbaLogDTO>(query, parametros);
        return result.ToList();
    }
    
    public async Task InserirLogAsync(string regra, string acao, string valorAnterior, string novoValor, string codigoInternoColaborador, int orgId)
    {
        var connection = dapperConnection.GetConnection();
        var query = @"
            INSERT INTO tb_verba_logs(regra, acao, valor_anterior, novo_valor, codigo_interno_colaborador, tb_org_id)
            VALUES (@Regra, @Acao, @ValorAnterior, @NovoValor, @CodigoInternoColaborador, @OrgId);
        ";
        var parametros = new
        {
            Regra = regra,
            Acao = acao,
            ValorAnterior = valorAnterior,
            NovoValor = novoValor,
            CodigoInternoColaborador = codigoInternoColaborador,
            OrgId = orgId
        };

        await connection.ExecuteAsync(query, parametros);
    }
}