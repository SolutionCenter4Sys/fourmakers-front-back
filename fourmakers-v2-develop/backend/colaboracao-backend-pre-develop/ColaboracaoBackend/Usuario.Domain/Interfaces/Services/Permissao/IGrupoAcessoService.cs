using DataTransferObject.Domain.Usuario.Permissao;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Usuario.Domain.Interfaces.Services.Permissao
{
    public interface IGrupoAcessoService
    {
        Task<IEnumerable<GrupoAcessoDTO>> ListarGruposAcesso(string cpfRequest, int orgId);
        Task<IEnumerable<GrupoAcessoFuncionalidadeSistemaDTO>> ListarGruposAcessoFuncionalidadesSistema(string cpfRequest, int orgId);
        Task AdicionarGrupoAcessoFuncionalidadeSistema(int grupoAcessoId, int funcionalidadeSistemaId, string cpfRequest, int orgId);
        Task RemoverGrupoAcessoFuncionalidadeSistema(int grupoAcessoId, int funcionalidadeSistemaId, string cpfRequest, int orgId);
        Task<GrupoAcessoDTO> CriarGrupoAcesso(CriarGrupoAcessoInput input, string cpfRequest, int orgId);
        Task AtualizarGrupoAcesso(EditarGrupoAcessoInput input, string cpfRequest, int orgId);
        Task<IEnumerable<PessoaGrupoAcessoDTO>> ListarPessoasPorGrupoAcesso(string cpfRequest, int orgId, int? grupoId = null);
    }
}