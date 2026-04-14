using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Hobby;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiClient.Domain.Interfaces
{
    public interface IHobbyClient
    {
        Task<ItemPerfilDTO> GetHobbyById(long id, string tokenUsuario);
        Task<List<HobbyColaboradorDTO>> ListarHobbiesColaborador(string cpf, string tokenUsuario);
        Task<StatusResult> RemoverHobbieColaborador(string token, string cpf, long id);
        Task<List<ItemPerfilResult>> InserirHobbieColaborador(string cpf, List<AdicionarRemoverItemDTO> dtos);
        Task<StatusResult> MergeHobbieColaborador(MergeItemPerfilDTO dtos, string token);
    }
}