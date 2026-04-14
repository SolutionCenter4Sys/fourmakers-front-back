using Colaboracao.Helper.Enum;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill;
using System.Threading.Tasks;

namespace MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados.GestorExternoPerfilSkill
{
    public interface IGestorExternoPerfilSkillValidatorService
    {
        Task ValidaGestorExternoPerfilSkill(GestorExternoPerfilSkillInput gestorExternoPerfilSkillInput, CRUDEnum create);
    }
}