using Colaboracao.Core;
using Competencia.Domain.Interfaces.Factorys;
using Competencia.Domain.Interfaces.Models;
using Core.Domain;
using DataTransferObject.Domain.Dominio;
using System;
using System.Collections.Generic;

namespace Competencia.Domain.Impl.Models
{
    public class DominioModel : IDominioModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<IDominioModel, IDominioDomainFactory> _repositoryDominio;
        private readonly ILogCore _log;

        public DominioModel(ILogCore log, IUnitOfWork unitOfWork, IRepository<IDominioModel, IDominioDomainFactory> repositoryDominio)
        {
            _log = log;
            _unitOfWork = unitOfWork;
            _repositoryDominio = repositoryDominio;
            DominioDTO = new DominioDTO();
        }

        public DominioDTO DominioDTO { get; set; }

        public IDominioModel GetById(long id)
        {
            this.DominioDTO.Id = id;
            return _repositoryDominio.GetModel(this);
        }

        public IDominioModel GetUsuarioCriacaoId(string cpfColaborador, IDominioDomainFactory factory)
        {
            return _repositoryDominio.GetModelByKey(cpfColaborador, factory);
        }

        public List<IDominioModel> List(string busca, int cursor, int limite, IDominioDomainFactory factory)
        {
            try
            {
                return _repositoryDominio.ListModel(busca, cursor, limite, factory);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IDominioModel SaveModel()
        {
            using (var dbTrans = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var ret = _repositoryDominio.SaveModel(this);
                    this.DominioDTO.Id = ret.DominioDTO.Id;

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
    }
}