using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Nivel;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Idioma
{
    public class ListaIdiomaResult : StatusResult
    {
        public ListaIdiomaResult()
        {
            Idioma = new List<ItemPerfilDTO>();
        }

        public List<ItemPerfilDTO> Idioma { get; set; }

        public List<NivelDTO> Nivel { get; set; }
    }
}