using Colaboracao.Core;
using Core.Domain;
using System;
using Usuario.Domain.Interfaces.Factorys;
using Usuario.Domain.Interfaces.Models;

namespace Usuario.Domain.Impl.Models
{
    public class TokenModel : ITokenModel
    {
        private readonly ILogCore _log;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<ITokenModel, ITokenDomainFactory> _repositoryTokenModel;

        public TokenModel(ILogCore log, IUnitOfWork unitOfWork, IRepository<ITokenModel, ITokenDomainFactory> repositoryTokenModel)
        {
            this._log = log;
            this._unitOfWork = unitOfWork;
            this._repositoryTokenModel = repositoryTokenModel;
        }

        public long Id { get; set; }
        public string Token { get; set; }
        public DateTime Validade { get; set; }
        public sbyte Ativo { get; set; }
        public long UsuarioId { get; set; }
        public string UsuarioCpf { get; set; }

        public ITokenModel GetModel(string tokenAcesso, ITokenDomainFactory factory)
        {
            this.Token = tokenAcesso;
            return _repositoryTokenModel.GetModelByKey(tokenAcesso, factory);
        }

        public ITokenModel GetTokenById(long id)
        {
            this.Id = id;
            return _repositoryTokenModel.GetModel(this);
        }

        public ITokenModel SaveModel()
        {
            return _repositoryTokenModel.SaveModel(this);
        }

        public ITokenModel UpdateModel()
        {
            return _repositoryTokenModel.UpdateModel(this);
        }
    }
}