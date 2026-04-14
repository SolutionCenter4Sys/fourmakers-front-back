using System;

namespace DataTransferObject.Domain.Usuario.Permissao
{
    public class GrupoAcessoFuncionalidadeSistemaResult
    {
        public int GrupoAcessoId { get; set; }
        public string GrupoAcessoDescricao { get; set; }
        public bool GrupoAcessoAtivo { get; set; }
        public int GrupoAcessoOrgId { get; set; }
        public DateTime? GrupoAcessoDataCriacao { get; set; }
        public DateTime? GrupoAcessoDataAlteracao { get; set; }
        public bool GrupoAcessoAcessoTodosClientes { get; set; }
        public int FuncionalidadeSistemaId { get; set; }
        public string FuncionalidadeSistemaDescricao { get; set; }
        public bool FuncionalidadeSistemaAtivo { get; set; }
        public DateTime? FuncionalidadeSistemaDataCriacao { get; set; }
        public DateTime? FuncionalidadeSistemaDataAlteracao { get; set; }
    }
}