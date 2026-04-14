using Colaboracao.Core;
using Core.Domain;
using Usuario.Domain.Impl.Models;
using Usuario.Domain.Interfaces.Factorys;
using Usuario.Domain.Interfaces.Models;

namespace Usuario.Domain.Impl.Factorys
{
    public class TokenSistemaDomainFactory : ITokenSistemaDomainFactory
    {
        private ILogCore _log;
        private IUnitOfWork _unitOfWork;
        private IRepository<ITokenSistemaModel, ITokenSistemaDomainFactory> _repositoryTokenSistemaModel;

        public TokenSistemaDomainFactory(ILogCore log, IUnitOfWork unitOfWork, IRepository<ITokenSistemaModel, ITokenSistemaDomainFactory> repositoryTokenSistemaModel)
        {
            _log = log;
            _unitOfWork = unitOfWork;
            _repositoryTokenSistemaModel = repositoryTokenSistemaModel;
        }

        public ITokenSistemaModel buildTokenSistemaModel()
        {
            return new TokenSistemaModel(_log, _unitOfWork, _repositoryTokenSistemaModel);
        }
    }
}