using System.Collections.Generic;
using DataTransferObject.Domain.Match;

namespace DataTransferObject.Domain.Vaga
{
    public class ListarCandidatosAderentesEInscritosResult
    {
        public IEnumerable<ListarCandidatosAderentesResult> CandidatosAderentes { get; set; }
        public IEnumerable<ListarCandidatosInscritosResult> CandidatosInscritos { get; set; }
        public int TotalCandidatosAderentes { get; set; }
        public Dictionary<string, int> TotalCandidatosInscritos { get; set; }
        
        public ListarCandidatosAderentesEInscritosResult()
        {
            CandidatosAderentes = new List<ListarCandidatosAderentesResult>();
            CandidatosInscritos = new List<ListarCandidatosInscritosResult>();
            TotalCandidatosAderentes = 0;
            TotalCandidatosInscritos = new Dictionary<string, int>();
        }
    }
}
