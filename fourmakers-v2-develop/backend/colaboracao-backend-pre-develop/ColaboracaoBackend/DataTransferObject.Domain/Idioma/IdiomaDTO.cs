namespace DataTransferObject.Domain.Idioma
{
    public class IdiomaDTO
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public string CpfUsuarioCriacao { get; set; }
        public bool Pendente { get; set; }
    }
}