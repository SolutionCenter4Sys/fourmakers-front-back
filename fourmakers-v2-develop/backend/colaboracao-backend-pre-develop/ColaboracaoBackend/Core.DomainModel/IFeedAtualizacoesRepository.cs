using System.Collections.Generic;

namespace Core.Domain
{
    public interface IFeedAtualizacoesRepository<TModel, TFactory>
    {
        TModel CurtirFeed(TModel model, TFactory factory);
        TModel DescurtirFeed(TModel model, TFactory factory);
        List<TModel> GetFeed(TModel model, int cursor, int limite, TFactory factory);
        List<TModel> GetFeedV2(TModel model, int cursor, int limite, TFactory factory);
    }
}