using System.Data;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using Core.Domain.Fourmakers;
using Dapper;

namespace Colaboracao.Infra.Repositories.Fourmakers;

public class FourmakersRepository(IDBConnection dapperConnection) : IFourmakersRepository
{
    private readonly IDbConnection _connection = dapperConnection.GetConnection();

    public async Task InserirAvaliacaoAsync(int orgId, string codigoInternoColaborador, int servicoRate, int recomendacaoRate, string? experienciaDescricao, string aspectoDescricao)
    {
        var query = @"
            INSERT INTO tb_fourmakers_recomendacao_avaliacao
            (tb_org_id, codigo_interno_colaborador, servico_rate, recomendacao_rate, experiencia_descricao, aspecto_descricao)
            VALUES
            (@OrgId, @CodigoInternoColaborador, @ServicoRate, @RecomendacaoRate, @ExperienciaDescricao, @AspectoDescricao)
        ";
        
        var parametros = new
        {
            OrgId = orgId,
            CodigoInternoColaborador = codigoInternoColaborador,
            ServicoRate = servicoRate,
            RecomendacaoRate = recomendacaoRate,
            ExperienciaDescricao = experienciaDescricao,
            AspectoDescricao = aspectoDescricao
        };
        
        await _connection.ExecuteAsync(query, parametros);
    }
}