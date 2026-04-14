using Competencia.Domain.Interfaces.Factorys;
using DataTransferObject.Domain.Dominio;
using System.Collections.Generic;

namespace Competencia.Domain.Interfaces.Models
{
    public interface IDominioModel
    {
        DominioDTO DominioDTO { get; set; }
        IDominioModel GetById(long id);
        IDominioModel GetUsuarioCriacaoId(string cpfColaborador, IDominioDomainFactory factory);
        List<IDominioModel> List(string busca, int cursor, int limite, IDominioDomainFactory factory);
        IDominioModel SaveModel();
    }
}