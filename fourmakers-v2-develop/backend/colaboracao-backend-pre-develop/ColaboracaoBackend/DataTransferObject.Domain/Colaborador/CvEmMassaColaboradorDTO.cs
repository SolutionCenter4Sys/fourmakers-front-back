using System.Collections.Generic;

namespace DataTransferObject.Domain.Colaborador
{
    public class CvEmMassaColaboradorDTO
    {
        public string Nome { get; set; }
        public List<string> Habilidades { get; set; }
        public string Link { get; set; }
    }
}