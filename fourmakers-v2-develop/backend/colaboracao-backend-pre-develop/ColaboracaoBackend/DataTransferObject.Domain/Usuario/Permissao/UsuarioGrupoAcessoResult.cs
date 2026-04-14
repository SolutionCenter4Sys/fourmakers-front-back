using System;

namespace DataTransferObject.Domain.Usuario.Permissao
{
    public class UsuarioGrupoAcessoResult
    {
        public long UsuarioId { get; set; }
        public string NomeCompleto { get; set; }
        public string Cpf { get; set; }
        public int GrupoAcessoId { get; set; }
        public string Descricao { get; set; }
        public DateTime? DataCriacao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public bool Ativo { get; set; }
        public int OrgId { get; set; }
    }
}