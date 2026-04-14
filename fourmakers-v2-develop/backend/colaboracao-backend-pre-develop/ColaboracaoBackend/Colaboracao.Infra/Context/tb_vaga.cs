using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_vaga
    {
        public long Id { get; set; }
        public int? id_vaga { get; set; }
        public string titulo { get; set; }
        public string status { get; set; }
        public string tipo { get; set; }
        public int? numero_de_vagas { get; set; }
        public string taxa_maxima_hora { get; set; }
        public string nivel_de_urgencia { get; set; }
        public string descricao { get; set; }
        public string funcao { get; set; }
        public DateTime? data_criacao { get; set; }
        public DateTime? data_atualizacao { get; set; }
        public DateTime? data_publicacao { get; set; }
        public DateTime? data_aceitacao { get; set; }
        public string tipo_localizacao { get; set; }
        public string observacao_localizacao { get; set; }
        public string estado { get; set; }
        public string cidade { get; set; }
        public bool? visibilidade { get; set; }
        public string email { get; set; }
    }
}