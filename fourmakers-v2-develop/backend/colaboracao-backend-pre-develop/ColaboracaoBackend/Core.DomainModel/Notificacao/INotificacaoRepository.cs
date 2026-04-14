using DataTransferObject.Domain.Notificacao;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Notificacao
{
    public interface INotificacaoRepository
    {
        Task<List<NotificacaoDTO>> ListarNotificacoesColaborador(string cpf, int orgId, bool apenasNaoLidas);
        Task<NotificacaoDTO> InserirNotificacaoColaborador(NotificacaoDTO notificacaoDTO);
        Task<int> ContarNotificacoesNaoLidasColaborador(string cpf, int orgId);
        Task<bool> MarcarNotificacoesComoLidasColaborador(string cpf, int orgId);
    }
}