using System.Collections.Generic;

namespace Core.Domain
{
    public interface ISimpleColaboradorRepository<TModel, TFactory>
    {
        List<TModel> BuscarListaColaboradores(string nomeCompleto, TFactory factory);
    }
}