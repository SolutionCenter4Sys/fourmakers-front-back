using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using Core.Domain.Reembolso.Solicitacao;
using Dapper;
using DataTransferObject.Domain.Financeiro.Reembolso.Solicitacao;

namespace Colaboracao.Infra.Repositories.Financeiro.Reembolso.Solicitacao;

public class SolicitacaoStatusRepository(IDBConnection dapperConnection) : ISolicitacaoStatusRepository
{
    public async Task<List<SolicitacaoStatusDTO>> ListarAsync()
    {
        var connection = dapperConnection.GetConnection();
        var query = @"
            SELECT
                tss.id AS Id,
                tss.descricao AS Descricao
            FROM tb_solicitacao_status tss
        ";

        var result = await connection.QueryAsync<SolicitacaoStatusDTO>(query);
        return result.ToList();
    }
}