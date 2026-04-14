using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.GestaoPessoa.GestaoDesempenho.Gestor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.GestaoPessoa.GestaoDesempenho.Gestor
{
    public interface IGestaoDesempenhoGestorRepository
    {
        Task<string> ObterCodigoExternoGestorAsync(string codigoInternoColaboradorGestor, int orgId);
        Task<List<ColaboradorSubordinadoDTO>> ObterColaboradoresSubordinadosAsync(string codExternoGestor, int orgId);
        Task<List<ColaboradorDetalhesDTO>> ObterColaboradoresSubordinadosComDetalhesAsync(string codExternoGestor, int orgId);
        Task<Dictionary<string, DateTime?>> ObterUltimasDataOneOnOneAsync(List<string> codigosInternosColaboradores);
        Task<Dictionary<string, DateTime?>> ObterUltimasDataFeedbackAsync(List<string> codigosInternosColaboradores);
        Task<List<OneOnOneRegistroCriticoDTO>> ObterRegistrosCriticosAsync(string codExternoGestor, int orgId);
        Task InserirFeedbackAsync(string id, string codigoInternoColaboradorSuperior, InserirFeedbackRequestDTO request);
        Task<string> InserirOneOnOneAsync(string codigoInternoColaboradorSuperior, InserirOneOnOneRequestDTO request);
        Task VincularPautasSugeridasAoOneOnOneAsync(string oneOnOneId, List<string> pautasSugeridasIds);
        Task<DashboardColaboradorDTO> ObterDashboardColaboradorAsync(string codigoInternoColaboradorAvaliado, int orgId);
        Task<List<FeedbackDetalheDTO>> ObterFeedbacksColaboradorAsync(string codigoInternoColaboradorAvaliado);
        Task<List<OneOnOneDetalheDTO>> ObterOneOnOnesColaboradorAsync(string codigoInternoColaboradorAvaliado);
        Task<InserirPautaSugeridaGestorResponseDTO> UpsertPautaSugeridaGestorAsync(string codigoInternoColaboradorSuperior, InserirPautaSugeridaGestorRequestDTO request);
        Task<bool> AtualizarRegistroCriticoOneOnOneAsync(string oneOnOneId, bool registroCritico);
    }
}
