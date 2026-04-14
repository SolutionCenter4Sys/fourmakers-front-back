using Colaboracao.Core.Interfaces;
using Core.Domain.Social.AtendimentoFourmakers;
using Dapper;
using DataTransferObject.Domain.Social.AtendimentoFourmakers;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Social.AtendimentoFourmakers
{
    public class KbChunkRepository : IKbChunkRepository
    {
        private readonly IDBConnection _dapperConnection;

        public KbChunkRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task<IEnumerable<KbChunkSimilarResult>> BuscarPorSimilaridadeAsync(
            string embeddingJson, int orgId, int topK, double minSimilarity)
        {
            const string sql = "CALL sp_buscar_chunks_similares(@EmbeddingJson, @OrgId, @TopK, @MinSimilarity)";

            var connection = _dapperConnection.GetConnection();
            return await connection.QueryAsync<KbChunkSimilarResult>(sql, new
            {
                EmbeddingJson = embeddingJson,
                OrgId = orgId,
                TopK = topK,
                MinSimilarity = minSimilarity
            });
        }

        public async Task<int> InserirEmBatchAsync(IEnumerable<KbChunkInsertInput> chunks, int orgId, IDbTransaction transaction = null)
        {
            const string sql = @"
                INSERT INTO tb_kb_chunk (id, tb_org_id, conteudo, embedding, tipo_fonte, fonte_id, titulo, indice_chunk)
                VALUES (UUID(), @OrgId, @Conteudo, @EmbeddingJson, @TipoFonte, @FonteId, @Titulo, @IndiceChunk)";

            var connection = _dapperConnection.GetConnection();
            var total = 0;
            foreach (var chunk in chunks)
            {
                total += await connection.ExecuteAsync(sql, new
                {
                    OrgId = orgId,
                    chunk.Conteudo,
                    chunk.EmbeddingJson,
                    chunk.TipoFonte,
                    chunk.FonteId,
                    chunk.Titulo,
                    chunk.IndiceChunk
                }, transaction);
            }
            return total;
        }

        public async Task<int> DeletarPorFonteAsync(string fonteId, int orgId, IDbTransaction transaction = null)
        {
            const string sql = @"
                DELETE FROM tb_kb_chunk
                WHERE fonte_id = @FonteId AND tb_org_id = @OrgId";

            var connection = _dapperConnection.GetConnection();
            return await connection.ExecuteAsync(sql, new { FonteId = fonteId, OrgId = orgId }, transaction);
        }

        public async Task<int> ContarAsync(int orgId)
        {
            const string sql = "SELECT COUNT(1) FROM tb_kb_chunk WHERE tb_org_id = @OrgId";
            var connection = _dapperConnection.GetConnection();
            return await connection.ExecuteScalarAsync<int>(sql, new { OrgId = orgId });
        }

        public async Task<IEnumerable<KbChunkResumoResult>> ListarPorFonteAsync(string fonteId, int orgId)
        {
            const string sql = @"
                SELECT
                    CAST(id AS CHAR(36)) AS Id,
                    indice_chunk AS IndiceChunk,
                    conteudo AS Conteudo,
                    data_criacao AS DataCriacao
                FROM tb_kb_chunk
                WHERE fonte_id = @FonteId AND tb_org_id = @OrgId
                ORDER BY indice_chunk ASC";

            var connection = _dapperConnection.GetConnection();
            return await connection.QueryAsync<KbChunkResumoResult>(sql, new { FonteId = fonteId, OrgId = orgId });
        }
    }
}
