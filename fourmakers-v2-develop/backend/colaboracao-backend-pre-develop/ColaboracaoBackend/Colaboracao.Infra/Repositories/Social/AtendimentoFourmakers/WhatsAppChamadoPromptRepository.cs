using Colaboracao.Core.Interfaces;
using Core.Domain.Social.AtendimentoFourmakers;
using Dapper;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Colaboracao.Infra.Repositories.Social.AtendimentoFourmakers
{
    public class WhatsAppChamadoPromptRepository : IWhatsAppChamadoPromptRepository
    {
        private readonly IDBConnection _dapperConnection;

        public WhatsAppChamadoPromptRepository(IDBConnection dapperConnection)
        {
            _dapperConnection = dapperConnection;
        }

        public async Task InserirOuAtualizarAsync(string telefone, DateTime expiraEm, int orgId, IDbTransaction transaction = null)
        {
            const string sql = @"
                INSERT INTO tb_whatsapp_chamado_prompt (id, tb_org_id, telefone, expira_em)
                VALUES (UUID(), @OrgId, @Telefone, @ExpiraEm)
                ON DUPLICATE KEY UPDATE
                    expira_em = @ExpiraEm";

            var connection = _dapperConnection.GetConnection();
            await connection.ExecuteAsync(sql, new
            {
                OrgId = orgId,
                Telefone = telefone,
                ExpiraEm = expiraEm
            }, transaction);
        }

        public async Task<bool> ExisteAtivoAsync(string telefone, int orgId)
        {
            const string sql = @"
                SELECT COUNT(1) FROM tb_whatsapp_chamado_prompt
                WHERE telefone = @Telefone AND tb_org_id = @OrgId AND expira_em > NOW()";

            var connection = _dapperConnection.GetConnection();
            var count = await connection.ExecuteScalarAsync<int>(sql, new { Telefone = telefone, OrgId = orgId });
            return count > 0;
        }

        public async Task DeletarExpiradosAsync()
        {
            const string sql = "DELETE FROM tb_whatsapp_chamado_prompt WHERE expira_em <= NOW()";
            var connection = _dapperConnection.GetConnection();
            await connection.ExecuteAsync(sql);
        }
    }
}
