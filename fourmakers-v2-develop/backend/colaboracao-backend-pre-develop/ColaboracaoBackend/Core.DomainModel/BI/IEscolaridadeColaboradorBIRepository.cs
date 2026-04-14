using DataTransferObject.Domain.Escolaridade;
using System.Collections.Generic;

namespace Core.Domain.BI
{
    public interface IEscolaridadeColaboradorBIRepository
    {
        List<KeyValuePair<string, EscolaridadeDTO>> Listar();
    }
}