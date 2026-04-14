using System.Collections.Generic;
using DataTransferObject.Domain.Colaborador;

namespace Core.DomainModel
{
    public interface INacionalidadeDtoRepository
    {
        List<NacionalidadeDTO> BuscarTodos();
    }
}
