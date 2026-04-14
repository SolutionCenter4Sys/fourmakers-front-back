using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.SRS.Candidate
{
    public class CvEmMassaAderenciaColaboradorDTO : CvEmMassaColaboradorDTO
    {
        public decimal NivelAderencia { get; set; }
    }
}
