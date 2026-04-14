using System.Collections.Generic;

namespace Core.Domain
{
    public interface ICompetenciaGenericoRepository<TModel, TFactory>
    {
        TModel AtualizaCompetenciaColaborador(TModel model);
        List<long> ListarIdsPorCompetenciaId(long id);
    }
}