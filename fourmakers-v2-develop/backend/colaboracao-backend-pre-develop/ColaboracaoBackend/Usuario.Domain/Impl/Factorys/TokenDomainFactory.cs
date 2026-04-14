using Colaboracao.Core;
using Core.Domain;
using System;
using Usuario.Domain.Impl.Models;
using Usuario.Domain.Interfaces.Factorys;
using Usuario.Domain.Interfaces.Models;

namespace Usuario.Domain.Impl.Factorys
{
    public class TokenDomainFactory : ITokenDomainFactory
    {
        private ILogCore _log;
        private IUnitOfWork _unitOfWork;
        private IRepository<ITokenModel, ITokenDomainFactory> _repositoryTokenModel;

        public TokenDomainFactory(ILogCore log, IUnitOfWork unitOfWork, IRepository<ITokenModel, ITokenDomainFactory> repositoryTokenModel)
        {
            _log = log;
            _unitOfWork = unitOfWork;
            _repositoryTokenModel = repositoryTokenModel;
        }

        public ITokenModel buildTokenModel()
        {
            return new TokenModel(_log, _unitOfWork, _repositoryTokenModel);
        }

        public ITokenModel buildTokenModel(long id, string token, DateTime validade, sbyte ativo, long usuarioId, string usuarioCpf)
        {
            return new TokenModel(_log, _unitOfWork, _repositoryTokenModel)
            {
                Id = id,
                Token = token,
                Validade = validade,
                Ativo = ativo
            };
        }
    }
}