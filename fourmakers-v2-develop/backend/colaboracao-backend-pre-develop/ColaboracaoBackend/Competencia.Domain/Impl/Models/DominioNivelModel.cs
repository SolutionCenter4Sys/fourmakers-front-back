using Colaboracao.Core;
using Competencia.Domain.Interfaces.Factorys;
using Competencia.Domain.Interfaces.Models;
using Core.Domain;
using DataTransferObject.Domain.Nivel;
using System;
using System.Collections.Generic;

namespace Competencia.Domain.Impl.Models
{
    public class DominioNivelModel : IDominioNivelModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<IDominioNivelModel, IDominioDomainFactory> _repositoryDominioNivel;
        private readonly ILogCore _log;

        public DominioNivelModel(ILogCore log, IUnitOfWork unitOfWork, IRepository<IDominioNivelModel, IDominioDomainFactory> repositoryDominioNivel)
        {
            _log = log;
            _unitOfWork = unitOfWork;
            _repositoryDominioNivel = repositoryDominioNivel;
            NivelDTO = new NivelDTO();
        }

        public NivelDTO NivelDTO { get; set; }
        public long DominioId { get; set; }

        public IDominioNivelModel GetDominioNivel(IDominioNivelModel model)
        {
            try
            {
                return _repositoryDominioNivel.GetModel(model);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<IDominioNivelModel> ListNivelDominio(IDominioDomainFactory factory)
        {
            try
            {
                var model = factory.buildDominioNivelModel();
                model.DominioId = _repositoryDominioNivel.GetModelByKey("DOMINIONEGOCIO", factory).DominioId;

                return _repositoryDominioNivel.ListModel(model, factory);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}