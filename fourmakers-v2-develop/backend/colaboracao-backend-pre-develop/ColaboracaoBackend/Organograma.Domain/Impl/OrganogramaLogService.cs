using Colaboracao.Core.Interfaces;
using Core.Domain.Organograma;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Usuario;
using Organograma.Domain.Interfaces;

using Logs.Infra.Attributes;

namespace Organograma.Domain.Impl
{
    [LogDomainClass]
    public class OrganogramaLogService : IOrganogramaLogService
    {
        private readonly IOrganogramaLogRepository _organogramaLogRepository;


        public OrganogramaLogService(IOrganogramaLogRepository organogramaLogRepository)
        {
            _organogramaLogRepository = organogramaLogRepository;
        }
             
        //public async Task OrganogramaInserirLog(OrganogramaDeptoListarPorIdResponseDTO paramAtual, OrganogramaDeptoListarPorIdResponseDTO paramAlterado, string cpf, string tipo)
        //    => await _organogramaLogRepository.OrganogramaInserirLog(paramAtual, paramAlterado, cpf, tipo);


        //public async Task PosicaoInserirLog(OrganogramaPosicaoListarPorIdResponseDTO param, string cpf, string tipo)
        //    => await _organogramaLogRepository.PosicaoInserirLog(param, cpf, tipo);


    }
} 