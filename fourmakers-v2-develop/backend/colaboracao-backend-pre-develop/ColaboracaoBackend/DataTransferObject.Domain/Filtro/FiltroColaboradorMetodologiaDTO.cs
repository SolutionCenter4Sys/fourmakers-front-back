namespace DataTransferObject.Domain.Filtro
{
    public class FiltroColaboradorMetodologiaDTO
    {
        public string ColaboradorCpf { get; set; }
        public long MetodologiaId { get; set; }
        public long? NivelId { get; set; }
        public string ColaboradorNome { get; set; }
    }
}