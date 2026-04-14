namespace DataTransferObject.Domain.SRS.Candidate
{
    public class CadastrarAtualizarPreAprovarCandidadoLinkedinInput
    {
        public string UrlLinkedin { get; set; }
        public int? CodigoDaVaga { get; set; }
        public int UserId { get; set; }
    }
}