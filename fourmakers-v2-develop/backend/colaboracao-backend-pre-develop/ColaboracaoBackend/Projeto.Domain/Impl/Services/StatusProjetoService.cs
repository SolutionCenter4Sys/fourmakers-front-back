using Core.Domain.Projeto;
using DataTransferObject.Domain.Projeto;
using Projeto.Domain.Interfaces.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

using Logs.Infra.Attributes;

namespace Projeto.Domain.Impl.Services
{
    [LogDomainClass]
    public class StatusProjetoService : IStatusProjetoService
    {
        private readonly IStatusProjetoRepository _statusProjetoRepository;
        public StatusProjetoService(IStatusProjetoRepository statusProjetoRepository)
        {
            _statusProjetoRepository = statusProjetoRepository;
        }

        Task<List<StatusProjetosDTO>> IStatusProjetoService.ListarStatus(int orgId)
        {
            return _statusProjetoRepository.ListarStatusProjeto(orgId);
        }
    }
}