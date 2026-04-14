using DataTransferObject.Domain.Interesse;
using System.Collections.Generic;

namespace Competencia.Domain.Interfaces.Services
{
    public interface IInteresseService
    {
        List<InteresseDTO> ListarInteresses(string busca, int cursor, int limite);
        InteresseDTO InserirInteresse(string descricao);
        InteresseDTO GetInteressesById(long id);
        InteresseColaboradorDTO InserirInteresseColaborador(long id, string cpf, bool interesseAtivo, int tipoId, int skillId, bool minhaJornada, string gestorExternoPerfil, long nivelId, string usuarioLogado);
        void RemoverInteresseColaborador(long id, string cpf);
        List<InteresseColaboradorDTO> ListarInteressesColaborador(string cpfColaborador);
    }
}
