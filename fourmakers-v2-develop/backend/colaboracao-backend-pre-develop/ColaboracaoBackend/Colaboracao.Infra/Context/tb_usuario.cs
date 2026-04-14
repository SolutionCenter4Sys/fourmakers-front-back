using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_usuario
    {
        public tb_usuario()
        {
            tb_competencia = new HashSet<tb_competencia>();
            tb_dominionegocio = new HashSet<tb_dominionegocio>();
            tb_empresa_usuario = new HashSet<tb_empresa_usuario>();
            tb_formacao = new HashSet<tb_formacao>();
            tb_graugraduacao = new HashSet<tb_graugraduacao>();
            tb_hobbies = new HashSet<tb_hobbies>();
            tb_interesse = new HashSet<tb_interesse>();
            tb_metodologia = new HashSet<tb_metodologia>();
            tb_modeloreferencia = new HashSet<tb_modeloreferencia>();
            tb_token_resete_senha = new HashSet<tb_token_resete_senha>();
            tb_token_sso = new HashSet<tb_token_sso>();
            tb_usuario_grupo_acesso = new HashSet<tb_usuario_grupo_acesso>();
            tb_usuario_tokenacesso = new HashSet<tb_usuario_tokenacesso>();
            tb_vaga_favorito = new HashSet<tb_vaga_favorito>();
        }

        public long id { get; set; }
        public string codigo_interno_colaborador { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }
        public sbyte primeiro_acesso_realizado { get; set; }
        public sbyte ativo { get; set; }
        public string slack_id { get; set; }
        public string slack_team_id { get; set; }
        public string slack_username { get; set; }
        public string fcm_token { get; set; }
        public sbyte sistemico { get; set; }
        public DateTime? data_expiracao { get; set; }

        public DateTime? dataAceiteTermo { get; set; }
        public int tb_org_id { get; set; }

        public virtual tb_colaborador codigo_interno_colaboradorNavigation { get; set; }
        public virtual ICollection<tb_competencia> tb_competencia { get; set; }
        public virtual ICollection<tb_dominionegocio> tb_dominionegocio { get; set; }
        public virtual ICollection<tb_empresa_usuario> tb_empresa_usuario { get; set; }
        public virtual ICollection<tb_formacao> tb_formacao { get; set; }
        public virtual ICollection<tb_graugraduacao> tb_graugraduacao { get; set; }
        public virtual ICollection<tb_hobbies> tb_hobbies { get; set; }
        public virtual ICollection<tb_interesse> tb_interesse { get; set; }
        public virtual ICollection<tb_metodologia> tb_metodologia { get; set; }
        public virtual ICollection<tb_modeloreferencia> tb_modeloreferencia { get; set; }
        public virtual ICollection<tb_token_resete_senha> tb_token_resete_senha { get; set; }
        public virtual ICollection<tb_token_sso> tb_token_sso { get; set; }
        public virtual ICollection<tb_usuario_grupo_acesso> tb_usuario_grupo_acesso { get; set; }
        public virtual ICollection<tb_usuario_tokenacesso> tb_usuario_tokenacesso { get; set; }
        public virtual ICollection<tb_vaga_favorito> tb_vaga_favorito { get; set; }
    }
}