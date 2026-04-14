using Competencia.Domain.Interfaces.Factorys;
using DataTransferObject.Domain.Endosso;
using System;
using System.Collections.Generic;

namespace Competencia.Domain.Interfaces.Models
{
    public interface IDominioEndossoColaboradorModel
    {
        long IdDominioColaborador { get; set; }
        string cpfColaborador { get; set; }
        DateTime dataEndosso { get; set; }
        int? TipoEndossoId { get; set; }
        List<EndossoColaboradorDTO> EndossoConcedido { get; set; }
        List<IDominioEndossoColaboradorModel> GetEndossoConcedido(IDominioEndossoColaboradorModel model, IDominioDomainFactory factory);
    }
}