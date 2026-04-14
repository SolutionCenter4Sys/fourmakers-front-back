using Colaboracao.Core;
using Competencia.Domain.Interfaces.Factorys;
using Competencia.Domain.Interfaces.Models;
using Core.Domain;
using DataTransferObject.Domain.Endosso;
using System;

namespace Competencia.Domain.Impl.Models
{
    public class DominioTipoEndossoModel : IDominioTipoEndossoModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<IDominioTipoEndossoModel, IDominioDomainFactory> _repositoryDominioTipoEndosso;
        private readonly ILogCore _log;

        public DominioTipoEndossoModel(ILogCore log, IUnitOfWork unitOfWork, IRepository<IDominioTipoEndossoModel, IDominioDomainFactory> repositoryDominioTipoEndosso)
        {
            _log = log;
            _unitOfWork = unitOfWork;
            _repositoryDominioTipoEndosso = repositoryDominioTipoEndosso;
            TipoEndossoDTO = new TipoEndossoDTO();
        }
        public TipoEndossoDTO TipoEndossoDTO { get; set; }

        public IDominioTipoEndossoModel GetTipoEndosso(IDominioTipoEndossoModel model)
        {
            try
            {
                return _repositoryDominioTipoEndosso.GetModel(model);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}