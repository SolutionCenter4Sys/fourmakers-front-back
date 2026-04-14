using System;

namespace DataTransferObject.Domain.Usuario.Permissao
{
    public class FuncionalidadeSistemaDTO
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public bool Ativo { get; set; }
        public DateTime? DataCriacao { get; set; }
        public DateTime? DataAlteracao { get; set; }
    }
}