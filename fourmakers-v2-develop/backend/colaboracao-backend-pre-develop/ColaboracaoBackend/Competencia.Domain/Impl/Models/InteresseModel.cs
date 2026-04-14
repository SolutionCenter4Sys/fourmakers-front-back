using Colaboracao.Core;
using Competencia.Domain.Interfaces.Factorys;
using Competencia.Domain.Interfaces.Models;
using Core.Domain;
using DataTransferObject.Domain.Interesse;
using System;
using System.Collections.Generic;

namespace Competencia.Domain.Impl.Models
{
    public class InteresseModel : IInteresseModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<IInteresseModel, IInteresseDomainFactory> _repository;
        private readonly ILogCore _log;

        public InteresseModel(ILogCore log, IUnitOfWork unitOfWork, IRepository<IInteresseModel, IInteresseDomainFactory> repository)
        {
            _log = log;
            _unitOfWork = unitOfWork;
            _repository = repository;
            InteresseDTO = new InteresseDTO();
        }

        public InteresseDTO InteresseDTO { get; set; }
        public string CpfUsuario { get; set; }

        public List<IInteresseModel> Listar(string busca, int cursor, int limite, IInteresseDomainFactory factory)
        {
            try
            {
                return _repository.ListModel(busca, cursor, limite, factory);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IInteresseModel SaveModel()
        {
            using (var dbTrans = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var ret = _repository.SaveModel(this);
                    this.InteresseDTO.IdInteresse = ret.InteresseDTO.IdInteresse;

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

        public IInteresseModel GetById(long id)
        {
            this.InteresseDTO.IdInteresse = id;
            return _repository.GetModel(this);
        }

        public IInteresseModel GetByCpf(string cpfUsuario, IInteresseDomainFactory factory)
        {
            this.CpfUsuario = cpfUsuario;
            return _repository.GetModelByKey(this.CpfUsuario, factory);
        }
    }
}