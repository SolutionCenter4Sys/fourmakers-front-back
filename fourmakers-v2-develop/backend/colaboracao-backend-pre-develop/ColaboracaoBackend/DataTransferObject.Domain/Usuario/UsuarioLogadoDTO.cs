namespace DataTransferObject.Domain.Usuario
{
    public class UsuarioLogadoDTO
    {
        public string Cpf { get; set; }
        public string Email { get; set; }
        public string CodColaborador { get; set; }
        public int OrgId { get; set; }
        public string Token { get; set; }
        public TipoLoginEnum TipoLogin { get; set; }
    }
}