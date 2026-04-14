using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Interesse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiClient.Domain.Interfaces
{
    public interface IInteresseClient
    {
        Task<ItemPerfilDTO> GetInteresseById(long id, string tokenUsuario);
        Task<List<InteresseColaboradorDTO>> ListarInteressesColaborador(string cpf, string tokenUsuario);
        Task<StatusResult> RemoverInteresseColaborador(string token, string cpf, long id);
        Task<List<ItemPerfilResult>> InserirInteresseColaborador(string cpf, List<AdicionarRemoverItemDTO> dtos);
        Task<StatusResult> MergeInteresseColaborador(MergeItemPerfilDTO dtos, string token);
    }
}