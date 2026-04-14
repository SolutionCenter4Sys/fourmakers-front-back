using Colaboracao.Core;
using Competencia.Domain.Interfaces.Factorys;
using Competencia.Domain.Interfaces.Models;
using Core.Domain;
using DataTransferObject.Domain.Hobby;
using System;
using System.Collections.Generic;

namespace Competencia.Domain.Impl.Models
{
    public class HobbyColaboradorModel : IHobbyColaboradorModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<IHobbyColaboradorModel, IHobbyDomainFactory> _repository;
        private readonly ILogCore _log;

        public HobbyColaboradorModel(ILogCore log, IUnitOfWork unitOfWork, IRepository<IHobbyColaboradorModel, IHobbyDomainFactory> repository)
        {
            _log = log;
            _unitOfWork = unitOfWork;
            _repository = repository;
            HobbyColaboradorDTO = new HobbyColaboradorDTO();
        }

        public HobbyColaboradorDTO HobbyColaboradorDTO { get; set; }
        public long HobbyId { get; set; }
        public string ColaboradorCpf { get; set; }

        public IHobbyColaboradorModel SaveModel()
        {
            using (var dbTrans = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var ret = _repository.SaveModel(this);
                    this.HobbyColaboradorDTO.Id = ret.HobbyColaboradorDTO.Id;

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

        public List<IHobbyColaboradorModel> ListarHobbiesDeColaboradores(IHobbyDomainFactory factory)
        {
            return _repository.ListModel(this, factory);
        }

        public void RemoverHobbyDoColaborador()
        {
            _repository.DeleteModel(this);
        }
    }
}