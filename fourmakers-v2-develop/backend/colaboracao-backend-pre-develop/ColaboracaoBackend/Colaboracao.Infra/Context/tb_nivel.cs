using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_nivel
    {
        public tb_nivel()
        {
            tb_colaborador_competencia = new HashSet<tb_colaborador_competencia>();
            tb_colaborador_dominionegocio = new HashSet<tb_colaborador_dominionegocio>();
            tb_colaborador_formacao = new HashSet<tb_colaborador_formacao>();
            tb_colaborador_idioma = new HashSet<tb_colaborador_idioma>();
            tb_colaborador_metodologia = new HashSet<tb_colaborador_metodologia>();
            tb_colaborador_modeloreferencia = new HashSet<tb_colaborador_modeloreferencia>();
            tb_colaborador_softskill = new HashSet<tb_colaborador_softskill>();
            tb_filtro_competencia_nivel = new HashSet<tb_filtro_competencia_nivel>();
            tb_filtro_dominionegocio_nivel = new HashSet<tb_filtro_dominionegocio_nivel>();
            tb_filtro_formacao_nivel = new HashSet<tb_filtro_formacao_nivel>();
            tb_filtro_metodologia_nivel = new HashSet<tb_filtro_metodologia_nivel>();
            tb_filtro_modeloreferencia_nivel = new HashSet<tb_filtro_modeloreferencia_nivel>();
            tb_filtro_softskills = new HashSet<tb_filtro_softskills>();
            tb_historico_cv = new HashSet<tb_historico_cv>();
        }

        public long id { get; set; }
        public string descricao { get; set; }

        public int prioridade_unificacao { get; set; }
        public int? ordem_exibicao { get; set; }

        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }
        public long tb_item_perfil_id { get; set; }

        public virtual tb_item_perfil tb_item_perfil { get; set; }
        public virtual ICollection<tb_colaborador_competencia> tb_colaborador_competencia { get; set; }
        public virtual ICollection<tb_colaborador_dominionegocio> tb_colaborador_dominionegocio { get; set; }
        public virtual ICollection<tb_colaborador_formacao> tb_colaborador_formacao { get; set; }
        public virtual ICollection<tb_colaborador_idioma> tb_colaborador_idioma { get; set; }
        public virtual ICollection<tb_colaborador_metodologia> tb_colaborador_metodologia { get; set; }
        public virtual ICollection<tb_colaborador_modeloreferencia> tb_colaborador_modeloreferencia { get; set; }
        public virtual ICollection<tb_colaborador_softskill> tb_colaborador_softskill { get; set; }
        public virtual ICollection<tb_filtro_competencia_nivel> tb_filtro_competencia_nivel { get; set; }
        public virtual ICollection<tb_filtro_dominionegocio_nivel> tb_filtro_dominionegocio_nivel { get; set; }
        public virtual ICollection<tb_filtro_formacao_nivel> tb_filtro_formacao_nivel { get; set; }
        public virtual ICollection<tb_filtro_metodologia_nivel> tb_filtro_metodologia_nivel { get; set; }
        public virtual ICollection<tb_filtro_modeloreferencia_nivel> tb_filtro_modeloreferencia_nivel { get; set; }
        public virtual ICollection<tb_filtro_softskills> tb_filtro_softskills { get; set; }
        public virtual ICollection<tb_historico_cv> tb_historico_cv { get; set; }
    }
}