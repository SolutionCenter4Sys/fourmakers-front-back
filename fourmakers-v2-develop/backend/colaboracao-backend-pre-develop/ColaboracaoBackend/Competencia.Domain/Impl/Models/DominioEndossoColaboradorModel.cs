using Colaboracao.Core;
using Competencia.Domain.Interfaces.Factorys;
using Competencia.Domain.Interfaces.Models;
using Core.Domain;
using DataTransferObject.Domain.Endosso;
using System;
using System.Collections.Generic;

namespace Competencia.Domain.Impl.Models
{
    public class DominioEndossoColaboradorModel : IDominioEndossoColaboradorModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<IDominioEndossoColaboradorModel, IDominioDomainFactory> _repositoryDominioEndossoColaborador;
        private readonly ILogCore _log;

        public DominioEndossoColaboradorModel(ILogCore log, IUnitOfWork unitOfWork, IRepository<IDominioEndossoColaboradorModel, IDominioDomainFactory> repositoryDominioEndossoColaborador)
        {
            _log = log;
            _unitOfWork = unitOfWork;
            _repositoryDominioEndossoColaborador = repositoryDominioEndossoColaborador;
            EndossoConcedido = new List<EndossoColaboradorDTO>();
        }

        public long IdDominioColaborador { get; set; }
        public string cpfColaborador { get; set; }
        public DateTime dataEndosso { get; set; }
        public int? TipoEndossoId { get; set; }
        public List<EndossoColaboradorDTO> EndossoConcedido { get; set; }

        public List<IDominioEndossoColaboradorModel> GetEndossoConcedido(IDominioEndossoColaboradorModel model, IDominioDomainFactory factory)
        {
            try
            {
                return _repositoryDominioEndossoColaborador.ListModel(model, factory);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}