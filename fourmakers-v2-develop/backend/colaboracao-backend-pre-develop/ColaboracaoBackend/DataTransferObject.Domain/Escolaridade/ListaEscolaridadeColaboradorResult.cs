using DataTransferObject.Domain.Base;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Escolaridade
{
    public class ListaEscolaridadeColaboradorResult : StatusResult
    {
        public List<EscolaridadeDTO> Escolaridade { get; set; }
    }
}