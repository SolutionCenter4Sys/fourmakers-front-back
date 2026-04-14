using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Vaga
{
    public class CountCandidatosInscritosResult
    {
        public Dictionary<string, int> TotalCandidatosInscritos { get; set; } = new Dictionary<string, int>();
    }
}
