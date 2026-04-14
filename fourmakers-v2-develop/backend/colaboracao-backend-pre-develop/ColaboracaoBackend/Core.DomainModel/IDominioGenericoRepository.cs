namespace Core.Domain
{
    public interface IDominioGenericoRepository<TModel, TFactory>
    {
        TModel AlterarDominioColaborador(TModel model);
    }
}