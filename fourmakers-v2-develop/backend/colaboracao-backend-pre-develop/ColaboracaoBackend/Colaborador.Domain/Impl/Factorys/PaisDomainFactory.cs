using Colaboracao.Core;
using Colaborador.Domain.Impl.Models;
using Colaborador.Domain.Interfaces.Factorys;
using Colaborador.Domain.Interfaces.Models;
using Core.Domain;
using DataTransferObject.Domain.Colaborador;

namespace Colaborador.Domain.Impl.Factorys
{
    public class PaisDomainFactory : IPaisDomainFactory
    {
        private readonly ILogCore _log;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaisRepository<IPaisModel, IPaisDomainFactory> _paisRepository;

        public PaisDomainFactory(ILogCore log,
                                 IUnitOfWork unitOfWork,
                                 IPaisRepository<IPaisModel, IPaisDomainFactory> paisRepository)
        {
            _log = log;
            _unitOfWork = unitOfWork;
            _paisRepository = paisRepository;
        }

        public IPaisModel buildPaisModel()
        {
            return new PaisModel(_log, _unitOfWork, _paisRepository, new PaisDTO());
        }

        public IPaisModel buildPaisModel(int id, string descricao)
        {
            return new PaisModel(_log, _unitOfWork, _paisRepository, new PaisDTO())
            {
                PaisDTO = new PaisDTO()
                {
                    Id = id,
                    Descricao = descricao
                }
            };
        }
    }
}