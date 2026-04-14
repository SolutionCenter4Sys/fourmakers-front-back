using Colaboracao.Core;
using Colaborador.Domain.Impl.Models;
using Colaborador.Domain.Interfaces.Factorys;
using Colaborador.Domain.Interfaces.Models;
using Core.Domain;

namespace Colaborador.Domain.Impl.Factorys
{
    public class FotoDomainFactory : IFotoDomainFactory
    {
        private readonly ILogCore _log;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<IFotoModel, IFotoDomainFactory> _repositoryFotoModel;

        public FotoDomainFactory(ILogCore log, IUnitOfWork unitOfWork, IRepository<IFotoModel, IFotoDomainFactory> repositoryFotoModel)
        {
            _log = log;
            _unitOfWork = unitOfWork;
            _repositoryFotoModel = repositoryFotoModel;
        }

        public IFotoModel buildFotoModel()
        {
            return new FotoModel(_log, _unitOfWork, _repositoryFotoModel);
        }
    }
}