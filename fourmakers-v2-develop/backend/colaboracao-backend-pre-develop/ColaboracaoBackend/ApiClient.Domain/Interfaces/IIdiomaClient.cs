using DataTransferObject.Domain.Idioma;
using DataTransferObject.Domain.SRS.Candidate;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiClient.Domain.Interfaces
{
    public interface IIdiomaClient
    {
        Task<List<SkillValuePair>> GetIdiomaInfoByDescricao(List<string> idiomas, string token);
        Task<List<IdiomaColaboradorDTO>> ListarIdiomaColaborador(string cpf, string tokenUsuario);
    }
}