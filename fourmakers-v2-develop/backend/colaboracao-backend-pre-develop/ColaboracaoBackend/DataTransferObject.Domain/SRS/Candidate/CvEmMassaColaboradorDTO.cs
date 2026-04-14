using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.SRS.Candidate
{
    public class CvEmMassaColaboradorDTO
    {
        public string Nome { get; set; }
        public List<string> Habilidades { get; set; }
        public string Link { get; set; }
    }
}
