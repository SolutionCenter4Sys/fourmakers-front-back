using Colaboracao.Core;
using Colaborador.Domain.Interfaces.Factorys;
using Colaborador.Domain.Interfaces.Models;
using Core.Domain;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Dependentes;
using System;
using System.Collections.Generic;

namespace Colaborador.Domain.Impl.Models
{
    public class DependenteModel : IDependenteModel
    {
        private readonly ILogCore _log;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDependenteRepository<IDependenteModel, IDependenteDomainFactory> _repositoryDependenteModel;
        private ILogCore log;
        private IUnitOfWork unitOfWork;

        public DependenteModel(ILogCore log, IUnitOfWork unitOfWork, IDependenteRepository<IDependenteModel, IDependenteDomainFactory> repositoryDependenteModel)
        {
            _log = log;
            _unitOfWork = unitOfWork;
            _repositoryDependenteModel = repositoryDependenteModel;
            DependentesDTO = new DependentesDTO();
            AlterarDependentesDTO = new AlterarDependentesDTO();
            AdicionarDependentesDTO = new AdicionarDependentesDTO();
            RemoverDependentesDTO = new RemoverDependentesDTO();
            Colaborador = new ColaboradorDTO();
        }

        public DependentesDTO DependentesDTO { get; set; }
        public AdicionarDependentesDTO AdicionarDependentesDTO { get; set; }
        public AlterarDependentesDTO AlterarDependentesDTO { get; set; }
        public RemoverDependentesDTO RemoverDependentesDTO { get; set; }
        public ColaboradorDTO Colaborador { get; set; }
        public int QuantidadeDependentes { get; set; }

        public IDependenteModel SaveModel(string cpf)
        {
            using (var dbTrans = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var ret = _repositoryDependenteModel.SaveModel(this, cpf);
                    this.AdicionarDependentesDTO.TipoDependenteId = ret.AdicionarDependentesDTO.TipoDependenteId;

                    dbTrans.Commit();

                    return this;
                }
                catch (Exception)
                {
                    dbTrans.Rollback();
                    throw;
                }
            }
        }

        public IDependenteModel UpdateModel(string cpf)
        {
            var ret = _repositoryDependenteModel.UpdateModel(this, cpf);
            this.AlterarDependentesDTO.Id = ret.AlterarDependentesDTO.Id;
            this.AlterarDependentesDTO.TipoDependente.Id = ret.AlterarDependentesDTO.TipoDependente.Id;
            this.AlterarDependentesDTO.TipoDependente.Descricao = ret.AlterarDependentesDTO.TipoDependente.Descricao;

            return this;
        }

        public IDependenteModel DeleteModel(string cpf)
        {
            using (var dbTrans = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var ret = _repositoryDependenteModel.DeleteModel(this, cpf);
                    this.RemoverDependentesDTO.Id = ret.RemoverDependentesDTO.Id;

                    dbTrans.Commit();

                    return this;
                }
                catch (Exception)
                {
                    dbTrans.Rollback();
                    throw;
                }
            }
        }

        public List<IDependenteModel> GetModel(string cpfColaborador, IDependenteDomainFactory factory)
        {
            this.Colaborador.Cpf = cpfColaborador;

            return _repositoryDependenteModel.GetModel(this, factory);
        }

        public IDependenteModel GetByCpf(string cpfUsuario, IDependenteDomainFactory factory)
        {
            this.RemoverDependentesDTO.ColaboradorCpf = cpfUsuario;
            return _repositoryDependenteModel.GetModelByKey(this.RemoverDependentesDTO.ColaboradorCpf, factory);
        }
    }
}