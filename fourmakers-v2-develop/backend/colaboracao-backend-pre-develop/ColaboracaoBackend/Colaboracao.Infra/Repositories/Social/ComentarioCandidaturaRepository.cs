using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using Dapper;
using DataTransferObject.Domain;
using Core.Domain.Social;
using DataTransferObject.Domain.Candidato;

namespace Colaboracao.Infra.Repositories.Social
{
    public class ComentarioCandidaturaRepository : IComentarioCandidaturaRepository
    {
        private readonly IDBConnection _dapperConnection;

        public ComentarioCandidaturaRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }
        
        private async Task HydrateArquivosAsync(List<ComentarioCandidaturaDTO> comentarios)
        {
            if (comentarios == null || comentarios.Count == 0) return;

            var ids = comentarios.Select(c => c.Id).Distinct().ToList();

            const string sqlDocs = @"
                SELECT
                    d.id                AS Id,
                    d.tb_candidato_vaga_id AS IdCandidatura,
                    d.tb_comentario_id  AS IdComentario,
                    d.tb_colaborador_codigo_interno_colaborador_criador AS CodColaboradorCriador,
                    d.data_archived     AS DataArquivo,
                    d.link_arquivo      AS LinkArquivo
                FROM tb_candidato_vaga_documentos d
                WHERE d.tb_comentario_id IN @Ids";

            var conn = _dapperConnection.GetConnection();
            var docs = (await conn.QueryAsync<CandidaturaArquivosDTO>(sqlDocs, new { Ids = ids })).ToList();

            var lookup = docs.GroupBy<CandidaturaArquivosDTO, object>(d => d.IdComentario)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var c in comentarios)
            {
                c.Arquivos = lookup.TryGetValue(c.Id, out var lista) ? lista : [];
            }
        }
        
        public async Task<IEnumerable<CandidaturaArquivosDTO>> GetArquivosByComentarioIdAsync(string comentarioId)
        {
            const string sql = @"
                SELECT
                    d.id                AS Id,
                    d.tb_candidato_vaga_id AS CandidaturaId,
                    d.tb_comentario_id  AS ComentarioId,
                    d.tb_colaborador_codigo_interno_colaborador_criador AS CodigoInternoColaboradorCriador,
                    d.data_archived     AS DataArchived,
                    d.link_arquivo      AS LinkArquivo
                FROM tb_candidato_vaga_documentos d
                WHERE d.tb_comentario_id = @ComentarioId
                ORDER BY d.data_archived DESC";

            var conn = _dapperConnection.GetConnection();
            return await conn.QueryAsync<CandidaturaArquivosDTO>(sql, new { ComentarioId = comentarioId });
        }

        public async Task<ComentarioCandidaturaDTO> GetByIdAsync(string id)
        {
            string sql = @"
                SELECT 
                    c.id AS Id,
                    c.texto AS Texto,
                    c.data_criacao AS DataCriacao,
                    c.data_alteracao AS DataAlteracao,
                    c.tb_colaborador_codigo_interno_colaborador AS CodigoInternoColaborador,
                    tc.nome_completo AS CodigoInternoColaboradorNome,
                    cv.tb_candidato_vaga_id AS CandidaturaId
                FROM tb_comentario c
                LEFT JOIN tb_candidato_vaga_comentario cv ON c.id = cv.tb_comentario_id
                INNER JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = c.tb_colaborador_codigo_interno_colaborador
                WHERE c.id = @Id";

            var connection = _dapperConnection.GetConnection();
            var comentario = await connection.QueryFirstOrDefaultAsync<ComentarioCandidaturaDTO>(sql, new { Id = id });
            if (comentario == null) return null;
            
            var arquivos = await GetArquivosByComentarioIdAsync(id);
            comentario.Arquivos = arquivos.ToList();
            
            return comentario;
        }

        public async Task<IEnumerable<ComentarioCandidaturaDTO>> GetByColaboradorCodigoAsync(string colaboradorCodigo)
        {
            const string sql = @"
                SELECT 
                    c.id AS Id,
                    c.texto AS Texto,
                    c.data_criacao AS DataCriacao,
                    c.data_alteracao AS DataAlteracao,
                    c.tb_colaborador_codigo_interno_colaborador AS CodigoInternoColaborador,
                    tc.nome_completo AS CodigoInternoColaboradorNome,
                    cv.tb_candidato_vaga_id AS CandidaturaId
                FROM tb_comentario c
                INNER JOIN tb_candidato_vaga_comentario cv ON c.id = cv.tb_comentario_id
                INNER JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = c.tb_colaborador_codigo_interno_colaborador
                WHERE c.tb_colaborador_codigo_interno_colaborador = @ColaboradorCodigo
                ORDER BY c.data_criacao DESC";

            var connection = _dapperConnection.GetConnection();
            var comentarios =
                (await connection.QueryAsync<ComentarioCandidaturaDTO>(sql,
                    new { ColaboradorCodigo = colaboradorCodigo })).ToList();
            await HydrateArquivosAsync(comentarios);
            return comentarios;
        }

        public async Task<IEnumerable<ComentarioCandidaturaDTO>> GetByCandidaturaIdAsync(string candidatoVagaId)
        {
            const string sql = @"
                SELECT 
                    c.id AS Id,
                    c.texto AS Texto,
                    c.data_criacao AS DataCriacao,
                    c.data_alteracao AS DataAlteracao,
                    c.tb_colaborador_codigo_interno_colaborador AS CodigoInternoColaborador,
                    tc.nome_completo AS CodigoInternoColaboradorNome,
                    cv.tb_candidato_vaga_id AS CandidaturaId
                FROM tb_comentario c
                INNER JOIN tb_candidato_vaga_comentario cv ON c.id = cv.tb_comentario_id
                INNER JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = c.tb_colaborador_codigo_interno_colaborador
                WHERE cv.tb_candidato_vaga_id = @CandidatoVagaId
                ORDER BY c.data_criacao DESC";

            var connection = _dapperConnection.GetConnection();
            var comentarios =
                (await connection.QueryAsync<ComentarioCandidaturaDTO>(sql, new { CandidatoVagaId = candidatoVagaId })).ToList();
            
            await HydrateArquivosAsync(comentarios);
            return comentarios;
        }

        public async Task AddAsync(ComentarioCandidaturaDTO comentario, bool transacaoAberta = false)
        {
            const string sql = @"
                INSERT INTO tb_comentario (
                    id,
                    texto,
                    data_criacao,
                    data_alteracao,
                    tb_colaborador_codigo_interno_colaborador
                )
                VALUES (
                    @Id,
                    @Texto,
                    @DataCriacao,
                    @DataAlteracao,
                    @CodigoInternoColaborador
                )";

            const string sqlRelacao = @"
                INSERT INTO tb_candidato_vaga_comentario (
                    tb_candidato_vaga_id,
                    tb_comentario_id
                )
                VALUES (
                    @CandidatoVagaId,
                    @Id
                )";

            var connection = _dapperConnection.GetConnection();

            if(transacaoAberta)
            {
                await connection.ExecuteAsync(sql, comentario);
                await connection.ExecuteAsync(sqlRelacao, new { CandidatoVagaId = comentario.CandidaturaId, Id = comentario.Id });
                return;
            }

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    await connection.ExecuteAsync(sql, comentario, transaction);
                    await connection.ExecuteAsync(sqlRelacao, new { CandidatoVagaId = comentario.CandidaturaId, Id = comentario.Id }, transaction);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public async Task UpdateAsync(ComentarioCandidaturaDTO comentario)
        {
            const string sql = @"
                UPDATE tb_comentario
                SET texto = @Texto,
                    data_alteracao = @DataAlteracao
                WHERE id = @Id";

            var connection = _dapperConnection.GetConnection();
            await connection.ExecuteAsync(sql, comentario);
        }
    }
}