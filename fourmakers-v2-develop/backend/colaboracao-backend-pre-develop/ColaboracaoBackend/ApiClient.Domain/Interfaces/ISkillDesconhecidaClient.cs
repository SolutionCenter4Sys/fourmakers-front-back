using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiClient.Domain.Interfaces
{
    public interface ISkillDesconhecidaClient
    {
        Task<List<KeyValuePair<string, long>>> GetSkillDesconhecidaInfoByDescricao(List<string> skills, string token);
    }
}