using Colaboracao.Core.Interfaces;
using Colaborador.Domain.Interfaces.Services;
using Core.Domain.Colaborador;
using DataTransferObject.Domain.Colaborador;
using System.Collections.Generic;
using System.Threading.Tasks;

using Logs.Infra.Attributes;

namespace Colaborador.Domain.Impl.Services
{
    [LogDomainClass]
    public class VistoColaboradorService : IVistoColaboradorService
    {
        private readonly IVistoColaboradorRepository _vistoColaboradorRepository;
        private readonly ITokens _token;
        private readonly IAspNetUser _aspNetUser;

        public VistoColaboradorService(
            IVistoColaboradorRepository vistoColaboradorRepository,
            ITokens token,
            IAspNetUser aspNetUser)
        {
            _vistoColaboradorRepository = vistoColaboradorRepository;
            _token = token;
            _aspNetUser = aspNetUser;
        }

        public async Task<int> AdicionarVistoColaborador(string cpfColaborador, VistoColaboradorDTO vistoColaboradorDTO)
        {
            return await _vistoColaboradorRepository.AdicionarVistoColaborador(cpfColaborador, vistoColaboradorDTO);
        }

        public async Task<bool> AlterarVistoColaborador(string cpfColaborador, VistoColaboradorDTO vistoColaboradorDTO)
        {
            return await _vistoColaboradorRepository.AlterarVistoColaborador(cpfColaborador, vistoColaboradorDTO);
        }

        public async Task<bool> RemoverVistoColaborador(string cpfColaborador, int idVistoColaborador)
        {
            return await _vistoColaboradorRepository.RemoverVistoColaborador(cpfColaborador, idVistoColaborador);
        }

        public async Task<List<VistoColaboradorDTO>> ObterVistosPorCpfColaborador(string cpfColaborador)
        {
            return await _vistoColaboradorRepository.ObterVistosPorCpfColaborador(cpfColaborador);
        }
    }
}