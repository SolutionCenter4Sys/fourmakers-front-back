using Colaboracao.Core.Interfaces;
using Core.Domain.Social.AtendimentoFourmakers;
using Dapper;
using DataTransferObject.Domain.Social.AtendimentoFourmakers;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Social.AtendimentoFourmakers
{
    public class AssistenteConfigRepository : IAssistenteConfigRepository
    {
        private readonly IDBConnection _dapperConnection;

        public AssistenteConfigRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task<AssistenteConfigResult> ObterAsync(int orgId)
        {
            const string sql = @"
                SELECT
                    nome_assistente AS NomeAssistente,
                    mensagem_boas_vindas AS MensagemBoasVindas,
                    acoes_rapidas AS AcoesRapidasJson,
                    limiar_similaridade_chamado AS LimiarSimilaridadeChamadoDb,
                    rag_top_k AS RagTopK,
                    rag_similaridade_minima AS RagSimilaridadeMinima,
                    instrucao_sistema_extra AS InstrucaoSistemaExtra,
                    data_alteracao AS DataAlteracao
                FROM tb_assistente_config
                WHERE tb_org_id = @OrgId
                LIMIT 1";

            var connection = _dapperConnection.GetConnection();
            var row = await connection.QueryFirstOrDefaultAsync<dynamic>(sql, new { OrgId = orgId });

            if (row == null) return null;

            var result = new AssistenteConfigResult
            {
                NomeAssistente = row.NomeAssistente,
                MensagemBoasVindas = row.MensagemBoasVindas,
                LimiarSimilaridadeChamadoPercent = (int)System.Math.Round((double)(row.LimiarSimilaridadeChamadoDb ?? 0.35) * 100),
                RagTopK = (int?)row.RagTopK,
                RagSimilaridadeMinima = (double?)row.RagSimilaridadeMinima,
                InstrucaoSistemaExtra = row.InstrucaoSistemaExtra,
                DataAlteracao = row.DataAlteracao
            };

            if (row.AcoesRapidasJson != null)
            {
                try { result.AcoesRapidas = JsonSerializer.Deserialize<List<string>>((string)row.AcoesRapidasJson); }
                catch { result.AcoesRapidas = new List<string>(); }
            }

            return result;
        }

        public async Task<bool> SalvarAsync(AssistenteConfigUpdateInput input, int orgId, IDbTransaction transaction = null)
        {
            const string sql = @"
                INSERT INTO tb_assistente_config
                    (id, tb_org_id, nome_assistente, mensagem_boas_vindas, acoes_rapidas,
                     limiar_similaridade_chamado, rag_top_k, rag_similaridade_minima,
                     instrucao_sistema_extra, alterado_por_codigo_interno_colaborador)
                VALUES
                    (UUID(), @OrgId, @NomeAssistente, @MensagemBoasVindas, @AcoesRapidasJson,
                     @LimiarSimilaridadeChamado, @RagTopK, @RagSimilaridadeMinima,
                     @InstrucaoSistemaExtra, @AlteradoPorCodigoInternoColaborador)
                ON DUPLICATE KEY UPDATE
                    nome_assistente = COALESCE(@NomeAssistente, nome_assistente),
                    mensagem_boas_vindas = COALESCE(@MensagemBoasVindas, mensagem_boas_vindas),
                    acoes_rapidas = COALESCE(@AcoesRapidasJson, acoes_rapidas),
                    limiar_similaridade_chamado = COALESCE(@LimiarSimilaridadeChamado, limiar_similaridade_chamado),
                    rag_top_k = COALESCE(@RagTopK, rag_top_k),
                    rag_similaridade_minima = COALESCE(@RagSimilaridadeMinima, rag_similaridade_minima),
                    instrucao_sistema_extra = COALESCE(@InstrucaoSistemaExtra, instrucao_sistema_extra),
                    alterado_por_codigo_interno_colaborador = @AlteradoPorCodigoInternoColaborador";

            var connection = _dapperConnection.GetConnection();
            var rows = await connection.ExecuteAsync(sql, new
            {
                OrgId = orgId,
                input.NomeAssistente,
                input.MensagemBoasVindas,
                input.AcoesRapidasJson,
                input.LimiarSimilaridadeChamado,
                input.RagTopK,
                input.RagSimilaridadeMinima,
                input.InstrucaoSistemaExtra,
                input.AlteradoPorCodigoInternoColaborador
            }, transaction);
            return rows > 0;
        }
    }
}
