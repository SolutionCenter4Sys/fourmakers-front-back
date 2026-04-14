using DataTransferObject.Domain.GestaoPessoa.GestaoDesempenho.Colaborador;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.GestaoPessoa.GestaoDesempenho.Colaborador
{
    public interface IGestaoDesempenhoColaboradorRepository
    {
        Task<List<ColaboradorFeedbackDetalheDTO>> ObterMeusFeedbacksAsync(string codigoInternoColaborador);
        Task<List<ColaboradorOneOnOneDetalheDTO>> ObterMeusOneOnOnesAsync(string codigoInternoColaborador);
        Task<List<ColaboradorPautaSugeridaDTO>> ObterPautasSugeridasPorColaboradorAvalidadoAsync(string codigoInternoColaborador);
        Task<List<RegistroCriticoDTO>> ObterMeusRegistrosCriticosAsync(string codigoInternoColaborador);
        Task<bool> InserirVisualizacaoFeedbackAsync(string feedbackId, string codigoInternoColaborador);
        Task<bool> InserirVisualizacaoOneOnOneAsync(string oneOnOneId, string codigoInternoColaborador);
        Task<InserirPautaSugeridaColaboradorResponseDTO> UpsertPautaSugeridaColaboradorAsync(string codigoInternoColaboradorAvaliado, InserirPautaSugeridaColaboradorRequestDTO request);
    }
}
