using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_colaborador_holerite
    {
        public long id { get; set; }
        public string path { get; set; }
        public string cpf { get; set; }
        public DateTime? emissao { get; set; }
        public int mes { get; set; }
        public int ano { get; set; }
        public long orgId { get; set; }
        public DateTime? data_criacao { get; set; }
    }
}