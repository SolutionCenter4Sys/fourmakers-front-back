using DataTransferObject.Domain.Usuario;

namespace Colaboracao.Core.Interfaces
{
    public interface IAspNetUser
    {
        UsuarioLogadoDTO GetUsuarioLogado();
    }
}