using Colaboracao.Core;
using Core.Domain;
using System;
using System.Collections.Generic;
using Usuario.Domain.Interfaces.Factorys;
using Usuario.Domain.Interfaces.Models;

namespace Usuario.Domain.Impl.Models
{
    public class TokenUsuarioAcessoModel : ITokenUsuarioAcessoModel
    {
        private readonly ILogCore _log;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<ITokenUsuarioAcessoModel, ITokenUsuarioAcessoDomainFactory> _repositoryTokenUsuarioAcessoModel;

        public TokenUsuarioAcessoModel(ILogCore log, IUnitOfWork unitOfWork, IRepository<ITokenUsuarioAcessoModel, ITokenUsuarioAcessoDomainFactory> repositoryTokenUsuarioAcessoModel)
        {
            this._log = log;
            this._unitOfWork = unitOfWork;
            this._repositoryTokenUsuarioAcessoModel = repositoryTokenUsuarioAcessoModel;
        }

        public long Id { get; set; }
        public long TokenAcessoId { get; set; }
        public long UsuarioId { get; set; }
        public DateTime DataCriacao { get; set; }
        public bool Ativo { get; set; }

        public ITokenModel GerarNovoToken(ITokenDomainFactory _tokenDomainFactory, ITokenUsuarioAcessoDomainFactory _tokenUsuarioAcessoDomainFactory, IUsuarioModel usuario)
        {
            using (var dbTrans = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var tokenModel = _tokenDomainFactory.buildTokenModel();
                    var tokenUsuarioAcessoModel = _tokenUsuarioAcessoDomainFactory.buildTokenUsuarioAcessoModel();
                    /*var tokensUsuario = tokenUsuarioAcessoModel.ListByUserId(usuario.Usuario.UsuarioId, _tokenUsuarioAcessoDomainFactory);

                    if (tokensUsuario != null)
                    {
                        foreach (var token in tokensUsuario)
                        {
                            var tokenBanco = tokenModel.GetTokenById(token.TokenAcessoId);
                            tokenBanco.Ativo = 0;
                            tokenBanco.UpdateModel();
                        }
                    }*/

                    var novoToken = _tokenDomainFactory.buildTokenModel();
                    novoToken.Ativo = 1;
                    novoToken.Validade = DateTime.Now.AddDays(60);
                    novoToken.Token = Criptografia.Encrypt(usuario.Usuario.Cpf + "|" + DateTime.Now.ToUniversalTime() + "|" + new Random().Next(0, 99999) + "|2");
                    novoToken.SaveModel();

                    tokenUsuarioAcessoModel.TokenAcessoId = novoToken.Id;
                    tokenUsuarioAcessoModel.UsuarioId = usuario.Usuario.UsuarioId;
                    tokenUsuarioAcessoModel.SaveModel();

                    dbTrans.Commit();

                    return novoToken;
                }
                catch (Exception)
                {
                    dbTrans.Rollback();
                    throw;
                }
            }
        }

        public List<ITokenUsuarioAcessoModel> ListByUserId(long usuarioId, ITokenUsuarioAcessoDomainFactory factory)
        {
            this.UsuarioId = usuarioId;
            return _repositoryTokenUsuarioAcessoModel.ListModel(this, factory);
        }

        public ITokenUsuarioAcessoModel SaveModel()
        {
            return _repositoryTokenUsuarioAcessoModel.SaveModel(this);
        }

        public ITokenUsuarioAcessoModel UpdateModel()
        {
            using (var dbTrans = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var ret = _repositoryTokenUsuarioAcessoModel.UpdateModel(this);

                    dbTrans.Commit();

                    return ret;
                }
                catch (Exception)
                {
                    dbTrans.Rollback();
                    throw;
                }
            }
        }
    }
}