using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.SkillDesconhecida;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Competencia.Domain.Interfaces.Services
{
    public interface ISkillDesconhecidaService
    {
        /// <summary>
        /// Insere uma nova skill desconhecida para um colaborador
        /// </summary>
        /// <param name="descricao">Descrição da skill desconhecida</param>
        /// <param name="codInternoColaborador">Código interno do colaborador</param>
        void InserirSkillDesconhecidaColaborador(int idSkillDesconhecida, long? nivelId, string cpf, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL);

        /// <summary>
        /// Lista todas as skills desconhecidas de um colaborador
        /// </summary>
        /// <param name="codInternoColaborador">Código interno do colaborador</param>
        /// <returns>Lista de skills desconhecidas do colaborador</returns>
        Task<List<SkillDesconhecidaColaboradorDTO>> ListarSkillDesconhecidasColaborador(string codInternoColaborador);

        Task<List<SkillDesconhecidaDTO>> ListarSkillsDesconhecidas(string busca, int limite);
        Task<SkillDesconhecidaDTO> InserirSkillDesconhecida(string descricao);
        Task<List<KeyValuePair<string, int>>> GetSkillDesconhecidaInfoByDescricao(List<string> skills);
    }
}