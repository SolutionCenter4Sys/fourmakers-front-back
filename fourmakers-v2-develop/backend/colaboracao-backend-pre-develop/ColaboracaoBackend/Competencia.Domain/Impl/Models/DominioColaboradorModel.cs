using Colaboracao.Core;
using Competencia.Domain.Interfaces.Factorys;
using Competencia.Domain.Interfaces.Models;
using Core.Domain;
using DataTransferObject.Domain.Dominio;
using System;
using System.Collections.Generic;

namespace Competencia.Domain.Impl.Models
{
    public class DominioColaboradorModel : IDominioColaboradorModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<IDominioColaboradorModel, IDominioDomainFactory> _repositoryDominioColaborador;
        private readonly IDominioGenericoRepository<IDominioColaboradorModel, IDominioDomainFactory> _repositoryDominioGenerico;
        private readonly ILogCore _log;

        public DominioColaboradorModel(ILogCore log, IUnitOfWork unitOfWork, IRepository<IDominioColaboradorModel, IDominioDomainFactory> repositoryDominioColaborador, IDominioGenericoRepository<IDominioColaboradorModel, IDominioDomainFactory> repositoryDominioGenerico)
        {
            _log = log;
            _unitOfWork = unitOfWork;
            _repositoryDominioColaborador = repositoryDominioColaborador;
            _repositoryDominioGenerico = repositoryDominioGenerico;
            DominioColaboradorDTO = new DominioColaboradorDTO();
        }

        public DominioColaboradorDTO DominioColaboradorDTO { get; set; }

        public IDominioColaboradorModel AlteraDominioColaborador()
        {
            using (var dbTrans = _unitOfWork.BeginTransaction())
            {
                try
                {
                    _repositoryDominioGenerico.AlterarDominioColaborador(this);

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

        public List<IDominioColaboradorModel> ListDominioColaborador(IDominioDomainFactory factory)
        {
            return _repositoryDominioColaborador.ListModel(this, factory);
        }

        public void RemoveDominioColaborador()
        {
            _repositoryDominioColaborador.DeleteModel(this);
        }

        public IDominioColaboradorModel SaveModel()
        {
            using (var dbTrans = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var ret = _repositoryDominioColaborador.SaveModel(this);
                    this.DominioColaboradorDTO.Id = ret.DominioColaboradorDTO.Id;

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