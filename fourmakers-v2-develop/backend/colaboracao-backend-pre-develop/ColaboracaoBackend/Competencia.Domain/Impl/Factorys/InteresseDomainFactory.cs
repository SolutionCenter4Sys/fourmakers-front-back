using Colaboracao.Core;
using Competencia.Domain.Impl.Models;
using Competencia.Domain.Interfaces.Factorys;
using Competencia.Domain.Interfaces.Models;
using Core.Domain;
using DataTransferObject.Domain.Interesse;
using System;

namespace Competencia.Domain.Impl.Factorys
{
    public class InteresseDomainFactory : IInteresseDomainFactory
    {
        private ILogCore _log;
        private IUnitOfWork _unitOfWork;
        private IRepository<IInteresseModel, IInteresseDomainFactory> _repositoryInteresseModel;
        private IRepository<IInteresseColaboradorModel, IInteresseDomainFactory> _repositoryInteresseColaboradorModel;
        public InteresseDomainFactory(ILogCore log, IUnitOfWork unitOfWork, IRepository<IInteresseModel, IInteresseDomainFactory> repositoryInteresseModel, IRepository<IInteresseColaboradorModel, IInteresseDomainFactory> repositoryInteresseColaboradorModel)
        {
            _log = log;
            _unitOfWork = unitOfWork;
            _repositoryInteresseModel = repositoryInteresseModel;
            _repositoryInteresseColaboradorModel = repositoryInteresseColaboradorModel;
        }

        public IInteresseColaboradorModel buildInteresseColaboradorModel(long idInteresseColaborador, long interesseId, string colaboradorCpf, DateTime dataAlteracao)
        {
            return new InteresseColaboradorModel(_log, _unitOfWork, _repositoryInteresseColaboradorModel)
            {
                InteresseColaboradorDTO = new InteresseColaboradorDTO()
                {
                    Id = idInteresseColaborador,
                    Data = dataAlteracao
                },
                InteresseId = interesseId,
                ColaboradorCpf = colaboradorCpf
            };
        }

        public IInteresseColaboradorModel buildInteresseColaboradorModel()
        {
            return new InteresseColaboradorModel(_log, _unitOfWork, _repositoryInteresseColaboradorModel);
        }

        public IInteresseModel buildInteresseModel(long id, string descricao, long usuarioCriacaoId)
        {
            return new InteresseModel(_log, _unitOfWork, _repositoryInteresseModel)
            {
                InteresseDTO = new InteresseDTO()
                {
                    IdInteresse = id,
                    Descricao = descricao,
                    UsuarioCriacaoId = usuarioCriacaoId
                }
            };
        }

        public IInteresseModel buildInteresseModel()
        {
            return new InteresseModel(_log, _unitOfWork, _repositoryInteresseModel);
        }

        public IInteresseModel buildInteresseModel(long usuarioCriacaoId)
        {
            return new InteresseModel(_log, _unitOfWork, _repositoryInteresseModel)
            {
                InteresseDTO = new InteresseDTO()
                {
                    UsuarioCriacaoId = usuarioCriacaoId
                }
            };
        }
    }
}