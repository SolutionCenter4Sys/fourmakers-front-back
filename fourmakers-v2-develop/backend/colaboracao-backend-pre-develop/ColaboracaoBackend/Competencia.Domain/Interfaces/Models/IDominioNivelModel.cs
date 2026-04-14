using Competencia.Domain.Interfaces.Factorys;
using DataTransferObject.Domain.Nivel;
using System.Collections.Generic;

namespace Competencia.Domain.Interfaces.Models
{
    public interface IDominioNivelModel
    {
        NivelDTO NivelDTO { get; set; }
        long DominioId { get; set; }
        IDominioNivelModel GetDominioNivel(IDominioNivelModel model);
        List<IDominioNivelModel> ListNivelDominio(IDominioDomainFactory factory);
    }
}