using DataTransferObject.Domain.Marketing.Comunicacao.Tag;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Marketing.Comunicacao.Tag
{
    public interface IComunicacaoTagRepository
    {
        Task<bool> AtualizarTagAsync(string tagId, string nome, int orgId);
        Task<bool> RemoverTagAsync(string tagId, int orgId);
        Task<IEnumerable<TagResumoDTO>> ObterTodasPorOrgAsync(int orgId);
    }
}
