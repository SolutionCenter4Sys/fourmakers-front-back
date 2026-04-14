using DataTransferObject.Domain.Usuario.Permissao;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Usuario.Domain.Interfaces.Services.Permissao
{
    public interface IFuncionalidadeSistemaService
    {
        Task<IEnumerable<FuncionalidadeSistemaDTO>> ListarFuncionalidadesSistema(string cpfRequest, int orgId);
        Task<IEnumerable<PessoaFuncionalidadeSistemaDTO>> ListarPessoasPorFuncionalidadeSistema(string cpfRequest, int orgId, int? funcionalidadeId = null);
    }
}