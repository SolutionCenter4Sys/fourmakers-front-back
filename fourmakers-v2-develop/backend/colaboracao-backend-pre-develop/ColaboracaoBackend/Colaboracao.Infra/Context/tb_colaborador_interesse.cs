using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_colaborador_interesse
    {
        //public tb_colaborador_interesse()
        //{
        //    tb_like_interesse = new HashSet<tb_like_interesse>();
        //}

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }
        public string codigo_interno_colaborador { get; set; }
        public long interesse_id { get; set; }
        public int ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }
        public int tipo_id { get; set; }
        public int skill_id { get; set; }
        public int nivel_id { get; set; }

        //public virtual tb_colaborador codigo_interno_colaboradorNavigation { get; set; }
        
        //public virtual tb_interesse interesse { get; set; }

        //public virtual ICollection<tb_like_interesse> tb_like_interesse { get; set; }
    }
}