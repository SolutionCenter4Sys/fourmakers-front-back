using Colaboracao.Core;
using Colaborador.Domain.Impl.Models;
using Colaborador.Domain.Interfaces.Factorys;
using Colaborador.Domain.Interfaces.Models;
using Core.Domain;
using DataTransferObject.Domain.Dependentes;
using System;

namespace Colaborador.Domain.Impl.Factorys
{
    public class DependenteDomainFactory : IDependenteDomainFactory
    {
        private readonly ILogCore _log;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDependenteRepository<IDependenteModel, IDependenteDomainFactory> _repositoryDependenteModel;

        public DependenteDomainFactory(ILogCore log, IUnitOfWork unitOfWork, IDependenteRepository<IDependenteModel, IDependenteDomainFactory> repositoryDependenteModel)
        {
            _log = log;
            _unitOfWork = unitOfWork;
            _repositoryDependenteModel = repositoryDependenteModel;
        }

        public IDependenteModel buildDependenteModel()
        {
            return new DependenteModel(_log, _unitOfWork, _repositoryDependenteModel);
        }

        public IDependenteModel buildDependenteModel(long usuarioCriacaoId)
        {
            return new DependenteModel(_log, _unitOfWork, _repositoryDependenteModel)
            {
                DependentesDTO = new DependentesDTO()
                {
                    UsuarioCriacaoId = usuarioCriacaoId
                }
            };
        }

        public IDependenteModel buildDependenteModel(long idDependente, string nomeCompletoDependente, DateTime dataNascimentoDependente, string rgDependente, string cpfDependente, sbyte portadorDeficienciaDependente, string requerAjudaQualDependente, int tipoDependenteId, string descricaoTipoDependente, int quantidadeDependentes)
        {
            return new DependenteModel(_log, _unitOfWork, _repositoryDependenteModel)
            {
                DependentesDTO = new DataTransferObject.Domain.Dependentes.DependentesDTO()
                {
                    Id = idDependente,
                    NomeCompleto = nomeCompletoDependente,
                    DataNascimento = dataNascimentoDependente,
                    Rg = rgDependente,
                    Cpf = cpfDependente,
                    TipoDependente = new TipoDependenteDTO()
                    {
                        Id = tipoDependenteId,
                        Descricao = descricaoTipoDependente
                    },
                    PortadorDeficiencia = portadorDeficienciaDependente,
                    RequerAjudaQual = requerAjudaQualDependente,
                },
                QuantidadeDependentes = quantidadeDependentes
            };
        }
    }
}