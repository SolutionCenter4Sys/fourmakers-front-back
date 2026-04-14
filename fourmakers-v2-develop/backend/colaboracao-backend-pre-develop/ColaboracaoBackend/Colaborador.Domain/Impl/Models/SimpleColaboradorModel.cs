using Colaboracao.Core;
using Colaborador.Domain.Interfaces.Factorys;
using Colaborador.Domain.Interfaces.Models;
using Core.Domain;
using DataTransferObject.Domain.Colaborador;
using System.Collections.Generic;
using System.Linq;

namespace Colaborador.Domain.Impl.Models
{
    public class SimpleColaboradorModel : ISimpleColaboradorModel
    {
        private readonly ILogCore _log;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISimpleColaboradorRepository<ISimpleColaboradorModel, ISimpleColaboradorDomainFactory> _repositoryColaboradorModel;

        public SimpleColaboradorModel(ILogCore log, IUnitOfWork unitOfWork, ISimpleColaboradorRepository<ISimpleColaboradorModel, ISimpleColaboradorDomainFactory> repositoryColaboradorModel, SimpleColaboradorDTO simpleColaboradorDTO)
        {
            _log = log;
            _unitOfWork = unitOfWork;
            _repositoryColaboradorModel = repositoryColaboradorModel;
            SimpleColaboradorDTO = simpleColaboradorDTO;
        }

        public SimpleColaboradorDTO SimpleColaboradorDTO { get; set; }

        public List<SimpleColaboradorDTO> BuscarListaColaboradores(string nomeCompleto, ISimpleColaboradorDomainFactory factory)
        {
            this.SimpleColaboradorDTO.NomeCompleto = nomeCompleto;
            var x = _repositoryColaboradorModel.BuscarListaColaboradores(nomeCompleto, factory);

            return x.Select(m => m.SimpleColaboradorDTO).ToList();
        }
    }
}