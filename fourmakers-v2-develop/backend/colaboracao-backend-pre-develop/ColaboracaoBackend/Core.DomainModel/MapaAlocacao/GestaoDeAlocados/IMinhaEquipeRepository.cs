using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.MapaAlocacao.GestaoDeAlocados
{
    public interface IMinhaEquipeRepository
    {
        Task<List<MinhaEquipeDTO>> BuscarLideradosPorGestor(string codColaboradorGestor, string perfilId, int orgId);
        Task<List<GestorColaboradoresSkill>> ListaIndicadoresDosLiderados(string codColaborador,string perfilId, int orgId);
        Task<List<GestorCandidatosMatchResponse>> ListaAderenciaDosLideradosPorGestor(string codGestorAdm, string perfilId, int orgId);
        Task<List<GestorCandidatosMatchResponsePerfil>> ListaAderenciaDosLideradosPorGestorSemParamPerfil(string codGestorAdm, string codGestorOper, int orgId);
        Task<List<GestorCandidatosMatchResponse>> ListaAderenciaDoColaborador(string codColaborador, string perfilId, int orgId);
        Task<List<GestorCandidatosMatchResponse>> ListaAderenciaDoColaboradorAdmOper(string codColaborador, int orgId);
        Task<List<GestorColaboradoresSkillAdmOper>> ListaIndicadoresDosLideradosAdmOper(int orgId, int limite, int cursor, string codGestorAdm, string codGestorOper, string codCliente);
        Task<List<TotalizacaoIndicadoresPorGestorAdmOper>> TotalizacaoIndicadoresDosLideradosAdmOper(int orgId, string codGestorAdm, string codGestorOper);

    }
}
