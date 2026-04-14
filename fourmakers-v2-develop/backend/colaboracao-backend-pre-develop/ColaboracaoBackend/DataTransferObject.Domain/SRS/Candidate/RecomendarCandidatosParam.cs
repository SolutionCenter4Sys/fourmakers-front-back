using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.SRS.Candidate
{
    public class RecomendarCandidatosParam
    {
        public string Assunto { get; set; }
        public List<string> Emails { get; set; }
        public List<CvEmMassaAderenciaColaboradorDTO> Colaboradores { get; set; }
        public int VagaId { get; set; }
        public string FormatoNomeExibicao { get; set; }
    }
}
