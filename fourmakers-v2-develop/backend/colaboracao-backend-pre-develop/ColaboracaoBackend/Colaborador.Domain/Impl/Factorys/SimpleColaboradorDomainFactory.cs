using Colaboracao.Core;
using Colaborador.Domain.Impl.Models;
using Colaborador.Domain.Interfaces.Factorys;
using Colaborador.Domain.Interfaces.Models;
using Core.Domain;
using DataTransferObject.Domain.Colaborador;

namespace Colaborador.Domain.Impl.Factorys
{
    public class SimpleColaboradorDomainFactory : ISimpleColaboradorDomainFactory
    {
        private readonly ILogCore _log;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISimpleColaboradorRepository<ISimpleColaboradorModel, ISimpleColaboradorDomainFactory> _repositoryColaboradorModel;

        public SimpleColaboradorDomainFactory(ILogCore log, IUnitOfWork unitOfWork, ISimpleColaboradorRepository<ISimpleColaboradorModel, ISimpleColaboradorDomainFactory> repositoryColaboradorModel)
        {
            _log = log;
            _unitOfWork = unitOfWork;
            _repositoryColaboradorModel = repositoryColaboradorModel;
        }

        public ISimpleColaboradorModel buildSimpleColaboradorModel()
        {
            return new SimpleColaboradorModel(_log, _unitOfWork, _repositoryColaboradorModel, new SimpleColaboradorDTO());
        }

        public ISimpleColaboradorModel buildSimpleColaboradorModel(string cpf, string nome_completo)
        {
            return new SimpleColaboradorModel(
                _log,
                _unitOfWork,
                _repositoryColaboradorModel,
                new SimpleColaboradorDTO
                {
                    Cpf = cpf,
                    NomeCompleto = nome_completo
                });
        }
    }
}