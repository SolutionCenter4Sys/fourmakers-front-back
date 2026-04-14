using Competencia.Domain.Interfaces.Factorys;
using DataTransferObject.Domain.Interesse;
using System.Collections.Generic;

namespace Competencia.Domain.Interfaces.Models
{
    public interface IInteresseModel
    {
        InteresseDTO InteresseDTO { get; set; }
        string CpfUsuario { get; set; }
        IInteresseModel SaveModel();
        IInteresseModel GetById(long id);
        IInteresseModel GetByCpf(string cpfUsuario, IInteresseDomainFactory factory);
        List<IInteresseModel> Listar(string busca, int cursor, int limite, IInteresseDomainFactory factory);
    }
}