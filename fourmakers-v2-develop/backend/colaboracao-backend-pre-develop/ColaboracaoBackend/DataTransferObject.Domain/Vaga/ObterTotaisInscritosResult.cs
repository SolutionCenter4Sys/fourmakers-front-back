using System.Collections.Generic;

namespace DataTransferObject.Domain.Vaga
{
    public class ObterTotaisInscritosResult
    {
        public Dictionary<string, int> TotalCandidatosInscritos { get; set; }
        
        public ObterTotaisInscritosResult()
        {
            TotalCandidatosInscritos = new Dictionary<string, int>();
        }
    }
}
