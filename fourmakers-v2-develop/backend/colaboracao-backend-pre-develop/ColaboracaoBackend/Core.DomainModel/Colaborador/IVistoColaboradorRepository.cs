using DataTransferObject.Domain.Colaborador;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Colaborador
{
    public interface IVistoColaboradorRepository
    {
        Task<int> AdicionarVistoColaborador(string cpfColaborador, VistoColaboradorDTO vistoColaboradorDTO);

        Task<bool> AlterarVistoColaborador(string cpfColaborador, VistoColaboradorDTO vistoColaboradorDTO);

        Task<bool> RemoverVistoColaborador(string cpfColaborador, int idVistoColaborador);

        Task<List<VistoColaboradorDTO>> ObterVistosPorCpfColaborador(string cpfColaborador);
    }
}