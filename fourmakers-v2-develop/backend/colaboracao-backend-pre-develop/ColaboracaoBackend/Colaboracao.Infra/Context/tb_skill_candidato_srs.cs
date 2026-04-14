using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_skill_candidato_srs
    {
        public int id { get; set; }
        public long? candidato_id { get; set; }
        public int? categoria_id { get; set; }
        public int? descricao_id { get; set; }
        public int? nivel_id { get; set; }
        public DateTime? data_criacao { get; set; }
        public DateTime? data_alteracao { get; set; }
    }
}