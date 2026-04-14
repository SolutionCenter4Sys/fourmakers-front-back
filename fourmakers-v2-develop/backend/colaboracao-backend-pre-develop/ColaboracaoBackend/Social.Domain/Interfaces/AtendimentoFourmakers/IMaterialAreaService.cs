using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Social.AtendimentoFourmakers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Social.Domain.Interfaces.AtendimentoFourmakers
{
    public interface IMaterialAreaService
    {
        Task<ApiGenericResult<List<MaterialAreaResult>>> ListarAsync(int orgId);
        Task<ApiGenericResult<MaterialAreaResult>> CriarAsync(CriarMaterialAreaInput input, int orgId);
        Task<ApiGenericResult<MaterialAreaResult>> AtualizarAsync(string id, AtualizarMaterialAreaInput input, int orgId);
        Task<ApiGenericResult<bool>> RemoverAsync(string id, int orgId);
    }
}
