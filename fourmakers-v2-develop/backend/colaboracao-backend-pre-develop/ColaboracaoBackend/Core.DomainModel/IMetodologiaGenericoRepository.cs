namespace Core.Domain
{
    public interface IMetodologiaGenericoRepository<TModel, TFactory>
    {
        TModel AlterarMetodologiaColaborador(TModel model);
    }
}