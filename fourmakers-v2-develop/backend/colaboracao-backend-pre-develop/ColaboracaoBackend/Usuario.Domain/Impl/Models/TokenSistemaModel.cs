using Colaboracao.Core;
using Core.Domain;
using System;
using Usuario.Domain.Interfaces.Factorys;
using Usuario.Domain.Interfaces.Models;

namespace Usuario.Domain.Impl.Models
{
    public class TokenSistemaModel : ITokenSistemaModel
    {
        private readonly ILogCore _log;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<ITokenSistemaModel, ITokenSistemaDomainFactory> _repositoryTokenSistemaModel;

        public TokenSistemaModel(ILogCore log, IUnitOfWork unitOfWork, IRepository<ITokenSistemaModel, ITokenSistemaDomainFactory> repositoryTokenSistemaModel)
        {
            this._log = log;
            this._unitOfWork = unitOfWork;
            this._repositoryTokenSistemaModel = repositoryTokenSistemaModel;
        }

        public long Id { get; set; }
        public string Sistema { get; set; }
        public string Token { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAlteracao { get; set; }
        public sbyte Ativo { get; set; }

        public ITokenSistemaModel GetModelByToken(string token, ITokenSistemaDomainFactory factory)
        {
            var tokenBanco = _repositoryTokenSistemaModel.GetModelByKey(token, factory);

            if (tokenBanco != null)
            {
                return tokenBanco;
            }

            return null;
        }
    }
}