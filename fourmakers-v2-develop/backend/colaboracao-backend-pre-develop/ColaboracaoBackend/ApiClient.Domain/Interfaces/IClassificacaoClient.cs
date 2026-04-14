using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.Classificacao.Candidato;
using DataTransferObject.Domain.Classificacao.Perfil;

namespace ApiClient.Domain.Interfaces;

public interface IClassificacaoClient
{
    Task<CandidatoClassificacaoResult> ClassificarCandidatosAsync(List<CandidatoClassificacaoDTO> candidatoClassificacaoDTOs);
    Task<PerfilClassificacaoResult> ClassificarPerfisAsync(List<PerfilClassificacaoDTO> perfilClassificacaoDTOs);
}