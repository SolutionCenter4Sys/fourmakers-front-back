using Colaboracao.Core;
using Core.Domain;
using Usuario.Domain.Impl.Models;
using Usuario.Domain.Interfaces.Factorys;
using Usuario.Domain.Interfaces.Models;

namespace Usuario.Domain.Impl.Factorys
{
    public class TokenUsuarioAcessoDomainFactory : ITokenUsuarioAcessoDomainFactory
    {
        private ILogCore _log;
        private IUnitOfWork _unitOfWork;
        private IRepository<ITokenUsuarioAcessoModel, ITokenUsuarioAcessoDomainFactory> _repositoryTokenUsuarioAcessoModel;

        public TokenUsuarioAcessoDomainFactory(ILogCore log, IUnitOfWork unitOfWork, IRepository<ITokenUsuarioAcessoModel, ITokenUsuarioAcessoDomainFactory> repositoryTokenUsuarioAcessoModel)
        {
            _log = log;
            _unitOfWork = unitOfWork;
            _repositoryTokenUsuarioAcessoModel = repositoryTokenUsuarioAcessoModel;
        }
        public ITokenUsuarioAcessoModel buildTokenUsuarioAcessoModel()
        {
            return new TokenUsuarioAcessoModel(_log, _unitOfWork, _repositoryTokenUsuarioAcessoModel);
        }

        public ITokenUsuarioAcessoModel buildTokenUsuarioAcessoModel(long id, long usuarioId, long tokenAcessoId)
        {
            return new TokenUsuarioAcessoModel(_log, _unitOfWork, _repositoryTokenUsuarioAcessoModel)
            {
                Id = id,
                UsuarioId = usuarioId,
                TokenAcessoId = tokenAcessoId
            };
        }
    }
}