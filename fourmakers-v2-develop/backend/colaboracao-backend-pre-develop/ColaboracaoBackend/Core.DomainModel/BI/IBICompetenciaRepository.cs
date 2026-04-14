using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.Competencia;

namespace Core.Domain.BI;

public interface IBICompetenciaRepository
{
    Task<List<CompetenciaGroupDTO>> ObterCompetenciasPorTipoEListaDeIds(int ItemPerfilTipoID, List<long> ids);
}