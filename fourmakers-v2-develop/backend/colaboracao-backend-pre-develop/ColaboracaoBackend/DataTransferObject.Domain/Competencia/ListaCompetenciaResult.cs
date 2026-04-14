using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Nivel;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Competencia
{
    public class ListaCompetenciaResult : StatusResult
    {
        public ListaCompetenciaResult()
        {
            Competencias = new List<ItemPerfilDTO>();
        }
        public List<ItemPerfilDTO> Competencias { get; set; }
        public List<NivelDTO> Nivel { get; set; }
    }
}