using DataTransferObject.Domain.Interesse;
using System.Collections.Generic;

namespace Core.DomainModel
{
    public interface IInteresseColaboradorDtoRepository
    {
        InteresseColaboradorDTO SaveInteresseColaboradorDapper(long interesseId, string cpf, int tipoId, int skillId, bool interesseAtivo);
        List<InteresseColaboradorDTO> ListInteressesColaborador(string cpf);
        void RemoveInteresseColaborador(long interesseId, string cpf);
    }
}
