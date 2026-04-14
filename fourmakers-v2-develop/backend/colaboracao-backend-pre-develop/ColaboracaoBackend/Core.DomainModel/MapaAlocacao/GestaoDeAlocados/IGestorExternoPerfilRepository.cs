using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfil;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.MapaAlocacao.GestaoDeAlocados
{
    public interface IGestorExternoPerfilRepository
    {
        Task<IEnumerable<GestorExternoPerfilResult>> ListarGestorExternoPerfisAsync();
        Task<GestorExternoPerfilResult> ObterGestorExternoPerfilPorIdAsync(Guid id);
        Task<GestorExternoPerfilResult> ObterGestorExternoPerfilPorIdLimiteAsync(Guid id, int cursor, int limite);
        Task<IEnumerable<GestorExternoPerfilResult>> ObterGestorExternoPerfilsPorCodigoGestorExternoAsync(string codGestorExterno, int orgId);
        Task<GestorExternoPerfilResult> InserirGestorExternoPerfilAsync(GestorExternoPerfilInput parametroRepositoryInput);
        Task<GestorExternoPerfilResult> AtualizarGestorExternoPerfilAsync(GestorExternoPerfilInput parametroRepositoryInput);
        Task<bool> DeletarGestorExternoPerfilAsync(Guid id);
        Task<IEnumerable<GestorExternoPerfilResult>> ObterGestorExternoPerfilPorListaIdsAsync(List<Guid> ids);
        Task<IEnumerable<CadastrarPerfisLegadosEmLoteDTO>> BuscarIdsPerfis(int quantidade);
        Task IniciarProcessamento(CadastrarPerfisLegadosEmLoteDTO idPerfil);
        Task FinalizarProcessamento(CadastrarPerfisLegadosEmLoteDTO idPerfil, string retorno);
    }
}