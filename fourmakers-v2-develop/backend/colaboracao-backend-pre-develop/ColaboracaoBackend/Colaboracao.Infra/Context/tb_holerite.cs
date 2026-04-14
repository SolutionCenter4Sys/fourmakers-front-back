using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_holerite
    {
        public long id { get; set; }
        public string competencia_mes { get; set; }
        public string competencia_ano { get; set; }
        public int status { get; set; }
        public string lg_id_tarefa { get; set; }
        public string lg_blob_url_arquivo { get; set; }
        public string s3_url_arquivo { get; set; }
        public DateTime? data_emissao { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }
        public long tb_colaborador_lg_id { get; set; }
        public int processo { get; set; }

        public virtual tb_colaborador_lg tb_colaborador_lg { get; set; }
    }
}