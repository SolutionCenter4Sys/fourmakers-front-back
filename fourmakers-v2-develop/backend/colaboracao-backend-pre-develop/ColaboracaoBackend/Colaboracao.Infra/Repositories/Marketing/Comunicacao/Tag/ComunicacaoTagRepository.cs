using Colaboracao.Core.Interfaces;
using Core.Domain.Marketing.Comunicacao.Tag;
using Dapper;
using DataTransferObject.Domain.Marketing.Comunicacao.Tag;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Marketing.Comunicacao.Tag
{
    public class ComunicacaoTagRepository : IComunicacaoTagRepository
    {
        private readonly IDBConnection _dapperConnection;

        public ComunicacaoTagRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task<bool> AtualizarTagAsync(string tagId, string nome, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var sql = @"
                UPDATE tb_mkt_tag
                SET nome = @Nome
                WHERE id = @TagId
                    AND tb_org_id = @OrgId";

            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                TagId = tagId,
                Nome = nome,
                OrgId = orgId
            });

            return rowsAffected > 0;
        }

        public async Task<bool> RemoverTagAsync(string tagId, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var sqlRemoveRelacoes = @"
                DELETE pt FROM tb_mkt_publicacao_tag pt
                WHERE pt.tb_mkt_tag_id = @TagId";

            await connection.ExecuteAsync(sqlRemoveRelacoes, new { TagId = tagId });

            var sql = @"
                DELETE FROM tb_mkt_tag
                WHERE id = @TagId
                    AND tb_org_id = @OrgId";

            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                TagId = tagId,
                OrgId = orgId
            });

            return rowsAffected > 0;
        }

        public async Task<IEnumerable<TagResumoDTO>> ObterTodasPorOrgAsync(int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var sql = @"
                SELECT id AS Id, nome AS Nome
                FROM tb_mkt_tag
                WHERE tb_org_id = @OrgId
                ORDER BY nome";

            var resultado = await connection.QueryAsync<TagResumoDTO>(sql, new { OrgId = orgId });
            return resultado ?? Enumerable.Empty<TagResumoDTO>();
        }
    }
}
