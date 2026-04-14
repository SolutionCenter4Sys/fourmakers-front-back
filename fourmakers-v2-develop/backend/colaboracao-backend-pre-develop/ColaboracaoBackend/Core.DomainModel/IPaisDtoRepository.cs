using System.Collections.Generic;
using DataTransferObject.Domain.Colaborador;

namespace Core.DomainModel
{
    public interface IPaisDtoRepository
    {
        List<PaisDTO> BuscarTodos();
    }
}
