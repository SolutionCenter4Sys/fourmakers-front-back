using Colaborador.Domain.Interfaces.Models;
using System;

namespace Colaborador.Domain.Interfaces.Factorys
{
    public interface IDependenteDomainFactory
    {
        IDependenteModel buildDependenteModel();
        IDependenteModel buildDependenteModel(long registroUser);
        IDependenteModel buildDependenteModel(long idDependente, string nomeCompletoDependente, DateTime dataNascimentoDependente, string rgDependente, string cpfDependente, sbyte portadorDeficienciaDependente, string requerAjudaQualDependente, int tipoDependenteId, string descricaoTipoDependente, int quantidadeDependentes);
    }
}