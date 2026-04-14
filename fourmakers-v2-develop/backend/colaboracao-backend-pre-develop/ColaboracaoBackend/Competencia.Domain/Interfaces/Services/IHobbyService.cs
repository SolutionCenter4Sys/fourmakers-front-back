using Competencia.Domain.Interfaces.Models;
using System.Collections.Generic;

namespace Competencia.Domain.Interfaces.Services
{
    public interface IHobbyService
    {
        List<IHobbyModel> ListarHobbies(string busca, int cursor, int limite);
        IHobbyModel InserirHobby(string descricao);
        IHobbyModel GetHobbiesById(long id);
        IHobbyColaboradorModel InserirHobbieColaborador(long id, string cpf);
        IHobbyColaboradorModel RemoverHobbieColaborador(long id, string cpf);
        List<IHobbyColaboradorModel> ListarHobbiesColaborador(string cpfColaborador);
    }
}