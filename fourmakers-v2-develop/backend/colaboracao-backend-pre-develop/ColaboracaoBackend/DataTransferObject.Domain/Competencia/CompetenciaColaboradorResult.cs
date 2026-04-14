using DataTransferObject.Domain.Base;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Competencia
{
    public class CompetenciaColaboradorResult : StatusResult
    {
        public List<ItemPerfilResult> Respostas { get; set; }
    }
}