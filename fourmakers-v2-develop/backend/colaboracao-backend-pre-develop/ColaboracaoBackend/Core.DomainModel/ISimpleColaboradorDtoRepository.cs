using System.Collections.Generic;
using DataTransferObject.Domain.Colaborador;

namespace Core.DomainModel
{
    public interface ISimpleColaboradorDtoRepository
    {
        List<SimpleColaboradorDTO> BuscarListaColaboradores(string nomeCompleto);
    }
}
