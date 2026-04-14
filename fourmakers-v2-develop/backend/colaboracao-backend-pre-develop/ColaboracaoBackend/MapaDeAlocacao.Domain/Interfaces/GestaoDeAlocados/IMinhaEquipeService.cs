using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.Match;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados
{
    public interface IMinhaEquipeService
    {
        Task<ApiGenericResult<List<MinhaEquipeDTO>>> BuscarLideradosPorGestor(string codColaboradorGestor, string perfilId, int orgId);
        Task<ApiGenericResult<List<GestorColaboradoresSkill>>> ListaIndicadoresDosLiderados(string codColaboradorGestor, string perfilId, int orgId);
        Task<ApiGenericResult<List<GestorColaboradoresSkillAdmOper>>> ListaIndicadoresDosLideradosAdmOper(string cpfRequest, int orgId, int limite, int cursor, string codGestorAdm, string codGestorOper, string codCliente);
        Task<ApiGenericResult<List<TotalizacaoIndicadoresPorGestorAdmOper>>> TotalizacaoIndicadoresDosLideradosAdmOper(string cpfRequest, int orgId, string codGestorAdm, string codGestorOper);
        Task<ApiGenericResult<List<GestorCandidatosMatchResponse>>> ListaAderenciaDosLideradosPorGestor(string codGestorAdm, string perfilId, int orgId);
        Task<ApiGenericResult<List<GestorCandidatosMatchResponsePerfil>>> ListaAderenciaDosLideradosPorGestorSemParamPerfil(string cpfRequest, string codGestorAdm, string codGestorOper, int orgId);
        Task<ApiGenericResult<List<GestorCandidatosMatchResponse>>> ListaAderenciaDoColaborador(string codColaborador, string perfilId, int orgId);
        Task<ApiGenericResult<List<GestorCandidatosMatchResponse>>> ListaAderenciaDoColaboradorAdmOper(string codColaborador, int orgId);
       
    }
};
