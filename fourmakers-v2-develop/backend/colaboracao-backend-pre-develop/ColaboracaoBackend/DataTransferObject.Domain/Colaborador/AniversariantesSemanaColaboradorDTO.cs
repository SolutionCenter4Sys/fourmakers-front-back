namespace DataTransferObject.Domain.Colaborador
{
    public class AniversariantesSemanaColaboradorDTO
    {
        public string CodigoColaborador { get; set; }
        public string Nome { get; set; }
        public string DataNascimento { get; set; }
        public string Email { get; set; }
        public string ImagemId { get; set; }
        public string DiaDaSemana { get; set; }
        public string UrlFoto { get; set; }
        public string UrlFotoThumb { get; set; }
        public string UrlFotoThumbMini { get; set; }
        public string UrlFotoThumbVeryMini { get; set; }
        public bool EhAniversarianteHoje { get; set; } = false;
    }
}