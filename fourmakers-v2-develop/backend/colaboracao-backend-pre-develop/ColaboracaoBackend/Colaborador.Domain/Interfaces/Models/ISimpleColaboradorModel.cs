using Colaborador.Domain.Interfaces.Factorys;
using DataTransferObject.Domain.Colaborador;
using System.Collections.Generic;

namespace Colaborador.Domain.Interfaces.Models
{
    public interface ISimpleColaboradorModel
    {
        SimpleColaboradorDTO SimpleColaboradorDTO { get; set; }

        List<SimpleColaboradorDTO> BuscarListaColaboradores(string nomeCompleto, ISimpleColaboradorDomainFactory factory);
    }
}