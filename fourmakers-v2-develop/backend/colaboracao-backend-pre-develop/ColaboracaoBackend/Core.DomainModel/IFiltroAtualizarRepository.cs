namespace Core.Domain
{
    public interface IFiltroAtualizarRepository<TModel, TFactory>
    {
        TModel AtualizarFiltro(TModel model);
    }
}