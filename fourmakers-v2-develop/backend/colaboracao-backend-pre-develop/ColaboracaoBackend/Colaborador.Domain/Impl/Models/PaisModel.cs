using Colaboracao.Core;
using Colaborador.Domain.Interfaces.Factorys;
using Colaborador.Domain.Interfaces.Models;
using Core.Domain;
using DataTransferObject.Domain.Colaborador;
using System;
using System.Collections.Generic;

namespace Colaborador.Domain.Impl.Models
{
    public class PaisModel : IPaisModel
    {
        private readonly ILogCore _log;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaisRepository<IPaisModel, IPaisDomainFactory> _paisRepository;

        public PaisModel(ILogCore log,
                         IUnitOfWork unitOfWork,
                         IPaisRepository<IPaisModel, IPaisDomainFactory> paisRepository,
                         PaisDTO paisDTO)
        {
            _log = log;
            _unitOfWork = unitOfWork;
            _paisRepository = paisRepository;
            PaisDTO = paisDTO;
        }

        public PaisDTO PaisDTO { get; set; }

        public List<IPaisModel> BuscarTodos(IPaisDomainFactory PaisDomainFactory)
        {
            try
            {
                return _paisRepository.BuscarTodos(PaisDomainFactory);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}