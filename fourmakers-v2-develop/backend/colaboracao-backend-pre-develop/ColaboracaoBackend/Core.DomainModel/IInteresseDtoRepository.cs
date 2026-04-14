using DataTransferObject.Domain.Interesse;
using System.Collections.Generic;

namespace Core.DomainModel
{
    public interface IInteresseDtoRepository
    {
        InteresseDTO GetInteresseById(long id);
        long GetUsuarioCriacaoIdByCpf(string cpf);
        InteresseDTO SaveInteresse(string descricao, long usuarioCriacaoId);
        List<InteresseDTO> ListInteresses(string busca, int cursor, int limite);
    }
}
