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
    public class PassaporteColaboradorService : IPassaporteColaboradorService
    {
        private readonly IPassaporteColaboradorRepository _passaporteColaboradorRepository;
        private readonly ITokens _token;
        private readonly IAspNetUser _aspNetUser;

        public PassaporteColaboradorService(
            IPassaporteColaboradorRepository passaporteColaboradorRepository,
            ITokens token,
            IAspNetUser aspNetUser)
        {
            _passaporteColaboradorRepository = passaporteColaboradorRepository;
            _token = token;
            _aspNetUser = aspNetUser;
        }

        public async Task<int> AdicionarPassaporteColaborador(string cpfColaborador, PassaporteColaboradorDTO passaporteColaboradorDTO)
        {
            return await _passaporteColaboradorRepository.AdicionarPassaporteColaborador(cpfColaborador, passaporteColaboradorDTO);
        }

        public async Task<bool> AlterarPassaporteColaborador(string cpfColaborador, PassaporteColaboradorDTO passaporteColaboradorDTO)
        {
            return await _passaporteColaboradorRepository.AlterarPassaporteColaborador(cpfColaborador, passaporteColaboradorDTO);
        }

        public async Task<bool> RemoverPassaporteColaborador(string cpfColaborador, int idPassaporteColaborador)
        {
            return await _passaporteColaboradorRepository.RemoverPassaporteColaborador(cpfColaborador, idPassaporteColaborador);
        }

        public async Task<List<PassaporteColaboradorDTO>> ObterPassaportesPorCpfColaborador(string cpfColaborador)
        {
            return await _passaporteColaboradorRepository.ObterPassaportesPorCpfColaborador(cpfColaborador);
        }
    }
}