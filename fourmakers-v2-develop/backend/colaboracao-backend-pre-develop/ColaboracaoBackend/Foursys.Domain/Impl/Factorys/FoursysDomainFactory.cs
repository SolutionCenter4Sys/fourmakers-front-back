using Colaboracao.Core;
using Core.Domain;
using DataTransferObject.Domain.Cargo;
using DataTransferObject.Domain.Diretoria;
using DataTransferObject.Domain.Foursys;
using Foursys.Domain.Impl.Models;
using Foursys.Domain.Interfaces.Factorys;
using Foursys.Domain.Interfaces.Models;

namespace Foursys.Domain.Impl.Factorys
{
    public class FoursysDomainFactory : IFoursysDomainFactory
    {
        private readonly ILogCore _log;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFoursysRepository<IFoursysModel, IFoursysDomainFactory> _repositoryFoursysModel;

        public FoursysDomainFactory(ILogCore log, IUnitOfWork unitOfWork, IFoursysRepository<IFoursysModel, IFoursysDomainFactory> repositoryFoursysModel)
        {
            _log = log;
            _unitOfWork = unitOfWork;
            _repositoryFoursysModel = repositoryFoursysModel;
        }

        public IFoursysModel buildFoursysCargoModel(int id, string cargo, int totalResultCount, int filteredResultCount)
        {
            return new FoursysModel(_log, _unitOfWork, _repositoryFoursysModel)
            {
                CargoDTO = new CargoDTO()
                {
                    Id = id,
                    Cargo = cargo
                },
                TotalResultCount = totalResultCount,
                FilteredResultCount = filteredResultCount
            };
        }

        public IFoursysModel buildFoursysCargoModel()
        {
            return new FoursysModel(_log, _unitOfWork, _repositoryFoursysModel);
        }

        public IFoursysModel buildFoursysDiretoriaModel(string id, string diretoria, int totalResultCount)
        {
            return new FoursysModel(_log, _unitOfWork, _repositoryFoursysModel)
            {
                DiretoriaDTO = new DiretoriaDTO()
                {
                    Id = id,
                    Diretoria = diretoria
                },
                TotalResultCount = totalResultCount
            };
        }

        public IFoursysModel buildFoursysDiretoriaModel()
        {
            return new FoursysModel(_log, _unitOfWork, _repositoryFoursysModel);
        }

        public IFoursysModel buildListarUnidades(string id, string descricao)
        {
            return new FoursysModel(_log, _unitOfWork, _repositoryFoursysModel)
            {
                UnidadesDTO = new UnidadesDTO()
                {
                    Id = id,
                    Descricao = descricao
                }
            };
        }

        public IFoursysModel buildListarUnidades()
        {
            return new FoursysModel(_log, _unitOfWork, _repositoryFoursysModel);
        }
    }
}