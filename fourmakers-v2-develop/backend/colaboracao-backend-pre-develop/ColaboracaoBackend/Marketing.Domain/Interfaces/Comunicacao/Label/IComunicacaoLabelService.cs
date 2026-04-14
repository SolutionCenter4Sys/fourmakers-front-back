using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Marketing.Comunicacao.Label;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Marketing.Domain.Interfaces.Comunicacao.Label
{
    public interface IComunicacaoLabelService
    {
        Task<ApiGenericResult<IEnumerable<LabelResumoDTO>>> ObterTodasLabelsAsync(int orgId, IEnumerable<string> tipos = null);
        Task<ApiGenericResult> AtualizarLabelAsync(string labelId, AtualizarLabelRequestDTO request, int orgId);
        Task<ApiGenericResult> RemoverLabelAsync(string labelId, int orgId);
    }
}
