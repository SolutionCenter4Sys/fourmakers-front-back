using DataTransferObject.Domain.Marketing.Comunicacao.Label;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Marketing.Comunicacao.Label
{
    public interface IComunicacaoLabelRepository
    {
        Task<bool> AtualizarLabelAsync(string labelId, string nome, string tipo, int orgId);
        Task<bool> RemoverLabelAsync(string labelId, int orgId);
        Task<IEnumerable<LabelResumoDTO>> ObterTodasPorOrgAsync(int orgId, IEnumerable<string> tipos = null);
    }
}
