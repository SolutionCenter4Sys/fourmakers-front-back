using Colaboracao.Core;
using Core.Domain;
using DataTransferObject.Domain.Usuario;
using Usuario.Domain.Impl.Models;
using Usuario.Domain.Interfaces.Factorys;
using Usuario.Domain.Interfaces.Models;

namespace Usuario.Domain.Impl.Factorys
{
    public class UsuarioDomainFactory : IUsuarioDomainFactory
    {
        private ILogCore _log;
        private IUnitOfWork _unitOfWork;
        private IUsuarioRepository<IUsuarioModel, IUsuarioDomainFactory> _repositoryUsuarioModel;

        public UsuarioDomainFactory(ILogCore log, IUnitOfWork unitOfWork, IUsuarioRepository<IUsuarioModel, IUsuarioDomainFactory> repositoryUsuarioModel)
        {
            _log = log;
            _unitOfWork = unitOfWork;
            _repositoryUsuarioModel = repositoryUsuarioModel;
        }

        public IUsuarioModel buildUsuarioModel(long usuarioId, string cloudId, string cpf, string email)
        {
            return new UsuarioModel(_log, _unitOfWork, _repositoryUsuarioModel)
            {
                Usuario = new UsuarioColaboradorDTO
                {
                    Cpf = cpf,
                    Email = email,
                    UsuarioId = usuarioId
                }
            };
        }
        public IUsuarioModel buildUsuarioModel()
        {
            return new UsuarioModel(_log, _unitOfWork, _repositoryUsuarioModel);
        }
    }
}