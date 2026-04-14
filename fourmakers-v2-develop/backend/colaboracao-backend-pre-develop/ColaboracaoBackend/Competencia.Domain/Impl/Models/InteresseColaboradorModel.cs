using Colaboracao.Core;
using Competencia.Domain.Interfaces.Factorys;
using Competencia.Domain.Interfaces.Models;
using Core.Domain;
using DataTransferObject.Domain.Interesse;
using System;
using System.Collections.Generic;

namespace Competencia.Domain.Impl.Models
{
    public class InteresseColaboradorModel : IInteresseColaboradorModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<IInteresseColaboradorModel, IInteresseDomainFactory> _repository;
        private readonly ILogCore _log;

        public InteresseColaboradorModel(ILogCore log, IUnitOfWork unitOfWork, IRepository<IInteresseColaboradorModel, IInteresseDomainFactory> repository)
        {
            _log = log;
            _unitOfWork = unitOfWork;
            _repository = repository;
            InteresseColaboradorDTO = new InteresseColaboradorDTO();
        }

        public InteresseColaboradorDTO InteresseColaboradorDTO { get; set; } = new InteresseColaboradorDTO();
        public long InteresseId { get; set; }
        public string ColaboradorCpf { get; set; }
        public int TipoId { get; set; } 
        public int SkillId { get; set; } 
        public bool InteresseAtivo { get; set; } // 1 - Ativo, 0 - Inativo

        public IInteresseColaboradorModel SaveModel()
        {
            using (var dbTrans = _unitOfWork.BeginTransaction())
            {
                try
                {
                    //Salva o interesse do colaborador
                    var ret = _repository.SaveModel(this);
                    
                    this.InteresseColaboradorDTO.Id = ret.InteresseColaboradorDTO.Id;

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

        public IInteresseColaboradorModel SaveModelDapper()
        {
            using (var dbTrans = _unitOfWork.BeginTransaction())
            {
                try
                {
                    //Salva o interesse do colaborador
                    var ret = _repository.SaveModelDapper(this);

                    this.InteresseColaboradorDTO.Id = ret.InteresseColaboradorDTO.Id;

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

        public List<IInteresseColaboradorModel> ListarInteressesDeColaboradores(IInteresseDomainFactory factory)
        {
            return _repository.ListModel(this, factory);
        }

        public void RemoverInteresseDoColaborador()
        {
            _repository.DeleteModel(this);
        }
    }
}