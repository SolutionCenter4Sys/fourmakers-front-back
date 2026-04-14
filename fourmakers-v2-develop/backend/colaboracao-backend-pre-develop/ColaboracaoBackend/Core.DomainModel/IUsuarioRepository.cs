using System.Collections.Generic;

namespace Core.Domain
{
    public interface IUsuarioRepository<TModel, TFactory>
    {
        TModel GetModel(TModel model);
        TModel GetModelByKey(string key, TFactory factory, int orgId = 0);
        TModel GetModelByKeyAndOrg(string key, int orgId, TFactory factory);
        TModel SaveModel(TModel model);
        List<TModel> ListModel(string busca, int cursor, int limite, TFactory factory);
        List<TModel> ListModel(TModel model, TFactory factory);
        void DeleteModel(TModel model);
        TModel UpdateModel(TModel model);
    }
}