using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_empresa_usuario
    {
        public DateTime data_convite { get; set; }
        public DateTime? data_aceite { get; set; }
        public sbyte confirmado { get; set; }
        public sbyte pendente { get; set; }
        public sbyte ativo { get; set; }
        public sbyte responsavel_acesso { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }
        public string tb_empresa_cnpj { get; set; }
        public long tb_usuario_id { get; set; }

        public virtual tb_empresa tb_empresa_cnpjNavigation { get; set; }
        public virtual tb_usuario tb_usuario { get; set; }
    }
}