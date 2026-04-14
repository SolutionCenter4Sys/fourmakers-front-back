using DataTransferObject.Domain.Colaborador;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Competencia.Domain.Interfaces.Services
{
    public interface ISkillGenericService
    {
        /// <summary>
        /// Busca ou cria skills de forma genérica, retornando um dicionário com a descrição normalizada e o ID
        /// </summary>
        /// <param name="skills">Lista de descrições das skills</param>
        /// <param name="tipoSkill">Tipo da skill (HardSkill, SoftSkill, Metodologia, Dominio)</param>
        /// <param name="cpf">CPF do usuário que está criando (usado se precisar criar novas skills)</param>
        /// <returns>Lista de pares chave-valor com descrição normalizada e ID da skill</returns>
        Task<List<KeyValuePair<string, long>>> GetSkillInfoByDescricaoAsync(List<string> skills, TipoSkillEnum tipoSkill, string cpf);
    }
}
