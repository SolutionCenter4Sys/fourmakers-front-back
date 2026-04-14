using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class ColaboradorContext : DbContext
    {
        public ColaboradorContext()
        {
        }

        public ColaboradorContext(DbContextOptions<ColaboradorContext> options)
            : base(options)
        {
        }

        public virtual DbSet<buscacolaborador> buscacolaborador { get; set; }
        public virtual DbSet<cadastrocandidatosmes> cadastrocandidatosmes { get; set; }
        public virtual DbSet<colaboradorpordiretoria> colaboradorpordiretoria { get; set; }
        public virtual DbSet<estatisticas_escolaridade> estatisticas_escolaridade { get; set; }
        public virtual DbSet<estatisticas_etnia> estatisticas_etnia { get; set; }
        public virtual DbSet<estatisticas_genero> estatisticas_genero { get; set; }
        public virtual DbSet<estatisticas_idade> estatisticas_idade { get; set; }
        public virtual DbSet<estatisticas_orientacao_sexual> estatisticas_orientacao_sexual { get; set; }
        public virtual DbSet<estatisticas_tempo_servico> estatisticas_tempo_servico { get; set; }
        public virtual DbSet<graficocompetenciacolab> graficocompetenciacolab { get; set; }
        public virtual DbSet<subgraficocolaboradorcompetencia> subgraficocolaboradorcompetencia { get; set; }
        public virtual DbSet<subgraficocompetenciacandidato> subgraficocompetenciacandidato { get; set; }
        public virtual DbSet<tb_acesso> tb_acesso { get; set; }
        public virtual DbSet<tb_ad_sso> tb_ad_sso { get; set; }
        public virtual DbSet<tb_apontamento_periodo_fechado> tb_apontamento_periodo_fechado { get; set; }
        public virtual DbSet<tb_apontamento_periodo_fechado_log> tb_apontamento_periodo_fechado_log { get; set; }
        public virtual DbSet<tb_atividade> tb_atividade { get; set; }
        public virtual DbSet<tb_bancos> tb_bancos { get; set; }
        public virtual DbSet<tb_candidato> tb_candidato { get; set; }
        public virtual DbSet<tb_candidatos_vaga> tb_candidatos_vaga { get; set; }
        public virtual DbSet<tb_cargo> tb_cargo { get; set; }
        public virtual DbSet<tb_certificado> tb_certificado { get; set; }
        public virtual DbSet<tb_cliente_org> tb_cliente_org { get; set; }
        public virtual DbSet<tb_colaborador> tb_colaborador { get; set; }
        public virtual DbSet<tb_colaborador_alocado> tb_colaborador_alocado { get; set; }
        public virtual DbSet<tb_colaborador_apontamento> tb_colaborador_apontamento { get; set; }
        public virtual DbSet<tb_colaborador_apontamento_log> tb_colaborador_apontamento_log { get; set; }
        public virtual DbSet<tb_colaborador_cargo> tb_colaborador_cargo { get; set; }
        public virtual DbSet<tb_colaborador_cch> tb_colaborador_cch { get; set; }
        public virtual DbSet<tb_colaborador_comentario> tb_colaborador_comentario { get; set; }
        public virtual DbSet<tb_colaborador_competencia> tb_colaborador_competencia { get; set; }
        public virtual DbSet<tb_colaborador_competencia_certificado> tb_colaborador_competencia_certificado { get; set; }
        public virtual DbSet<tb_colaborador_dependente> tb_colaborador_dependente { get; set; }
        public virtual DbSet<tb_colaborador_dominionegocio> tb_colaborador_dominionegocio { get; set; }
        public virtual DbSet<tb_colaborador_formacao> tb_colaborador_formacao { get; set; }
        public virtual DbSet<tb_colaborador_graugraduacao> tb_colaborador_graugraduacao { get; set; }
        public virtual DbSet<tb_colaborador_hierarquia> tb_colaborador_hierarquia { get; set; }
        public virtual DbSet<tb_colaborador_hobbies> tb_colaborador_hobbies { get; set; }
        public virtual DbSet<tb_colaborador_holerite> tb_colaborador_holerite { get; set; }
        public virtual DbSet<tb_colaborador_idioma> tb_colaborador_idioma { get; set; }
        public virtual DbSet<tb_colaborador_interesse> tb_colaborador_interesse { get; set; }
        public virtual DbSet<tb_colaborador_lg> tb_colaborador_lg { get; set; }
        public virtual DbSet<tb_colaborador_metodologia> tb_colaborador_metodologia { get; set; }
        public virtual DbSet<tb_colaborador_modeloreferencia> tb_colaborador_modeloreferencia { get; set; }
        public virtual DbSet<tb_colaborador_nacionalidade> tb_colaborador_nacionalidade { get; set; }
        public virtual DbSet<tb_colaborador_org> tb_colaborador_org { get; set; }
        public virtual DbSet<tb_colaborador_pais> tb_colaborador_pais { get; set; }
        public virtual DbSet<tb_colaborador_passaporte> tb_colaborador_passaporte { get; set; }
        public virtual DbSet<tb_colaborador_periodo_alocacao> tb_colaborador_periodo_alocacao { get; set; }
        public virtual DbSet<tb_colaborador_periodo_alocacao_calculo_mensal> tb_colaborador_periodo_alocacao_calculo_mensal { get; set; }
        public virtual DbSet<tb_colaborador_projeto> tb_colaborador_projeto { get; set; }
        public virtual DbSet<tb_colaborador_projeto_org> tb_colaborador_projeto_org { get; set; }
        public virtual DbSet<tb_colaborador_referencia_hardskill> tb_colaborador_referencia_hardskill { get; set; }
        public virtual DbSet<tb_colaborador_saude> tb_colaborador_saude { get; set; }
        public virtual DbSet<tb_colaborador_sobre> tb_colaborador_sobre { get; set; }
        public virtual DbSet<tb_colaborador_softskill> tb_colaborador_softskill { get; set; }
        public virtual DbSet<tb_colaborador_status> tb_colaborador_status { get; set; }
        public virtual DbSet<tb_colaborador_visto> tb_colaborador_visto { get; set; }
        public virtual DbSet<tb_comentario_tipo> tb_comentario_tipo { get; set; }
        public virtual DbSet<tb_competencia> tb_competencia { get; set; }
        public virtual DbSet<tb_confirmacao_email> tb_confirmacao_email { get; set; }
        public virtual DbSet<tb_contato_colaborador> tb_contato_colaborador { get; set; }
        public virtual DbSet<tb_contato_emergencia> tb_contato_emergencia { get; set; }
        public virtual DbSet<tb_convite_empresa> tb_convite_empresa { get; set; }
        public virtual DbSet<tb_curriculo_colaborador> tb_curriculo_colaborador { get; set; }
        public virtual DbSet<tb_disponibilidade> tb_disponibilidade { get; set; }
        public virtual DbSet<tb_dominionegocio> tb_dominionegocio { get; set; }
        public virtual DbSet<tb_empresa> tb_empresa { get; set; }
        public virtual DbSet<tb_empresa_usuario> tb_empresa_usuario { get; set; }
        public virtual DbSet<tb_endereco> tb_endereco { get; set; }
        public virtual DbSet<tb_endosso_competencia> tb_endosso_competencia { get; set; }
        public virtual DbSet<tb_endosso_dominionegocio> tb_endosso_dominionegocio { get; set; }
        public virtual DbSet<tb_endosso_formacao> tb_endosso_formacao { get; set; }
        public virtual DbSet<tb_endosso_metodologia> tb_endosso_metodologia { get; set; }
        public virtual DbSet<tb_endosso_modeloreferencia> tb_endosso_modeloreferencia { get; set; }
        public virtual DbSet<tb_escolaridade> tb_escolaridade { get; set; }
        public virtual DbSet<tb_estado_civil> tb_estado_civil { get; set; }
        public virtual DbSet<tb_estagio_processo_seletivo> tb_estagio_processo_seletivo { get; set; }
        public virtual DbSet<tb_etnia> tb_etnia { get; set; }
        public virtual DbSet<tb_experiencia> tb_experiencia { get; set; }
        public virtual DbSet<tb_experiencia_empresa_sugestao> tb_experiencia_empresa_sugestao { get; set; }
        public virtual DbSet<tb_experiencia_projeto> tb_experiencia_projeto { get; set; }
        public virtual DbSet<tb_experiencia_projeto_sugestao> tb_experiencia_projeto_sugestao { get; set; }
        public virtual DbSet<tb_feriado> tb_feriado { get; set; }
        public virtual DbSet<tb_filtro> tb_filtro { get; set; }
        public virtual DbSet<tb_filtro_competencia_nivel> tb_filtro_competencia_nivel { get; set; }
        public virtual DbSet<tb_filtro_dominionegocio_nivel> tb_filtro_dominionegocio_nivel { get; set; }
        public virtual DbSet<tb_filtro_formacao_nivel> tb_filtro_formacao_nivel { get; set; }
        public virtual DbSet<tb_filtro_hobbies> tb_filtro_hobbies { get; set; }
        public virtual DbSet<tb_filtro_interesse> tb_filtro_interesse { get; set; }
        public virtual DbSet<tb_filtro_metodologia_nivel> tb_filtro_metodologia_nivel { get; set; }
        public virtual DbSet<tb_filtro_modeloreferencia_nivel> tb_filtro_modeloreferencia_nivel { get; set; }
        public virtual DbSet<tb_filtro_softskills> tb_filtro_softskills { get; set; }
        public virtual DbSet<tb_fonteorigem_candidato> tb_fonteorigem_candidato { get; set; }
        public virtual DbSet<tb_formacao> tb_formacao { get; set; }
        public virtual DbSet<tb_funcionalidade_rota> tb_funcionalidade_rota { get; set; }
        public virtual DbSet<tb_funcionalidade_sistema> tb_funcionalidade_sistema { get; set; }
        public virtual DbSet<tb_grau_parentesco> tb_grau_parentesco { get; set; }
        public virtual DbSet<tb_graugraduacao> tb_graugraduacao { get; set; }
        public virtual DbSet<tb_grupo_acesso> tb_grupo_acesso { get; set; }
        public virtual DbSet<tb_grupo_acesso_funcionalidade_sistema> tb_grupo_acesso_funcionalidade_sistema { get; set; }
        public virtual DbSet<tb_historico_competencia> tb_historico_competencia { get; set; }
        public virtual DbSet<tb_hobbies> tb_hobbies { get; set; }
        public virtual DbSet<tb_holerite> tb_holerite { get; set; }
        public virtual DbSet<tb_identidade_genero> tb_identidade_genero { get; set; }
        public virtual DbSet<tb_idioma> tb_idioma { get; set; }
        public virtual DbSet<tb_imagem> tb_imagem { get; set; }
        public virtual DbSet<tb_indicacao_premiada_parcial> tb_indicacao_premiada_parcial { get; set; }
        public virtual DbSet<tb_interesse> tb_interesse { get; set; }
        public virtual DbSet<tb_item_perfil> tb_item_perfil { get; set; }
        public virtual DbSet<tb_historico_cv> tb_historico_cv { get; set; }
        public virtual DbSet<tb_like_cargo> tb_like_cargo { get; set; }
        public virtual DbSet<tb_like_competencia> tb_like_competencia { get; set; }
        public virtual DbSet<tb_like_dominionegocio> tb_like_dominionegocio { get; set; }
        public virtual DbSet<tb_like_formacao> tb_like_formacao { get; set; }
        public virtual DbSet<tb_like_hobbies> tb_like_hobbies { get; set; }
        public virtual DbSet<tb_like_interesse> tb_like_interesse { get; set; }
        public virtual DbSet<tb_like_metodologia> tb_like_metodologia { get; set; }
        public virtual DbSet<tb_like_modeloreferencia> tb_like_modeloreferencia { get; set; }
        public virtual DbSet<tb_like_noticia> tb_like_noticia { get; set; }
        public virtual DbSet<tb_log> tb_log { get; set; }
        public virtual DbSet<tb_metodologia> tb_metodologia { get; set; }
        public virtual DbSet<tb_modalidade> tb_modalidade { get; set; }
        public virtual DbSet<tb_modalidade_contratacao> tb_modalidade_contratacao { get; set; }
        public virtual DbSet<tb_modeloreferencia> tb_modeloreferencia { get; set; }
        public virtual DbSet<tb_nacionalidade> tb_nacionalidade { get; set; }
        public virtual DbSet<tb_nivel> tb_nivel { get; set; }
        public virtual DbSet<tb_nivel_escolaridade> tb_nivel_escolaridade { get; set; }
        public virtual DbSet<tb_nivel_referencia_hardskill> tb_nivel_referencia_hardskill { get; set; }
        public virtual DbSet<tb_noticia> tb_noticia { get; set; }
        public virtual DbSet<tb_notificacao> tb_notificacao { get; set; }
        public virtual DbSet<tb_origem_historico_cv> tb_origem_historico_cv { get; set; }
        public virtual DbSet<tb_org> tb_org { get; set; }
        public virtual DbSet<tb_orientacao_sexual> tb_orientacao_sexual { get; set; }
        public virtual DbSet<tb_pais> tb_pais { get; set; }
        public virtual DbSet<tb_parametro> tb_parametro { get; set; }
        public virtual DbSet<tb_parametro_configuracao> tb_parametro_configuracao { get; set; }
        public virtual DbSet<tb_parametro_nivel> tb_parametro_nivel { get; set; }
        public virtual DbSet<tb_periodo_alocacao> tb_periodo_alocacao { get; set; }
        public virtual DbSet<tb_pessoa_juridica> tb_pessoa_juridica { get; set; }
        public virtual DbSet<tb_projeto> tb_projeto { get; set; }
        public virtual DbSet<tb_projeto_cch> tb_projeto_cch { get; set; }
        public virtual DbSet<tb_projeto_gerente> tb_projeto_gerente { get; set; }
        public virtual DbSet<tb_projeto_org> tb_projeto_org { get; set; }
        public virtual DbSet<tb_projeto_org_atividade> tb_projeto_org_atividade { get; set; }
        public virtual DbSet<tb_projetohora_cch> tb_projetohora_cch { get; set; }
        public virtual DbSet<tb_regime_tributario> tb_regime_tributario { get; set; }
        public virtual DbSet<tb_skill_candidato_srs> tb_skill_candidato_srs { get; set; }
        public virtual DbSet<tb_skill_vaga> tb_skill_vaga { get; set; }
        public virtual DbSet<tb_skill_vaga_srs> tb_skill_vaga_srs { get; set; }
        public virtual DbSet<tb_softskill> tb_softskill { get; set; }
        public virtual DbSet<tb_projeto_proposta> tb_projeto_proposta { get; set; }
        public virtual DbSet<tb_status_apontamento> tb_status_apontamento { get; set; }
        public virtual DbSet<tb_status_apontamento_grupo> tb_status_apontamento_grupo { get; set; }
        public virtual DbSet<tb_status_colaborador> tb_status_colaborador { get; set; }
        public virtual DbSet<tb_status_endosso> tb_status_endosso { get; set; }
        public virtual DbSet<tb_status_projeto> tb_status_projeto { get; set; }
        public virtual DbSet<tb_tbd_alocado> tb_tbd_alocado { get; set; }
        public virtual DbSet<tb_template_email> tb_template_email { get; set; }
        public virtual DbSet<tb_template_email_rotina> tb_template_email_rotina { get; set; }
        public virtual DbSet<tb_tipo_carga_horaria> tb_tipo_carga_horaria { get; set; }
        public virtual DbSet<tb_tipo_cargo> tb_tipo_cargo { get; set; }
        public virtual DbSet<tb_tipo_contratacao> tb_tipo_contratacao { get; set; }
        public virtual DbSet<tb_tipo_dependente> tb_tipo_dependente { get; set; }
        public virtual DbSet<tb_tipo_diploma> tb_tipo_diploma { get; set; }
        public virtual DbSet<tb_tipo_endosso> tb_tipo_endosso { get; set; }
        public virtual DbSet<tb_tipo_historico_cv> tb_tipo_historico_cv { get; set; }
        public virtual DbSet<tb_token_acesso> tb_token_acesso { get; set; }
        public virtual DbSet<tb_token_resete_senha> tb_token_resete_senha { get; set; }
        public virtual DbSet<tb_token_sistema> tb_token_sistema { get; set; }
        public virtual DbSet<tb_token_sso> tb_token_sso { get; set; }
        public virtual DbSet<tb_trending> tb_trending { get; set; }
        public virtual DbSet<tb_usuario> tb_usuario { get; set; }
        public virtual DbSet<tb_usuario_grupo_acesso> tb_usuario_grupo_acesso { get; set; }
        public virtual DbSet<tb_usuario_permissao_log> tb_usuario_permissao_log { get; set; }
        public virtual DbSet<tb_usuario_tokenacesso> tb_usuario_tokenacesso { get; set; }
        public virtual DbSet<tb_vaga> tb_vaga { get; set; }
        public virtual DbSet<tb_vaga_competencia> tb_vaga_competencia { get; set; }
        public virtual DbSet<tb_vaga_favorito> tb_vaga_favorito { get; set; }
        public virtual DbSet<tb_vagas_indicadas> tb_vagas_indicadas { get; set; }
        public virtual DbSet<tb_vagas_srs> tb_vagas_srs { get; set; }
        public virtual DbSet<tb_video> tb_video { get; set; }
        public virtual DbSet<tb_vigencia> tb_vigencia { get; set; }
        public virtual DbSet<trendingview> trendingview { get; set; }
        public virtual DbSet<vw_alocacacao_recurso_calculo_mensal> vw_alocacacao_recurso_calculo_mensal { get; set; }
        public virtual DbSet<vw_alocacao_hierarquia_calculo_mensal> vw_alocacao_hierarquia_calculo_mensal { get; set; }
        public virtual DbSet<vw_apontamento_mensal> vw_apontamento_mensal { get; set; }
        public virtual DbSet<vw_apontamento_mensal_visao_gerente> vw_apontamento_mensal_visao_gerente { get; set; }
        public virtual DbSet<vw_colaborador_apontamento> vw_colaborador_apontamento { get; set; }
        public virtual DbSet<vw_colaboradores_gestor> vw_colaboradores_gestor { get; set; }
        public virtual DbSet<vw_competencias_sugeridas> vw_competencias_sugeridas { get; set; }
        public virtual DbSet<vw_filtro_mapaalocacao> vw_filtro_mapaalocacao { get; set; }
        public virtual DbSet<vw_gestores_colaboradores_org> vw_gestores_colaboradores_org { get; set; }
        public virtual DbSet<vw_gestores_estatisticas_org> vw_gestores_estatisticas_org { get; set; }
        public virtual DbSet<vw_gestores_org> vw_gestores_org { get; set; }
        public virtual DbSet<vw_mapa_alocacao_colaborador_tbd> vw_mapa_alocacao_colaborador_tbd { get; set; }
        public virtual DbSet<vw_totalizadores_unidades> vw_totalizadores_unidades { get; set; }
        public virtual DbSet<tb_colaborador_alocado_skill> tb_colaborador_alocado_skill { get; set; }
        public virtual DbSet<tb_perfil> tb_perfil { get; set; }
        public virtual DbSet<tb_perfil_alocacao> tb_perfil_alocacao { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb3_general_ci");

            modelBuilder.Entity<buscacolaborador>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("buscacolaborador");

                entity.Property(e => e.BuscaGR1)
                    .IsRequired()
                    .HasMaxLength(455)
                    .HasDefaultValueSql("''");

                entity.Property(e => e.BuscaGR2)
                    .IsRequired()
                    .HasColumnType("mediumtext");

                entity.Property(e => e.BuscaGR3)
                    .IsRequired()
                    .HasColumnType("mediumtext");

                entity.Property(e => e.BuscaGR4).HasColumnType("mediumtext");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.candidato).HasDefaultValueSql("'0'");

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.documento_colaborador).HasMaxLength(256);
            });

            modelBuilder.Entity<cadastrocandidatosmes>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("cadastrocandidatosmes");
            });

            modelBuilder.Entity<colaboradorpordiretoria>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("colaboradorpordiretoria");

                entity.Property(e => e.descricao)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.diretoria_id)
                    .IsRequired()
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<estatisticas_escolaridade>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("estatisticas_escolaridade");
            });

            modelBuilder.Entity<estatisticas_etnia>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("estatisticas_etnia");
            });

            modelBuilder.Entity<estatisticas_genero>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("estatisticas_genero");
            });

            modelBuilder.Entity<estatisticas_idade>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("estatisticas_idade");
            });

            modelBuilder.Entity<estatisticas_orientacao_sexual>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("estatisticas_orientacao_sexual");
            });

            modelBuilder.Entity<estatisticas_tempo_servico>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("estatisticas_tempo_servico");

                entity.HasComment("View 'gcolb_hml.estatisticas_tempo_servico' references invalid table(s) or column(s) or function(s) or definer/invoker of view lack rights to use them");
            });

            modelBuilder.Entity<graficocompetenciacolab>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("graficocompetenciacolab");

                entity.Property(e => e.descricao)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<tb_projeto_proposta>(entity =>
            {
                entity.HasKey(e => new { e.cod_proposta, e.tb_org_id, e.cod_projeto })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0 });

                entity.HasIndex(e => new { e.cod_projeto, e.tb_org_id }, "cod_projeto");

                entity.Property(e => e.cod_proposta).HasMaxLength(20);
            });

            modelBuilder.Entity<subgraficocolaboradorcompetencia>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("subgraficocolaboradorcompetencia");

                entity.Property(e => e.descricao)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<subgraficocompetenciacandidato>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("subgraficocompetenciacandidato");

                entity.Property(e => e.descricao)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<tb_acesso>(entity =>
            {
                entity.Property(e => e.status)
                    .IsRequired()
                    .HasMaxLength(45);
            });

            modelBuilder.Entity<tb_ad_sso>(entity =>
            {
                entity.HasIndex(e => e.tb_org_id, "fk_tb_ad_sso_tb_org1_idx");

                entity.Property(e => e.base_url)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.client_id)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.client_secret_value)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.code_verifier_plain)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.graph_path)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.redirect_url)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.scope)
                    .HasMaxLength(500);

                entity.Property(e => e.tenant)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.token_path)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.HasOne(d => d.tb_org)
                    .WithMany(p => p.tb_ad_sso)
                    .HasForeignKey(d => d.tb_org_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_ad_sso_tb_org1");
            });

            modelBuilder.Entity<tb_apontamento_periodo_fechado>(entity =>
            {
                entity.HasIndex(e => e.tb_org_id, "tb_org_id");

                entity.Property(e => e.codigo_interno_colaborador_alteracao).HasMaxLength(36);

                entity.Property(e => e.codigo_interno_colaborador_criacao).HasMaxLength(36);

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_fim).HasColumnType("datetime");

                entity.HasOne(d => d.tb_org)
                    .WithMany(p => p.tb_apontamento_periodo_fechado)
                    .HasForeignKey(d => d.tb_org_id)
                    .HasConstraintName("tb_apontamento_periodo_fechado_ibfk_1");
            });

            modelBuilder.Entity<tb_apontamento_periodo_fechado_log>(entity =>
            {
                entity.HasIndex(e => e.tb_apontamento_periodo_fechado_id, "tb_apontamento_periodo_fechado_id");

                entity.HasIndex(e => e.tb_org_id, "tb_org_id");

                entity.Property(e => e.codigo_interno_colaborador_alteracao).HasMaxLength(36);

                entity.Property(e => e.codigo_interno_colaborador_criacao).HasMaxLength(36);

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_fim_anterior).HasColumnType("date");

                entity.Property(e => e.data_fim_nova).HasColumnType("date");

                entity.HasOne(d => d.tb_apontamento_periodo_fechado)
                    .WithMany(p => p.tb_apontamento_periodo_fechado_log)
                    .HasForeignKey(d => d.tb_apontamento_periodo_fechado_id)
                    .HasConstraintName("tb_apontamento_periodo_fechado_log_ibfk_1");

                entity.HasOne(d => d.tb_org)
                    .WithMany(p => p.tb_apontamento_periodo_fechado_log)
                    .HasForeignKey(d => d.tb_org_id)
                    .HasConstraintName("tb_apontamento_periodo_fechado_log_ibfk_2");
            });

            modelBuilder.Entity<tb_atividade>(entity =>
            {
                entity.HasIndex(e => e.tb_org_id, "fk_tb_atividade_tb_org_id");

                entity.Property(e => e.descricao)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.HasOne(d => d.tb_org)
                    .WithMany(p => p.tb_atividade)
                    .HasForeignKey(d => d.tb_org_id)
                    .HasConstraintName("fk_tb_atividade_tb_org_id");
            });

            modelBuilder.Entity<tb_bancos>(entity =>
            {
                entity.HasKey(e => e.codigo)
                    .HasName("PRIMARY");

                entity.Property(e => e.codigo).HasMaxLength(12);

                entity.Property(e => e.nome)
                    .IsRequired()
                    .HasMaxLength(120);
            });

            modelBuilder.Entity<tb_candidato>(entity =>
            {
                entity.HasIndex(e => e.codigo_interno_colaborador, "fk_tb_candidato_tb_colaborador1_idx");

                entity.HasIndex(e => e.estagio_processo_seletivo_id, "fk_tb_candidato_tb_estagio_processo_seletivo1_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.cargo_atual_ultimo).HasMaxLength(50);

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_estagio_processo)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.path_curriculo).HasMaxLength(300);

                entity.Property(e => e.salario_atual_ultimo).HasPrecision(10, 2);

                entity.Property(e => e.tipo_contrato_atual_ultimo).HasMaxLength(20);

                entity.HasOne(d => d.codigo_interno_colaboradorNavigation)
                    .WithMany(p => p.tb_candidato)
                    .HasForeignKey(d => d.codigo_interno_colaborador)
                    .HasConstraintName("fk_tb_candidato_tb_colaborador1");

                entity.HasOne(d => d.estagio_processo_seletivo)
                    .WithMany(p => p.tb_candidato)
                    .HasForeignKey(d => d.estagio_processo_seletivo_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_candidato_tb_estagio_processo_seletivo1");
            });

            modelBuilder.Entity<tb_candidatos_vaga>(entity =>
            {
                entity.Property(e => e.email).HasMaxLength(255);

                entity.Property(e => e.nome).HasMaxLength(255);

                entity.Property(e => e.status).HasMaxLength(50);
            });

            modelBuilder.Entity<tb_cargo>(entity =>
            {
                entity.HasIndex(e => e.descricao, "descricao_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descricao)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<tb_certificado>(entity =>
            {
                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_conclusao).HasColumnType("timestamp");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descricao).HasColumnType("text");

                entity.Property(e => e.instituicao).HasMaxLength(255);

                entity.HasOne(d => d.codigo_interno_colaboradorNavigation)
                    .WithMany(p => p.tb_certificado)
                    .HasForeignKey(d => d.codigo_interno_colaborador)
                    .HasConstraintName("tb_certificado_tb_colaborador_FK");

                entity.Property(e => e.path)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<tb_cliente_org>(entity =>
            {
                entity.HasIndex(e => new { e.nome_cliente, e.tb_org_id }, "UQ_nome_cliente_tb_org_id")
                    .IsUnique();

                entity.HasIndex(e => new { e.codigo_cliente, e.tb_org_id }, "codigo_cliente")
                    .IsUnique();

                entity.HasIndex(e => e.tb_org_id, "tb_org_id");

                entity.Property(e => e.codigo_cliente)
                    .IsRequired()
                    .HasMaxLength(45);

                entity.Property(e => e.tipo_cadastro)
                    .HasMaxLength(50);

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.tb_org)
                    .WithMany(p => p.tb_cliente_org)
                    .HasForeignKey(d => d.tb_org_id)
                    .HasConstraintName("tb_cliente_org_ibfk_1");
            });

            modelBuilder.Entity<tb_colaborador>(entity =>
            {
                entity.HasKey(e => e.codigo_interno_colaborador)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.endereco_id, "fk_colaborador_endereco1_idx");

                entity.HasIndex(e => e.imagem_id, "fk_colaborador_imagem1_idx");

                entity.HasIndex(e => e.colaborador_saude_id, "fk_tb_colaborador_tb_colaborador_saude1_idx");

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.contato_outro).HasMaxLength(200);

                entity.Property(e => e.contato_principal).HasMaxLength(17);

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_nascimento).HasColumnType("date");

                entity.Property(e => e.documento_colaborador).HasMaxLength(256);
                entity.Property(e => e.url_linkedin).HasMaxLength(100);
                entity.Property(e => e.data_sync_linkedin).HasColumnType("timestamp");

                entity.Property(e => e.email_alternativo).HasMaxLength(100);

                entity.Property(e => e.escolaridade).HasMaxLength(100);

                entity.Property(e => e.estado_civil).HasMaxLength(100);

                entity.Property(e => e.etnia).HasMaxLength(100);

                entity.Property(e => e.genero).HasMaxLength(100);

                entity.Property(e => e.matricula).HasMaxLength(45);

                entity.Property(e => e.nacionalidade).HasMaxLength(50);

                entity.Property(e => e.nome_completo)
                    .IsRequired()
                    .HasMaxLength(120);

                entity.Property(e => e.orientacao_sexual).HasMaxLength(100);

                entity.Property(e => e.passaporte).HasMaxLength(100);

                entity.Property(e => e.rg).HasMaxLength(30);

                entity.HasOne(d => d.colaborador_saude)
                    .WithMany(p => p.tb_colaborador)
                    .HasForeignKey(d => d.colaborador_saude_id)
                    .HasConstraintName("fk_tb_colaborador_tb_colaborador_saude1");

                entity.HasOne(d => d.endereco)
                    .WithMany(p => p.tb_colaborador)
                    .HasForeignKey(d => d.endereco_id)
                    .HasConstraintName("fk_colaborador_endereco1");

                entity.HasOne(d => d.imagem)
                    .WithMany(p => p.tb_colaborador)
                    .HasForeignKey(d => d.imagem_id)
                    .HasConstraintName("fk_colaborador_imagem1");
            });

            modelBuilder.Entity<tb_colaborador_alocado>(entity =>
            {
                entity.HasIndex(e => e.codigo_interno_colaborador, "fk_tb_colaborador_alocado_tb_colaborador1_idx");

                entity.HasIndex(e => e.tb_org_id, "fk_tb_colaborador_alocado_tb_org1_idx");

                entity.HasIndex(e => e.cod_tbd_alocado, "fk_tb_tbd_alocado");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.codigo_colaborador).HasMaxLength(255);

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.codigo_projeto)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.email_gestor).HasMaxLength(255);

                entity.HasOne(d => d.codigo_interno_colaboradorNavigation)
                    .WithMany(p => p.tb_colaborador_alocado)
                    .HasForeignKey(d => d.codigo_interno_colaborador)
                    .HasConstraintName("fk_tb_colaborador_alocado_tb_colaborador1");

                entity.HasOne(d => d.tb_org)
                    .WithMany(p => p.tb_colaborador_alocado)
                    .HasForeignKey(d => d.tb_org_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_colaborador_alocado_tb_org1");
            });

            modelBuilder.Entity<tb_colaborador_apontamento>(entity =>
            {
                entity.HasIndex(e => e.tb_atividade_id, "tb_atividade_id");

                entity.HasIndex(e => new { e.tb_org_id, e.codigo_interno_colaborador }, "tb_org_id");

                entity.HasIndex(e => new { e.tb_projeto_org_cod_projeto, e.tb_org_id }, "tb_projeto_org_cod_projeto");

                entity.HasIndex(e => e.tb_status_apontamento_id, "tb_status_apontamento_id");

                entity.HasIndex(e => e.tb_vigencia_id, "tb_vigencia_id");

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.codigo_interno_colaborador_alteracao).HasMaxLength(36);

                entity.Property(e => e.codigo_interno_colaborador_criacao).HasMaxLength(36);

                entity.Property(e => e.codigo_interno_colaborador_justificativa).HasMaxLength(36);

                entity.Property(e => e.data).HasColumnType("date");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_justificativa).HasColumnType("datetime");

                entity.Property(e => e.justificativa).HasMaxLength(500);

                entity.Property(e => e.observacao).HasMaxLength(500);

                entity.Property(e => e.tipo_apontamento_id).HasColumnType("enum('diario','mensal')");

                entity.HasOne(d => d.tb_atividade)
                    .WithMany(p => p.tb_colaborador_apontamento)
                    .HasForeignKey(d => d.tb_atividade_id)
                    .HasConstraintName("tb_colaborador_apontamento_ibfk_5");

                entity.HasOne(d => d.tb_status_apontamento)
                    .WithMany(p => p.tb_colaborador_apontamento)
                    .HasForeignKey(d => d.tb_status_apontamento_id)
                    .HasConstraintName("tb_colaborador_apontamento_ibfk_3");

                entity.HasOne(d => d.tb_vigencia)
                    .WithMany(p => p.tb_colaborador_apontamento)
                    .HasForeignKey(d => d.tb_vigencia_id)
                    .HasConstraintName("tb_colaborador_apontamento_ibfk_4");

                entity.HasOne(d => d.tb_colaborador_org)
                    .WithMany(p => p.tb_colaborador_apontamento)
                    .HasForeignKey(d => new { d.tb_org_id, d.codigo_interno_colaborador })
                    .HasConstraintName("tb_colaborador_apontamento_ibfk_1");

                entity.HasOne(d => d.tb_)
                    .WithMany(p => p.tb_colaborador_apontamento)
                    .HasForeignKey(d => new { d.tb_projeto_org_cod_projeto, d.tb_org_id })
                    .HasConstraintName("tb_colaborador_apontamento_ibfk_2");
            });

            modelBuilder.Entity<tb_colaborador_apontamento_log>(entity =>
            {
                entity.HasIndex(e => e.colaborador_apontamento_id, "colaborador_apontamento_id");

                entity.HasIndex(e => e.tb_atividade_id, "tb_atividade_id");

                entity.HasIndex(e => new { e.tb_org_id, e.codigo_interno_colaborador }, "tb_org_id");

                entity.HasIndex(e => e.tb_status_apontamento_anterior_id, "tb_status_apontamento_anterior_id");

                entity.HasIndex(e => e.tb_status_apontamento_novo_id, "tb_status_apontamento_novo_id");

                entity.HasIndex(e => e.tb_vigencia_id, "tb_vigencia_id");

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.codigo_interno_colaborador_criacao).HasMaxLength(36);

                entity.Property(e => e.data_criacao).HasColumnType("datetime");

                entity.Property(e => e.justificativa).HasMaxLength(255);

                entity.Property(e => e.tb_projeto_org_cod_projeto).HasMaxLength(255);

                entity.HasOne(d => d.tb_status_apontamento_anterior)
                    .WithMany(p => p.tb_colaborador_apontamento_logtb_status_apontamento_anterior)
                    .HasForeignKey(d => d.tb_status_apontamento_anterior_id)
                    .HasConstraintName("tb_colaborador_apontamento_log_ibfk_5");

                entity.HasOne(d => d.tb_status_apontamento_novo)
                    .WithMany(p => p.tb_colaborador_apontamento_logtb_status_apontamento_novo)
                    .HasForeignKey(d => d.tb_status_apontamento_novo_id)
                    .HasConstraintName("tb_colaborador_apontamento_log_ibfk_6");

                entity.HasOne(d => d.tb_vigencia)
                    .WithMany(p => p.tb_colaborador_apontamento_log)
                    .HasForeignKey(d => d.tb_vigencia_id)
                    .HasConstraintName("tb_colaborador_apontamento_log_ibfk_2");

                entity.HasOne(d => d.tb_colaborador_org)
                    .WithMany(p => p.tb_colaborador_apontamento_log)
                    .HasForeignKey(d => new { d.tb_org_id, d.codigo_interno_colaborador })
                    .HasConstraintName("tb_colaborador_apontamento_log_ibfk_4");
            });

            modelBuilder.Entity<tb_colaborador_cargo>(entity =>
            {
                entity.HasIndex(e => e.cargo_id, "fk_colaborador_has_cargo_cargo1_idx");

                entity.HasIndex(e => e.codigo_interno_colaborador, "fk_colaborador_has_cargo_colaborador1_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.cargo)
                    .WithMany(p => p.tb_colaborador_cargo)
                    .HasForeignKey(d => d.cargo_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_colaborador_has_cargo_cargo1");

                entity.HasOne(d => d.codigo_interno_colaboradorNavigation)
                    .WithMany(p => p.tb_colaborador_cargo)
                    .HasForeignKey(d => d.codigo_interno_colaborador)
                    .HasConstraintName("fk_colaborador_has_cargo_colaborador1");
            });

            modelBuilder.Entity<tb_colaborador_cch>(entity =>
            {
                entity.HasKey(e => e.cdProfissional)
                    .HasName("PRIMARY");

                entity.Property(e => e.cdProfissional).ValueGeneratedNever();

                entity.Property(e => e.nmProfissional)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<tb_colaborador_comentario>(entity =>
            {
                entity.HasIndex(e => e.codigo_interno_colaborador, "fk_colaborador_comentario_colaborador_idx");

                entity.HasIndex(e => e.comentario_tipo_id, "fk_colaborador_comentario_comentario_tipo_idx");

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.texto)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.codigo_interno_colaboradorNavigation)
                    .WithMany(p => p.tb_colaborador_comentario)
                    .HasForeignKey(d => d.codigo_interno_colaborador)
                    .HasConstraintName("fk_colaborador_comentario_colaborador");

                entity.HasOne(d => d.comentario_tipo)
                    .WithMany(p => p.tb_colaborador_comentario)
                    .HasForeignKey(d => d.comentario_tipo_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_colaborador_comentario_comentario_tipo");
            });

            modelBuilder.Entity<tb_colaborador_competencia>(entity =>
            {
                entity.HasIndex(e => e.tb_nivel_id, "fk_tb_colaborador_competencia_tb_nivel1_idx");

                entity.HasIndex(e => e.codigo_interno_colaborador, "fk_tb_colaborador_has_tb_competencia_tb_colaborador1_idx");

                entity.HasIndex(e => e.competencia_id, "fk_tb_colaborador_has_tb_competencia_tb_competencia1_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.codigo_interno_colaboradorNavigation)
                    .WithMany(p => p.tb_colaborador_competencia)
                    .HasForeignKey(d => d.codigo_interno_colaborador)
                    .HasConstraintName("fk_tb_colaborador_has_tb_competencia_tb_colaborador1");

                entity.HasOne(d => d.competencia)
                    .WithMany(p => p.tb_colaborador_competencia)
                    .HasForeignKey(d => d.competencia_id)
                    .HasConstraintName("fk_tb_colaborador_has_tb_competencia_tb_competencia1");

                entity.HasOne(d => d.tb_nivel)
                    .WithMany(p => p.tb_colaborador_competencia)
                    .HasForeignKey(d => d.tb_nivel_id)
                    .HasConstraintName("fk_tb_colaborador_competencia_tb_nivel1");
            });

            modelBuilder.Entity<tb_colaborador_competencia_certificado>(entity =>
            {
                entity.HasIndex(e => e.tb_certificado_id, "fk_tb_colaborador_competencia_has_tb_certificado_tb_certifi_idx");

                entity.HasIndex(e => e.tb_colaborador_competencia_id, "fk_tb_colaborador_competencia_has_tb_certificado_tb_colabor_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.tb_certificado)
                    .WithMany(p => p.tb_colaborador_competencia_certificado)
                    .HasForeignKey(d => d.tb_certificado_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_colaborador_competencia_has_tb_certificado_tb_certifica1");

                entity.HasOne(d => d.tb_colaborador_competencia)
                    .WithMany(p => p.tb_colaborador_competencia_certificado)
                    .HasForeignKey(d => d.tb_colaborador_competencia_id)
                    .HasConstraintName("fk_tb_colaborador_competencia_has_tb_certificado_tb_colaborad1");
            });

            modelBuilder.Entity<tb_colaborador_dependente>(entity =>
            {
                entity.HasIndex(e => e.codigo_interno_colaborador, "fk_tb_colaborador_dependente_tb_colaborador1_idx");

                entity.HasIndex(e => e.tipo_dependente_id, "fk_tb_colaborador_dependente_tb_tipo_dependente1_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.cpf).HasMaxLength(11);

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_nascimento).HasColumnType("datetime");

                entity.Property(e => e.nome_completo)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.requer_ajuda_qual).HasColumnType("text");

                entity.Property(e => e.rg).HasMaxLength(45);

                entity.HasOne(d => d.codigo_interno_colaboradorNavigation)
                    .WithMany(p => p.tb_colaborador_dependente)
                    .HasForeignKey(d => d.codigo_interno_colaborador)
                    .HasConstraintName("fk_tb_colaborador_dependente_tb_colaborador1");

                entity.HasOne(d => d.tipo_dependente)
                    .WithMany(p => p.tb_colaborador_dependente)
                    .HasForeignKey(d => d.tipo_dependente_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_colaborador_dependente_tb_tipo_dependente1");
            });

            modelBuilder.Entity<tb_colaborador_dominionegocio>(entity =>
            {
                entity.HasIndex(e => e.tb_nivel_id, "fk_tb_colaborador_dominionegocio_tb_nivel1_idx");

                entity.HasIndex(e => e.codigo_interno_colaborador, "fk_tb_colaborador_has_tb_dominionegocio_tb_colaborador1_idx");

                entity.HasIndex(e => e.dominionegocio_id, "fk_tb_colaborador_has_tb_dominionegocio_tb_dominionegocio1_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.codigo_interno_colaboradorNavigation)
                    .WithMany(p => p.tb_colaborador_dominionegocio)
                    .HasForeignKey(d => d.codigo_interno_colaborador)
                    .HasConstraintName("fk_tb_colaborador_has_tb_dominionegocio_tb_colaborador1");

                entity.HasOne(d => d.dominionegocio)
                    .WithMany(p => p.tb_colaborador_dominionegocio)
                    .HasForeignKey(d => d.dominionegocio_id)
                    .HasConstraintName("fk_tb_colaborador_has_tb_dominionegocio_tb_dominionegocio1");

                entity.HasOne(d => d.tb_nivel)
                    .WithMany(p => p.tb_colaborador_dominionegocio)
                    .HasForeignKey(d => d.tb_nivel_id)
                    .HasConstraintName("fk_tb_colaborador_dominionegocio_tb_nivel1");
            });

            modelBuilder.Entity<tb_colaborador_formacao>(entity =>
            {
                entity.HasIndex(e => e.tb_certificado_id, "fk_tb_colaborador_formacao_tb_certificado1_idx");

                entity.HasIndex(e => e.tb_nivel_id, "fk_tb_colaborador_formacao_tb_nivel1_idx");

                entity.HasIndex(e => e.codigo_interno_colaborador, "fk_tb_colaborador_has_tb_formacao_tb_colaborador1_idx");

                entity.HasIndex(e => e.formacao_id, "fk_tb_colaborador_has_tb_formacao_tb_formacao1_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_conclusao).HasColumnType("timestamp");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.emissor).HasMaxLength(150);

                entity.HasOne(d => d.codigo_interno_colaboradorNavigation)
                    .WithMany(p => p.tb_colaborador_formacao)
                    .HasForeignKey(d => d.codigo_interno_colaborador)
                    .HasConstraintName("fk_tb_colaborador_has_tb_formacao_tb_colaborador1");

                entity.HasOne(d => d.formacao)
                    .WithMany(p => p.tb_colaborador_formacao)
                    .HasForeignKey(d => d.formacao_id)
                    .HasConstraintName("fk_tb_colaborador_has_tb_formacao_tb_formacao1");

                entity.HasOne(d => d.tb_certificado)
                    .WithMany(p => p.tb_colaborador_formacao)
                    .HasForeignKey(d => d.tb_certificado_id)
                    .HasConstraintName("fk_tb_colaborador_formacao_tb_certificado1");

                entity.HasOne(d => d.tb_nivel)
                    .WithMany(p => p.tb_colaborador_formacao)
                    .HasForeignKey(d => d.tb_nivel_id)
                    .HasConstraintName("fk_tb_colaborador_formacao_tb_nivel1");
            });

            modelBuilder.Entity<tb_colaborador_graugraduacao>(entity =>
            {
                entity.HasIndex(e => e.codigo_interno_colaborador, "fk_tb_colaborador_has_tb_graugraduacao_tb_colaborador1_idx");

                entity.HasIndex(e => e.graugraduacao_id, "fk_tb_colaborador_has_tb_graugraduacao_tb_graugraduacao1");

                entity.Property(e => e.id).ValueGeneratedNever();

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.codigo_interno_colaboradorNavigation)
                    .WithMany(p => p.tb_colaborador_graugraduacao)
                    .HasForeignKey(d => d.codigo_interno_colaborador)
                    .HasConstraintName("fk_tb_colaborador_has_tb_graugraduacao_tb_colaborador1");

                entity.HasOne(d => d.graugraduacao)
                    .WithMany(p => p.tb_colaborador_graugraduacao)
                    .HasForeignKey(d => d.graugraduacao_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_colaborador_has_tb_graugraduacao_tb_graugraduacao1");
            });

            modelBuilder.Entity<tb_colaborador_hierarquia>(entity =>
            {
                entity.HasKey(e => new { e.cod_colaborador_externo, e.cod_colaborador_superior, e.tb_org_id })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0 });

                entity.HasIndex(e => e.tb_org_id, "fk_tb_colaborador_hierarquia_tb_org1_idx");

                entity.Property(e => e.cod_colaborador_superior).HasMaxLength(45);

                entity.HasOne(d => d.tb_org)
                    .WithMany(p => p.tb_colaborador_hierarquia)
                    .HasForeignKey(d => d.tb_org_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_colaborador_hierarquia_tb_org1");
            });

            modelBuilder.Entity<tb_colaborador_hobbies>(entity =>
            {
                entity.HasIndex(e => e.codigo_interno_colaborador, "fk_tb_colaborador_has_tb_hobbies_tb_colaborador1_idx");

                entity.HasIndex(e => e.hobbies_id, "fk_tb_colaborador_has_tb_hobbies_tb_hobbies1_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.codigo_interno_colaboradorNavigation)
                    .WithMany(p => p.tb_colaborador_hobbies)
                    .HasForeignKey(d => d.codigo_interno_colaborador)
                    .HasConstraintName("fk_tb_colaborador_has_tb_hobbies_tb_colaborador1");

                entity.HasOne(d => d.hobbies)
                    .WithMany(p => p.tb_colaborador_hobbies)
                    .HasForeignKey(d => d.hobbies_id)
                    .HasConstraintName("fk_tb_colaborador_has_tb_hobbies_tb_hobbies1");
            });

            modelBuilder.Entity<tb_colaborador_holerite>(entity =>
            {

                entity.Property(e => e.cpf)
                    .IsRequired()
                    .HasMaxLength(11);

                entity.Property(e => e.data_criacao)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.emissao).HasColumnType("datetime");

                entity.Property(e => e.path)
                    .IsRequired()
                    .HasMaxLength(350);
            });

            modelBuilder.Entity<tb_colaborador_idioma>(entity =>
            {
                entity.HasIndex(e => e.codigo_interno_colaborador, "fk_tb_colaborador_idioma_tb_colaborador1_idx");

                entity.HasIndex(e => e.idioma_id, "fk_tb_colaborador_idioma_tb_idioma1_idx");

                entity.HasIndex(e => e.tb_nivel_id, "fk_tb_colaborador_idioma_tb_nivel1_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.codigo_interno_colaboradorNavigation)
                    .WithMany(p => p.tb_colaborador_idioma)
                    .HasForeignKey(d => d.codigo_interno_colaborador)
                    .HasConstraintName("fk_tb_colaborador_idioma_tb_colaborador1");

                entity.HasOne(d => d.idioma)
                    .WithMany(p => p.tb_colaborador_idioma)
                    .HasForeignKey(d => d.idioma_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_colaborador_idioma_tb_idioma1");

                entity.HasOne(d => d.tb_nivel)
                    .WithMany(p => p.tb_colaborador_idioma)
                    .HasForeignKey(d => d.tb_nivel_id)
                    .HasConstraintName("fk_tb_colaborador_idioma_tb_nivel1");
            });

            modelBuilder.Entity<tb_colaborador_interesse>(entity =>
            {
                entity.HasIndex(e => e.codigo_interno_colaborador, "fk_tb_colaborador_has_tb_interesse_tb_colaborador1_idx");

                //entity.HasIndex(e => e.interesse_id, "fk_tb_colaborador_has_tb_interesse_tb_interesse1_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                //entity.HasOne(d => d.codigo_interno_colaboradorNavigation)
                //    .WithMany(p => p.tb_colaborador_interesse)
                //    .HasForeignKey(d => d.codigo_interno_colaborador)
                //    .HasConstraintName("fk_tb_colaborador_has_tb_interesse_tb_colaborador1");

                //entity.HasOne(d => d.interesse)
                //    .WithMany(p => p.tb_colaborador_interesse)
                //    .HasForeignKey(d => d.interesse_id)
                //    .HasConstraintName("fk_tb_colaborador_has_tb_interesse_tb_interesse1");
            });

            modelBuilder.Entity<tb_colaborador_lg>(entity =>
            {
                entity.Property(e => e.cpf)
                    .IsRequired()
                    .HasMaxLength(11);

                entity.Property(e => e.lg_matricula)
                    .IsRequired()
                    .HasMaxLength(75);
            });

            modelBuilder.Entity<tb_colaborador_metodologia>(entity =>
            {
                entity.HasIndex(e => e.codigo_interno_colaborador, "fk_tb_colaborador_has_tb_metodologia_tb_colaborador1_idx");

                entity.HasIndex(e => e.metodologia_id, "fk_tb_colaborador_has_tb_metodologia_tb_metodologia1_idx");

                entity.HasIndex(e => e.tb_nivel_id, "fk_tb_colaborador_metodologia_tb_nivel1_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.codigo_interno_colaboradorNavigation)
                    .WithMany(p => p.tb_colaborador_metodologia)
                    .HasForeignKey(d => d.codigo_interno_colaborador)
                    .HasConstraintName("fk_tb_colaborador_has_tb_metodologia_tb_colaborador1");

                entity.HasOne(d => d.metodologia)
                    .WithMany(p => p.tb_colaborador_metodologia)
                    .HasForeignKey(d => d.metodologia_id)
                    .HasConstraintName("fk_tb_colaborador_has_tb_metodologia_tb_metodologia1");

                entity.HasOne(d => d.tb_nivel)
                    .WithMany(p => p.tb_colaborador_metodologia)
                    .HasForeignKey(d => d.tb_nivel_id)
                    .HasConstraintName("fk_tb_colaborador_metodologia_tb_nivel1");
            });

            modelBuilder.Entity<tb_colaborador_modeloreferencia>(entity =>
            {
                entity.HasIndex(e => e.codigo_interno_colaborador, "fk_tb_colaborador_has_tb_modeloreferencia_tb_colaborador1_idx");

                entity.HasIndex(e => e.modeloreferencia_id, "fk_tb_colaborador_has_tb_modeloreferencia_tb_modeloreferenc_idx");

                entity.HasIndex(e => e.tb_nivel_id, "fk_tb_colaborador_modeloreferencia_tb_nivel1_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.codigo_interno_colaboradorNavigation)
                    .WithMany(p => p.tb_colaborador_modeloreferencia)
                    .HasForeignKey(d => d.codigo_interno_colaborador)
                    .HasConstraintName("fk_tb_colaborador_has_tb_modeloreferencia_tb_colaborador1");

                entity.HasOne(d => d.modeloreferencia)
                    .WithMany(p => p.tb_colaborador_modeloreferencia)
                    .HasForeignKey(d => d.modeloreferencia_id)
                    .HasConstraintName("fk_tb_colaborador_has_tb_modeloreferencia_tb_modeloreferencia1");

                entity.HasOne(d => d.tb_nivel)
                    .WithMany(p => p.tb_colaborador_modeloreferencia)
                    .HasForeignKey(d => d.tb_nivel_id)
                    .HasConstraintName("fk_tb_colaborador_modeloreferencia_tb_nivel1");
            });

            modelBuilder.Entity<tb_colaborador_nacionalidade>(entity =>
            {

                entity.Property(e => e.CpfColaborador)
                    .IsRequired()
                    .HasMaxLength(11);
            });

            modelBuilder.Entity<tb_colaborador_org>(entity =>
            {
                entity.HasKey(e => new { e.tb_org_id, e.codigo_interno_colaborador })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.HasIndex(e => e.codigo_interno_colaborador, "fk_tb_colaborador_org_tb_colaborador1_idx");

                entity.HasIndex(e => e.tb_org_id, "fk_tb_colaborador_org_tb_org1_idx");

                entity.HasIndex(e => new { e.cod_colaborador_externo, e.tb_org_id }, "idx_tb_colab_org_cod_org");

                entity.HasIndex(e => new { e.tb_org_id, e.cod_colaborador_externo, e.ativo }, "uc_tb_org_id_cod_colaborador_externo_ativo")
                    .IsUnique();

                entity.HasIndex(e => new { e.tb_org_id, e.codigo_interno_colaborador, e.ativo }, "uc_tb_org_id_tb_colaborador_cpf_ativo")
                    .IsUnique();

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.cargo)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.cod_colaborador_externo).IsRequired();

                entity.Property(e => e.cod_departamento)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.cod_diretoria)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.codigo_cargo)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.data_admissao).HasColumnType("timestamp");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_inativacao).HasColumnType("timestamp");

                entity.Property(e => e.departamento)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.diretoria)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.HasOne(d => d.codigo_interno_colaboradorNavigation)
                    .WithMany(p => p.tb_colaborador_org)
                    .HasForeignKey(d => d.codigo_interno_colaborador)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_colaborador_org_tb_colaborador1");

                entity.HasOne(d => d.tb_org)
                    .WithMany(p => p.tb_colaborador_org)
                    .HasForeignKey(d => d.tb_org_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_colaborador_org_tb_org1");
            });

            modelBuilder.Entity<tb_colaborador_pais>(entity =>
            {

                entity.Property(e => e.CpfColaborador)
                    .IsRequired()
                    .HasMaxLength(11);
            });

            modelBuilder.Entity<tb_colaborador_passaporte>(entity =>
            {
                entity.HasIndex(e => e.codigo_interno_colaborador, "fk_tb_colaborador_has_tb_colaborador_passaporte1_idx");

                entity.HasIndex(e => e.tb_nacionalidade_id, "fk_tb_nacionalidade_has_tb_colaborador_passaporte1_idx");

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.validade).HasColumnType("date");

                entity.HasOne(d => d.codigo_interno_colaboradorNavigation)
                    .WithMany(p => p.tb_colaborador_passaporte)
                    .HasForeignKey(d => d.codigo_interno_colaborador)
                    .HasConstraintName("fk_tb_colaborador_has_tb_colaborador_passaporte1");

                entity.HasOne(d => d.tb_nacionalidade)
                    .WithMany(p => p.tb_colaborador_passaporte)
                    .HasForeignKey(d => d.tb_nacionalidade_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_nacionalidade_has_tb_colaborador_passaporte1");
            });

            modelBuilder.Entity<tb_colaborador_periodo_alocacao>(entity =>
            {
                entity.HasIndex(e => e.tb_atividade_id, "fk_tb_colaborador_periodo_alocacao_tb_atividade_idx");

                entity.HasIndex(e => e.codigo_interno_colaborador, "fk_tb_colaborador_periodo_alocacao_tb_colaborador1_idx");

                entity.HasIndex(e => e.tb_org_id, "fk_tb_colaborador_periodo_alocacao_tb_org1_idx");

                entity.HasIndex(e => e.cod_tbd_alocado, "fk_tb_colaborador_periodo_alocacao_tb_tbd_alocado_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.codigo_colaborador).HasMaxLength(255);

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.codigo_projeto)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_fim).HasColumnType("timestamp");

                entity.Property(e => e.data_inicio).HasColumnType("timestamp");

                entity.Property(e => e.observacao).HasColumnType("text");

                entity.Property(e => e.oportunidade).HasColumnType("text");

                entity.Property(e => e.prioritario).HasDefaultValueSql("'0'");

                entity.HasOne(d => d.codigo_interno_colaboradorNavigation)
                    .WithMany(p => p.tb_colaborador_periodo_alocacao)
                    .HasForeignKey(d => d.codigo_interno_colaborador)
                    .HasConstraintName("fk_tb_colaborador_periodo_alocacao_tb_colaborador1");

                entity.HasOne(d => d.tb_atividade)
                    .WithMany(p => p.tb_colaborador_periodo_alocacao)
                    .HasForeignKey(d => d.tb_atividade_id)
                    .HasConstraintName("fk_tb_colaborador_periodo_alocacao_tb_atividade");

                entity.HasOne(d => d.tb_org)
                    .WithMany(p => p.tb_colaborador_periodo_alocacao)
                    .HasForeignKey(d => d.tb_org_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_colaborador_periodo_alocacao_tb_org1");

                entity.HasOne(d => d.cod_tbd_alocadoNavigation)
                    .WithMany(p => p.tb_colaborador_periodo_alocacao)
                    .HasForeignKey(d => d.cod_tbd_alocado)
                    .HasConstraintName("fk_tb_colaborador_periodo_alocacao_tb_tbd_alocado");
            });

            modelBuilder.Entity<tb_colaborador_periodo_alocacao_calculo_mensal>(entity =>
            {
                entity.HasIndex(e => e.cod_tbd_alocado, "cod_tbd_alocado");

                entity.HasIndex(e => e.codigo_colaborador, "codigo_colaborador");

                entity.HasIndex(e => new { e.mes, e.ano }, "idx_mes_ano");

                entity.HasIndex(e => e.status_colaborador_periodo_alocacao, "idx_status_colaborador_periodo_alocacao");

                entity.HasIndex(e => e.tb_org_id, "tb_org_id");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.horas).HasPrecision(10, 2);

                entity.Property(e => e.status_colaborador_periodo_alocacao).IsRequired();

                entity.HasOne(d => d.tb_org)
                    .WithMany(p => p.tb_colaborador_periodo_alocacao_calculo_mensal)
                    .HasForeignKey(d => d.tb_org_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tb_colaborador_periodo_alocacao_calculo_mensal_ibfk_3");
            });

            modelBuilder.Entity<tb_colaborador_projeto>(entity =>
            {
                entity.HasIndex(e => e.codigo_interno_colaborador, "fk_tb_colaborador_projeto_tb_colaborador1_idx");

                entity.HasIndex(e => e.modalidade_id, "fk_tb_colaborador_projeto_tb_modalidade1_idx");

                entity.HasIndex(e => e.projeto_id, "fk_tb_colaborador_projeto_tb_projeto1_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_final).HasColumnType("timestamp");

                entity.Property(e => e.data_inicio).HasColumnType("timestamp");

                entity.Property(e => e.valor_definido).HasPrecision(10);

                entity.HasOne(d => d.codigo_interno_colaboradorNavigation)
                    .WithMany(p => p.tb_colaborador_projeto)
                    .HasForeignKey(d => d.codigo_interno_colaborador)
                    .HasConstraintName("fk_tb_colaborador_projeto_tb_colaborador1");

                entity.HasOne(d => d.modalidade)
                    .WithMany(p => p.tb_colaborador_projeto)
                    .HasForeignKey(d => d.modalidade_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_colaborador_projeto_tb_modalidade1");

                entity.HasOne(d => d.projeto)
                    .WithMany(p => p.tb_colaborador_projeto)
                    .HasForeignKey(d => d.projeto_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_colaborador_projeto_tb_projeto1");
            });

            modelBuilder.Entity<tb_colaborador_projeto_org>(entity =>
            {
                entity.HasKey(e => new { e.cod_colaborador, e.cod_projeto, e.tb_org_id })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0 });

                entity.HasIndex(e => e.tb_org_id, "fk_tb_colaborador_projeto_org_tb_org1_idx");

                entity.Property(e => e.nome_projeto)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.HasOne(d => d.tb_org)
                    .WithMany(p => p.tb_colaborador_projeto_org)
                    .HasForeignKey(d => d.tb_org_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_colaborador_projeto_org_tb_org1");
            });

            modelBuilder.Entity<tb_colaborador_referencia_hardskill>(entity =>
            {
                entity.HasKey(e => new { e.tb_competencia_id, e.codigo_interno_colaborador, e.tb_nivel_referencia_hardskill_id })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0 });

                entity.HasIndex(e => e.codigo_interno_colaborador, "fk_tb_colaborador_keeper_tb_colaborador1_idx");

                entity.HasIndex(e => e.tb_competencia_id, "fk_tb_colaborador_keeper_tb_competencia1_idx");

                entity.HasIndex(e => e.tb_nivel_referencia_hardskill_id, "fk_tb_colaborador_referencia_hardskill_tb_nivel_referencia__idx");

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.codigo_interno_colaboradorNavigation)
                    .WithMany(p => p.tb_colaborador_referencia_hardskill)
                    .HasForeignKey(d => d.codigo_interno_colaborador)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_colaborador_keeper_tb_colaborador1");

                entity.HasOne(d => d.tb_competencia)
                    .WithMany(p => p.tb_colaborador_referencia_hardskill)
                    .HasForeignKey(d => d.tb_competencia_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_colaborador_keeper_tb_competencia1");

                entity.HasOne(d => d.tb_nivel_referencia_hardskill)
                    .WithMany(p => p.tb_colaborador_referencia_hardskill)
                    .HasForeignKey(d => d.tb_nivel_referencia_hardskill_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_colaborador_referencia_hardskill_tb_nivel_referencia_ha1");
            });

            modelBuilder.Entity<tb_colaborador_saude>(entity =>
            {
                entity.Property(e => e.condicao_saude_relevante).HasColumnType("text");

                entity.Property(e => e.pcd).HasMaxLength(120);
            });

            modelBuilder.Entity<tb_colaborador_sobre>(entity =>
            {
                entity.HasIndex(e => e.codigo_interno_colaborador, "fk_colaborador_cpf1_idx");

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descricao).HasColumnType("text");

                entity.HasOne(d => d.codigo_interno_colaboradorNavigation)
                    .WithMany(p => p.tb_colaborador_sobre)
                    .HasForeignKey(d => d.codigo_interno_colaborador)
                    .HasConstraintName("fk_colaborador_cpf1");
            });

            modelBuilder.Entity<tb_colaborador_softskill>(entity =>
            {
                entity.HasIndex(e => e.codigo_interno_colaborador, "fk_tb_colaborador_has_tb_softskill_tb_colaborador1_idx");

                entity.HasIndex(e => e.softskill_id, "fk_tb_colaborador_has_tb_softskill_tb_softskill1_idx");

                entity.HasIndex(e => e.tb_nivel_id, "fk_tb_colaborador_softskill_tb_nivel1_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.codigo_interno_colaboradorNavigation)
                    .WithMany(p => p.tb_colaborador_softskill)
                    .HasForeignKey(d => d.codigo_interno_colaborador)
                    .HasConstraintName("fk_tb_colaborador_has_tb_softskill_tb_colaborador1");

                entity.HasOne(d => d.softskill)
                    .WithMany(p => p.tb_colaborador_softskill)
                    .HasForeignKey(d => d.softskill_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_colaborador_has_tb_softskill_tb_softskill1");

                entity.HasOne(d => d.tb_nivel)
                    .WithMany(p => p.tb_colaborador_softskill)
                    .HasForeignKey(d => d.tb_nivel_id)
                    .HasConstraintName("fk_tb_colaborador_softskill_tb_nivel1");
            });

            modelBuilder.Entity<tb_colaborador_status>(entity =>
            {
                entity.HasIndex(e => e.codigo_interno_colaborador, "fk_tb_status_colaborador_has_tb_colaborador_tb_colaborador1_idx");

                entity.HasIndex(e => e.status_colaborador_id, "fk_tb_status_colaborador_has_tb_colaborador_tb_status_colab_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.codigo_interno_colaboradorNavigation)
                    .WithMany(p => p.tb_colaborador_status)
                    .HasForeignKey(d => d.codigo_interno_colaborador)
                    .HasConstraintName("fk_tb_status_colaborador_has_tb_colaborador_tb_colaborador1");

                entity.HasOne(d => d.status_colaborador)
                    .WithMany(p => p.tb_colaborador_status)
                    .HasForeignKey(d => d.status_colaborador_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_status_colaborador_has_tb_colaborador_tb_status_colabor1");
            });

            modelBuilder.Entity<tb_colaborador_visto>(entity =>
            {
                entity.HasIndex(e => e.codigo_interno_colaborador, "fk_tb_colaborador_has_tb_colaborador_visto1_idx");

                entity.HasIndex(e => e.tb_pais_id, "fk_tb_pais_has_tb_colaborador_visto1_idx");

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.validade).HasColumnType("date");

                entity.HasOne(d => d.codigo_interno_colaboradorNavigation)
                    .WithMany(p => p.tb_colaborador_visto)
                    .HasForeignKey(d => d.codigo_interno_colaborador)
                    .HasConstraintName("fk_tb_colaborador_has_tb_colaborador_visto1");

                entity.HasOne(d => d.tb_pais)
                    .WithMany(p => p.tb_colaborador_visto)
                    .HasForeignKey(d => d.tb_pais_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_pais_has_tb_colaborador_visto1");
            });

            modelBuilder.Entity<tb_comentario_tipo>(entity =>
            {
                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descricao)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<tb_competencia>(entity =>
            {
                entity.HasIndex(e => e.descricao, "descricao_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.usuario_criacao_id, "fk_tb_competencia_tb_usuario1_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descricao)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.usuario_criacao)
                    .WithMany(p => p.tb_competencia)
                    .HasForeignKey(d => d.usuario_criacao_id)
                    .HasConstraintName("fk_tb_competencia_tb_usuario1");
            });

            modelBuilder.Entity<tb_confirmacao_email>(entity =>
            {
                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(15);

                entity.Property(e => e.email).HasMaxLength(255);

                entity.Property(e => e.hora_expiracao_codigo).HasColumnType("datetime");

                entity.Property(e => e.nome_colaborador).HasMaxLength(255);
            });

            modelBuilder.Entity<tb_contato_colaborador>(entity =>
            {
                entity.HasIndex(e => e.seguidor_codigo_interno_colaborador, "fk_tb_contato_colaborador_tb_colaborador1_idx");

                entity.HasIndex(e => e.seguindo_codigo_interno_colaborador, "fk_tb_contato_colaborador_tb_colaborador2_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.seguidor_codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.seguindo_codigo_interno_colaborador).HasMaxLength(36);

                entity.HasOne(d => d.seguidor_codigo_interno_colaboradorNavigation)
                    .WithMany(p => p.tb_contato_colaboradorseguidor_codigo_interno_colaboradorNavigation)
                    .HasForeignKey(d => d.seguidor_codigo_interno_colaborador)
                    .HasConstraintName("fk_tb_contato_colaborador_tb_colaborador1");

                entity.HasOne(d => d.seguindo_codigo_interno_colaboradorNavigation)
                    .WithMany(p => p.tb_contato_colaboradorseguindo_codigo_interno_colaboradorNavigation)
                    .HasForeignKey(d => d.seguindo_codigo_interno_colaborador)
                    .HasConstraintName("fk_tb_contato_colaborador_tb_colaborador2");
            });

            modelBuilder.Entity<tb_contato_emergencia>(entity =>
            {
                entity.HasIndex(e => e.id, "tb_contato_emergencia_id_IDX");

                entity.HasIndex(e => e.codigo_interno_colaborador, "tb_contato_emergencia_tb_colaborador_cpf_IDX");

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.grau_parentesco)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.nome)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.telefone)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.codigo_interno_colaboradorNavigation)
                    .WithMany(p => p.tb_contato_emergencia)
                    .HasForeignKey(d => d.codigo_interno_colaborador)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("tb_contato_emergencia_tb_colaborador_FK");
            });

            modelBuilder.Entity<tb_convite_empresa>(entity =>
            {
                entity.HasIndex(e => e.tb_empresa_cnpj, "fk_tb_convite_empresa_tb_empresa1_idx");

                entity.Property(e => e.cpf)
                    .IsRequired()
                    .HasMaxLength(11);

                entity.Property(e => e.email)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.nome_completo)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.tb_empresa_cnpj)
                    .IsRequired()
                    .HasMaxLength(14);

                entity.Property(e => e.token)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.validade).HasColumnType("timestamp");

                entity.HasOne(d => d.tb_empresa_cnpjNavigation)
                    .WithMany(p => p.tb_convite_empresa)
                    .HasForeignKey(d => d.tb_empresa_cnpj)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_convite_empresa_tb_empresa1");
            });

            modelBuilder.Entity<tb_curriculo_colaborador>(entity =>
            {
                entity.HasIndex(e => e.codigo_interno_colaborador, "fk_tb_curriculo_colaborador_tb_colaborador1_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.path_curriculo)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.HasOne(d => d.codigo_interno_colaboradorNavigation)
                    .WithMany(p => p.tb_curriculo_colaborador)
                    .HasForeignKey(d => d.codigo_interno_colaborador)
                    .HasConstraintName("fk_tb_curriculo_colaborador_tb_colaborador1");
            });

            modelBuilder.Entity<tb_disponibilidade>(entity =>
            {
                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descricao)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<tb_dominionegocio>(entity =>
            {
                entity.HasIndex(e => e.descricao, "descricao_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.usuario_criacao_id, "fk_tb_dominionegocio_tb_usuario1_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descricao)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.usuario_criacao)
                    .WithMany(p => p.tb_dominionegocio)
                    .HasForeignKey(d => d.usuario_criacao_id)
                    .HasConstraintName("fk_tb_dominionegocio_tb_usuario1");
            });

            modelBuilder.Entity<tb_empresa>(entity =>
            {
                entity.HasKey(e => e.cnpj)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.tb_org_id, "fk_tb_empresa_tb_org1_idx");

                entity.Property(e => e.cnpj).HasMaxLength(14);

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descricao).HasMaxLength(800);

                entity.Property(e => e.linkedin).HasMaxLength(240);

                entity.Property(e => e.nome_fantasia)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.razao_social)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.site).HasMaxLength(240);

                entity.Property(e => e.tb_org_id).HasDefaultValueSql("'3'");

                entity.HasOne(d => d.tb_org)
                    .WithMany(p => p.tb_empresa)
                    .HasForeignKey(d => d.tb_org_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_empresa_tb_org1");
            });

            modelBuilder.Entity<tb_empresa_usuario>(entity =>
            {
                entity.HasKey(e => new { e.tb_empresa_cnpj, e.tb_usuario_id })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.HasIndex(e => e.tb_empresa_cnpj, "fk_tb_empresa_usuario_tb_empresa1_idx");

                entity.HasIndex(e => e.tb_usuario_id, "fk_tb_empresa_usuario_tb_usuario1_idx");

                entity.Property(e => e.tb_empresa_cnpj).HasMaxLength(14);

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_aceite).HasColumnType("timestamp");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_convite)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.pendente).HasDefaultValueSql("'1'");

                entity.HasOne(d => d.tb_empresa_cnpjNavigation)
                    .WithMany(p => p.tb_empresa_usuario)
                    .HasForeignKey(d => d.tb_empresa_cnpj)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_empresa_usuario_tb_empresa1");

                entity.HasOne(d => d.tb_usuario)
                    .WithMany(p => p.tb_empresa_usuario)
                    .HasForeignKey(d => d.tb_usuario_id)
                    .HasConstraintName("fk_tb_empresa_usuario_tb_usuario1");
            });

            modelBuilder.Entity<tb_endereco>(entity =>
            {
                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.bairro).HasMaxLength(100);

                entity.Property(e => e.cep).HasMaxLength(8);

                entity.Property(e => e.cidade).HasMaxLength(100);

                entity.Property(e => e.com_quem_mora).HasMaxLength(100);

                entity.Property(e => e.complemento).HasMaxLength(150);

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.endereco).HasMaxLength(150);

                entity.Property(e => e.estado).HasMaxLength(2);

                entity.Property(e => e.internacional_linha_dois).HasMaxLength(150);

                entity.Property(e => e.internacional_linha_um).HasMaxLength(150);
            });

            modelBuilder.Entity<tb_endosso_competencia>(entity =>
            {
                entity.HasIndex(e => e.codigo_interno_colaborador, "fk_tb_colaborador_competencia_has_tb_colaborador_tb_colabor_idx");

                entity.HasIndex(e => e.colaborador_competencia_id, "fk_tb_colaborador_competencia_has_tb_colaborador_tb_colabor_idx1");

                entity.HasIndex(e => e.tb_status_endosso_id, "fk_tb_endosso_competencia_tb_status_endosso1_idx");

                entity.HasIndex(e => e.tb_tipo_endosso_id, "fk_tb_endosso_competencia_tb_tipo_endosso1_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.codigo_interno_colaboradorNavigation)
                    .WithMany(p => p.tb_endosso_competencia)
                    .HasForeignKey(d => d.codigo_interno_colaborador)
                    .HasConstraintName("fk_tb_colaborador_competencia_has_tb_colaborador_tb_colaborad2");

                entity.HasOne(d => d.colaborador_competencia)
                    .WithMany(p => p.tb_endosso_competencia)
                    .HasForeignKey(d => d.colaborador_competencia_id)
                    .HasConstraintName("fk_tb_colaborador_competencia_has_tb_colaborador_tb_colaborad1");

                entity.HasOne(d => d.tb_status_endosso)
                    .WithMany(p => p.tb_endosso_competencia)
                    .HasForeignKey(d => d.tb_status_endosso_id)
                    .HasConstraintName("fk_tb_endosso_competencia_tb_status_endosso1");

                entity.HasOne(d => d.tb_tipo_endosso)
                    .WithMany(p => p.tb_endosso_competencia)
                    .HasForeignKey(d => d.tb_tipo_endosso_id)
                    .HasConstraintName("fk_tb_endosso_competencia_tb_tipo_endosso1");
            });

            modelBuilder.Entity<tb_endosso_dominionegocio>(entity =>
            {
                entity.HasIndex(e => e.colaborador_dominionegocio_id, "fk_tb_colaborador_has_tb_colaborador_dominionegocio_tb_cola_idx");

                entity.HasIndex(e => e.codigo_interno_colaborador, "fk_tb_colaborador_has_tb_colaborador_dominionegocio_tb_cola_idx1");

                entity.HasIndex(e => e.tb_status_endosso_id, "fk_tb_endosso_dominionegocio_tb_status_endosso1_idx");

                entity.HasIndex(e => e.tb_tipo_endosso_id, "fk_tb_endosso_dominionegocio_tb_tipo_endosso1_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.codigo_interno_colaboradorNavigation)
                    .WithMany(p => p.tb_endosso_dominionegocio)
                    .HasForeignKey(d => d.codigo_interno_colaborador)
                    .HasConstraintName("fk_tb_colaborador_has_tb_colaborador_dominionegocio_tb_colabo1");

                entity.HasOne(d => d.colaborador_dominionegocio)
                    .WithMany(p => p.tb_endosso_dominionegocio)
                    .HasForeignKey(d => d.colaborador_dominionegocio_id)
                    .HasConstraintName("fk_tb_colaborador_has_tb_colaborador_dominionegocio_tb_colabo2");

                entity.HasOne(d => d.tb_status_endosso)
                    .WithMany(p => p.tb_endosso_dominionegocio)
                    .HasForeignKey(d => d.tb_status_endosso_id)
                    .HasConstraintName("fk_tb_endosso_dominionegocio_tb_status_endosso1");

                entity.HasOne(d => d.tb_tipo_endosso)
                    .WithMany(p => p.tb_endosso_dominionegocio)
                    .HasForeignKey(d => d.tb_tipo_endosso_id)
                    .HasConstraintName("fk_tb_endosso_dominionegocio_tb_tipo_endosso1");
            });

            modelBuilder.Entity<tb_endosso_formacao>(entity =>
            {
                entity.HasIndex(e => e.colaborador_formacao_id, "fk_tb_colaborador_has_tb_colaborador_formacao_tb_colaborado_idx");

                entity.HasIndex(e => e.codigo_interno_colaborador, "fk_tb_colaborador_has_tb_colaborador_formacao_tb_colaborado_idx1");

                entity.HasIndex(e => e.tb_status_endosso_id, "fk_tb_endosso_formacao_tb_status_endosso1_idx");

                entity.HasIndex(e => e.tb_tipo_endosso_id, "fk_tb_endosso_formacao_tb_tipo_endosso1_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.codigo_interno_colaboradorNavigation)
                    .WithMany(p => p.tb_endosso_formacao)
                    .HasForeignKey(d => d.codigo_interno_colaborador)
                    .HasConstraintName("fk_tb_colaborador_has_tb_colaborador_formacao_tb_colaborador1");

                entity.HasOne(d => d.colaborador_formacao)
                    .WithMany(p => p.tb_endosso_formacao)
                    .HasForeignKey(d => d.colaborador_formacao_id)
                    .HasConstraintName("fk_tb_colaborador_has_tb_colaborador_formacao_tb_colaborador_1");

                entity.HasOne(d => d.tb_status_endosso)
                    .WithMany(p => p.tb_endosso_formacao)
                    .HasForeignKey(d => d.tb_status_endosso_id)
                    .HasConstraintName("fk_tb_endosso_formacao_tb_status_endosso1");

                entity.HasOne(d => d.tb_tipo_endosso)
                    .WithMany(p => p.tb_endosso_formacao)
                    .HasForeignKey(d => d.tb_tipo_endosso_id)
                    .HasConstraintName("fk_tb_endosso_formacao_tb_tipo_endosso1");
            });

            modelBuilder.Entity<tb_endosso_metodologia>(entity =>
            {
                entity.HasIndex(e => e.colaborador_metodologia_id, "fk_tb_colaborador_has_tb_colaborador_metodologia_tb_colabor_idx");

                entity.HasIndex(e => e.codigo_interno_colaborador, "fk_tb_colaborador_has_tb_colaborador_metodologia_tb_colabor_idx1");

                entity.HasIndex(e => e.tb_status_endosso_id, "fk_tb_endosso_metodologia_tb_status_endosso1_idx");

                entity.HasIndex(e => e.tb_tipo_endosso_id, "fk_tb_endosso_metodologia_tb_tipo_endosso1_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.codigo_interno_colaboradorNavigation)
                    .WithMany(p => p.tb_endosso_metodologia)
                    .HasForeignKey(d => d.codigo_interno_colaborador)
                    .HasConstraintName("fk_tb_colaborador_has_tb_colaborador_metodologia_tb_colaborad1");

                entity.HasOne(d => d.colaborador_metodologia)
                    .WithMany(p => p.tb_endosso_metodologia)
                    .HasForeignKey(d => d.colaborador_metodologia_id)
                    .HasConstraintName("fk_tb_colaborador_has_tb_colaborador_metodologia_tb_colaborad2");

                entity.HasOne(d => d.tb_status_endosso)
                    .WithMany(p => p.tb_endosso_metodologia)
                    .HasForeignKey(d => d.tb_status_endosso_id)
                    .HasConstraintName("fk_tb_endosso_metodologia_tb_status_endosso1");

                entity.HasOne(d => d.tb_tipo_endosso)
                    .WithMany(p => p.tb_endosso_metodologia)
                    .HasForeignKey(d => d.tb_tipo_endosso_id)
                    .HasConstraintName("fk_tb_endosso_metodologia_tb_tipo_endosso1");
            });

            modelBuilder.Entity<tb_endosso_modeloreferencia>(entity =>
            {
                entity.HasIndex(e => e.codigo_interno_colaborador, "fk_tb_colaborador_modeloreferencia_has_tb_colaborador_tb_co_idx");

                entity.HasIndex(e => e.colaborador_modeloreferencia_id, "fk_tb_colaborador_modeloreferencia_has_tb_colaborador_tb_co_idx1");

                entity.HasIndex(e => e.tb_status_endosso_id, "fk_tb_endosso_modeloreferencia_tb_status_endosso1_idx");

                entity.HasIndex(e => e.tb_tipo_endosso_id, "fk_tb_endosso_modeloreferencia_tb_tipo_endosso1_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.codigo_interno_colaboradorNavigation)
                    .WithMany(p => p.tb_endosso_modeloreferencia)
                    .HasForeignKey(d => d.codigo_interno_colaborador)
                    .HasConstraintName("fk_tb_colaborador_modeloreferencia_has_tb_colaborador_tb_cola2");

                entity.HasOne(d => d.colaborador_modeloreferencia)
                    .WithMany(p => p.tb_endosso_modeloreferencia)
                    .HasForeignKey(d => d.colaborador_modeloreferencia_id)
                    .HasConstraintName("fk_tb_colaborador_modeloreferencia_has_tb_colaborador_tb_cola1");

                entity.HasOne(d => d.tb_status_endosso)
                    .WithMany(p => p.tb_endosso_modeloreferencia)
                    .HasForeignKey(d => d.tb_status_endosso_id)
                    .HasConstraintName("fk_tb_endosso_modeloreferencia_tb_status_endosso1");

                entity.HasOne(d => d.tb_tipo_endosso)
                    .WithMany(p => p.tb_endosso_modeloreferencia)
                    .HasForeignKey(d => d.tb_tipo_endosso_id)
                    .HasConstraintName("fk_tb_endosso_modeloreferencia_tb_tipo_endosso1");
            });

            modelBuilder.Entity<tb_escolaridade>(entity =>
            {
                entity.HasIndex(e => e.tb_formacao_id, "fk_tb_escolaridade_tb_formacao1_idx");

                entity.HasIndex(e => e.codigo_interno_colaborador, "fk_tb_formacao_tb_colaborador1_idx");

                entity.HasIndex(e => e.tipo_diploma_id, "fk_tb_formacao_tb_tipo_diploma1_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_inicio).HasColumnType("timestamp");

                entity.Property(e => e.data_termino).HasColumnType("timestamp");

                entity.Property(e => e.descricao).HasMaxLength(300);

                entity.Property(e => e.instituicao)
                    .IsRequired()
                    .HasMaxLength(120);

                entity.Property(e => e.path_diploma).HasMaxLength(500);

                entity.HasOne(d => d.codigo_interno_colaboradorNavigation)
                    .WithMany(p => p.tb_escolaridade)
                    .HasForeignKey(d => d.codigo_interno_colaborador)
                    .HasConstraintName("fk_tb_formacao_tb_colaborador1");

                entity.HasOne(d => d.tb_formacao)
                    .WithMany(p => p.tb_escolaridade)
                    .HasForeignKey(d => d.tb_formacao_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_tb_escolaridade_tb_formacao1");

                entity.HasOne(d => d.tipo_diploma)
                    .WithMany(p => p.tb_escolaridade)
                    .HasForeignKey(d => d.tipo_diploma_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_formacao_tb_tipo_diploma1");
            });

            modelBuilder.Entity<tb_estado_civil>(entity =>
            {
                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descricao)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<tb_estagio_processo_seletivo>(entity =>
            {
                entity.HasIndex(e => e.descricao, "descricao_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descricao)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<tb_etnia>(entity =>
            {
                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descricao)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<tb_experiencia>(entity =>
            {
                entity.HasIndex(e => e.codigo_interno_colaborador, "fk_tb_experiencia_tb_colaborador1_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.atual).HasDefaultValueSql("'1'");

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_inicio).HasColumnType("timestamp");

                entity.Property(e => e.data_saida).HasColumnType("timestamp");

                entity.Property(e => e.descricao)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.empresa)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.titulo)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.codigo_interno_colaboradorNavigation)
                    .WithMany(p => p.tb_experiencia)
                    .HasForeignKey(d => d.codigo_interno_colaborador)
                    .HasConstraintName("fk_tb_experiencia_tb_colaborador1");
            });

            modelBuilder.Entity<tb_experiencia_empresa_sugestao>(entity =>
            {
                entity.HasKey(e => e.nome)
                    .HasName("PRIMARY");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<tb_experiencia_projeto>(entity =>
            {
                entity.HasIndex(e => e.experiencia_id, "experiencia_id");

                entity.Property(e => e.data_fim).HasColumnType("date");

                entity.Property(e => e.data_inicio).HasColumnType("date");

                entity.Property(e => e.nome_projeto)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.HasOne(d => d.experiencia)
                    .WithMany(p => p.tb_experiencia_projeto)
                    .HasForeignKey(d => d.experiencia_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tb_experiencia_projeto_ibfk_1");
            });

            modelBuilder.Entity<tb_experiencia_projeto_sugestao>(entity =>
            {
                entity.HasKey(e => e.nome)
                    .HasName("PRIMARY");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<tb_feriado>(entity =>
            {
                entity.HasIndex(e => e.tb_org_id, "fk_feriado_tb_org_id");

                entity.Property(e => e.data).HasColumnType("date");

                entity.Property(e => e.data_atualizacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.nome)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.tipo)
                    .IsRequired()
                    .HasColumnType("enum('NACIONAL','ESTADUAL','MUNICIPAL')");

                entity.HasOne(d => d.tb_org)
                    .WithMany(p => p.tb_feriado)
                    .HasForeignKey(d => d.tb_org_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_feriado_tb_org_id");
            });

            modelBuilder.Entity<tb_filtro>(entity =>
            {
                entity.HasIndex(e => e.descricao, "descricao_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descricao)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<tb_filtro_competencia_nivel>(entity =>
            {
                entity.HasIndex(e => e.competencia_id, "fk_tb_competencia_nivel_competencia_id_idx");

                entity.HasIndex(e => e.filtro_id, "fk_tb_competencia_nivel_filtro_id_idx");

                entity.HasIndex(e => e.nivel_id, "fk_tb_competencia_nivel_nivel_id_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.competencia)
                    .WithMany(p => p.tb_filtro_competencia_nivel)
                    .HasForeignKey(d => d.competencia_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_competencia_nivel_competencia_id");

                entity.HasOne(d => d.filtro)
                    .WithMany(p => p.tb_filtro_competencia_nivel)
                    .HasForeignKey(d => d.filtro_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_competencia_nivel_filtro_id");

                entity.HasOne(d => d.nivel)
                    .WithMany(p => p.tb_filtro_competencia_nivel)
                    .HasForeignKey(d => d.nivel_id)
                    .HasConstraintName("fk_tb_competencia_nivel_nivel_id");
            });

            modelBuilder.Entity<tb_filtro_dominionegocio_nivel>(entity =>
            {
                entity.HasIndex(e => e.dominionegocio_id, "fk_tb_dominionegocio_nivel_dominionegocio_id_idx");

                entity.HasIndex(e => e.filtro_id, "fk_tb_dominionegocio_nivel_filtro_id_idx");

                entity.HasIndex(e => e.nivel_id, "fk_tb_dominionegocio_nivel_nivel_id_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.dominionegocio)
                    .WithMany(p => p.tb_filtro_dominionegocio_nivel)
                    .HasForeignKey(d => d.dominionegocio_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_dominionegocio_nivel_dominionegocio_id");

                entity.HasOne(d => d.filtro)
                    .WithMany(p => p.tb_filtro_dominionegocio_nivel)
                    .HasForeignKey(d => d.filtro_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_dominionegocio_nivel_filtro_id");

                entity.HasOne(d => d.nivel)
                    .WithMany(p => p.tb_filtro_dominionegocio_nivel)
                    .HasForeignKey(d => d.nivel_id)
                    .HasConstraintName("fk_tb_dominionegocio_nivel_nivel_id");
            });

            modelBuilder.Entity<tb_filtro_formacao_nivel>(entity =>
            {
                entity.HasIndex(e => e.filtro_id, "fk_tb_formacao_nivel_filtro_id_idx");

                entity.HasIndex(e => e.formacao_id, "fk_tb_formacao_nivel_formacao_id_idx");

                entity.HasIndex(e => e.nivel_id, "fk_tb_formacao_nivel_nivel_id_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.filtro)
                    .WithMany(p => p.tb_filtro_formacao_nivel)
                    .HasForeignKey(d => d.filtro_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_formacao_nivel_filtro_id");

                entity.HasOne(d => d.formacao)
                    .WithMany(p => p.tb_filtro_formacao_nivel)
                    .HasForeignKey(d => d.formacao_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_formacao_nivel_formacao_id");

                entity.HasOne(d => d.nivel)
                    .WithMany(p => p.tb_filtro_formacao_nivel)
                    .HasForeignKey(d => d.nivel_id)
                    .HasConstraintName("fk_tb_formacao_nivel_nivel_id");
            });

            modelBuilder.Entity<tb_filtro_hobbies>(entity =>
            {
                entity.HasIndex(e => e.filtro_id, "fk_tb_filtro_hobbies_filtro_id");

                entity.HasIndex(e => e.hobbies_id, "fk_tb_filtro_hobbies_hobbies_id_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.filtro)
                    .WithMany(p => p.tb_filtro_hobbies)
                    .HasForeignKey(d => d.filtro_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_filtro_hobbies_filtro_id");

                entity.HasOne(d => d.hobbies)
                    .WithMany(p => p.tb_filtro_hobbies)
                    .HasForeignKey(d => d.hobbies_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_filtro_hobbies_hobbies_id");
            });

            modelBuilder.Entity<tb_filtro_interesse>(entity =>
            {
                entity.HasIndex(e => e.filtro_id, "fk_tb_filtro_interesse_filtro_id");

                entity.HasIndex(e => e.interesse_id, "fk_tb_filtro_interesse_interesse_id_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.filtro)
                    .WithMany(p => p.tb_filtro_interesse)
                    .HasForeignKey(d => d.filtro_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_filtro_interesse_filtro_id");

                entity.HasOne(d => d.interesse)
                    .WithMany(p => p.tb_filtro_interesse)
                    .HasForeignKey(d => d.interesse_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_filtro_interesse_interesse_id");
            });

            modelBuilder.Entity<tb_filtro_metodologia_nivel>(entity =>
            {
                entity.HasIndex(e => e.tb_nivel_id, "fk_tb_filtro_metodologia_nivel_tb_nivel1_idx");

                entity.HasIndex(e => e.filtro_id, "fk_tb_metodologia_nivel_filtro_id_idx");

                entity.HasIndex(e => e.metodologia_id, "fk_tb_metodologia_nivel_metodologia_id_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.filtro)
                    .WithMany(p => p.tb_filtro_metodologia_nivel)
                    .HasForeignKey(d => d.filtro_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_metodologia_nivel_filtro_id");

                entity.HasOne(d => d.metodologia)
                    .WithMany(p => p.tb_filtro_metodologia_nivel)
                    .HasForeignKey(d => d.metodologia_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_metodologia_nivel_metodologia_id");

                entity.HasOne(d => d.tb_nivel)
                    .WithMany(p => p.tb_filtro_metodologia_nivel)
                    .HasForeignKey(d => d.tb_nivel_id)
                    .HasConstraintName("fk_tb_filtro_metodologia_nivel_tb_nivel1");
            });

            modelBuilder.Entity<tb_filtro_modeloreferencia_nivel>(entity =>
            {
                entity.HasIndex(e => e.filtro_id, "fk_tb_modeloreferencia_nivel_filtro_id _idx");

                entity.HasIndex(e => e.modeloreferencia_id, "fk_tb_modeloreferencia_nivel_modeloreferencia_id_idx");

                entity.HasIndex(e => e.nivel_id, "fk_tb_modeloreferencia_nivel_nivel_id_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.filtro)
                    .WithMany(p => p.tb_filtro_modeloreferencia_nivel)
                    .HasForeignKey(d => d.filtro_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_modeloreferencia_nivel_filtro_id ");

                entity.HasOne(d => d.modeloreferencia)
                    .WithMany(p => p.tb_filtro_modeloreferencia_nivel)
                    .HasForeignKey(d => d.modeloreferencia_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_modeloreferencia_nivel_modeloreferencia_id");

                entity.HasOne(d => d.nivel)
                    .WithMany(p => p.tb_filtro_modeloreferencia_nivel)
                    .HasForeignKey(d => d.nivel_id)
                    .HasConstraintName("fk_tb_modeloreferencia_nivel_nivel_id");
            });

            modelBuilder.Entity<tb_filtro_softskills>(entity =>
            {
                entity.HasIndex(e => e.tb_filtro_id, "fk_tb_filtro_softskills_tb_filtro1_idx");

                entity.HasIndex(e => e.tb_nivel_id, "fk_tb_filtro_softskills_tb_nivel1_idx");

                entity.HasIndex(e => e.tb_softskill_id, "fk_tb_filtro_softskills_tb_softskill1_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.tb_filtro)
                    .WithMany(p => p.tb_filtro_softskills)
                    .HasForeignKey(d => d.tb_filtro_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_filtro_softskills_tb_filtro1");

                entity.HasOne(d => d.tb_nivel)
                    .WithMany(p => p.tb_filtro_softskills)
                    .HasForeignKey(d => d.tb_nivel_id)
                    .HasConstraintName("fk_tb_filtro_softskills_tb_nivel1");

                entity.HasOne(d => d.tb_softskill)
                    .WithMany(p => p.tb_filtro_softskills)
                    .HasForeignKey(d => d.tb_softskill_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_filtro_softskills_tb_softskill1");
            });

            modelBuilder.Entity<tb_fonteorigem_candidato>(entity =>
            {
                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descricao)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<tb_formacao>(entity =>
            {
                entity.HasIndex(e => e.descricao, "descricao_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.usuario_criacao_id, "fk_tb_formacao_tb_usuario1_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descricao)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.emissor).HasMaxLength(100);

                entity.HasOne(d => d.usuario_criacao)
                    .WithMany(p => p.tb_formacao)
                    .HasForeignKey(d => d.usuario_criacao_id)
                    .HasConstraintName("fk_tb_formacao_tb_usuario1");
            });

            modelBuilder.Entity<tb_funcionalidade_rota>(entity =>
            {
                entity.HasIndex(e => new { e.tb_funcionalidade_sistema_id, e.tb_org_id }, "tb_funcionalidade_sistema_id")
                    .IsUnique();

                entity.HasIndex(e => e.tb_org_id, "tb_org_id");

                entity.Property(e => e.rota)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.HasOne(d => d.tb_funcionalidade_sistema)
                    .WithMany(p => p.tb_funcionalidade_rota)
                    .HasForeignKey(d => d.tb_funcionalidade_sistema_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tb_funcionalidade_rota_ibfk_1");

                entity.HasOne(d => d.tb_org)
                    .WithMany(p => p.tb_funcionalidade_rota)
                    .HasForeignKey(d => d.tb_org_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tb_funcionalidade_rota_ibfk_2");
            });

            modelBuilder.Entity<tb_funcionalidade_sistema>(entity =>
            {
                entity.HasIndex(e => e.descricao, "descricao_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descricao)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<tb_grau_parentesco>(entity =>
            {
                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descricao)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<tb_graugraduacao>(entity =>
            {
                entity.HasIndex(e => e.descricao, "descricao_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.usuario_criacao_id, "fk_tb_graugraduacao_tb_usuario1_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descricao)
                    .IsRequired()
                    .HasMaxLength(45);

                entity.HasOne(d => d.usuario_criacao)
                    .WithMany(p => p.tb_graugraduacao)
                    .HasForeignKey(d => d.usuario_criacao_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_graugraduacao_tb_usuario1");
            });

            modelBuilder.Entity<tb_grupo_acesso>(entity =>
            {
                entity.HasIndex(e => e.tb_org_id, "fk_tb_grupo_acesso_tb_org1_idx");

                entity.HasIndex(e => new { e.descricao, e.tb_org_id }, "uc_descricao_tb_org_id")
                    .IsUnique();

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descricao)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.tb_org)
                    .WithMany(p => p.tb_grupo_acesso)
                    .HasForeignKey(d => d.tb_org_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_grupo_acesso_tb_org1");
            });

            modelBuilder.Entity<tb_grupo_acesso_funcionalidade_sistema>(entity =>
            {
                entity.HasIndex(e => e.tb_funcionalidade_sistema_id, "fk_tb_grupo_acesso_has_tb_funcionalidade_sistema_tb_funcion_idx");

                entity.HasIndex(e => e.tb_grupo_acesso_id, "fk_tb_grupo_acesso_has_tb_funcionalidade_sistema_tb_grupo_a_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.tb_funcionalidade_sistema)
                    .WithMany(p => p.tb_grupo_acesso_funcionalidade_sistema)
                    .HasForeignKey(d => d.tb_funcionalidade_sistema_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_grupo_acesso_has_tb_funcionalidade_sistema_tb_funcional1");

                entity.HasOne(d => d.tb_grupo_acesso)
                    .WithMany(p => p.tb_grupo_acesso_funcionalidade_sistema)
                    .HasForeignKey(d => d.tb_grupo_acesso_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_grupo_acesso_has_tb_funcionalidade_sistema_tb_grupo_ace1");
            });

            modelBuilder.Entity<tb_historico_competencia>(entity =>
            {
                entity.Property(e => e.codigo_interno_colaborador_alteracao).HasMaxLength(36);

                entity.Property(e => e.codigo_interno_colaborador_criacao).HasMaxLength(36);

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descricao_competencia)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.observacao)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.situacao)
                    .IsRequired()
                    .HasColumnType("enum('Unificada','Aprovada','Reprovada','Adicionada','Editada')");

                entity.Property(e => e.tipo_competencia_enum)
                    .IsRequired()
                    .HasColumnType("enum('HardSkill','SoftSkill','Metodologia','Dominio','Idioma')");
            });

            modelBuilder.Entity<tb_hobbies>(entity =>
            {
                entity.HasIndex(e => e.usuario_criacao_id, "fk_tb_hobbies_tb_usuario1_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descricao)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.usuario_criacao)
                    .WithMany(p => p.tb_hobbies)
                    .HasForeignKey(d => d.usuario_criacao_id)
                    .HasConstraintName("fk_tb_hobbies_tb_usuario1");
            });

            modelBuilder.Entity<tb_holerite>(entity =>
            {
                entity.HasIndex(e => e.tb_colaborador_lg_id, "tb_colaborador_lg_id");

                entity.Property(e => e.competencia_ano)
                    .IsRequired()
                    .HasMaxLength(4);

                entity.Property(e => e.competencia_mes)
                    .IsRequired()
                    .HasMaxLength(2);

                entity.Property(e => e.data_alteracao).HasColumnType("timestamp");

                entity.Property(e => e.data_criacao).HasColumnType("timestamp");

                entity.Property(e => e.data_emissao).HasColumnType("timestamp");

                entity.Property(e => e.lg_blob_url_arquivo).HasMaxLength(500);

                entity.Property(e => e.lg_id_tarefa).HasMaxLength(50);

                entity.Property(e => e.s3_url_arquivo).HasMaxLength(500);

                entity.HasOne(d => d.tb_colaborador_lg)
                    .WithMany(p => p.tb_holerite)
                    .HasForeignKey(d => d.tb_colaborador_lg_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tb_holerite_ibfk_1");
            });

            modelBuilder.Entity<tb_identidade_genero>(entity =>
            {
                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descricao)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<tb_idioma>(entity =>
            {
                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descricao)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<tb_imagem>(entity =>
            {
                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.path)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<tb_indicacao_premiada_parcial>(entity =>
            {
                entity.Property(e => e.data_criacao).HasColumnType("timestamp");

                entity.Property(e => e.email_indicado).HasMaxLength(100);

                entity.Property(e => e.link_indicacao)
                    .IsRequired()
                    .HasMaxLength(350);

                entity.Property(e => e.linkedin).HasMaxLength(350);

                entity.Property(e => e.nome_indicado)
                    .IsRequired()
                    .HasMaxLength(120);

                entity.Property(e => e.path_curriculo).HasMaxLength(350);

                entity.Property(e => e.relacao_indicado)
                    .IsRequired()
                    .HasMaxLength(350);

                entity.Property(e => e.telefone_indicado)
                    .IsRequired()
                    .HasMaxLength(11);
            });

            modelBuilder.Entity<tb_interesse>(entity =>
            {
                entity.HasIndex(e => e.usuario_criacao_id, "fk_tb_interesse_tb_usuario1_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descricao)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.usuario_criacao)
                    .WithMany(p => p.tb_interesse)
                    .HasForeignKey(d => d.usuario_criacao_id)
                    .HasConstraintName("fk_tb_interesse_tb_usuario1");
            });

            modelBuilder.Entity<tb_item_perfil>(entity =>
            {
                entity.HasIndex(e => e.descricao, "descricao_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descricao)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<tb_historico_cv>(entity =>
            {
                entity.HasIndex(e => e.codigo_interno_colaborador, "fk_tb_historico_cv_codigo_interno_colaborador_idx");

                entity.HasIndex(e => e.tb_item_perfil_id, "fk_tb_historico_cv_tb_item_perfil_id_idx");

                entity.HasIndex(e => e.tb_nivel_id, "fk_tb_historico_cv_tb_nivel_id");

                entity.HasIndex(e => e.tb_origem_historico_cv_id, "fk_tb_historico_cv_tb_origem_historico_cv_id_idx");

                entity.HasIndex(e => e.tb_tipo_historico_cv_id, "fk_tb_historico_cv_tb_tipo_historico_cv_id_idx");

                entity.Property(e => e.id).HasMaxLength(36);

                entity.Property(e => e.codigo_interno_colaborador)
                    .IsRequired()
                    .HasMaxLength(36);

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.tb_origem_historico_cv_id)
                    .IsRequired()
                    .HasMaxLength(36);

                entity.Property(e => e.tb_tipo_historico_cv_id)
                    .IsRequired()
                    .HasMaxLength(36);

                entity.HasOne(d => d.codigo_interno_colaboradorNavigation)
                    .WithMany(p => p.tb_historico_cv)
                    .HasForeignKey(d => d.codigo_interno_colaborador)
                    .HasConstraintName("fk_tb_historico_cv_codigo_interno_colaborador");

                entity.HasOne(d => d.tb_item_perfil)
                    .WithMany(p => p.tb_historico_cv)
                    .HasForeignKey(d => d.tb_item_perfil_id)
                    .HasConstraintName("fk_tb_historico_cv_tb_item_perfil_id");

                entity.HasOne(d => d.tb_nivel)
                    .WithMany(p => p.tb_historico_cv)
                    .HasForeignKey(d => d.tb_nivel_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_tb_historico_cv_tb_nivel_id");

                entity.HasOne(d => d.tb_origem_historico_cv)
                    .WithMany(p => p.tb_historico_cv)
                    .HasForeignKey(d => d.tb_origem_historico_cv_id)
                    .HasConstraintName("fk_tb_historico_cv_tb_origem_historico_cv_id");

                entity.HasOne(d => d.tb_tipo_historico_cv)
                    .WithMany(p => p.tb_historico_cv)
                    .HasForeignKey(d => d.tb_tipo_historico_cv_id)
                    .HasConstraintName("fk_tb_historico_cv_tb_tipo_historico_cv_id");
            });

            modelBuilder.Entity<tb_like_cargo>(entity =>
            {
                entity.HasKey(e => new { e.codigo_interno_colaborador, e.colaborador_cargo_id })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.HasIndex(e => e.codigo_interno_colaborador, "fk_tb_colaborador_has_tb_colaborador_cargo_tb_colaborador1_idx");

                entity.HasIndex(e => e.colaborador_cargo_id, "fk_tb_colaborador_has_tb_colaborador_cargo_tb_colaborador_c_idx");

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.codigo_interno_colaboradorNavigation)
                    .WithMany(p => p.tb_like_cargo)
                    .HasForeignKey(d => d.codigo_interno_colaborador)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_colaborador_has_tb_colaborador_cargo_tb_colaborador1");

                entity.HasOne(d => d.colaborador_cargo)
                    .WithMany(p => p.tb_like_cargo)
                    .HasForeignKey(d => d.colaborador_cargo_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_colaborador_has_tb_colaborador_cargo_tb_colaborador_car1");
            });

            modelBuilder.Entity<tb_like_competencia>(entity =>
            {
                entity.HasKey(e => new { e.codigo_interno_colaborador, e.colaborador_competencia_id })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.HasIndex(e => e.colaborador_competencia_id, "fk_tb_colaborador_has_tb_colaborador_competencia_tb_colabor_idx");

                entity.HasIndex(e => e.codigo_interno_colaborador, "fk_tb_colaborador_has_tb_colaborador_competencia_tb_colabor_idx1");

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.codigo_interno_colaboradorNavigation)
                    .WithMany(p => p.tb_like_competencia)
                    .HasForeignKey(d => d.codigo_interno_colaborador)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_colaborador_has_tb_colaborador_competencia_tb_colaborad1");

                entity.HasOne(d => d.colaborador_competencia)
                    .WithMany(p => p.tb_like_competencia)
                    .HasForeignKey(d => d.colaborador_competencia_id)
                    .HasConstraintName("fk_tb_colaborador_has_tb_colaborador_competencia_tb_colaborad2");
            });

            modelBuilder.Entity<tb_like_dominionegocio>(entity =>
            {
                entity.HasKey(e => new { e.codigo_interno_colaborador, e.colaborador_dominionegocio_id })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.HasIndex(e => e.colaborador_dominionegocio_id, "fk_tb_colaborador_has_tb_colaborador_dominionegocio_tb_cola_idx");

                entity.HasIndex(e => e.codigo_interno_colaborador, "fk_tb_colaborador_has_tb_colaborador_dominionegocio_tb_cola_idx1");

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.codigo_interno_colaboradorNavigation)
                    .WithMany(p => p.tb_like_dominionegocio)
                    .HasForeignKey(d => d.codigo_interno_colaborador)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_colaborador_has_tb_colaborador_dominionegocio_tb_colabo3");

                entity.HasOne(d => d.colaborador_dominionegocio)
                    .WithMany(p => p.tb_like_dominionegocio)
                    .HasForeignKey(d => d.colaborador_dominionegocio_id)
                    .HasConstraintName("fk_tb_colaborador_has_tb_colaborador_dominionegocio_tb_colabo4");
            });

            modelBuilder.Entity<tb_like_formacao>(entity =>
            {
                entity.HasKey(e => new { e.codigo_interno_colaborador, e.colaborador_formacao_id })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.HasIndex(e => e.colaborador_formacao_id, "fk_tb_colaborador_has_tb_colaborador_formacao_tb_colaborado_idx");

                entity.HasIndex(e => e.codigo_interno_colaborador, "fk_tb_colaborador_has_tb_colaborador_formacao_tb_colaborado_idx1");

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.codigo_interno_colaboradorNavigation)
                    .WithMany(p => p.tb_like_formacao)
                    .HasForeignKey(d => d.codigo_interno_colaborador)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_colaborador_has_tb_colaborador_formacao_tb_colaborador2");

                entity.HasOne(d => d.colaborador_formacao)
                    .WithMany(p => p.tb_like_formacao)
                    .HasForeignKey(d => d.colaborador_formacao_id)
                    .HasConstraintName("fk_tb_colaborador_has_tb_colaborador_formacao_tb_colaborador_2");
            });

            modelBuilder.Entity<tb_like_hobbies>(entity =>
            {
                entity.HasKey(e => new { e.codigo_interno_colaborador, e.colaborador_hobbies_id })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.HasIndex(e => e.colaborador_hobbies_id, "fk_tb_colaborador_has_tb_colaborador_hobbies_tb_colaborador_idx");

                entity.HasIndex(e => e.codigo_interno_colaborador, "fk_tb_colaborador_has_tb_colaborador_hobbies_tb_colaborador_idx1");

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.codigo_interno_colaboradorNavigation)
                    .WithMany(p => p.tb_like_hobbies)
                    .HasForeignKey(d => d.codigo_interno_colaborador)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_colaborador_has_tb_colaborador_hobbies_tb_colaborador1");

                entity.HasOne(d => d.colaborador_hobbies)
                    .WithMany(p => p.tb_like_hobbies)
                    .HasForeignKey(d => d.colaborador_hobbies_id)
                    .HasConstraintName("fk_tb_colaborador_has_tb_colaborador_hobbies_tb_colaborador_h1");
            });

            modelBuilder.Entity<tb_like_interesse>(entity =>
            {
                entity.HasKey(e => new { e.codigo_interno_colaborador, e.colaborador_interesse_id })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.HasIndex(e => e.colaborador_interesse_id, "fk_tb_colaborador_has_tb_colaborador_interesse_tb_colaborad_idx");

                entity.HasIndex(e => e.codigo_interno_colaborador, "fk_tb_colaborador_has_tb_colaborador_interesse_tb_colaborad_idx1");

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                //entity.HasOne(d => d.codigo_interno_colaboradorNavigation)
                //    .WithMany(p => p.tb_like_interesse)
                //    .HasForeignKey(d => d.codigo_interno_colaborador)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("fk_tb_colaborador_has_tb_colaborador_interesse_tb_colaborador1");

                //entity.HasOne(d => d.colaborador_interesse)
                //    .WithMany(p => p.tb_like_interesse)
                //    .HasForeignKey(d => d.colaborador_interesse_id)
                //    .HasConstraintName("fk_tb_colaborador_has_tb_colaborador_interesse_tb_colaborador2");
            });

            modelBuilder.Entity<tb_like_metodologia>(entity =>
            {
                entity.HasKey(e => new { e.codigo_interno_colaborador, e.colaborador_metodologia_id })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.HasIndex(e => e.colaborador_metodologia_id, "fk_tb_colaborador_has_tb_colaborador_metodologia_tb_colabor_idx");

                entity.HasIndex(e => e.codigo_interno_colaborador, "fk_tb_colaborador_has_tb_colaborador_metodologia_tb_colabor_idx1");

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.codigo_interno_colaboradorNavigation)
                    .WithMany(p => p.tb_like_metodologia)
                    .HasForeignKey(d => d.codigo_interno_colaborador)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_colaborador_has_tb_colaborador_metodologia_tb_colaborad3");

                entity.HasOne(d => d.colaborador_metodologia)
                    .WithMany(p => p.tb_like_metodologia)
                    .HasForeignKey(d => d.colaborador_metodologia_id)
                    .HasConstraintName("fk_tb_colaborador_has_tb_colaborador_metodologia_tb_colaborad4");
            });

            modelBuilder.Entity<tb_like_modeloreferencia>(entity =>
            {
                entity.HasKey(e => new { e.codigo_interno_colaborador, e.colaborador_modeloreferencia_id })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.HasIndex(e => e.colaborador_modeloreferencia_id, "fk_tb_colaborador_has_tb_colaborador_modeloreferencia_tb_co_idx");

                entity.HasIndex(e => e.codigo_interno_colaborador, "fk_tb_colaborador_has_tb_colaborador_modeloreferencia_tb_co_idx1");

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.codigo_interno_colaboradorNavigation)
                    .WithMany(p => p.tb_like_modeloreferencia)
                    .HasForeignKey(d => d.codigo_interno_colaborador)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_colaborador_has_tb_colaborador_modeloreferencia_tb_cola1");

                entity.HasOne(d => d.colaborador_modeloreferencia)
                    .WithMany(p => p.tb_like_modeloreferencia)
                    .HasForeignKey(d => d.colaborador_modeloreferencia_id)
                    .HasConstraintName("fk_tb_colaborador_has_tb_colaborador_modeloreferencia_tb_cola2");
            });

            modelBuilder.Entity<tb_like_noticia>(entity =>
            {
                entity.HasKey(e => new { e.codigo_interno_colaborador, e.tb_noticia_id })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.HasIndex(e => e.codigo_interno_colaborador, "fk_tb_colaborador_has_tb_colaborador_hobbies_tb_colaborador_idx1");

                entity.HasIndex(e => e.tb_noticia_id, "fk_tb_like_hobbies_copy1_tb_noticia1_idx");

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.codigo_interno_colaboradorNavigation)
                    .WithMany(p => p.tb_like_noticia)
                    .HasForeignKey(d => d.codigo_interno_colaborador)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_colaborador_has_tb_colaborador_hobbies_tb_colaborador10");

                entity.HasOne(d => d.tb_noticia)
                    .WithMany(p => p.tb_like_noticia)
                    .HasForeignKey(d => d.tb_noticia_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_like_hobbies_copy1_tb_noticia1");
            });

            modelBuilder.Entity<tb_log>(entity =>
            {
                entity.Property(e => e.complete_message).HasColumnType("text");

                entity.Property(e => e.date)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.message).HasColumnType("text");

                entity.Property(e => e.stacktrace).HasColumnType("text");

                entity.Property(e => e.codigo_interno_colaborador_origin).HasColumnType("text");
            });

            modelBuilder.Entity<tb_metodologia>(entity =>
            {
                entity.HasIndex(e => e.descricao, "descricao_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.usuario_criacao_id, "fk_tb_metodologia_tb_usuario1_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descricao)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.usuario_criacao)
                    .WithMany(p => p.tb_metodologia)
                    .HasForeignKey(d => d.usuario_criacao_id)
                    .HasConstraintName("fk_tb_metodologia_tb_usuario1");
            });

            modelBuilder.Entity<tb_modalidade>(entity =>
            {
                entity.HasIndex(e => e.descricao, "descricao_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descricao)
                    .IsRequired()
                    .HasMaxLength(120);
            });

            modelBuilder.Entity<tb_modalidade_contratacao>(entity =>
            {
                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descricao)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<tb_modeloreferencia>(entity =>
            {
                entity.HasIndex(e => e.descricao, "descricao_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.usuario_criacao_id, "fk_tb_modeloreferencia_tb_usuario1_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descricao)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.usuario_criacao)
                    .WithMany(p => p.tb_modeloreferencia)
                    .HasForeignKey(d => d.usuario_criacao_id)
                    .HasConstraintName("fk_tb_modeloreferencia_tb_usuario1");
            });

            modelBuilder.Entity<tb_nacionalidade>(entity =>
            {

                entity.HasIndex(e => e.Descricao, "Descricao_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Descricao)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<tb_nivel>(entity =>
            {
                entity.HasIndex(e => e.tb_item_perfil_id, "fk_tb_nivel_tb_item_perfil1_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descricao)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.prioridade_unificacao)
                    .HasColumnName("prioridade_unificacao")
                    .HasColumnType("int");

                entity.Property(e => e.ordem_exibicao)
                    .HasColumnName("ordem_exibicao")
                    .HasColumnType("int");

                entity.HasOne(d => d.tb_item_perfil)
                    .WithMany(p => p.tb_nivel)
                    .HasForeignKey(d => d.tb_item_perfil_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_nivel_tb_item_perfil1");
            });

            modelBuilder.Entity<tb_nivel_escolaridade>(entity =>
            {
                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descricao)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<tb_nivel_referencia_hardskill>(entity =>
            {
                entity.HasIndex(e => e.descricao, "descricao_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.descricao)
                    .IsRequired()
                    .HasMaxLength(45);
            });

            modelBuilder.Entity<tb_noticia>(entity =>
            {
                entity.HasIndex(e => e.imagem_id, "fk_noticia_imagem_id_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descricao)
                    .IsRequired()
                    .HasMaxLength(5000);

                entity.Property(e => e.titulo)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.imagem)
                    .WithMany(p => p.tb_noticia)
                    .HasForeignKey(d => d.imagem_id)
                    .HasConstraintName("fk_noticia_imagem_id");
            });

            modelBuilder.Entity<tb_notificacao>(entity =>
            {
                entity.HasIndex(e => new { e.codigo_interno_colaborador, e.tb_org_id }, "idx_tb_colaborador_cpf_tb_org_id");

                entity.HasIndex(e => e.tb_funcionalidade_sistema_id, "tb_funcionalidade_sistema_id");

                entity.HasIndex(e => e.tb_org_id, "tb_org_id");

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.data_envio)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_leitura).HasColumnType("timestamp");

                entity.Property(e => e.lida).HasDefaultValueSql("'0'");

                entity.Property(e => e.mensagem)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.mensagem_html).HasColumnType("text");

                entity.Property(e => e.titulo)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.url_customizada).HasMaxLength(2048);

                entity.HasOne(d => d.codigo_interno_colaboradorNavigation)
                    .WithMany(p => p.tb_notificacao)
                    .HasForeignKey(d => d.codigo_interno_colaborador)
                    .HasConstraintName("tb_notificacao_ibfk_3");

                entity.HasOne(d => d.tb_funcionalidade_sistema)
                    .WithMany(p => p.tb_notificacao)
                    .HasForeignKey(d => d.tb_funcionalidade_sistema_id)
                    .HasConstraintName("tb_notificacao_ibfk_1");

                entity.HasOne(d => d.tb_org)
                    .WithMany(p => p.tb_notificacao)
                    .HasForeignKey(d => d.tb_org_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tb_notificacao_ibfk_2");
            });

            modelBuilder.Entity<tb_org>(entity =>
            {
                entity.HasIndex(e => e.descricao, "descricao_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.dominio_email, "dominio_email_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.subdominio, "subdominio_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.id).ValueGeneratedNever();

                entity.Property(e => e.descricao)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.dominio_email)
                    .IsRequired()
                    .HasMaxLength(45);

                entity.Property(e => e.subdominio)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<tb_orientacao_sexual>(entity =>
            {
                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descricao)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<tb_pais>(entity =>
            {

                entity.HasIndex(e => e.Descricao, "Descricao_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Descricao)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<tb_parametro>(entity =>
            {
                entity.HasIndex(e => e.codigo_parametro, "idx_codigo_parametro");

                entity.Property(e => e.codigo_modulo_sistema)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.codigo_parametro).IsRequired();

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descricao_parametro)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.nome_parametro)
                    .IsRequired()
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<tb_parametro_configuracao>(entity =>
            {
                entity.HasIndex(e => e.codigo_parametro, "codigo_parametro");

                entity.HasIndex(e => e.tb_grupo_acesso_id, "tb_grupo_acesso_id");

                entity.HasIndex(e => new { e.tb_org_id, e.codigo_interno_colaborador }, "tb_org_id");

                entity.HasIndex(e => e.tb_parametro_nivel_id, "tb_parametro_nivel_id");

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.valor_parametro).HasMaxLength(1000);

                entity.HasOne(d => d.tb_grupo_acesso)
                    .WithMany(p => p.tb_parametro_configuracao)
                    .HasForeignKey(d => d.tb_grupo_acesso_id)
                    .HasConstraintName("tb_parametro_configuracao_ibfk_4");

                entity.HasOne(d => d.tb_org)
                    .WithMany(p => p.tb_parametro_configuracao)
                    .HasForeignKey(d => d.tb_org_id)
                    .HasConstraintName("tb_parametro_configuracao_ibfk_1");

                entity.HasOne(d => d.tb_parametro_nivel)
                    .WithMany(p => p.tb_parametro_configuracao)
                    .HasForeignKey(d => d.tb_parametro_nivel_id)
                    .HasConstraintName("tb_parametro_configuracao_ibfk_5");

                entity.HasOne(d => d.tb_colaborador_org)
                    .WithMany(p => p.tb_parametro_configuracao)
                    .HasForeignKey(d => new { d.tb_org_id, d.codigo_interno_colaborador })
                    .HasConstraintName("tb_parametro_configuracao_ibfk_3");
            });

            modelBuilder.Entity<tb_parametro_nivel>(entity =>
            {
                entity.Property(e => e.id).ValueGeneratedNever();

                entity.Property(e => e.descricao_nivel).HasMaxLength(45);
            });

            modelBuilder.Entity<tb_periodo_alocacao>(entity =>
            {
                entity.HasIndex(e => e.tb_colaborador_alocado_id, "fk_tb_periodo_alocacao_tb_colaborador_alocado1_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_fim).HasColumnType("timestamp");

                entity.Property(e => e.data_inicio).HasColumnType("timestamp");

                entity.Property(e => e.observacao).HasColumnType("text");

                entity.Property(e => e.oportunidade).HasColumnType("text");

                entity.Property(e => e.prioritario).HasDefaultValueSql("'0'");

                entity.HasOne(d => d.tb_colaborador_alocado)
                    .WithMany(p => p.tb_periodo_alocacao)
                    .HasForeignKey(d => d.tb_colaborador_alocado_id)
                    .HasConstraintName("fk_tb_periodo_alocacao_tb_colaborador_alocado1");
            });

            modelBuilder.Entity<tb_pessoa_juridica>(entity =>
            {
                entity.HasKey(e => e.cnpj)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.tb_bancos_codigo, "fk_tb_pessoa_juridica_tb_bancos1_idx");

                entity.HasIndex(e => e.codigo_interno_colaborador, "fk_tb_pessoa_juridica_tb_colaborador1_idx");

                entity.HasIndex(e => e.tb_regime_tributario_id, "fk_tb_pessoa_juridica_tb_regime_tributario1_idx");

                entity.Property(e => e.cnpj).HasMaxLength(15);

                entity.Property(e => e.agencia).HasMaxLength(20);

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.conta_digito).HasMaxLength(20);

                entity.Property(e => e.nome_fantasia)
                    .IsRequired()
                    .HasMaxLength(120);

                entity.Property(e => e.razao_social)
                    .IsRequired()
                    .HasMaxLength(120);

                entity.Property(e => e.tb_bancos_codigo)
                    .IsRequired()
                    .HasMaxLength(12);

                entity.HasOne(d => d.codigo_interno_colaboradorNavigation)
                    .WithMany(p => p.tb_pessoa_juridica)
                    .HasForeignKey(d => d.codigo_interno_colaborador)
                    .HasConstraintName("fk_tb_pessoa_juridica_tb_colaborador1");

                entity.HasOne(d => d.tb_bancos_codigoNavigation)
                    .WithMany(p => p.tb_pessoa_juridica)
                    .HasForeignKey(d => d.tb_bancos_codigo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_pessoa_juridica_tb_bancos1");

                entity.HasOne(d => d.tb_regime_tributario)
                    .WithMany(p => p.tb_pessoa_juridica)
                    .HasForeignKey(d => d.tb_regime_tributario_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_pessoa_juridica_tb_regime_tributario1");
            });

            modelBuilder.Entity<tb_projeto>(entity =>
            {
                entity.HasIndex(e => e.tb_empresa_cnpj, "fk_tb_projeto_tb_empresa1_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_final).HasColumnType("timestamp");

                entity.Property(e => e.data_inicial).HasColumnType("timestamp");

                entity.Property(e => e.nome_projeto)
                    .IsRequired()
                    .HasMaxLength(120);

                entity.Property(e => e.tb_empresa_cnpj)
                    .IsRequired()
                    .HasMaxLength(14);

                entity.HasOne(d => d.tb_empresa_cnpjNavigation)
                    .WithMany(p => p.tb_projeto)
                    .HasForeignKey(d => d.tb_empresa_cnpj)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_projeto_tb_empresa1");
            });

            modelBuilder.Entity<tb_projeto_cch>(entity =>
            {
                entity.HasKey(e => e.cdProjeto)
                    .HasName("PRIMARY");

                entity.Property(e => e.cdProjeto).ValueGeneratedNever();

                entity.Property(e => e.nmProjeto)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<tb_projeto_gerente>(entity =>
            {
                entity.HasKey(e => new { e.cod_projeto, e.cod_colaborador_gerente, e.tipo_gerente, e.tb_org_id })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0, 0 });

                entity.HasIndex(e => new { e.cod_colaborador_gerente, e.tb_org_id }, "fk_tb_proj_ger_tb_colab_org");

                entity.HasIndex(e => e.tb_org_id, "fk_tb_projeto_gerente_tb_org1_idx");

                entity.HasOne(d => d.tb_org)
                    .WithMany(p => p.tb_projeto_gerente)
                    .HasForeignKey(d => d.tb_org_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_projeto_gerente_tb_org1");
            });

            modelBuilder.Entity<tb_projeto_org>(entity =>
            {
                entity.HasKey(e => new { e.cod_projeto, e.tb_org_id })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.HasIndex(e => e.tb_org_id, "fk_tb_projeto_org_tb_org1_idx");

                entity.Property(e => e.cod_cliente)
                    .IsRequired()
                    .HasMaxLength(45);

                entity.Property(e => e.cod_cliente_registro_carga)
                    .IsRequired()
                    .HasMaxLength(45);

                entity.Property(e => e.nome_cliente_registro_carga)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.cod_diretoria)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.cod_proposta).HasMaxLength(255);

                entity.Property(e => e.codigo_oportunidade).HasMaxLength(255);

                entity.Property(e => e.data_fim).HasColumnType("datetime");

                entity.Property(e => e.data_inicio).HasColumnType("datetime");

                entity.Property(e => e.diretoria)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.prioritario).HasDefaultValueSql("'0'");

                entity.Property(e => e.projeto)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.qtd_horas_executadas).HasPrecision(10);

                entity.Property(e => e.qtd_horas_planejadas).HasPrecision(10);

                entity.Property(e => e.status)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.tipo_cadastro)
                    .HasMaxLength(50);

                entity.HasOne(d => d.tb_org)
                    .WithMany(p => p.tb_projeto_org)
                    .HasForeignKey(d => d.tb_org_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_projeto_org_tb_org1");
            });

            modelBuilder.Entity<tb_projeto_org_atividade>(entity =>
            {
                entity.HasIndex(e => e.tb_atividade_id, "tb_atividade_id");

                entity.HasIndex(e => new { e.tb_projeto_org_cod_projeto, e.tb_projeto_tb_org_id }, "tb_projeto_org_cod_projeto");

                entity.HasOne(d => d.tb_atividade)
                    .WithMany(p => p.tb_projeto_org_atividade)
                    .HasForeignKey(d => d.tb_atividade_id)
                    .HasConstraintName("tb_projeto_org_atividade_ibfk_2");

                entity.HasOne(d => d.tb_projeto_)
                    .WithMany(p => p.tb_projeto_org_atividade)
                    .HasForeignKey(d => new { d.tb_projeto_org_cod_projeto, d.tb_projeto_tb_org_id })
                    .HasConstraintName("tb_projeto_org_atividade_ibfk_1");
            });

            modelBuilder.Entity<tb_projetohora_cch>(entity =>
            {
                entity.HasKey(e => e.cdProjeto)
                    .HasName("PRIMARY");

                entity.Property(e => e.cdProjeto).ValueGeneratedNever();

                entity.Property(e => e.dtFimProj).HasColumnType("timestamp");

                entity.Property(e => e.dtInicioProjeto).HasColumnType("timestamp");

                entity.Property(e => e.nmCliente).HasMaxLength(500);

                entity.Property(e => e.nmProjeto)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.nmPropostas).HasMaxLength(45);

                entity.Property(e => e.nmStatusProjeto).HasMaxLength(500);
            });

            modelBuilder.Entity<tb_regime_tributario>(entity =>
            {
                entity.Property(e => e.descricao)
                    .IsRequired()
                    .HasMaxLength(45);
            });

            modelBuilder.Entity<tb_skill_candidato_srs>(entity =>
            {
                entity.Property(e => e.data_alteracao).HasColumnType("datetime");

                entity.Property(e => e.data_criacao).HasColumnType("datetime");
            });

            modelBuilder.Entity<tb_skill_vaga>(entity =>
            {
                entity.HasIndex(e => e.tb_vagas_srs_id, "fk_tb_vaga_favorito_tb_vagas_srs1_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.tb_vagas_srs)
                    .WithMany(p => p.tb_skill_vaga)
                    .HasForeignKey(d => d.tb_vagas_srs_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_skill_vaga_tb_vagas_srs1");
            });

            modelBuilder.Entity<tb_skill_vaga_srs>(entity =>
            {
                entity.Property(e => e.data_alteracao).HasColumnType("datetime");

                entity.Property(e => e.data_criacao).HasColumnType("datetime");
            });

            modelBuilder.Entity<tb_softskill>(entity =>
            {
                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descricao)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<tb_status_apontamento>(entity =>
            {
                entity.HasIndex(e => e.tb_cod_status_grupo, "tb_cod_status_grupo");

                entity.HasIndex(e => e.cod_status_apontamento, "tb_status_apontamento_index")
                    .IsUnique();

                entity.Property(e => e.descricao).HasMaxLength(50);

                entity.HasOne(d => d.tb_cod_status_grupoNavigation)
                    .WithMany(p => p.tb_status_apontamento)
                    .HasPrincipalKey(p => p.cod_status_grupo)
                    .HasForeignKey(d => d.tb_cod_status_grupo)
                    .HasConstraintName("tb_status_apontamento_ibfk_1");
            });

            modelBuilder.Entity<tb_status_apontamento_grupo>(entity =>
            {
                entity.HasIndex(e => e.cod_status_grupo, "tb_status_apontamento_grupo_index")
                    .IsUnique();

                entity.Property(e => e.cod_status_grupo).IsRequired();

                entity.Property(e => e.descricao).HasMaxLength(50);
            });

            modelBuilder.Entity<tb_status_colaborador>(entity =>
            {
                entity.HasIndex(e => e.descricao, "descricao_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descricao)
                    .IsRequired()
                    .HasMaxLength(45);
            });

            modelBuilder.Entity<tb_status_endosso>(entity =>
            {
                entity.HasIndex(e => e.descricao, "descricao_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descricao)
                    .IsRequired()
                    .HasMaxLength(45);
            });

            modelBuilder.Entity<tb_status_projeto>(entity =>
            {
                entity.HasIndex(e => e.tb_org_id, "tb_org_id");

                entity.Property(e => e.descricao_status).HasMaxLength(255);

                entity.HasOne(d => d.tb_org)
                    .WithMany(p => p.tb_status_projeto)
                    .HasForeignKey(d => d.tb_org_id)
                    .HasConstraintName("tb_status_projeto_ibfk_1");
            });

            modelBuilder.Entity<tb_tbd_alocado>(entity =>
            {
                entity.HasKey(e => e.cod_tbd_alocado)
                    .HasName("PRIMARY");

                entity.HasIndex(e => new { e.cod_tbd_alocado, e.tb_org_id }, "idx_cod_org_id")
                    .IsUnique();

                entity.HasIndex(e => e.cod_tbd_alocado, "idx_tb_tbd_alocado_cod_tbd_alocado");

                entity.HasIndex(e => e.codigo_interno_colaborador_gestor, "codigo_interno_colaborador_gestor");

                entity.HasIndex(e => e.tb_org_id, "tb_org_id");

                entity.Property(e => e.cod_diretoria).HasMaxLength(255);

                entity.Property(e => e.cod_tbd_alocado).ValueGeneratedOnAdd();

                entity.Property(e => e.codigo_interno_colaborador_gestor).HasMaxLength(36);

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descricao).HasMaxLength(255);

                entity.Property(e => e.diretoria).HasMaxLength(255);

                entity.HasOne(d => d.codigo_interno_colaborador_gestorNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.codigo_interno_colaborador_gestor)
                    .HasConstraintName("tb_tbd_alocado_ibfk_2");

                entity.HasOne(d => d.tb_org)
                    .WithMany()
                    .HasForeignKey(d => d.tb_org_id)
                    .HasConstraintName("tb_tbd_alocado_ibfk_1");
            });

            modelBuilder.Entity<tb_template_email>(entity =>
            {
                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.codigo)
                    .IsRequired()
                    .HasMaxLength(45);

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descricao).HasMaxLength(200);

                entity.Property(e => e.template).HasColumnType("text");
            });

            modelBuilder.Entity<tb_template_email_rotina>(entity =>
            {
                entity.HasIndex(e => e.tb_org_id, "fk_tb_org_id_idx");

                entity.Property(e => e.corpo_email_parametrizado)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.data_alteracao).HasColumnType("timestamp");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_disparo).HasColumnType("timestamp");

                entity.Property(e => e.emails)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.mensagem_erro).HasMaxLength(400);

                entity.HasOne(d => d.tb_org)
                    .WithMany(p => p.tb_template_email_rotina)
                    .HasForeignKey(d => d.tb_org_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_org_id1");
            });

            modelBuilder.Entity<tb_tipo_carga_horaria>(entity =>
            {
                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descricao)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<tb_tipo_cargo>(entity =>
            {
                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descricao)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<tb_tipo_contratacao>(entity =>
            {
                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descricao)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<tb_tipo_dependente>(entity =>
            {
                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descricao)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<tb_tipo_diploma>(entity =>
            {
                entity.HasIndex(e => e.descricao, "descricao_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.descricao).IsRequired();
            });

            modelBuilder.Entity<tb_tipo_endosso>(entity =>
            {
                entity.HasIndex(e => e.descricao, "descricao_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descricao)
                    .IsRequired()
                    .HasMaxLength(45);
            });

            modelBuilder.Entity<tb_token_acesso>(entity =>
            {
                entity.HasIndex(e => e.token, "token_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.token)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.validade).HasColumnType("datetime");
            });

            modelBuilder.Entity<tb_token_resete_senha>(entity =>
            {
                entity.HasIndex(e => e.tb_usuario_id, "fk_tb_token_resete_senha_tb_usuario1_idx");

                entity.Property(e => e.token)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.validade).HasColumnType("timestamp");

                entity.HasOne(d => d.tb_usuario)
                    .WithMany(p => p.tb_token_resete_senha)
                    .HasForeignKey(d => d.tb_usuario_id)
                    .HasConstraintName("fk_tb_token_resete_senha_tb_usuario1");
            });

            modelBuilder.Entity<tb_token_sistema>(entity =>
            {
                entity.HasIndex(e => e.sistema, "sistema_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.tb_org_id, "tb_token_sistema_tb_org_FK");

                entity.HasIndex(e => e.token, "token_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.sistema)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.token)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.HasOne(d => d.tb_org)
                    .WithMany(p => p.tb_token_sistema)
                    .HasForeignKey(d => d.tb_org_id)
                    .HasConstraintName("tb_token_sistema_tb_org_FK");
            });

            modelBuilder.Entity<tb_token_sso>(entity =>
            {
                entity.HasIndex(e => e.tb_usuario_id, "fk_tb_token_sso_tb_usuario1_idx");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.refresh_token)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.token)
                    .IsRequired()
                    .HasColumnType("text");

                entity.HasOne(d => d.tb_usuario)
                    .WithMany(p => p.tb_token_sso)
                    .HasForeignKey(d => d.tb_usuario_id)
                    .HasConstraintName("fk_tb_token_sso_tb_usuario1");
            });

            modelBuilder.Entity<tb_trending>(entity =>
            {
                entity.Property(e => e.data_busca).HasColumnType("datetime");

                entity.Property(e => e.resultado_busca)
                    .IsRequired()
                    .HasColumnType("text");
            });

            modelBuilder.Entity<tb_usuario>(entity =>
            {
                entity.HasIndex(e => e.codigo_interno_colaborador, "cpf_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_expiracao).HasColumnType("timestamp");
                entity.Property(e => e.dataAceiteTermo).HasColumnType("timestamp");

                entity.Property(e => e.email)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.fcm_token).HasMaxLength(200);

                entity.Property(e => e.password)
                    .IsRequired()
                    .HasMaxLength(32);

                entity.Property(e => e.slack_id).HasMaxLength(100);

                entity.Property(e => e.slack_team_id).HasMaxLength(100);

                entity.Property(e => e.slack_username).HasMaxLength(100);

                entity.HasOne(d => d.codigo_interno_colaboradorNavigation)
                    .WithMany(p => p.tb_usuario)
                    .HasForeignKey(d => d.codigo_interno_colaborador)
                    .HasConstraintName("fk_usuario_colaborador");
            });

            modelBuilder.Entity<tb_usuario_grupo_acesso>(entity =>
            {
                entity.HasIndex(e => e.tb_grupo_acesso_id, "fk_tb_usuario_has_tb_grupo_acesso_tb_grupo_acesso1_idx");

                entity.HasIndex(e => e.tb_usuario_id, "fk_tb_usuario_has_tb_grupo_acesso_tb_usuario1_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.tb_grupo_acesso)
                    .WithMany(p => p.tb_usuario_grupo_acesso)
                    .HasForeignKey(d => d.tb_grupo_acesso_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_usuario_has_tb_grupo_acesso_tb_grupo_acesso1");

                entity.HasOne(d => d.tb_usuario)
                    .WithMany(p => p.tb_usuario_grupo_acesso)
                    .HasForeignKey(d => d.tb_usuario_id)
                    .HasConstraintName("fk_tb_usuario_has_tb_grupo_acesso_tb_usuario1");
            });

            modelBuilder.Entity<tb_usuario_permissao_log>(entity =>
            {
                entity.Property(e => e.codigo_interno_colaborador_criacao).HasMaxLength(36);

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.operacao)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<tb_usuario_tokenacesso>(entity =>
            {
                entity.HasIndex(e => e.token_acesso_id, "fk_usuario_has_tokenAcesso_tokenAcesso1_idx");

                entity.HasIndex(e => e.usuario_id, "fk_usuario_tokenAcesso_usuario1_idx");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.token_acesso)
                    .WithMany(p => p.tb_usuario_tokenacesso)
                    .HasForeignKey(d => d.token_acesso_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_usuario_tokenAcesso_tokenAcesso1");

                entity.HasOne(d => d.usuario)
                    .WithMany(p => p.tb_usuario_tokenacesso)
                    .HasForeignKey(d => d.usuario_id)
                    .HasConstraintName("fk_usuario_tokenAcesso_usuario2");
            });

            modelBuilder.Entity<tb_vaga>(entity =>
            {
                entity.Property(e => e.cidade).HasMaxLength(100);

                entity.Property(e => e.data_aceitacao).HasColumnType("datetime");

                entity.Property(e => e.data_atualizacao).HasColumnType("datetime");

                entity.Property(e => e.data_criacao).HasColumnType("datetime");

                entity.Property(e => e.data_publicacao).HasColumnType("datetime");

                entity.Property(e => e.descricao).HasColumnType("text");

                entity.Property(e => e.email).HasMaxLength(255);

                entity.Property(e => e.estado).HasMaxLength(50);

                entity.Property(e => e.funcao).HasColumnType("text");

                entity.Property(e => e.nivel_de_urgencia).HasMaxLength(50);

                entity.Property(e => e.observacao_localizacao).HasColumnType("text");

                entity.Property(e => e.status).HasMaxLength(50);

                entity.Property(e => e.taxa_maxima_hora).HasMaxLength(255);

                entity.Property(e => e.tipo).HasMaxLength(50);

                entity.Property(e => e.tipo_localizacao).HasMaxLength(50);

                entity.Property(e => e.titulo).HasMaxLength(255);
            });

            modelBuilder.Entity<tb_vaga_competencia>(entity =>
            {
                entity.HasIndex(e => e.conhecimento_de_negocio_id, "conhecimento_de_negocio_id");

                entity.HasIndex(e => e.hardskill_id, "hardskill_id");

                entity.HasIndex(e => e.idioma_id, "idioma_id");

                entity.HasIndex(e => e.metodologia_id, "metodologia_id");

                entity.HasIndex(e => e.softskill_id, "softskill_id");

                entity.HasIndex(e => e.vaga_id, "vaga_id");

                entity.HasOne(d => d.conhecimento_de_negocio)
                    .WithMany(p => p.tb_vaga_competencia)
                    .HasForeignKey(d => d.conhecimento_de_negocio_id)
                    .HasConstraintName("tb_vaga_competencia_ibfk_5");

                entity.HasOne(d => d.hardskill)
                    .WithMany(p => p.tb_vaga_competencia)
                    .HasForeignKey(d => d.hardskill_id)
                    .HasConstraintName("tb_vaga_competencia_ibfk_2");

                entity.HasOne(d => d.idioma)
                    .WithMany(p => p.tb_vaga_competencia)
                    .HasForeignKey(d => d.idioma_id)
                    .HasConstraintName("tb_vaga_competencia_ibfk_6");

                entity.HasOne(d => d.metodologia)
                    .WithMany(p => p.tb_vaga_competencia)
                    .HasForeignKey(d => d.metodologia_id)
                    .HasConstraintName("tb_vaga_competencia_ibfk_4");

                entity.HasOne(d => d.softskill)
                    .WithMany(p => p.tb_vaga_competencia)
                    .HasForeignKey(d => d.softskill_id)
                    .HasConstraintName("tb_vaga_competencia_ibfk_3");
            });

            modelBuilder.Entity<tb_vaga_favorito>(entity =>
            {
                entity.HasIndex(e => e.tb_usuario_id, "fk_tb_vaga_favorito_tb_usuario1_idx");

                entity.HasIndex(e => e.tb_vagas_srs_id, "fk_tb_vaga_favorito_tb_vagas_srs1_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.tb_usuario)
                    .WithMany(p => p.tb_vaga_favorito)
                    .HasForeignKey(d => d.tb_usuario_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_vaga_favorito_tb_usuario1");

                entity.HasOne(d => d.tb_vagas_srs)
                    .WithMany(p => p.tb_vaga_favorito)
                    .HasForeignKey(d => d.tb_vagas_srs_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_vaga_favorito_tb_vagas_srs1");
            });

            modelBuilder.Entity<tb_vagas_indicadas>(entity =>
            {
                entity.Property(e => e.data_aplicacao).HasColumnType("timestamp");

                entity.Property(e => e.data_criacao).HasColumnType("timestamp");

                entity.Property(e => e.link_URL)
                    .IsRequired()
                    .HasMaxLength(350);
            });

            modelBuilder.Entity<tb_vagas_srs>(entity =>
            {
                entity.HasKey(e => e.id_vaga)
                    .HasName("PRIMARY");

                entity.Property(e => e.id_vaga).ValueGeneratedNever();

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.cargo).HasMaxLength(200);

                entity.Property(e => e.data_abertura).HasColumnType("timestamp");

                entity.Property(e => e.data_alteracao).HasColumnType("timestamp");

                entity.Property(e => e.data_criacao).HasColumnType("timestamp");

                entity.Property(e => e.descricao).HasMaxLength(1000);

                entity.Property(e => e.habilidades).HasMaxLength(400);

                entity.Property(e => e.loc_trabalho).HasMaxLength(20);

                entity.Property(e => e.metodologia).HasMaxLength(100);

                entity.Property(e => e.nivel).HasMaxLength(45);

                entity.Property(e => e.state).HasMaxLength(64);

                entity.Property(e => e.status_vaga)
                    .IsRequired()
                    .HasMaxLength(45);

                entity.Property(e => e.titulo)
                    .IsRequired()
                    .HasMaxLength(70);

                entity.Property(e => e.vagas_abertas).HasDefaultValueSql("'0'");
            });

            modelBuilder.Entity<tb_video>(entity =>
            {
                entity.HasIndex(e => e.tb_acesso_id, "fk_tb_video_tb_acesso1_idx");

                entity.Property(e => e.ativo).HasDefaultValueSql("'1'");

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descricao)
                    .IsRequired()
                    .HasMaxLength(5000);

                entity.Property(e => e.path)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.titulo)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.tb_acesso)
                    .WithMany(p => p.tb_video)
                    .HasForeignKey(d => d.tb_acesso_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tb_video_tb_acesso1");
            });

            modelBuilder.Entity<trendingview>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("trendingview");

                entity.Property(e => e.data_consulta)
                    .HasMaxLength(10);

                entity.Property(e => e.resultado_busca)
                    .IsRequired()
                    .HasColumnType("text");
            });

            modelBuilder.Entity<vw_alocacacao_recurso_calculo_mensal>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vw_alocacacao_recurso_calculo_mensal");

                entity.Property(e => e.cod_diretoria).HasMaxLength(255);

                entity.Property(e => e.codigo_colaborador)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasDefaultValueSql("''");

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.codigo_interno_colaborador_gestor)
                    .HasMaxLength(36);

                entity.Property(e => e.diretoria).HasMaxLength(255);

                entity.Property(e => e.nome).HasMaxLength(255);
            });

            modelBuilder.Entity<vw_alocacao_hierarquia_calculo_mensal>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vw_alocacao_hierarquia_calculo_mensal");

                entity.Property(e => e.cod_colaborador_externo)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasDefaultValueSql("''");

                entity.Property(e => e.cod_colaborador_superior)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasDefaultValueSql("''");
            });

            modelBuilder.Entity<vw_apontamento_mensal>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vw_apontamento_mensal");

                entity.Property(e => e.cod_cliente)
                    .IsRequired()
                    .HasMaxLength(45);

                entity.Property(e => e.cod_gerente).HasMaxLength(255);

                entity.Property(e => e.cod_projeto)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.codigo_interno_colaborador_gerente).HasMaxLength(36);

                entity.Property(e => e.data_fim_projeto).HasColumnType("datetime");

                entity.Property(e => e.descricao_status_mensal).HasMaxLength(50);

                entity.Property(e => e.gerente)
                    .IsRequired()
                    .HasMaxLength(120)
                    .HasDefaultValueSql("''");

                entity.Property(e => e.nome_cliente)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.nome_projeto)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.observacao).HasMaxLength(500);

                entity.Property(e => e.tipo_gerente).HasMaxLength(255);

                entity.Property(e => e.total_horas).HasPrecision(42);
            });

            modelBuilder.Entity<vw_apontamento_mensal_visao_gerente>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vw_apontamento_mensal_visao_gerente");

                entity.Property(e => e.cod_gerente).HasMaxLength(255);

                entity.Property(e => e.cod_projeto)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.codigo_interno_colaborador_gerente).HasMaxLength(36);

                entity.Property(e => e.descricao_status_mensal).HasMaxLength(50);

                entity.Property(e => e.gerente).HasMaxLength(120);

                entity.Property(e => e.nome_projeto)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.tipo_gerente).HasMaxLength(255);

                entity.Property(e => e.total_horas).HasPrecision(42);
            });

            modelBuilder.Entity<vw_colaborador_apontamento>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vw_colaborador_apontamento");

                entity.Property(e => e.atividade_descricao)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.cod_cliente)
                    .IsRequired()
                    .HasMaxLength(45);

                entity.Property(e => e.cod_gerente).HasMaxLength(255);

                entity.Property(e => e.cod_projeto)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.codigo_interno_colaborador_gerente).HasMaxLength(36);

                entity.Property(e => e.data_justificativa).HasColumnType("datetime");

                entity.Property(e => e.data_registro).HasColumnType("date");

                entity.Property(e => e.gerente)
                    .IsRequired()
                    .HasMaxLength(120)
                    .HasDefaultValueSql("''");

                entity.Property(e => e.justificativa).HasMaxLength(500);

                entity.Property(e => e.nome_cliente)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.nome_projeto)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.nome_usuario_justificativa).HasMaxLength(120);

                entity.Property(e => e.observacao).HasMaxLength(500);

                entity.Property(e => e.status_apontamento).HasMaxLength(50);

                entity.Property(e => e.status_apontamento_grupo).HasMaxLength(50);

                entity.Property(e => e.tipo_apontamento).HasColumnType("enum('diario','mensal')");

                entity.Property(e => e.tipo_gerente).HasMaxLength(255);
            });

            modelBuilder.Entity<vw_colaboradores_gestor>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vw_colaboradores_gestor");

                entity.Property(e => e.cod_colaborador)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.cod_gerente)
                    .IsRequired()
                    .HasMaxLength(45);

                entity.Property(e => e.codigo_interno_colaborador)
                    .IsRequired()
                    .HasMaxLength(36);

                entity.Property(e => e.nome_completo)
                    .IsRequired()
                    .HasMaxLength(120);

                entity.Property(e => e.nome_completo_colaborador)
                    .IsRequired()
                    .HasMaxLength(120);

                entity.Property(e => e.nome_completo_gerente).HasMaxLength(120);
            });

            modelBuilder.Entity<vw_competencias_sugeridas>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vw_competencias_sugeridas");

                entity.Property(e => e.CompetenciaTipo)
                    .IsRequired()
                    .HasMaxLength(11)
                    .HasDefaultValueSql("''");

                entity.Property(e => e.DataCriacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'0000-00-00 00:00:00'");

                entity.Property(e => e.Descricao)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasDefaultValueSql("''");
            });

            modelBuilder.Entity<vw_filtro_mapaalocacao>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vw_filtro_mapaalocacao");

                entity.Property(e => e.cod_diretoria)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.codigo_interno_colaborador)
                    .IsRequired()
                    .HasMaxLength(36);

                entity.Property(e => e.diretoria)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.hardskills).HasColumnType("text");

                entity.Property(e => e.idiomas).HasColumnType("text");

                entity.Property(e => e.nome_completo)
                    .IsRequired()
                    .HasMaxLength(120);
            });

            modelBuilder.Entity<vw_gestores_colaboradores_org>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vw_gestores_colaboradores_org");

                entity.Property(e => e.cod_colaborador_externo_gestor)
                    .IsRequired()
                    .HasMaxLength(45);

                entity.Property(e => e.cod_colaborador_externo_subordinado)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.codigo_interno_colaborador_gestor)
                    .IsRequired()
                    .HasMaxLength(36);

                entity.Property(e => e.codigo_interno_colaborador_subordinado)
                    .IsRequired()
                    .HasMaxLength(36);

                entity.Property(e => e.nome_completo_gestor)
                    .IsRequired()
                    .HasMaxLength(120);

                entity.Property(e => e.nome_completo_subordinado)
                    .IsRequired()
                    .HasMaxLength(120);
            });

            modelBuilder.Entity<vw_gestores_estatisticas_org>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vw_gestores_estatisticas_org");

                entity.Property(e => e.codigo_interno_colaborador)
                    .IsRequired()
                    .HasMaxLength(36);
            });

            modelBuilder.Entity<vw_gestores_org>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vw_gestores_org");

                entity.Property(e => e.cod_colaborador_superior)
                    .IsRequired()
                    .HasMaxLength(45);

                entity.Property(e => e.cod_departamento)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.cod_diretoria)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.diretoria)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.nome_completo)
                    .IsRequired()
                    .HasMaxLength(120);
            });

            modelBuilder.Entity<vw_mapa_alocacao_colaborador_tbd>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vw_mapa_alocacao_colaborador_tbd");

                entity.Property(e => e.cod_profisisonal)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasDefaultValueSql("''");

                entity.Property(e => e.codigo_departamento).HasMaxLength(255);

                entity.Property(e => e.codigo_diretoria).HasMaxLength(255);

                entity.Property(e => e.codigo_interno_colaborador).HasMaxLength(36);

                entity.Property(e => e.codigo_interno_colaborador_gestor).HasMaxLength(36);

                entity.Property(e => e.nome_profissional).HasMaxLength(255);
            });

            modelBuilder.Entity<vw_totalizadores_unidades>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vw_totalizadores_unidades");

                entity.Property(e => e.Diretoria)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasDefaultValueSql("''");
            });

            modelBuilder.Entity<tb_colaborador_alocado_skill>(entity =>
            {
                entity.HasKey(e => new { e.tb_colaborador_periodo_alocacao_id, e.tb_item_perfil_id, e.skill_id })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0 });

                entity.HasIndex(e => e.codigo_interno_colaborador_criacao, "codigo_interno_colaborador_criacao");

                entity.HasIndex(e => e.tb_item_perfil_id, "tb_item_perfil_id");

                entity.HasIndex(e => e.tb_nivel_id, "tb_nivel_id");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.tb_colaborador_periodo_alocacao)
                    .WithMany(p => p.tb_colaborador_alocado_skill)
                    .HasForeignKey(d => d.tb_colaborador_periodo_alocacao_id)
                    .HasConstraintName("tb_colaborador_alocado_skill_ibfk_1");
            });

            modelBuilder.Entity<tb_perfil>(entity =>
            {
                entity.HasIndex(e => e.codigo_interno_colaborador_alteracao, "fk_tb_perfil_codigo_interno_colaborador_alteracao");

                entity.HasIndex(e => e.codigo_interno_colaborador_criacao, "fk_tb_perfil_codigo_interno_colaborador_criacao");

                entity.HasIndex(e => e.codigo_projeto, "fk_tb_perfil_codigo_projeto");

                entity.HasIndex(e => e.tb_org_id, "tb_org_id");

                entity.Property(e => e.codigo_interno_colaborador_alteracao).HasMaxLength(36);

                entity.Property(e => e.codigo_interno_colaborador_criacao).HasMaxLength(36);

                entity.Property(e => e.data_alteracao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.data_criacao)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.nome_perfil)
                    .IsRequired()
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<tb_perfil_alocacao>(entity =>
            {
                entity.HasIndex(e => e.tb_org_id, "tb_org_id");

                entity.HasIndex(e => e.tb_colaborador_periodo_alocacao_id, "tb_perfil_alocacao_tb_colaborador_periodo_alocacao_fk");

                entity.HasIndex(e => e.tb_gestor_externo_perfil_id, "tb_perfil_alocacao_tb_gestor_externo_perfil_fk");

                entity.HasIndex(e => e.tb_perfil_id, "tb_perfil_alocacao_tb_perfil_FK");

                entity.Property(e => e.id).HasMaxLength(36);

                entity.Property(e => e.tb_gestor_externo_perfil_id).HasMaxLength(36);

                entity.HasOne(d => d.tb_colaborador_periodo_alocacao)
                    .WithMany(p => p.tb_perfil_alocacao)
                    .HasForeignKey(d => d.tb_colaborador_periodo_alocacao_id)
                    .HasConstraintName("tb_perfil_alocacao_tb_colaborador_periodo_alocacao_fk");

                entity.HasOne(d => d.tb_perfil)
                    .WithMany(p => p.tb_perfil_alocacao)
                    .HasForeignKey(d => d.tb_perfil_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("tb_perfil_alocacao_tb_perfil_FK");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}