using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_vagas_srs
    {
        public tb_vagas_srs()
        {
            tb_skill_vaga = new HashSet<tb_skill_vaga>();
            tb_vaga_favorito = new HashSet<tb_vaga_favorito>();
        }

        public long id_vaga { get; set; }
        public string titulo { get; set; }
        public int? vagas_abertas { get; set; }
        public string nivel { get; set; }
        public DateTime? data_abertura { get; set; }
        public string status_vaga { get; set; }
        public string descricao { get; set; }
        public string cargo { get; set; }
        public string habilidades { get; set; }
        public string metodologia { get; set; }
        public DateTime? data_criacao { get; set; }
        public DateTime? data_alteracao { get; set; }
        public sbyte ativo { get; set; }
        public string loc_trabalho { get; set; }
        public string state { get; set; }
        public string textForLinkedin { get; set; }
        public int? tipoVaga { get; set; }
        public int? confidential_job { get; set; }
        public string termometro { get; set; }
        public string gestor_foursys { get; set; }
        public string? frequencia { get; set; }
        public DateTime? data_aprovacao { get; set; }
        public string? nome_aprovador { get; set; }
        public int maquina_cliente { get; set; }
        public int maquina_four { get; set; }
        public string notes { get; set; }

        public virtual ICollection<tb_skill_vaga> tb_skill_vaga { get; set; }
        public virtual ICollection<tb_vaga_favorito> tb_vaga_favorito { get; set; }
    }
}