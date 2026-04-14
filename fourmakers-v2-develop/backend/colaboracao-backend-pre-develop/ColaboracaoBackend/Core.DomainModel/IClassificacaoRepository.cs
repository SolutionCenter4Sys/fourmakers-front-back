using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.Classificacao.Candidato;
using DataTransferObject.Domain.Classificacao.Perfil;

namespace Core.Domain;

public interface IClassificacaoRepository
{
    Task<List<CandidatoClassificacaoDTO>> ListarCandidatoClassificacaoPorListaDeCodigoInterno(List<string> codigosInternos);
    Task AtualizarClassificacaoProfissionalColaborador(string codigoInternoColaborador, double score, string categoria);
    Task<PerfilClassificacaoDTO> ObterPerfilClassificacaoPorId(Guid gestorExternoPerfilId);
    Task AtualizarClassificacaoPerfil(Guid gestorExternoPerfilId, double score, string categoria);
    Task<PerfilClassificacaoDTO> ObterVagaClassificacaoPorId(string vagaId);
    Task AtualizarClassificacaoVaga(string vagaId, double score, string categoria);
    Task<bool> ExisteClassificacaoVaga(string vagaId);
    Task<string> ObterCategoriaVagaPorId(string vagaId);
}