using DataTransferObject.Domain.Colaborador;
using System.Collections.Generic;

namespace Colaborador.Domain.Interfaces.Services
{
    public interface IPaisService
    {
        List<PaisDTO> BuscarTodos();
    }
}