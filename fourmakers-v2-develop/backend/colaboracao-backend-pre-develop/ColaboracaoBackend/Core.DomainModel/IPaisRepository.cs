using System.Collections.Generic;

namespace Core.Domain
{
    public interface IPaisRepository<TModel, TFactory>
    {
        List<TModel> BuscarTodos(TFactory _paisDomainFactory);
    }
}