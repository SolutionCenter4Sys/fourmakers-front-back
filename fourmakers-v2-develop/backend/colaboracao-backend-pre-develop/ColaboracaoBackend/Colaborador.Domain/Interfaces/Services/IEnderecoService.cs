using System.Threading.Tasks;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Endereco;

namespace Colaborador.Domain.Interfaces.Services;

public interface IEnderecoService
{
    Task<ApiGenericResult<EnderecoDTO>> EditarEnderecoAsync(EnderecoInputDTO enderecoDTO, string codigoInternoColaborador);
    Task<ApiGenericResult<EnderecoDTO>> ObterEnderecoPorCodigoInternoColaboradorAsync(string codigoInternoColaborador);
    Task<ApiGenericResult<EnderecoDTO>> ObterEnderecoPorCodigoAsync(long id);
}