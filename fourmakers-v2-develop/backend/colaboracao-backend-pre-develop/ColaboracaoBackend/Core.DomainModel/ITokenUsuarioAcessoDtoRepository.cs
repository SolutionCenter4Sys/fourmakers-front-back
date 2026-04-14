using DataTransferObject.Domain;

namespace Core.DomainModel
{
    public interface ITokenUsuarioAcessoDtoRepository
    {
        TokenValidacaoAcessoDTO GetModelByKey(string key);
        TokenValidacaoAcessoDTO SaveModel(TokenValidacaoAcessoDTO model);
        void DeleteModel(TokenValidacaoAcessoDTO model);
    }
}
