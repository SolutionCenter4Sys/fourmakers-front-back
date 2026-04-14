using Colaboracao.Core;
using Core.Domain;
using DataTransferObject.Domain.Usuario;
using System;
using Usuario.Domain.Interfaces.Factorys;
using Usuario.Domain.Interfaces.Models;

namespace Usuario.Domain.Impl.Models
{
    public class UsuarioModel : IUsuarioModel
    {
        private readonly ILogCore _log;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsuarioRepository<IUsuarioModel, IUsuarioDomainFactory> _repositoryUsuarioModel;

        public UsuarioModel(ILogCore log, IUnitOfWork unitOfWork, IUsuarioRepository<IUsuarioModel, IUsuarioDomainFactory> repositoryUsuarioModel)
        {
            this._log = log;
            this._unitOfWork = unitOfWork;
            this._repositoryUsuarioModel = repositoryUsuarioModel;
            this.Usuario = new UsuarioColaboradorDTO();
        }

        public long Id { get; set; }
        public UsuarioColaboradorDTO Usuario { get; set; }
        public string Login { get; set; }
        public string Senha { get; set; }
        public sbyte PrimeiroAcessoRealizado { get; set; }
        public string FcmToken { get; set; }
        public sbyte Ativo { get; set; }
        public string Token { get; set; }
        public int Sistemico { get; set; }
        public DateTime? DataExpiracao { get; set; }
        public DateTime? DataAceiteTermo { get; set; }
        public int OrgId { get; set; } = 0;
        public IUsuarioModel GetUserByCpf(string cpf)
        {
            this.Usuario.Cpf = cpf;
            var usuarioBanco = _repositoryUsuarioModel.GetModel(this);

            if (usuarioBanco != null)
            {
                return usuarioBanco;
            }

            return null;
        }

        public IUsuarioModel GetUserByCpfAndOrg(string cpf, int orgId)
        {
            this.Usuario.Cpf = cpf;
            this.Usuario.OrgId = orgId;
            var usuarioBanco = _repositoryUsuarioModel.GetModel(this);

            if (usuarioBanco != null)
            {
                return usuarioBanco;
            }

            return null;
        }

        public IUsuarioModel GetUserByLogin(string login, IUsuarioDomainFactory factory, int orgId = 0)
        {
            this.Login = login;
            var usuarioBanco = _repositoryUsuarioModel.GetModelByKey(this.Login, factory, orgId);

            if (usuarioBanco != null)
            {
                return usuarioBanco;
            }

            return null;
        }

        public IUsuarioModel GetUserByLoginAndOrg(string login, int orgId,IUsuarioDomainFactory factory)
        {
            this.Login = login;
            var usuarioBanco = _repositoryUsuarioModel.GetModelByKeyAndOrg(this.Login, orgId, factory);

            if (usuarioBanco != null)
            {
                return usuarioBanco;
            }

            return null;
        }

        public void Logout(string login, IUsuarioDomainFactory factory)
        {
            this.Login = login;
            var usuarioBanco = _repositoryUsuarioModel.GetModelByKey(login, factory);

            if (usuarioBanco != null)
            {
                usuarioBanco.FcmToken = null;
                usuarioBanco.UpdateModel();
            }
        }
        public IUsuarioModel SaveModel()
        {
            return _repositoryUsuarioModel.SaveModel(this);
        }

        public IUsuarioModel UpdateModel()
        {
            return _repositoryUsuarioModel.UpdateModel(this);
        }
    }
}