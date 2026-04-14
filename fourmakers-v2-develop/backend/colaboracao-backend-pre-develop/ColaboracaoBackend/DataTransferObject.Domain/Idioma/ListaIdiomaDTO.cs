using DataTransferObject.Domain.Base;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Idioma
{
    public class ListaIdiomaInfo : StatusResult
    {
        public List<IdiomaDTO> Idioma { get; set; }
    }
}