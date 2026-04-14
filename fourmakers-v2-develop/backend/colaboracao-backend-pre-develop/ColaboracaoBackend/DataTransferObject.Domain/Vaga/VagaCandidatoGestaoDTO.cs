namespace DataTransferObject.Domain.Vaga
{
    public class VagaCandidatoGestaoDTO
    {
        public int VagaId { get; set; }
        public string Titulo { get; set; }
        public string StatusCandidato { get; set; }
        public string Candidato { get; set; }
        public string CodigoInternoColaborador { get; set; }
        public string StatusVaga { get; set; }
    }
}