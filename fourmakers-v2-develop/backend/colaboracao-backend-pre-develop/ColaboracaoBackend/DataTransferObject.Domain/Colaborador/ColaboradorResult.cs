using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Dependentes;
using DataTransferObject.Domain.SRS;
using DataTransferObject.Domain.Usuario;

namespace DataTransferObject.Domain.Colaborador
{
    public class ColaboradorResult : StatusResult
    {
        public ColaboradorResult()
        {
            DependenteColaborador = new BuscaDependentesResult();
        }

        public ColaboradorDTO Colaborador { get; set; }
        public RedeColaboradorDTO RedeColaborador { get; set; }
        public PerfilProfissionalDTO PerfilProfissional { get; set; }
        public AtividadeColaboradorDTO AtividadeColaborador { get; set; }
        public BuscaDependentesResult DependenteColaborador { get; set; }
        public SRSCandidateResult SRSCandidate { get; set; }
        public UsuarioColaboradorDTO Usuario { get; set; }
        public int TotalResultCount { get; set; }
    }
}