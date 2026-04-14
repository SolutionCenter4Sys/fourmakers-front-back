using DataTransferObject.Domain.Endosso;

namespace Competencia.Domain.Interfaces.Models
{
    public interface IDominioEndossoModel
    {
        long IdDominioColaborador { get; set; }
        StatusEndossoDTO StatusEndossoDTO { get; set; }
        IDominioEndossoModel GetDominioEndosso(IDominioEndossoModel model);
    }
}