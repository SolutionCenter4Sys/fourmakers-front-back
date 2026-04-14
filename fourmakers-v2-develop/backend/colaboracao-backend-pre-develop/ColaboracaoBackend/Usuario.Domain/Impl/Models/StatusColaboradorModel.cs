using Colaboracao.Core;
using Core.Domain;
using DataTransferObject.Domain.Colaborador;
using Usuario.Domain.Interfaces.Factorys;
using Usuario.Domain.Interfaces.Models;

namespace Usuario.Domain.Impl.Models
{
    public class StatusColaboradorModel : IStatusColaboradorModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<IStatusColaboradorModel, IStatusColaboradorDomainFactory> _repository;
        private readonly ILogCore _log;

        public StatusColaboradorModel(ILogCore log, IUnitOfWork unitOfWork, IRepository<IStatusColaboradorModel, IStatusColaboradorDomainFactory> repository)
        {
            _log = log;
            _unitOfWork = unitOfWork;
            _repository = repository;
            Status = new StatusColaboradorDTO();
        }

        public string CpfUsuario { get; set; }
        public StatusColaboradorDTO Status { get; set; }

        public IStatusColaboradorModel GetStatusByCpfColaborador(string cpf)
        {
            this.CpfUsuario = cpf;
            var statusBanco = _repository.GetModel(this);

            if (statusBanco != null)
            {
                return statusBanco;
            }

            return null;
        }
    }
}