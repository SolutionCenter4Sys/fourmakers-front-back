using Colaborador.Domain.Interfaces.Factorys;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Dependentes;
using System.Collections.Generic;

namespace Colaborador.Domain.Interfaces.Models
{
    public interface IDependenteModel
    {
        RemoverDependentesDTO RemoverDependentesDTO { get; set; }
        AdicionarDependentesDTO AdicionarDependentesDTO { get; set; }
        AlterarDependentesDTO AlterarDependentesDTO { get; set; }
        DependentesDTO DependentesDTO { get; set; }
        ColaboradorDTO Colaborador { get; set; }
        int QuantidadeDependentes { get; set; }

        IDependenteModel SaveModel(string cpf);
        IDependenteModel UpdateModel(string cpf);
        IDependenteModel DeleteModel(string cpf);

        IDependenteModel GetByCpf(string cpfUsuario, IDependenteDomainFactory factory);
        List<IDependenteModel> GetModel(string cpfColaborador, IDependenteDomainFactory factory);
    }
}