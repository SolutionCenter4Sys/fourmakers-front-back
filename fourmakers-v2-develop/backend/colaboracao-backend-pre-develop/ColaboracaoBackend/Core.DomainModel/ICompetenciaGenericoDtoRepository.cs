using System.Collections.Generic;

namespace Core.DomainModel
{
    public interface ICompetenciaGenericoDtoRepository
    {
        List<long> ListarIdsPorCompetenciaId(long id);
    }
}
