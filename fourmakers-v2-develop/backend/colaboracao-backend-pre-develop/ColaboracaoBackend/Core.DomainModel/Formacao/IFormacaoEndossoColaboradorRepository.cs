using DataTransferObject.Domain.Endosso;
using System.Collections.Generic;

namespace Core.Domain.Formacao
{
    public interface IFormacaoEndossoColaboradorRepository
    {
        public List<EndossoColaboradorDTO> ListEndossoColaborador(EndossoColaboradorDTO model);
        // void ListEndossoColaborador(List<EndossoColaboradorDTO> endossoConcedido);
    }
}