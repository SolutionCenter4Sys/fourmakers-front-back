using Colaboracao.Core;
using Colaborador.Domain.Impl.Models;
using Colaborador.Domain.Interfaces.Factorys;
using Colaborador.Domain.Interfaces.Models;
using Core.Domain;
using DataTransferObject.Domain.Colaborador;

namespace Colaborador.Domain.Impl.Factorys
{
    public class NacionalidadeDomainFactory : INacionalidadeDomainFactory
    {
        private readonly ILogCore _log;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INacionalidadeRepository<INacionalidadeModel, INacionalidadeDomainFactory> _nacionalidadeRepository;

        public NacionalidadeDomainFactory(ILogCore log,
                                                     IUnitOfWork unitOfWork,
                                                     INacionalidadeRepository<INacionalidadeModel, INacionalidadeDomainFactory> nacionalidadeRepository)
        {
            _log = log;
            _unitOfWork = unitOfWork;
            _nacionalidadeRepository = nacionalidadeRepository;
        }

        public INacionalidadeModel buildNacionalidadeModel()
        {
            return new NacionalidadeModel(_log, _unitOfWork, _nacionalidadeRepository, new NacionalidadeDTO());
        }

        public INacionalidadeModel buildNacionalidadeModel(int id, string descricao)
        {
            return new NacionalidadeModel(_log, _unitOfWork, _nacionalidadeRepository, new NacionalidadeDTO())
            {
                NacionalidadeDTO = new NacionalidadeDTO()
                {
                    Id = id,
                    Descricao = descricao
                }
            };
        }
    }
}