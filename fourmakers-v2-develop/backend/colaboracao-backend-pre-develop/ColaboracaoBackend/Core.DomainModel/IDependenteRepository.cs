using DataTransferObject.Domain.Dependentes;
using System.Collections.Generic;

namespace Core.Domain
{
    public interface IDependenteRepository<TModel, TFactory>
    {
        TModel GetModelByKey(string key, TFactory factory);
        List<TModel> GetModel(TModel model, TFactory factory);
        //TModel GetModelByKeyAlterar(string key, TFactory factory);
        TModel UpdateModel(TModel model, string cpf);
        TModel SaveModel(TModel model, string cpf);
        TModel DeleteModel(TModel model, string cpf);
        List<TipoDependenteDTO> ListarTipoDependente();
    }
}