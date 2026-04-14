using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.SRS.Vagas;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.VagasSRS;

namespace ApiClient.Domain.Interfaces
{
    public interface ISRSVagaClient
    {
        Task<ApiGenericResult<List<DropDownItemDTO>>> ListarSolicitantes(string token);
        Task<ApiGenericResult<List<DropDownItemDTO>>> ListarAprovadores(string token);
        Task<ApiGenericResult<List<DropDownItemDTO>>> ListarTermometroVagas(string token);
        Task<ApiGenericResult<List<DropDownItemDTO>>> ListarStackPrincipal(string token);
        Task<ApiGenericResult<List<DropDownItemDTO>>> ListarConfiguracaoMaquina(string token, string idContaCrm, int hardskillId);
        Task<ApiGenericResult<List<DropDownItemDTO>>> ListarTipoVaga(string token);
        Task<ApiGenericResult<List<DropDownItemDTO>>> ListarDuracaoContrato(string token);
        Task<ApiGenericResult<List<CargoDropdownItemDTO>>> ListarCargos(string token);
        Task<ApiGenericResult<List<DropDownItemDTO>>> ListarTipoContratacao(string token);
        Task<ApiGenericResult<List<DropDownItemDTO>>> ListarCargaHoraria(string token);
        Task<ApiGenericResult<List<DropDownItemDTO>>> ListarLocalTrabalho(string token);
        Task<ApiGenericResult<List<DropDownItemDTO>>> ListarUnidadesSRS(string token);
        Task<ApiGenericResult<List<JobOrderSemanticaDTO>>> ListarVagasParaSemanticaComSkill(string token, int cursor, int limite);
    }
}