using DataTransferObject.Domain.Endosso;

namespace Core.Domain.Formacao
{
    public interface IFormacaoEndossoRepository
    {
        public StatusEndossoDTO GetTipoEndosso(EndossoColaboradorDTO model);
    }
}