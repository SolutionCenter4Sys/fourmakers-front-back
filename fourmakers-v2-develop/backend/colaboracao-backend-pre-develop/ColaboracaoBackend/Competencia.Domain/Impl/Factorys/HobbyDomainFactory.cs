using Colaboracao.Core;
using Competencia.Domain.Impl.Models;
using Competencia.Domain.Interfaces.Factorys;
using Competencia.Domain.Interfaces.Models;
using Core.Domain;
using DataTransferObject.Domain.Hobby;
using System;

namespace Competencia.Domain.Impl.Factorys
{
    public class HobbyDomainFactory : IHobbyDomainFactory
    {
        private ILogCore _log;
        private IUnitOfWork _unitOfWork;
        private IRepository<IHobbyModel, IHobbyDomainFactory> _repositoryHobbyModel;
        private IRepository<IHobbyColaboradorModel, IHobbyDomainFactory> _repositoryHobbyColaboradorModel;
        public HobbyDomainFactory(ILogCore log, IUnitOfWork unitOfWork, IRepository<IHobbyModel, IHobbyDomainFactory> repositoryHobbyModel, IRepository<IHobbyColaboradorModel, IHobbyDomainFactory> repositoryHobbyColaboradorModel)
        {
            _log = log;
            _unitOfWork = unitOfWork;
            _repositoryHobbyModel = repositoryHobbyModel;
            _repositoryHobbyColaboradorModel = repositoryHobbyColaboradorModel;
        }
        public IHobbyColaboradorModel buildHobbyColaboradorModel(long idHobbyColaborador, long hobbyId, string colaboradorCpf, DateTime dataAlteracao)
        {
            return new HobbyColaboradorModel(_log, _unitOfWork, _repositoryHobbyColaboradorModel)
            {
                HobbyColaboradorDTO = new HobbyColaboradorDTO()
                {
                    Id = idHobbyColaborador,
                    Data = dataAlteracao
                },
                HobbyId = hobbyId,
                ColaboradorCpf = colaboradorCpf
            };
        }

        public IHobbyColaboradorModel buildHobbyColaboradorModel()
        {
            return new HobbyColaboradorModel(_log, _unitOfWork, _repositoryHobbyColaboradorModel);
        }

        public IHobbyModel buildHobbyModel(long id, string descricao, long usuarioCriacaoId)
        {
            return new HobbyModel(_log, _unitOfWork, _repositoryHobbyModel)
            {
                HobbyDTO = new HobbyDTO()
                {
                    IdHobby = id,
                    Descricao = descricao,
                    UsuarioCriacaoId = usuarioCriacaoId
                }
            };
        }
        public IHobbyModel buildHobbyModel()
        {
            return new HobbyModel(_log, _unitOfWork, _repositoryHobbyModel);
        }

        public IHobbyModel buildHobbyModel(long usuarioCriacaoId)
        {
            return new HobbyModel(_log, _unitOfWork, _repositoryHobbyModel)
            {
                HobbyDTO = new HobbyDTO()
                {
                    UsuarioCriacaoId = usuarioCriacaoId
                }
            };
        }
    }
}