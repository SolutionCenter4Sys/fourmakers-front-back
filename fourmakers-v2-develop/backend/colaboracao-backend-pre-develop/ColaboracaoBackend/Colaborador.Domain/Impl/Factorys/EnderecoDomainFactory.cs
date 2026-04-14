using Colaboracao.Core;
using Colaborador.Domain.Impl.Models;
using Colaborador.Domain.Interfaces.Factorys;
using Colaborador.Domain.Interfaces.Models;
using Core.Domain;

namespace Colaborador.Domain.Impl.Factorys
{
    public class EnderecoDomainFactory : IEnderecoDomainFactory
    {
        private readonly ILogCore _log;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<IEnderecoModel, IEnderecoDomainFactory> _repositoryEnderecoModel;

        public EnderecoDomainFactory(ILogCore log, IUnitOfWork unitOfWork, IRepository<IEnderecoModel, IEnderecoDomainFactory> repositoryEnderecoModel)
        {
            _log = log;
            _unitOfWork = unitOfWork;
            _repositoryEnderecoModel = repositoryEnderecoModel;
        }

        public IEnderecoModel buildEnderecoModel()
        {
            return new EnderecoModel(_log, _unitOfWork, _repositoryEnderecoModel);
        }
    }
}