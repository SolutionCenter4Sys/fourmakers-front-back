using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Dominio;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiClient.Domain.Interfaces
{
    public interface IDominioClient
    {
        Task<ItemPerfilDTO> GetDominioById(long id, string tokenUsuario);
        Task<List<DominioColaboradorDTO>> ListarDominiosColaborador(string cpf, string tokenUsuario);
        Task<StatusResult> RemoverDominioColaborador(string token, string cpf, long id);
        Task<List<ItemPerfilResult>> InserirDominioColaborador(string cpf, List<AdicionarRemoverItemDTO> dtos);
        Task<ItemPerfilResult> AlterarDominioColaborador(AdicionarRemoverItemDTO dtos, string token);
        Task<StatusResult> MergeDominioColaborador(MergeItemPerfilDTO dtos, string token);
        Task<List<KeyValuePair<string, long>>> GetDominioInfoByDescricao(List<string> dominios, string token);
    }
}