using Colaboracao.Core;
using Colaborador.Domain.Interfaces.Factorys;
using Colaborador.Domain.Interfaces.Models;
using Core.Domain;
using DataTransferObject.Domain.Colaborador;
using System;
using System.Collections.Generic;

namespace Colaborador.Domain.Impl.Models
{
    public class NacionalidadeModel : INacionalidadeModel
    {
        private readonly ILogCore _log;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INacionalidadeRepository<INacionalidadeModel, INacionalidadeDomainFactory> _nacionalidadeRepository;

        public NacionalidadeModel(ILogCore log,
                                  IUnitOfWork unitOfWork,
                                  INacionalidadeRepository<INacionalidadeModel, INacionalidadeDomainFactory> nacionalidadeRepository,
                                  NacionalidadeDTO nacionalidadeDTO)
        {
            _log = log;
            _unitOfWork = unitOfWork;
            _nacionalidadeRepository = nacionalidadeRepository;
            NacionalidadeDTO = nacionalidadeDTO;
        }

        public NacionalidadeDTO NacionalidadeDTO { get; set; }

        public List<INacionalidadeModel> BuscarTodos(INacionalidadeDomainFactory nacionalidadeDomainFactory)
        {
            try
            {
                return _nacionalidadeRepository.BuscarTodos(nacionalidadeDomainFactory);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}