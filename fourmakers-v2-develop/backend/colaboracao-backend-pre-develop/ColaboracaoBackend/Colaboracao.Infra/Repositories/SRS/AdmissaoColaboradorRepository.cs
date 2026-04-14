using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using Core.Domain.SRS;
using Dapper;

namespace Colaboracao.Infra.Repositories.SRS;

public class AdmissaoColaboradorRepository : IAdmissaoColaboradorRepository
{
    private readonly IDBConnection _dapperConnection;

    public AdmissaoColaboradorRepository(IDBConnection dapperConnection)
    {
        _dapperConnection = dapperConnection;
    }

    public async Task<bool> ExisteAsync(string codigoInternoColaborador)
    {
        var connection = _dapperConnection.GetConnection();
        const string sql = @"
            SELECT EXISTS (
                SELECT 1
                FROM tb_colaborador
                WHERE codigo_interno_colaborador = @Codigo
                  AND ativo = 1
            )";
        return await connection.ExecuteScalarAsync<bool>(sql, new { Codigo = codigoInternoColaborador });
    }
}
