using DataTransferObject.Domain.Notificacao;
using DataTransferObject.Domain.Usuario;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Firebase.Domain.Interfaces.Services
{
    public interface INotificacaoService
    {
        Task<List<NotificacaoDTO>> ListarNotificacoesColaborador(string cpf, int orgId, bool apenasNaoLidas);
        Task<int> ContarNotificacoesNaoLidasColaborador(string cpf, int orgId);
        Task<NotificacaoDTO> InserirNotificacaoColaborador(NotificacaoDTO notificacaoDTO);
        Task<NotificacaoDTO> EnviarNotificacaoColaborador(string cpf, int orgId, string titulo, string mensagem, string mensagemHtml, FuncionalidadeSistemaEnum? funcionalidade, string urlCustomizada = null);
        Task<bool> MarcarNotificacoesComoLidasColaborador(string cpf, int orgId);
    }
}