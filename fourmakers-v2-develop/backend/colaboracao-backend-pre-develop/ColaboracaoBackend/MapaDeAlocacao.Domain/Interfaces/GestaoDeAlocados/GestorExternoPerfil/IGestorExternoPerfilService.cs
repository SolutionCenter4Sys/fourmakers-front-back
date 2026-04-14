using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfil;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill;

namespace MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados.GestorExternoPerfil
{
    public interface IGestorExternoPerfilService
    {
        Task<ApiGenericResult<IEnumerable<GestorExternoPerfilResult>>> ListarGestorExternoPerfis(string cpfRequest, int orgId);
        Task<ApiGenericResult<GestorExternoPerfilResult>> ObterGestorExternoPerfilPorId(Guid id, string CpfRequest, int orgId);
        Task<ApiGenericResult<GestorExternoPerfilResult>> ObterGestorExternoPerfilPorIdLimite(Guid id, string CpfRequest, int orgId, int cursor, int limite);
        Task<ApiGenericResult<GestorExternoPerfilResult>> InserirGestorExternoPerfil(GestorExternoPerfilInput gestorExternoPerfilInput, string cpfRequest, int orgId);
        Task<ApiGenericResult<GestorExternoPerfilResult>> AtualizarGestorExternoPerfil(GestorExternoPerfilInput gestorExternoPerfilInput, Guid id, string cpfRequest, int orgId);
        Task<ApiGenericResult> DeletarGestorExternoPerfil(Guid id, string cpfRequest, int orgId);
        Task<ApiGenericResult<List<GestorExternoPerfilResult>>> ObterGestoresExternoPerfilPorListaDeIds(List<Guid> ids, int orgId);
        Task CadastrarPerfisLegado(CadastrarPerfisLegado param, string cpf, int orgId);
        Task<ApiGenericResult<IEnumerable<GestorExternoPerfilSkillResult>>> ObterListarDeSkillsGestorExternoPorGestorExternoIdAsync(Guid id);
    }
}