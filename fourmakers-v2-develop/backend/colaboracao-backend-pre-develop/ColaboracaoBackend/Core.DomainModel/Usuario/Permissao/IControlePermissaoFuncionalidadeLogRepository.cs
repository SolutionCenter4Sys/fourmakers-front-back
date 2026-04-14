using DataTransferObject.Domain.Usuario.Permissao;
using System.Threading.Tasks;

namespace Core.Domain.Usuario.Permissao
{
    public interface IPermissaoLogRepository
    {
        Task InserirLog(UsuarioPermissaoLogDTO log);
    }
}