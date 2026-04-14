using System.Collections.Generic;

namespace Core.Domain
{
    public interface IEndossoRepository<TModel, TFactory>
    {
        TModel ListarEndossosPorColaborador(string cpf, TFactory endossoDomainFactory);
        TModel ListarEndossosPorCompetenciaId(List<long> listaDeIds, TFactory endossoDomainFactory);
    }
}