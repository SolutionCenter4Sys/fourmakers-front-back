using Competencia.Domain.Interfaces.Factorys;
using DataTransferObject.Domain.Hobby;
using System.Collections.Generic;

namespace Competencia.Domain.Interfaces.Models
{
    public interface IHobbyColaboradorModel
    {
        HobbyColaboradorDTO HobbyColaboradorDTO { get; set; }
        long HobbyId { get; set; }
        string ColaboradorCpf { get; set; }
        IHobbyColaboradorModel SaveModel();
        void RemoverHobbyDoColaborador();
        List<IHobbyColaboradorModel> ListarHobbiesDeColaboradores(IHobbyDomainFactory factory);
    }
}