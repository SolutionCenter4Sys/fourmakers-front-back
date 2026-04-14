using DataTransferObject.Domain.Colaborador;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colaborador.Domain.Interfaces.Services
{
    public interface IPassaporteColaboradorService
    {
        Task<int> AdicionarPassaporteColaborador(string cpfColaborador, PassaporteColaboradorDTO passaporteColaboradorDTO);

        Task<bool> AlterarPassaporteColaborador(string cpfColaborador, PassaporteColaboradorDTO passaporteColaboradorDTO);

        Task<bool> RemoverPassaporteColaborador(string cpfColaborador, int idPassaporteColaborador);

        Task<List<PassaporteColaboradorDTO>> ObterPassaportesPorCpfColaborador(string cpfColaborador);
    }
}