using System.Threading.Tasks;
using DataTransferObject.Domain.Endereco;

namespace Core.Domain;

public interface IEnderecoDapperRepository
{
    Task<EnderecoDTO> ObterEnderecoPorCodigoInternoColaboradorAsync(string codigoInternoColaborador);
    Task<EnderecoDTO> ObterEnderecoPorIdAsync(long id);
    Task<EnderecoDTO> CriarEnderecoAsync(EnderecoDTO input);
    Task<EnderecoDTO> AtualizarEnderecoAsync(long id, EnderecoDTO input);
}