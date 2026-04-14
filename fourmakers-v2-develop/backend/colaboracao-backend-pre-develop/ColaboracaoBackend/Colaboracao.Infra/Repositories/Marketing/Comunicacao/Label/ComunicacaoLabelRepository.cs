using Colaboracao.Core.Interfaces;
using Core.Domain.Marketing.Comunicacao.Label;
using Dapper;
using DataTransferObject.Domain.Marketing.Comunicacao.Label;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Marketing.Comunicacao.Label
{
    public class ComunicacaoLabelRepository : IComunicacaoLabelRepository
    {
        private readonly IDBConnection _dapperConnection;

        public ComunicacaoLabelRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task<bool> AtualizarLabelAsync(string labelId, string nome, string tipo, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var sql = @"
                UPDATE tb_mkt_label
                SET nome = @Nome, tipo = @Tipo
                WHERE id = @LabelId
                    AND tb_org_id = @OrgId";

            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                LabelId = labelId,
                Nome = nome,
                Tipo = tipo ?? "informativo",
                OrgId = orgId
            });

            return rowsAffected > 0;
        }

        public async Task<bool> RemoverLabelAsync(string labelId, int orgId)
        {
            var connection = _dapperConnection.GetConnection();

            var sqlRemoveRelacoes = @"
                DELETE pl FROM tb_mkt_publicacao_label pl
                WHERE pl.tb_mkt_label_id = @LabelId";

            await connection.ExecuteAsync(sqlRemoveRelacoes, new { LabelId = labelId });

            var sql = @"
                DELETE FROM tb_mkt_label
                WHERE id = @LabelId
                    AND tb_org_id = @OrgId";

            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                LabelId = labelId,
                OrgId = orgId
            });

            return rowsAffected > 0;
        }

        public async Task<IEnumerable<LabelResumoDTO>> ObterTodasPorOrgAsync(int orgId, IEnumerable<string> tipos = null)
        {
            var connection = _dapperConnection.GetConnection();
            var tiposList = (tipos?.ToList() ?? new List<string> { "informativo", "documento" });

            var sql = @"
                SELECT id AS Id, nome AS Nome, tipo AS Tipo
                FROM tb_mkt_label
                WHERE tb_org_id = @OrgId
                    AND tipo IN @Tipos
                ORDER BY nome";

            var resultado = await connection.QueryAsync<LabelResumoDTO>(sql, new { OrgId = orgId, Tipos = tiposList });
            return resultado ?? Enumerable.Empty<LabelResumoDTO>();
        }
    }
}
