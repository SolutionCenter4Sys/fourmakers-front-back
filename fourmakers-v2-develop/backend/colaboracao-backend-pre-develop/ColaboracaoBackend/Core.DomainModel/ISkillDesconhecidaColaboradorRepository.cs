using DataTransferObject.Domain.SkillDesconhecida;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain
{
    public interface ISkillDesconhecidaColaboradorRepository
    {
        public void InserirSkillDesconhecidaColaborador(SkillDesconhecidaColaboradorDTO skillDesconhecidaColaborador);
        Task<List<SkillDesconhecidaColaboradorDTO>> ListarSkillDesconhecidasColaboador(string codInternoColaborador);
    }
}