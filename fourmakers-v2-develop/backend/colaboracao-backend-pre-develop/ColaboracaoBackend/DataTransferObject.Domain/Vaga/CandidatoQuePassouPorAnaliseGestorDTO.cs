using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Vaga
{
    public class CandidatoQuePassouPorAnaliseGestorDTO
    {
        public string CodigoColaborador { get; set; }
        public string NomeCandidato { get; set; }
        public string StatusAtual { get; set; }
        public string DataCandidatura { get; set; }
    }
}
