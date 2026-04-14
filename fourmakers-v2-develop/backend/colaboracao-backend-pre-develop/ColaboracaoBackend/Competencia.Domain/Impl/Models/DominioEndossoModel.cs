using Colaboracao.Core;
using Competencia.Domain.Interfaces.Factorys;
using Competencia.Domain.Interfaces.Models;
using Core.Domain;
using DataTransferObject.Domain.Endosso;
using System;

namespace Competencia.Domain.Impl.Models
{
    public class DominioEndossoModel : IDominioEndossoModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<IDominioEndossoModel, IDominioDomainFactory> _repositoryDominioEndosso;
        private readonly ILogCore _log;

        public DominioEndossoModel(ILogCore log, IUnitOfWork unitOfWork, IRepository<IDominioEndossoModel, IDominioDomainFactory> repositoryDominioEndosso)
        {
            _log = log;
            _unitOfWork = unitOfWork;
            _repositoryDominioEndosso = repositoryDominioEndosso;
            StatusEndossoDTO = new StatusEndossoDTO();
        }

        public long IdDominioColaborador { get; set; }
        public StatusEndossoDTO StatusEndossoDTO { get; set; }

        public IDominioEndossoModel GetDominioEndosso(IDominioEndossoModel model)
        {
            try
            {
                return _repositoryDominioEndosso.GetModel(model);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}