using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.SRS.Candidate;

namespace Colaborador.API.DTOs
{
    public class AlterarDadosColaboradorParam
    {
        public ColaboradorDTO ColaboradorDTO { get; set; }
        public byte[] Imagem { get; set; }
        public SRSCandidateDTO SRSCandidateDTO { get; set; }
    }
}