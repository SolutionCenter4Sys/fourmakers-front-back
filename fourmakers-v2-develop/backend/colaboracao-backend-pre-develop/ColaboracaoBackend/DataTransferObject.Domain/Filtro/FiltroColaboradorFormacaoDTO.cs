namespace DataTransferObject.Domain.Filtro
{
    public class FiltroColaboradorFormacaoDTO
    {
        public string ColaboradorCpf { get; set; }
        public string ColaboradorNome { get; set; }
        public long FormacaoId { get; set; }
        public long? NivelId { get; set; }
    }
}