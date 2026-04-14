using Colaboracao.Core;
using Colaborador.Domain.Interfaces.Factorys;
using Colaborador.Domain.Interfaces.Models;
using Core.Domain;
using System;

namespace Colaborador.Domain.Impl.Models
{
    public class FotoModel : IFotoModel
    {
        private readonly ILogCore _log;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<IFotoModel, IFotoDomainFactory> _repositoryFotoModel;

        public FotoModel(ILogCore log, IUnitOfWork unitOfWork, IRepository<IFotoModel, IFotoDomainFactory> repositoryFotoModel)
        {
            _log = log;
            _unitOfWork = unitOfWork;
            _repositoryFotoModel = repositoryFotoModel;
        }

        public long Id { get; set; }
        public string Path { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAlteracao { get; set; }
        public sbyte Ativo { get; set; }

        public IFotoModel SaveModel()
        {
            try
            {
                var ret = _repositoryFotoModel.SaveModel(this);
                this.Id = ret.Id;

                return this;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}