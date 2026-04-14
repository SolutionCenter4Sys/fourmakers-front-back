using DataTransferObject.Domain.Usuario;
using DataTransferObject.Domain.Usuario.Permissao;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Usuario.Permissao
{
    public interface IFuncionalidadeSistemaRepository
    {
        Task<List<FuncionalidadeSistemaDTO>> GetFuncionalidadeSistemaPorUsuario(int usuarioId, int orgId);
        Task<bool> ValidaAcessoFuncionalidade(string cpf, int orgId, FuncionalidadeSistemaEnum funcionalidade);
        Task<IEnumerable<FuncionalidadeSistemaDTO>> ListarFuncionalidadesSistema();
        Task<bool> FuncionalidadeNaoExiste(int funcionalidadeSistemaId);
        Task<IEnumerable<PessoaFuncionalidadeSistemaDTO>> ListarPessoasPorFuncionalidadeSistema(int orgId, int? funcionalidadeId = null);
    }
}