using System;

namespace DataTransferObject.Domain.Usuario.Permissao
{
    public class UsuarioPermissaoLogDTO
    {
        public int OrgId { get; set; }
        public DateTime DataAlteracao { get; set; }
        public string ColaboradorCpfCriacao { get; set; }
        public string Operacao { get; set; }
        public int? UsuarioId { get; set; }
        public int? GrupoAcessoId { get; set; }
        public int? FuncionalidadeSistemaId { get; set; }
    }
}