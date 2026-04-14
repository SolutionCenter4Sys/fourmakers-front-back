using System.Collections.Generic;

namespace Core.Domain
{
    public interface INacionalidadeRepository<TModel, TFactory>
    {
        List<TModel> BuscarTodos(TFactory _nacionalidadeDomainFactory);
    }
}