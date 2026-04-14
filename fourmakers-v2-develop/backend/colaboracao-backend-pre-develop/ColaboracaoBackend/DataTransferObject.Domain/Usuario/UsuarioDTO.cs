using System;

namespace DataTransferObject.Domain.Usuario
{
    public class UsuarioDTO
    {
        public long Id { get; set; }
        public string CodigoInternoColaborador { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string FcmToken { get; set; }
        public sbyte PrimeiroAcessoRealizado { get; set; }
        public sbyte Ativo { get; set; }
        public int Sistemico { get; set; }
        public DateTime? DataExpiracao { get; set; }
        public DateTime? DataAceiteTermo { get; set; }
        public int OrgId { get; set; }
    }
}