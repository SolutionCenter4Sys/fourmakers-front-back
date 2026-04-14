using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeAlocacao.Aderencia;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.Aderencia;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfil;
using DataTransferObject.Domain.Match;
using DataTransferObject.Domain.Vaga;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados
{
    public interface IAderenciaService
    {
        Task<ApiGenericResult<List<ColaboradorAderenciaDTO>>> ListarAderenciaPerfilGestorExterno(Guid perfilId, string filtro, int cursor, int limite, int orgId, string cpfRequest);

        Task<ApiGenericResult<PerfisAderentesDTO>> ListarPerfisAderentesColaborador(ListasPerfisAderentesParams listaAderenciaColaboradorParam, string cpfRequest);
        Task<List<CalculoAderenciaComPerfilEColaboradorDTO>> GerarListaAderenciaSimplificada(List<PerfilEColaboradorAderenciaDTO> listaColaboradoresEPerfis, string cpfRequest, int orgId);

        Task<ApiGenericResult<List<CalculoAderenciaComPerfilEColaboradorDTO>>> ListarAderenciaAlocadosColabEPerfil(string cpfRequest, int orgId);
        Task<List<ListarCandidatosAderentesResult>> ListarAderenciaAlocadosViaMatch(string cpf, Guid perfilId, int orgId, int cursor, int limite);
        Task<CandidatosMatchResponse> BuscarMatchPerfilAsync(ApiGenericResult<GestorExternoPerfilResult> perfil, string cpf, int orgId);
    }
}