using Colaborador.Domain.Interfaces.Factorys;
using DataTransferObject.Domain.Colaborador;
using System.Collections.Generic;

namespace Colaborador.Domain.Interfaces.Models
{
    public interface IPaisModel
    {
        PaisDTO PaisDTO { get; set; }

        List<IPaisModel> BuscarTodos(IPaisDomainFactory paisDomainFactory);
    }
}