using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using Core.Domain.Reembolso.Solicitacao;
using Dapper;

namespace Colaboracao.Infra.Repositories.Financeiro.Reembolso.Solicitacao;

public class SolicitacaoReembolsoDocumentoRepository(IDBConnection dapperConnection) : ISolicitacaoReembolsoDocumentoRepository
{
    public async Task InserirAsync(string url, string tipo, string relativePath, int reembolsoId)
    {
        var connection = dapperConnection.GetConnection();
        var query = @"
            INSERT INTO tb_solicitacao_documento
            (
                 tipo,
                 url,
                 relative_path,
                 tb_solicitacao_reembolso_id
            )
            VALUES 
            (
                @Tipo,
                @Url,
                @RelativePath,
                @ReembolsoId
            )
        ";

        var parametros = new
        {
            Tipo = tipo,
            Url = url,
            relativePath,
            ReembolsoId = reembolsoId
        };
        
        await connection.ExecuteAsync(query, parametros);
    }
}