using Colaborador.Domain.Interfaces.Factorys;
using DataTransferObject.Domain.Colaborador;
using System.Collections.Generic;

namespace Colaborador.Domain.Interfaces.Models
{
    public interface INacionalidadeModel
    {
        NacionalidadeDTO NacionalidadeDTO { get; set; }

        List<INacionalidadeModel> BuscarTodos(INacionalidadeDomainFactory nacionalidadeDomainFactory);
    }
}