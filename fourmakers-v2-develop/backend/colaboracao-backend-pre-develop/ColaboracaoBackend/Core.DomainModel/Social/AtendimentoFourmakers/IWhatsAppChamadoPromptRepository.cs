using System;
using System.Data;
using System.Threading.Tasks;

namespace Core.Domain.Social.AtendimentoFourmakers
{
    public interface IWhatsAppChamadoPromptRepository
    {
        Task InserirOuAtualizarAsync(string telefone, DateTime expiraEm, int orgId, IDbTransaction transaction = null);
        Task<bool> ExisteAtivoAsync(string telefone, int orgId);
        Task DeletarExpiradosAsync();
    }
}
