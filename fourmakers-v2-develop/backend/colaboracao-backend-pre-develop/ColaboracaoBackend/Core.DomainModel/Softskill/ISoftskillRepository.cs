using DataTransferObject.Domain.Softskill;
using System.Collections.Generic;

namespace Core.DomainModel.Softskill
{
    public interface ISoftskillRepository
    {
        List<long> ListarSoftskillsAtribuidas(string cpfColaborador);
        SoftskillDTO GetSoftskillById(long softskillId);
        SoftskillDTO BuscarIdUsuarioPorCpf(string cpf);
        List<SoftskillDTO> ListarSoftSkill(string busca, int cursor, int limite);
        SoftskillDTO AdicionarSoftSkill(SoftskillDTO softskill);
    }
}