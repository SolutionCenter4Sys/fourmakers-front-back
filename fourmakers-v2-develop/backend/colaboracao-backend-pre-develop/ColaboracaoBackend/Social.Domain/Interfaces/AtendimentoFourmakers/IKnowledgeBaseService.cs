using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Social.AtendimentoFourmakers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Social.Domain.Interfaces.AtendimentoFourmakers
{
    public interface IKnowledgeBaseService
    {
        Task<ApiGenericResult<IngestaoResult>> IngerirAsync(IngerirDocumentoInput input, string codigoInternoColaborador, string nomeColaborador, int orgId);
        Task<ApiGenericResult<RemocaoFonteResult>> RemoverFonteAsync(RemoverFonteInput input, string codigoInternoColaborador, string nomeColaborador, int orgId);
        Task<ApiGenericResult<KbTotaisResult>> ObterTotaisAsync(int orgId);
        Task<ApiGenericResult<List<KbFonteResult>>> ListarFontesAsync(int orgId);
        Task<ApiGenericResult<KbFonteDetalheResult>> ObterFontePorIdAsync(string fonteId, int orgId);
    }
}
