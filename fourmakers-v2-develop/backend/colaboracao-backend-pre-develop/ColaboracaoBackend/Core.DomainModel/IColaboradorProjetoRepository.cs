using System.Collections.Generic;

namespace Core.Domain
{
    public interface IColaboradorProjetoRepository<TModel, TFactory>
    {
        TModel SaveColaboradorProjeto(TModel model);
        List<TModel> ListarColaboradorProjeto(long projetoId, TFactory factory);
        void DeleteColaboradorProjeto(TModel model);
    }
}