namespace DataTransferObject.Domain.Filtro
{
    public class FiltroColaboradorDominioDTO
    {
        public string ColaboradorCpf { get; set; }
        public long DominioId { get; set; }
        public long? NivelId { get; set; }
        public string ColaboradorNome { get; set; }
    }
}