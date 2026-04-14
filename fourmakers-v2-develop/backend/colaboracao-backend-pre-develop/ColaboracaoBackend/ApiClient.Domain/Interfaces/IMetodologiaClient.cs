using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Metodologia;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiClient.Domain.Interfaces
{
    public interface IMetodologiaClient
    {
        Task<ItemPerfilDTO> GetMetodologiaById(long id, string tokenUsuario);
        Task<List<MetodologiaColaboradorDTO>> ListarMetodologiasColaborador(string cpf, string tokenUsuario);
        Task<StatusResult> RemoverMetodologiaColaborador(string token, string cpf, long id);
        Task<List<ItemPerfilResult>> InserirMetodologiaColaborador(string cpf, List<AdicionarRemoverItemDTO> dtos);
        Task<ItemPerfilResult> AlterarMetodologiaColaborador(AdicionarRemoverItemDTO dtos, string token);
        Task<StatusResult> MergeMetodologiaColaborador(MergeItemPerfilDTO dtos, string token);
        Task<List<KeyValuePair<string, long>>> GetMetodologiaInfoByDescricao(List<string> metodologias, string token);
    }
}