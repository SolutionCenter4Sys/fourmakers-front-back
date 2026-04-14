using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Usuario.Permissao
{
    public class GrupoAcessoFuncionalidadeSistemaDTO
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public bool Ativo { get; set; }
        public int OrgId { get; set; }
        public DateTime? DataCriacao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public bool AcessoTodosClientes { get; set; }
        public IEnumerable<FuncionalidadeSistemaDTO> FuncionalidadeSistema { get; set; }
    }
}