using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Marketing.Comunicacao.Tag;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Marketing.Domain.Interfaces.Comunicacao.Tag
{
    public interface IComunicacaoTagService
    {
        Task<ApiGenericResult<IEnumerable<TagResumoDTO>>> ObterTodasTagsAsync(int orgId);
        Task<ApiGenericResult> AtualizarTagAsync(string tagId, AtualizarTagRequestDTO request, int orgId);
        Task<ApiGenericResult> RemoverTagAsync(string tagId, int orgId);
    }
}
