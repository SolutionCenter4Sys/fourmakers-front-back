using DataTransferObject.Domain.Endosso;

namespace Competencia.Domain.Interfaces.Models
{
    public interface IDominioTipoEndossoModel
    {
        TipoEndossoDTO TipoEndossoDTO { get; set; }
        IDominioTipoEndossoModel GetTipoEndosso(IDominioTipoEndossoModel model);
    }
}