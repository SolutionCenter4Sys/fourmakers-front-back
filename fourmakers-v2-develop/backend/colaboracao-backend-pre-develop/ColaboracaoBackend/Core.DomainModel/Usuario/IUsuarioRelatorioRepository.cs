using DataTransferObject.Domain.Foursys;
using DataTransferObject.Domain.Usuario;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.DomainModel.Usuario
{
    public interface IUsuarioRelatorioRepository
    {
        List<UsuarioAcessoDTO> GetRelatorioAcessoUsuarios(UsuarioLogadoDTO usuarioLogadoDTO);
        List<ColaboradoresOrgIdResult> ListaColaboradoresOrgId(int orgId, int page, int pageSize);
        int GetColaboradoresOrgId(int orgId);
        string GetOrgDescricao(int orgId);
        Task<List<dynamic>> RelatorioColaboradores(int orgId);
    }
}