using Colaboracao.Core;
using Competencia.Domain.Interfaces.Factorys;
using Competencia.Domain.Interfaces.Models;
using Core.Domain;
using DataTransferObject.Domain.Hobby;
using System;
using System.Collections.Generic;

namespace Competencia.Domain.Impl.Models
{
    public class HobbyModel : IHobbyModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<IHobbyModel, IHobbyDomainFactory> _repository;
        private readonly ILogCore _log;

        public HobbyModel(ILogCore log, IUnitOfWork unitOfWork, IRepository<IHobbyModel, IHobbyDomainFactory> repository)
        {
            _log = log;
            _unitOfWork = unitOfWork;
            _repository = repository;
            HobbyDTO = new HobbyDTO();
        }

        public HobbyDTO HobbyDTO { get; set; }
        public string CpfUsuario { get; set; }

        public List<IHobbyModel> Listar(string busca, int cursor, int limite, IHobbyDomainFactory factory)
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

        public IHobbyModel SaveModel()
        {
            using (var dbTrans = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var ret = _repository.SaveModel(this);
                    this.HobbyDTO.IdHobby = ret.HobbyDTO.IdHobby;

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

        public IHobbyModel GetById(long id)
        {
            this.HobbyDTO.IdHobby = id;
            return _repository.GetModel(this);
        }

        public IHobbyModel GetByCpf(string cpfUsuario, IHobbyDomainFactory factory)
        {
            this.CpfUsuario = cpfUsuario;
            return _repository.GetModelByKey(this.CpfUsuario, factory);
        }
    }
}