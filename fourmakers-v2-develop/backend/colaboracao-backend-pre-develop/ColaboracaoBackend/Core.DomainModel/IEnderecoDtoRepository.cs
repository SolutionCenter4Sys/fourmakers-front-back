using DataTransferObject.Domain.Endereco;

namespace Core.DomainModel
{
    public interface IEnderecoDtoRepository
    {
        EnderecoDTO GetModelByKey(string key);
        EnderecoDTO SaveModel(EnderecoDTO model);
        EnderecoDTO UpdateModel(EnderecoDTO model);
    }
}
