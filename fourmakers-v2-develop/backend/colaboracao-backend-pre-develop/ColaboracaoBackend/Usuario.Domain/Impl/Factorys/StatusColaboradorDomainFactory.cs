using Colaboracao.Core;
using Core.Domain;
using DataTransferObject.Domain.Colaborador;
using Usuario.Domain.Impl.Models;
using Usuario.Domain.Interfaces.Factorys;
using Usuario.Domain.Interfaces.Models;

namespace Usuario.Domain.Impl.Factorys
{
    public class StatusColaboradorDomainFactory : IStatusColaboradorDomainFactory
    {
        private ILogCore _log;
        private IUnitOfWork _unitOfWork;
        private IRepository<IStatusColaboradorModel, IStatusColaboradorDomainFactory> _repositoryStatusModel;

        public StatusColaboradorDomainFactory(ILogCore log, IUnitOfWork unitOfWork, IRepository<IStatusColaboradorModel, IStatusColaboradorDomainFactory> repositoryStatusModel)
        {
            _log = log;
            _unitOfWork = unitOfWork;
            _repositoryStatusModel = repositoryStatusModel;
        }

        public IStatusColaboradorModel buildStatusModel(int statusId, string descricao, string cpfUsuario)
        {
            return new StatusColaboradorModel(_log, _unitOfWork, _repositoryStatusModel)
            {
                Status = new StatusColaboradorDTO
                {
                    Id = statusId,
                    Descricao = descricao
                },
                CpfUsuario = cpfUsuario
            };
        }
        public IStatusColaboradorModel buildStatusModel()
        {
            return new StatusColaboradorModel(_log, _unitOfWork, _repositoryStatusModel);
        }
    }
}