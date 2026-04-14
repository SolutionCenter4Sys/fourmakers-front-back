using Colaboracao.Core.Interfaces;
using Core.Domain.GestaoPessoa.GestaoDesempenho.Gestor;
using Dapper;
using DataTransferObject.Domain.GestaoPessoa.GestaoDesempenho.Gestor;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.GestaoPessoa.GestaoDesempenho.Gestor
{
    public class GestaoDesempenhoParametrizacaoRepository : IGestaoDesempenhoParametrizacaoRepository
    {
        private readonly IDBConnection _dapperConnection;

        public GestaoDesempenhoParametrizacaoRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task<ParametrizacaoOrgDTO> ObterParametrizacaoPorOrgAsync(int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var sql = @"
                SELECT
                    tb_org_id AS TbOrgId,
                    frequencia_esperada_one_on_one_dias AS FrequenciaEsperadaOneOnOneDias,
                    frequencia_esperada_feedback_dias AS FrequenciaEsperadaFeedbackDias
                FROM tb_gest_desemp_parametrizacao_org
                WHERE tb_org_id = @OrgId";

            var parametrizacao = await connection.QueryFirstOrDefaultAsync<ParametrizacaoOrgDTO>(
                sql,
                new { OrgId = orgId }
            );

            return parametrizacao;
        }

        public async Task<bool> UpsertParametrizacaoAsync(ParametrizacaoOrgDTO parametrizacao)
        {
            var connection = _dapperConnection.GetConnection();

            var sql = @"
                INSERT INTO tb_gest_desemp_parametrizacao_org
                    (tb_org_id, frequencia_esperada_one_on_one_dias, frequencia_esperada_feedback_dias)
                VALUES
                    (@OrgId, @FrequenciaEsperadaOneOnOneDias, @FrequenciaEsperadaFeedbackDias)
                ON DUPLICATE KEY UPDATE
                    frequencia_esperada_one_on_one_dias = @FrequenciaEsperadaOneOnOneDias,
                    frequencia_esperada_feedback_dias = @FrequenciaEsperadaFeedbackDias";

            var linhasAfetadas = await connection.ExecuteAsync(sql, parametrizacao);

            return linhasAfetadas > 0;
        }
    }
}
