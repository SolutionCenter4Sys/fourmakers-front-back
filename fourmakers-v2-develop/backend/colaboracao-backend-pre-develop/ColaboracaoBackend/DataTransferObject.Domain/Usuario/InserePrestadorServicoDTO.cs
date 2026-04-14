namespace DataTransferObject.Domain.Usuario
{
    public class InserePrestadorServicoDTO
    {
        //public string Cnpj { get; set; }
        public PrestadorServicoDTO PrestadorServico { get; set; }
        public RegimeTributarioDTO RegimeTributario { get; set; }
    }
}