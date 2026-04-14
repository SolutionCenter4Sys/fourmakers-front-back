using System.Collections.Generic;
using DataTransferObject.Domain.Dependentes;

namespace Core.DomainModel
{
    public interface IDependenteDtoRepository
    {
        DependentesDTO GetModelByKey(string key);
        List<DependentesDTO> GetModel(string cpf);
        DependentesDTO UpdateModel(DependentesDTO model, string cpf);
        DependentesDTO SaveModel(DependentesDTO model, string cpf);
        DependentesDTO DeleteModel(DependentesDTO model, string cpf);
        List<TipoDependenteDTO> ListarTipoDependente();
    }
}
