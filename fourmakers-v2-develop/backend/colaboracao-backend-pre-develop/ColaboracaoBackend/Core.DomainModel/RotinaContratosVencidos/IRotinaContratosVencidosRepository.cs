using DataTransferObject.Domain.RotinaNotificacaoContratosVencidos;
using DataTransferObject.Domain.Usuario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.RotinaContratosVencidos
{
    public interface IRotinaContratosVencidosRepository
    {
        Task<NotificacaoContratosVencidos> BuscarEmailContratosNotificacao();
        Task<UsuarioColaboradorDTO> BuscarDadosColaboradorPorEmail(string email);
    }
}
