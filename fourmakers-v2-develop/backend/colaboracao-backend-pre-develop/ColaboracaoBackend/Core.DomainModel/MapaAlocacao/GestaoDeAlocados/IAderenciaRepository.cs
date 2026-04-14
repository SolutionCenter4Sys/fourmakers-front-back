using DataTransferObject.Domain.MapaDeAlocacao.Aderencia;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.Aderencia;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.DomainModel.MapaAlocacao.GestaoDeAlocados
{
    public interface IAderenciaRepository
    {
        Task<List<ColaboradorAderenciaDTO>> ListarColaboradoresAderentesPorOrgId(List<string> orgsId, string filtro, List<string> skillsDescricao, string codigoInternoColaborador = null, Guid? gestorPerfilExterno = null);
        Task<List<ColaboradorAderenciaDTO>> ListarColaboradoresAderentesPorOrgIdLimite(List<string> orgsId, string filtro, List<string> skillsDescricao, int cursor, int limite = 10, string codigoInternoColaborador = null, Guid? gestorPerfilExterno = null);

        Task<List<PerfilAderenteDTO>> ListarPerfisDaOrgID(int orgId, string? filtro);
        Task<List<ColaboradorAderenciaSimplificadoDTO>> ListarColaboradoresAderentesPorListaId(List<string> ids, int orgId);
        Task<List<PerfilEColaboradorAderenciaDTO>> ListarAlocadosColabEPerfil(int orgId);

    }
}