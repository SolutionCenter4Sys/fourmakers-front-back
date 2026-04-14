using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Softskill;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiClient.Domain.Interfaces
{
    public interface ISoftskillClient
    {
        Task<ItemPerfilDTO> GetSoftskillById(long id, string tokenUsuario);
        Task<List<SoftskillColaboradorDTO>> ListarSoftskillsColaborador(string cpf, string tokenUsuario);
        Task<StatusResult> RemoverSoftskillColaborador(string token, string cpf, long id);
        Task<List<ItemPerfilResult>> InserirSoftskillColaborador(string cpf, List<AdicionarRemoverItemDTO> dtos);
        Task<ItemPerfilResult> AlterarSoftskillColaborador(AdicionarRemoverItemDTO dtos, string token);
        Task<List<KeyValuePair<string, long>>> GetSoftSkillInfoByDescricao(List<string> softSkills, string token);
    }
}