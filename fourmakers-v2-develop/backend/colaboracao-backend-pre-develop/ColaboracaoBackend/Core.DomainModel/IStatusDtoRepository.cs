using System.Collections.Generic;
using DataTransferObject.Domain.Colaborador;

namespace Core.DomainModel
{
    public interface IStatusDtoRepository
    {
        StatusColaboradorDTO GetModelByKey(string key);
        StatusColaboradorDTO GetStatusByCpf(string cpf);
        List<StatusColaboradorDTO> ListModel(string busca, int cursor, int limite);
    }
}
