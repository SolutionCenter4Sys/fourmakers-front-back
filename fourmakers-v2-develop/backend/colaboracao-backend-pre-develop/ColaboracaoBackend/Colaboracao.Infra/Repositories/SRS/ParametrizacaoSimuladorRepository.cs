using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using Core.Domain.SRS;
using Dapper;
using DataTransferObject.Domain.SRS;

namespace Colaboracao.Infra.Repositories.SRS;

public class ParametrizacaoSimuladorRepository : IParametrizacaoSimuladorRepository
{
    private readonly IDBConnection _dapperConnection;

    public ParametrizacaoSimuladorRepository(IDBConnection dapperConnection)
    {
        _dapperConnection = dapperConnection;
    }

    public async Task<ParametrizacaoSimuladorResult?> ObterPorOrgAsync(int tbOrgId)
    {
        const string sql = @"
            SELECT
                tb_org_id AS TbOrgId,
                porcentagem_minima_piso AS PorcentagemMinimaPiso,
                porcentagem_excedente_custo AS PorcentagemExcedenteCusto,
                porcentagem_margem_custo AS PorcentagemMargemCusto,
                quantidade_maxima_calculos AS QuantidadeMaximaCalculos,
                quantidade_horas_custo AS QuantidadeHorasCusto
            FROM tb_parametrizacao_simulador
            WHERE tb_org_id = @TbOrgId
            LIMIT 1";

        var connection = _dapperConnection.GetConnection();
        return await connection.QueryFirstOrDefaultAsync<ParametrizacaoSimuladorResult>(sql, new { TbOrgId = tbOrgId });
    }
}
