using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Marketing.Comunicacao.Grupo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Marketing.Domain.Interfaces.Comunicacao.Grupo
{
    public interface IComunicacaoGrupoService
    {
        Task<ApiGenericResult<List<ColaboradorDisponivelModeloContratacaoResumoDTO>>> ObterSugestoesModeloContratacaoColaboradoresDisponiveisAsync(int orgId);

        Task<ApiGenericResult<List<ColaboradorDisponivelDiretoriaResumoDTO>>> ObterSugestoesDiretoriaColaboradoresDisponiveisAsync(int orgId);

        Task<ApiGenericResult<ColaboradoresResponseDTO>> ListarColaboradoresDisponiveisParaAdicionarAsync(string filtro, int orgId, List<string> codModeloContratacao = null, List<string> codDiretoria = null);
        Task<ApiGenericResult<List<GrupoResumoDTO>>> ListarGrupoResumoAsync(int orgId);
        Task<ApiGenericResult<GrupoDetalheDTO>> ObterGrupoAsync(string grupoId, int orgId);
        Task<ApiGenericResult<GrupoResumoDTO>> InserirGrupoAsync(string cpf, int orgId, InserirGrupoRequestDTO request);
        Task<ApiGenericResult> AtualizarGrupoAsync(string grupoId, int orgId, AtualizarGrupoRequestDTO request);
        Task<ApiGenericResult> DeletarGrupoAsync(string grupoId, int orgId);
        Task<ApiGenericResult<PermissoesGrupoUsuarioResponseDTO>> ObterPermissoesGruposUsuarioLogadoAsync(int orgId, string codigoInternoColaborador);
    }
}
