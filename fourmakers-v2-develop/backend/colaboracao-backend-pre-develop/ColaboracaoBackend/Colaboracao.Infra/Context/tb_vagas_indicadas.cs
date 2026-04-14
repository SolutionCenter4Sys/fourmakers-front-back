using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_vagas_indicadas
    {
        public long id { get; set; }
        public long id_vaga { get; set; }
        public long id_usuario_indicou { get; set; }
        public string nome_candidato { get; set; }
        public string url_linkedin_candidato { get; set; }

        public DateTime? data_criacao { get; set; }
        public DateTime? data_aplicacao { get; set; }
        public string link_URL { get; set; }
        public bool ativo { get; set; }
    }
}