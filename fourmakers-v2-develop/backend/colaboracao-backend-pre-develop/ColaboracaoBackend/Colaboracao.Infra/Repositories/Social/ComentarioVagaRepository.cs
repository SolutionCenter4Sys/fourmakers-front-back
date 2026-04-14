using Colaboracao.Core.Interfaces;
using Core.Domain.Social;
using Dapper;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Candidato;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Social
{
    public class ComentarioVagaRepository : IComentarioVagaRepository
    {
        private readonly IDBConnection _dapperConnection;

        public ComentarioVagaRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task<ComentarioVagaDTO> GetByIdAsync(string id)
        {
            string sql = @"
                SELECT 
                    c.id AS Id,
                    c.texto AS Texto,
                    c.data_criacao AS DataCriacao,
                    c.data_alteracao AS DataAlteracao,
                    c.tb_colaborador_codigo_interno_colaborador AS CodigoInternoColaborador,
                    tc.nome_completo AS CodigoInternoColaboradorNome,
                    cv.tb_vaga_id  AS VagaId
                FROM tb_comentario c
                INNER JOIN tb_comentario_vaga cv ON c.id = cv.tb_comentario_id
                INNER JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = c.tb_colaborador_codigo_interno_colaborador
                WHERE c.id = @Id";

            var connection = _dapperConnection.GetConnection();
            var comentario = await connection.QueryFirstOrDefaultAsync<ComentarioVagaDTO>(sql, new { Id = id });
            if (comentario == null) return null;
            
            //var arquivos = await GetArquivosByComentarioIdAsync(id);
            //comentario.Arquivos = arquivos.ToList();
            
            return comentario;
        }

        public async Task<IEnumerable<ComentarioVagaDTO>> GetByVagaIdAsync(string vagaId)
        {
            const string sql = @"
                SELECT 
                    c.id AS Id,
                    c.texto AS Texto,
                    c.data_criacao AS DataCriacao,
                    c.data_alteracao AS DataAlteracao,
                    c.tb_colaborador_codigo_interno_colaborador AS CodigoInternoColaborador,
                    tc.nome_completo AS CodigoInternoColaboradorNome,
                    cv.tb_vaga_id  AS VagaId
                FROM tb_comentario c
                INNER JOIN tb_comentario_vaga cv ON c.id = cv.tb_comentario_id
                INNER JOIN tb_colaborador tc ON tc.codigo_interno_colaborador = c.tb_colaborador_codigo_interno_colaborador
                WHERE cv.tb_vaga_id = @vagaId
                ORDER BY c.data_criacao DESC";

            var connection = _dapperConnection.GetConnection();
            var comentarios =
                (await connection.QueryAsync<ComentarioVagaDTO>(sql, new { vagaId })).ToList();
            
            return comentarios;
        }

        public async Task AddAsync(ComentarioVagaDTO comentario, bool transacaoAberta = false)
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
                INSERT INTO tb_comentario_vaga (
                    tb_vaga_id,
                    tb_comentario_id
                )
                VALUES (
                    @VagaId,
                    @Id
            )";

            var connection = _dapperConnection.GetConnection();

            if(transacaoAberta)
            {
                await connection.ExecuteAsync(sql, comentario);
                await connection.ExecuteAsync(sqlRelacao, new { comentario.VagaId, comentario.Id });
                return;
            }

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    await connection.ExecuteAsync(sql, comentario, transaction);
                    await connection.ExecuteAsync(sqlRelacao, new { comentario.VagaId, comentario.Id }, transaction);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public async Task UpdateAsync(ComentarioVagaDTO comentario)
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