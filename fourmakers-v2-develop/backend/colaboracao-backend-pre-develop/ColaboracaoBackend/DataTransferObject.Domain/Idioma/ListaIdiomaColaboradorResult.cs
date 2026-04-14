using DataTransferObject.Domain.Base;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Idioma
{
    public class ListaIdiomaColaboradorResult : StatusResult
    {
        public ListaIdiomaColaboradorResult()
        {
            Idioma = new List<IdiomaColaboradorDTO>();
        }

        public List<IdiomaColaboradorDTO> Idioma { get; set; }
    }
}