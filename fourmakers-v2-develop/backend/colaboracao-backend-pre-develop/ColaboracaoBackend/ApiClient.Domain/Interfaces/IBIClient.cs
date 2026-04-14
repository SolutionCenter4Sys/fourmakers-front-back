using DataTransferObject.Domain.Filtro;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiClient.Domain.Interfaces
{
    public interface IBIClient
    {
        Task<bool> ArmazenaTrending(List<FiltroCompetenciaNivelDTO> paramCompetenciaNivel, List<FiltroFormacaoNivelDTO> paramFormacaoNivel, List<FiltroMetodologiaNivelDTO> paramMetodologiaNivel,
            List<FiltroDominioNivelDTO> paramDominioNivel, List<FiltroModeloReferenciaNivelDTO> paramModeloNivel, List<FiltroInteresseDTO> paramInteresse, List<FiltroHobbyDTO> paramHobby, string tokenUsuario);
    }
}