using DataTransferObject.Domain.Base;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Colaborador
{
    public class CompetenciasSumarioResult : StatusResult
    {
        public List<ColaboradorSumarioDTO> ListaColaboradoresSumario { get; set; }
    }
}