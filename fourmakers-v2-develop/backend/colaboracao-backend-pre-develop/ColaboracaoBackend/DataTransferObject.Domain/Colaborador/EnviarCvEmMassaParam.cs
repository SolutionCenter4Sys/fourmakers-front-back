using System.Collections.Generic;

namespace DataTransferObject.Domain.Colaborador
{
    public class EnviarCvEmMassaParam
    {
        public string Assunto { get; set; }
        public List<string> Emails { get; set; }
        public List<CvEmMassaColaboradorDTO> Colaboradores { get; set; }
    }
}