using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using Core.Domain.Usuario.Restricao;
using Dapper;

namespace Colaboracao.Infra.Repositories.Usuario.Restricao;

public class RestricaoDeAcessoRepository(IDBConnection dapperConnection) : IRestricaoDeAcessoRepository
{
    private readonly IDbConnection _connection = dapperConnection.GetConnection();
    public async Task<List<string>> ListarRestricoesPorCodigoInternoETipo(string codigoInternoColaborador, int? orgId, string restricaoTipo)
    {
        var query = @"
            SELECT 
                valor
            FROM tb_restricao_acesso_colaborador 
            WHERE codigo_interno_colaborador = @CodigoInternoColaborador
            AND restricao_tipo = @RestricaoTipo
            AND (@OrgId IS NULL OR tb_org_id = @OrgId);
        ";

        var parametros = new
        {
            CodigoInternoColaborador = codigoInternoColaborador,
            OrgId = orgId,
            RestricaoTipo = restricaoTipo
        };
        
        var result = await _connection.QueryAsync<string>(query, parametros);
        return result.ToList();
    }
}