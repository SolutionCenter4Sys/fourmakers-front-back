using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_colaborador
    {
        public tb_colaborador()
        {
            tb_candidato = new HashSet<tb_candidato>();
            tb_colaborador_alocado = new HashSet<tb_colaborador_alocado>();
            tb_colaborador_cargo = new HashSet<tb_colaborador_cargo>();
            tb_colaborador_comentario = new HashSet<tb_colaborador_comentario>();
            tb_colaborador_competencia = new HashSet<tb_colaborador_competencia>();
            tb_certificado = new HashSet<tb_certificado>();
            tb_colaborador_dependente = new HashSet<tb_colaborador_dependente>();
            tb_colaborador_dominionegocio = new HashSet<tb_colaborador_dominionegocio>();
            tb_colaborador_formacao = new HashSet<tb_colaborador_formacao>();
            tb_colaborador_graugraduacao = new HashSet<tb_colaborador_graugraduacao>();
            tb_colaborador_hobbies = new HashSet<tb_colaborador_hobbies>();
            tb_colaborador_idioma = new HashSet<tb_colaborador_idioma>();
            tb_colaborador_interesse = new HashSet<tb_colaborador_interesse>();
            tb_colaborador_metodologia = new HashSet<tb_colaborador_metodologia>();
            tb_colaborador_modeloreferencia = new HashSet<tb_colaborador_modeloreferencia>();
            tb_colaborador_org = new HashSet<tb_colaborador_org>();
            tb_colaborador_passaporte = new HashSet<tb_colaborador_passaporte>();
            tb_colaborador_periodo_alocacao = new HashSet<tb_colaborador_periodo_alocacao>();
            tb_colaborador_projeto = new HashSet<tb_colaborador_projeto>();
            tb_colaborador_referencia_hardskill = new HashSet<tb_colaborador_referencia_hardskill>();
            tb_colaborador_sobre = new HashSet<tb_colaborador_sobre>();
            tb_colaborador_softskill = new HashSet<tb_colaborador_softskill>();
            tb_colaborador_status = new HashSet<tb_colaborador_status>();
            tb_colaborador_visto = new HashSet<tb_colaborador_visto>();
            tb_contato_colaboradorseguidor_codigo_interno_colaboradorNavigation = new HashSet<tb_contato_colaborador>();
            tb_contato_colaboradorseguindo_codigo_interno_colaboradorNavigation = new HashSet<tb_contato_colaborador>();
            tb_contato_emergencia = new HashSet<tb_contato_emergencia>();
            tb_curriculo_colaborador = new HashSet<tb_curriculo_colaborador>();
            tb_endosso_competencia = new HashSet<tb_endosso_competencia>();
            tb_endosso_dominionegocio = new HashSet<tb_endosso_dominionegocio>();
            tb_endosso_formacao = new HashSet<tb_endosso_formacao>();
            tb_endosso_metodologia = new HashSet<tb_endosso_metodologia>();
            tb_endosso_modeloreferencia = new HashSet<tb_endosso_modeloreferencia>();
            tb_escolaridade = new HashSet<tb_escolaridade>();
            tb_experiencia = new HashSet<tb_experiencia>();
            tb_like_cargo = new HashSet<tb_like_cargo>();
            tb_like_competencia = new HashSet<tb_like_competencia>();
            tb_like_dominionegocio = new HashSet<tb_like_dominionegocio>();
            tb_like_formacao = new HashSet<tb_like_formacao>();
            tb_like_hobbies = new HashSet<tb_like_hobbies>();
            tb_like_interesse = new HashSet<tb_like_interesse>();
            tb_like_metodologia = new HashSet<tb_like_metodologia>();
            tb_like_modeloreferencia = new HashSet<tb_like_modeloreferencia>();
            tb_like_noticia = new HashSet<tb_like_noticia>();
            tb_notificacao = new HashSet<tb_notificacao>();
            tb_pessoa_juridica = new HashSet<tb_pessoa_juridica>();
            tb_historico_cv = new HashSet<tb_historico_cv>();
        }

        public string codigo_interno_colaborador { get; set; }
        public string nome_completo { get; set; }
        public DateTime? data_nascimento { get; set; }
        public string rg { get; set; }
        public string matricula { get; set; }
        public long? endereco_id { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }
        public string contato_principal_ddi { get; set; }
        public string contato_principal { get; set; }
        public string contato_outro { get; set; }
        public long? imagem_id { get; set; }
        public sbyte candidato { get; set; }
        public string passaporte { get; set; }
        public int? colaborador_saude_id { get; set; }
        public string estado_civil { get; set; }
        public string genero { get; set; }
        public string etnia { get; set; }
        public string orientacao_sexual { get; set; }
        public string escolaridade { get; set; }
        public sbyte? refugiado { get; set; }
        public string email_alternativo { get; set; }
        public string nacionalidade { get; set; }
        public string documento_colaborador { get; set; }
        public string url_linkedin { get; set; }
        public DateTime? data_sync_linkedin { get; set; }
        public sbyte visualizar_busca_aderencia { get; set; }

        public virtual tb_colaborador_saude colaborador_saude { get; set; }
        public virtual tb_endereco endereco { get; set; }
        public virtual tb_imagem imagem { get; set; }
        public virtual ICollection<tb_usuario> tb_usuario { get; set; }
        public virtual ICollection<tb_candidato> tb_candidato { get; set; }
        public virtual ICollection<tb_colaborador_alocado> tb_colaborador_alocado { get; set; }
        public virtual ICollection<tb_colaborador_cargo> tb_colaborador_cargo { get; set; }
        public virtual ICollection<tb_colaborador_comentario> tb_colaborador_comentario { get; set; }
        public virtual ICollection<tb_colaborador_competencia> tb_colaborador_competencia { get; set; }
        public virtual ICollection<tb_certificado> tb_certificado { get; set; }
        public virtual ICollection<tb_colaborador_dependente> tb_colaborador_dependente { get; set; }
        public virtual ICollection<tb_colaborador_dominionegocio> tb_colaborador_dominionegocio { get; set; }
        public virtual ICollection<tb_colaborador_formacao> tb_colaborador_formacao { get; set; }
        public virtual ICollection<tb_colaborador_graugraduacao> tb_colaborador_graugraduacao { get; set; }
        public virtual ICollection<tb_colaborador_hobbies> tb_colaborador_hobbies { get; set; }
        public virtual ICollection<tb_colaborador_idioma> tb_colaborador_idioma { get; set; }
        public virtual ICollection<tb_colaborador_interesse> tb_colaborador_interesse { get; set; }
        public virtual ICollection<tb_colaborador_metodologia> tb_colaborador_metodologia { get; set; }
        public virtual ICollection<tb_colaborador_modeloreferencia> tb_colaborador_modeloreferencia { get; set; }
        public virtual ICollection<tb_colaborador_org> tb_colaborador_org { get; set; }
        public virtual ICollection<tb_colaborador_passaporte> tb_colaborador_passaporte { get; set; }
        public virtual ICollection<tb_colaborador_periodo_alocacao> tb_colaborador_periodo_alocacao { get; set; }
        public virtual ICollection<tb_colaborador_projeto> tb_colaborador_projeto { get; set; }
        public virtual ICollection<tb_colaborador_referencia_hardskill> tb_colaborador_referencia_hardskill { get; set; }
        public virtual ICollection<tb_colaborador_sobre> tb_colaborador_sobre { get; set; }
        public virtual ICollection<tb_colaborador_softskill> tb_colaborador_softskill { get; set; }
        public virtual ICollection<tb_colaborador_status> tb_colaborador_status { get; set; }
        public virtual ICollection<tb_colaborador_visto> tb_colaborador_visto { get; set; }
        public virtual ICollection<tb_contato_colaborador> tb_contato_colaboradorseguidor_codigo_interno_colaboradorNavigation { get; set; }
        public virtual ICollection<tb_contato_colaborador> tb_contato_colaboradorseguindo_codigo_interno_colaboradorNavigation { get; set; }
        public virtual ICollection<tb_contato_emergencia> tb_contato_emergencia { get; set; }
        public virtual ICollection<tb_curriculo_colaborador> tb_curriculo_colaborador { get; set; }
        public virtual ICollection<tb_endosso_competencia> tb_endosso_competencia { get; set; }
        public virtual ICollection<tb_endosso_dominionegocio> tb_endosso_dominionegocio { get; set; }
        public virtual ICollection<tb_endosso_formacao> tb_endosso_formacao { get; set; }
        public virtual ICollection<tb_endosso_metodologia> tb_endosso_metodologia { get; set; }
        public virtual ICollection<tb_endosso_modeloreferencia> tb_endosso_modeloreferencia { get; set; }
        public virtual ICollection<tb_escolaridade> tb_escolaridade { get; set; }
        public virtual ICollection<tb_experiencia> tb_experiencia { get; set; }
        public virtual ICollection<tb_like_cargo> tb_like_cargo { get; set; }
        public virtual ICollection<tb_like_competencia> tb_like_competencia { get; set; }
        public virtual ICollection<tb_like_dominionegocio> tb_like_dominionegocio { get; set; }
        public virtual ICollection<tb_like_formacao> tb_like_formacao { get; set; }
        public virtual ICollection<tb_like_hobbies> tb_like_hobbies { get; set; }
        public virtual ICollection<tb_like_interesse> tb_like_interesse { get; set; }
        public virtual ICollection<tb_like_metodologia> tb_like_metodologia { get; set; }
        public virtual ICollection<tb_like_modeloreferencia> tb_like_modeloreferencia { get; set; }
        public virtual ICollection<tb_like_noticia> tb_like_noticia { get; set; }
        public virtual ICollection<tb_notificacao> tb_notificacao { get; set; }
        public virtual ICollection<tb_pessoa_juridica> tb_pessoa_juridica { get; set; }
        public virtual ICollection<tb_historico_cv> tb_historico_cv { get; set; }
    }
}