namespace Core.Domain.Properties
{
    public interface ISoftskillGenericoRepository<TModel, TFactory>
    {
        TModel AlterarSoftskillColaborador(TModel model);
    }
}