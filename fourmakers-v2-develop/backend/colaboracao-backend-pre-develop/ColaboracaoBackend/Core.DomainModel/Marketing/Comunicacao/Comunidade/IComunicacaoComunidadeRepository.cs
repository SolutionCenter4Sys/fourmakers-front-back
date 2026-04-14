using DataTransferObject.Domain.Marketing.Comunicacao.Comunidade;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Marketing.Comunicacao.Comunidade
{
    public interface IComunicacaoComunidadeRepository
    {
        Task<ComunidadesResumoResponseDTO> ObterListaComunidadesResumoAsync(int orgId, string codigoInternoColaborador);
        Task<ComunidadeDetalheDTO> ObterComunidadePorIdAsync(Guid id, int orgId, string codigoInternoColaborador);
        Task<bool> ColaboradorEModeradorDaComunidadeAsync(Guid comunidadeId, string codigoInternoColaborador, int orgId);
        Task<List<string>> ObterCodigosParticipantesComunidadeAsync(Guid comunidadeId, int orgId);
        Task<string> ObterCodigoInternoColaboradorCriacaoComunidadeAsync(Guid comunidadeId, int orgId);
        Task<string> InserirComunidadeAsync(string codigoInternoColaboradorCriacao, int orgId, InserirComunidadeRequestDTO request);
        /// <param name="atualizarCapaUrl">Quando falso, mantém <c>capa_url</c> já persistida; quando verdadeiro, grava <see cref="AtualizarComunidadeRequestDTO.CapaUrl"/>.</param>
        Task<bool> AtualizarComunidadeAsync(Guid id, int orgId, AtualizarComunidadeRequestDTO request, string codigoInternoColaboradorUltimaAlteracao, DateTime dataUltimaAlteracao, bool atualizarCapaUrl);
        Task<bool> ParticiparComunidadeAsync(Guid comunidadeId, string codigoInternoColaborador, int orgId);
        Task<bool> SairComunidadeAsync(Guid comunidadeId, string codigoInternoColaborador, int orgId);
        /// <summary>Preenche <see cref="ComunidadeResumoDTO.Criador"/> e <see cref="ComunidadeResumoDTO.UltimoAlterador"/> a partir dos códigos já definidos no DTO.</summary>
        Task EnriquecerColaboradoresResumoComunidadeAsync(ComunidadeResumoDTO comunidade, int orgId);
    }
}
