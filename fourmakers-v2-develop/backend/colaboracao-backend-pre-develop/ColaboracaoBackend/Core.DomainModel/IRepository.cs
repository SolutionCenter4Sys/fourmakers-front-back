using System.Collections.Generic;

namespace Core.Domain
{
    public interface IRepository<TModel, TFactory>
    {
        TModel GetModel(TModel model);
        TModel GetModelByKey(string key, TFactory factory);
        TModel SaveModel(TModel model);
        TModel SaveModelDapper(TModel model);
        List<TModel> ListModel(string busca, int cursor, int limite, TFactory factory);
        List<TModel> ListModel(TModel model, TFactory factory);
        void DeleteModel(TModel model);
        TModel UpdateModel(TModel model);
    }
}