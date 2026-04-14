using Competencia.Domain.Interfaces.Factorys;
using DataTransferObject.Domain.Dominio;
using System.Collections.Generic;

namespace Competencia.Domain.Interfaces.Models
{
    public interface IDominioColaboradorModel
    {
        DominioColaboradorDTO DominioColaboradorDTO { get; set; }
        List<IDominioColaboradorModel> ListDominioColaborador(IDominioDomainFactory factory);
        void RemoveDominioColaborador();
        IDominioColaboradorModel SaveModel();
        IDominioColaboradorModel AlteraDominioColaborador();
    }
}