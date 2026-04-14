using DataTransferObject.Domain.Colaborador;
using System.Collections.Generic;

namespace Colaborador.Domain.Interfaces.Services
{
    public interface INacionalidadeService
    {
        List<NacionalidadeDTO> BuscarTodos();
    }
}