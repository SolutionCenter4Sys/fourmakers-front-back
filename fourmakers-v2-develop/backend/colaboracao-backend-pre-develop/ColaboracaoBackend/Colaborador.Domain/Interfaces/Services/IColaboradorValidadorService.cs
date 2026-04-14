using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador;

namespace Colaborador.Domain.Interfaces.Services
{
    public interface IColaboradorValidadorService
    {
        void ValidaColaboradorCadastro(CadastroColaboradorInput cadastroColaboradorInput, string cpfUsuario, int orgId, ApiGenericResult<string> ret);
        void ValidaColaboradorEditar(CadastroColaboradorInput cadastroColaboradorInput, string cpfUsuario, int orgId, ApiGenericResult<string> ret);
    }
}