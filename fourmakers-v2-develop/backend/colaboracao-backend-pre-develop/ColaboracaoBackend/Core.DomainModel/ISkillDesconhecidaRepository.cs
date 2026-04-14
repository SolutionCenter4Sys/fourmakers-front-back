using DataTransferObject.Domain.SkillDesconhecida;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.DomainModel
{
    public interface ISkillDesconhecidaRepository
    {
        Task<SkillDesconhecidaDTO> AddSkillDesconhecida(string descricao, long usuarioId);
        Task<long> GetUsuarioCriacaoId(string cpf);
        Task<List<SkillDesconhecidaDTO>> ListSkillDesconhecida(string busca, int limite);
    }
}