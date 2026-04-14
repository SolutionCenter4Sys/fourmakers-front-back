using Competencia.Domain.Interfaces.Factorys;
using DataTransferObject.Domain.Hobby;
using System.Collections.Generic;

namespace Competencia.Domain.Interfaces.Models
{
    public interface IHobbyModel
    {
        HobbyDTO HobbyDTO { get; set; }
        string CpfUsuario { get; set; }
        IHobbyModel SaveModel();
        List<IHobbyModel> Listar(string busca, int cursor, int limite, IHobbyDomainFactory factory);
        IHobbyModel GetByCpf(string cpfUsuario, IHobbyDomainFactory factory);
        IHobbyModel GetById(long id);
    }
}