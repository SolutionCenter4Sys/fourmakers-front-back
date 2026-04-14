start transaction;

SET FOREIGN_KEY_CHECKS=0;

ALTER TABLE tb_colaborador
RENAME COLUMN cpf to codigo_interno_colaborador;
ALTER TABLE tb_colaborador
MODIFY COLUMN codigo_interno_colaborador varchar(36);

ALTER TABLE tb_contato_colaborador
RENAME COLUMN seguidor_cpf to seguidor_codigo_interno_colaborador;
ALTER TABLE tb_contato_colaborador
MODIFY COLUMN seguidor_codigo_interno_colaborador varchar(36);

ALTER TABLE tb_contato_colaborador
RENAME COLUMN seguindo_cpf to seguindo_codigo_interno_colaborador;
ALTER TABLE tb_contato_colaborador
MODIFY COLUMN seguindo_codigo_interno_colaborador varchar(36);

ALTER TABLE tb_colaborador_comentario
RENAME COLUMN colaborador_cpf to codigo_interno_colaborador;
ALTER TABLE tb_colaborador_comentario
MODIFY COLUMN codigo_interno_colaborador varchar(36);

ALTER TABLE tb_colaborador_dependente 
RENAME COLUMN colaborador_cpf to codigo_interno_colaborador;
ALTER TABLE tb_colaborador_dependente
MODIFY COLUMN codigo_interno_colaborador varchar(36);

ALTER TABLE tb_tbd_alocado 
RENAME COLUMN tb_colaborador_cpf_gestor to codigo_interno_colaborador;
ALTER TABLE tb_tbd_alocado
MODIFY COLUMN codigo_interno_colaborador varchar(36);

ALTER TABLE tb_colaborador_alocado 
RENAME COLUMN tb_colaborador_cpf to codigo_interno_colaborador;
ALTER TABLE tb_colaborador_alocado
MODIFY COLUMN codigo_interno_colaborador varchar(36);

ALTER TABLE tb_colaborador_graugraduacao 
RENAME COLUMN colaborador_cpf to codigo_interno_colaborador;
ALTER TABLE tb_colaborador_graugraduacao
MODIFY COLUMN codigo_interno_colaborador varchar(36);

ALTER TABLE tb_colaborador_idioma 
RENAME COLUMN colaborador_cpf to codigo_interno_colaborador;
ALTER TABLE tb_colaborador_idioma
MODIFY COLUMN codigo_interno_colaborador varchar(36);

ALTER TABLE tb_colaborador_org 
RENAME COLUMN tb_colaborador_cpf to codigo_interno_colaborador;
ALTER TABLE tb_colaborador_org
MODIFY COLUMN codigo_interno_colaborador varchar(36);

ALTER TABLE tb_parametro_configuracao 
RENAME COLUMN tb_colaborador_org_cpf to codigo_interno_colaborador;
ALTER TABLE tb_parametro_configuracao
MODIFY COLUMN codigo_interno_colaborador varchar(36);

ALTER TABLE tb_colaborador_apontamento 
RENAME COLUMN tb_colaborador_org_tb_colaborador_cpf to codigo_interno_colaborador;
ALTER TABLE tb_colaborador_apontamento
MODIFY COLUMN codigo_interno_colaborador varchar(36);
ALTER TABLE tb_colaborador_apontamento 
RENAME COLUMN tb_colaborador_cpf_criacao to codigo_interno_colaborador_criacao;
ALTER TABLE tb_colaborador_apontamento
MODIFY COLUMN codigo_interno_colaborador_criacao varchar(36);
ALTER TABLE tb_colaborador_apontamento 
RENAME COLUMN tb_colaborador_cpf_alteracao to codigo_interno_colaborador_alteracao;
ALTER TABLE tb_colaborador_apontamento
MODIFY COLUMN codigo_interno_colaborador_alteracao varchar(36);
ALTER TABLE tb_colaborador_apontamento 
RENAME COLUMN tb_colaborador_cpf_justificativa to codigo_interno_colaborador_justificativa;
ALTER TABLE tb_colaborador_apontamento
MODIFY COLUMN codigo_interno_colaborador_justificativa varchar(36);

ALTER TABLE tb_colaborador_apontamento_log
RENAME COLUMN tb_colaborador_org_tb_colaborador_cpf to codigo_interno_colaborador;
ALTER TABLE tb_colaborador_apontamento_log
MODIFY COLUMN codigo_interno_colaborador varchar(36);
ALTER TABLE tb_colaborador_apontamento_log 
RENAME COLUMN tb_colaborador_cpf_criacao to codigo_interno_colaborador_criacao;
ALTER TABLE tb_colaborador_apontamento_log
MODIFY COLUMN codigo_interno_colaborador_criacao varchar(36);

ALTER TABLE tb_colaborador_periodo_alocacao 
RENAME COLUMN tb_colaborador_cpf to codigo_interno_colaborador;
ALTER TABLE tb_colaborador_periodo_alocacao
MODIFY COLUMN codigo_interno_colaborador varchar(36);

ALTER TABLE tb_colaborador_passaporte 
RENAME COLUMN colaborador_cpf to codigo_interno_colaborador;
ALTER TABLE tb_colaborador_passaporte
MODIFY COLUMN codigo_interno_colaborador varchar(36);

ALTER TABLE tb_colaborador_projeto 
RENAME COLUMN colaborador_cpf to codigo_interno_colaborador;
ALTER TABLE tb_colaborador_projeto
MODIFY COLUMN codigo_interno_colaborador varchar(36);

ALTER TABLE tb_colaborador_referencia_hardskill 
RENAME COLUMN tb_colaborador_cpf to codigo_interno_colaborador;
ALTER TABLE tb_colaborador_referencia_hardskill
MODIFY COLUMN codigo_interno_colaborador varchar(36);

ALTER TABLE tb_colaborador_sobre 
RENAME COLUMN cpf to codigo_interno_colaborador;
ALTER TABLE tb_colaborador_sobre
MODIFY COLUMN codigo_interno_colaborador varchar(36);

ALTER TABLE tb_endosso_competencia 
RENAME COLUMN colaborador_cpf to codigo_interno_colaborador;
ALTER TABLE tb_endosso_competencia
MODIFY COLUMN codigo_interno_colaborador varchar(36);

ALTER TABLE tb_colaborador_softskill 
RENAME COLUMN colaborador_cpf to codigo_interno_colaborador;
ALTER TABLE tb_colaborador_softskill
MODIFY COLUMN codigo_interno_colaborador varchar(36);

ALTER TABLE tb_colaborador_competencia 
RENAME COLUMN colaborador_cpf to codigo_interno_colaborador;
ALTER TABLE tb_colaborador_competencia
MODIFY COLUMN codigo_interno_colaborador varchar(36);

ALTER TABLE tb_colaborador_status 
RENAME COLUMN colaborador_cpf to codigo_interno_colaborador;
ALTER TABLE tb_colaborador_status
MODIFY COLUMN codigo_interno_colaborador varchar(36);

ALTER TABLE tb_colaborador_visto 
RENAME COLUMN colaborador_cpf to codigo_interno_colaborador;
ALTER TABLE tb_colaborador_visto
MODIFY COLUMN codigo_interno_colaborador varchar(36);

ALTER TABLE tb_contato_emergencia 
RENAME COLUMN tb_colaborador_cpf to codigo_interno_colaborador;
ALTER TABLE tb_contato_emergencia
MODIFY COLUMN codigo_interno_colaborador varchar(36);

ALTER TABLE tb_like_competencia 
RENAME COLUMN colaborador_cpf to codigo_interno_colaborador;
ALTER TABLE tb_like_competencia
MODIFY COLUMN codigo_interno_colaborador varchar(36);

ALTER TABLE tb_curriculo_colaborador 
RENAME COLUMN tb_colaborador_cpf to codigo_interno_colaborador;
ALTER TABLE tb_curriculo_colaborador
MODIFY COLUMN codigo_interno_colaborador varchar(36);

ALTER TABLE tb_escolaridade 
RENAME COLUMN colaborador_cpf to codigo_interno_colaborador;
ALTER TABLE tb_escolaridade
MODIFY COLUMN codigo_interno_colaborador varchar(36);

ALTER TABLE tb_experiencia 
RENAME COLUMN colaborador_cpf to codigo_interno_colaborador;
ALTER TABLE tb_experiencia
MODIFY COLUMN codigo_interno_colaborador varchar(36);

ALTER TABLE tb_endosso_dominionegocio 
RENAME COLUMN colaborador_cpf to codigo_interno_colaborador;
ALTER TABLE tb_endosso_dominionegocio
MODIFY COLUMN codigo_interno_colaborador varchar(36);

ALTER TABLE tb_like_noticia 
RENAME COLUMN colaborador_cpf to codigo_interno_colaborador;
ALTER TABLE tb_like_noticia
MODIFY COLUMN codigo_interno_colaborador varchar(36);

ALTER TABLE tb_notificacao 
RENAME COLUMN tb_colaborador_cpf to codigo_interno_colaborador;
ALTER TABLE tb_notificacao
MODIFY COLUMN codigo_interno_colaborador varchar(36);

ALTER TABLE tb_colaborador_dominionegocio 
RENAME COLUMN colaborador_cpf to codigo_interno_colaborador;
ALTER TABLE tb_colaborador_dominionegocio
MODIFY COLUMN codigo_interno_colaborador varchar(36);

ALTER TABLE tb_pessoa_juridica 
RENAME COLUMN tb_colaborador_cpf to codigo_interno_colaborador;
ALTER TABLE tb_pessoa_juridica
MODIFY COLUMN codigo_interno_colaborador varchar(36);

ALTER TABLE tb_like_dominionegocio 
RENAME COLUMN colaborador_cpf to codigo_interno_colaborador;
ALTER TABLE tb_like_dominionegocio
MODIFY COLUMN codigo_interno_colaborador varchar(36);

ALTER TABLE tb_usuario 
RENAME COLUMN cpf to codigo_interno_colaborador;
ALTER TABLE tb_usuario
MODIFY COLUMN codigo_interno_colaborador varchar(36);

ALTER TABLE tb_endosso_formacao 
RENAME COLUMN colaborador_cpf to codigo_interno_colaborador;
ALTER TABLE tb_endosso_formacao
MODIFY COLUMN codigo_interno_colaborador varchar(36);

ALTER TABLE tb_endosso_metodologia 
RENAME COLUMN colaborador_cpf to codigo_interno_colaborador;
ALTER TABLE tb_endosso_metodologia
MODIFY COLUMN codigo_interno_colaborador varchar(36);

ALTER TABLE tb_colaborador_formacao 
RENAME COLUMN colaborador_cpf to codigo_interno_colaborador;
ALTER TABLE tb_colaborador_formacao
MODIFY COLUMN codigo_interno_colaborador varchar(36);

ALTER TABLE tb_like_cargo 
RENAME COLUMN colaborador_cpf to codigo_interno_colaborador;
ALTER TABLE tb_like_cargo
MODIFY COLUMN codigo_interno_colaborador varchar(36);

ALTER TABLE tb_colaborador_cargo 
RENAME COLUMN colaborador_cpf to codigo_interno_colaborador;
ALTER TABLE tb_colaborador_cargo
MODIFY COLUMN codigo_interno_colaborador varchar(36);

ALTER TABLE tb_like_formacao 
RENAME COLUMN colaborador_cpf to codigo_interno_colaborador;
ALTER TABLE tb_like_formacao
MODIFY COLUMN codigo_interno_colaborador varchar(36);

ALTER TABLE tb_colaborador_metodologia 
RENAME COLUMN colaborador_cpf to codigo_interno_colaborador;
ALTER TABLE tb_colaborador_metodologia
MODIFY COLUMN codigo_interno_colaborador varchar(36);

ALTER TABLE tb_like_metodologia 
RENAME COLUMN colaborador_cpf to codigo_interno_colaborador;
ALTER TABLE tb_like_metodologia
MODIFY COLUMN codigo_interno_colaborador varchar(36);

ALTER TABLE tb_endosso_modeloreferencia 
RENAME COLUMN colaborador_cpf to codigo_interno_colaborador;
ALTER TABLE tb_endosso_modeloreferencia
MODIFY COLUMN codigo_interno_colaborador varchar(36);

ALTER TABLE tb_colaborador_modeloreferencia 
RENAME COLUMN colaborador_cpf to codigo_interno_colaborador;
ALTER TABLE tb_colaborador_modeloreferencia
MODIFY COLUMN codigo_interno_colaborador varchar(36);

ALTER TABLE tb_like_modeloreferencia 
RENAME COLUMN colaborador_cpf to codigo_interno_colaborador;
ALTER TABLE tb_like_modeloreferencia
MODIFY COLUMN codigo_interno_colaborador varchar(36);

ALTER TABLE tb_colaborador_hobbies 
RENAME COLUMN colaborador_cpf to codigo_interno_colaborador;
ALTER TABLE tb_colaborador_hobbies
MODIFY COLUMN codigo_interno_colaborador varchar(36);

ALTER TABLE tb_like_hobbies 
RENAME COLUMN colaborador_cpf to codigo_interno_colaborador;
ALTER TABLE tb_like_hobbies
MODIFY COLUMN codigo_interno_colaborador varchar(36);

ALTER TABLE tb_colaborador_interesse 
RENAME COLUMN colaborador_cpf to codigo_interno_colaborador;
ALTER TABLE tb_colaborador_interesse
MODIFY COLUMN codigo_interno_colaborador varchar(36);

ALTER TABLE tb_like_interesse 
RENAME COLUMN colaborador_cpf to codigo_interno_colaborador;
ALTER TABLE tb_like_interesse
MODIFY COLUMN codigo_interno_colaborador varchar(36);

ALTER TABLE tb_apontamento_periodo_fechado 
RENAME COLUMN tb_colaborador_cpf_criacao to codigo_interno_colaborador_criacao;
ALTER TABLE tb_apontamento_periodo_fechado
MODIFY COLUMN codigo_interno_colaborador_criacao varchar(36);

ALTER TABLE tb_apontamento_periodo_fechado 
RENAME COLUMN tb_colaborador_cpf_alteracao to codigo_interno_colaborador_alteracao;
ALTER TABLE tb_apontamento_periodo_fechado
MODIFY COLUMN codigo_interno_colaborador_alteracao varchar(36);

ALTER TABLE tb_apontamento_periodo_fechado_log
RENAME COLUMN tb_colaborador_cpf_criacao to codigo_interno_colaborador_criacao;
ALTER TABLE tb_apontamento_periodo_fechado_log
MODIFY COLUMN codigo_interno_colaborador_criacao varchar(36);

ALTER TABLE tb_apontamento_periodo_fechado_log 
RENAME COLUMN tb_colaborador_cpf_alteracao to codigo_interno_colaborador_alteracao;
ALTER TABLE tb_apontamento_periodo_fechado_log
MODIFY COLUMN codigo_interno_colaborador_alteracao varchar(36);

ALTER TABLE tb_historico_competencia
RENAME COLUMN tb_colaborador_cpf_criacao to codigo_interno_colaborador_criacao;
ALTER TABLE tb_historico_competencia
MODIFY COLUMN codigo_interno_colaborador_criacao varchar(36);

ALTER TABLE tb_historico_competencia 
RENAME COLUMN tb_colaborador_cpf_alteracao to codigo_interno_colaborador_alteracao;
ALTER TABLE tb_historico_competencia
MODIFY COLUMN codigo_interno_colaborador_alteracao varchar(36);

ALTER TABLE tb_usuario_permissao_log 
RENAME COLUMN tb_colaborador_cpf_criacao to codigo_interno_colaborador_criacao;
ALTER TABLE tb_usuario_permissao_log
MODIFY COLUMN codigo_interno_colaborador_criacao varchar(36);

ALTER TABLE tb_candidato 
RENAME COLUMN colaborador_cpf to codigo_interno_colaborador;
ALTER TABLE tb_candidato
MODIFY COLUMN codigo_interno_colaborador varchar(36);


-- Nova coluna CPF
ALTER TABLE tb_colaborador ADD COLUMN documento_colaborador varchar(256);
update tb_colaborador set documento_colaborador = codigo_interno_colaborador where codigo_interno_colaborador is not null;


ALTER TABLE tb_colaborador_apontamento_log DROP FOREIGN KEY tb_colaborador_apontamento_log_ibfk_4;

ALTER TABLE tb_colaborador_alocado DROP FOREIGN KEY fk_tb_colaborador_alocado_tb_colaborador1;
ALTER TABLE tb_colaborador_alocado ADD CONSTRAINT fk_tb_colaborador_alocado_tb_colaborador1 FOREIGN KEY (codigo_interno_colaborador) REFERENCES tb_colaborador(codigo_interno_colaborador) ON DELETE CASCADE ON UPDATE CASCADE;


ALTER TABLE tb_colaborador_org DROP FOREIGN KEY fk_tb_colaborador_org_tb_colaborador1;
ALTER TABLE tb_colaborador_org ADD CONSTRAINT fk_tb_colaborador_org_tb_colaborador1 FOREIGN KEY (codigo_interno_colaborador) REFERENCES tb_colaborador(codigo_interno_colaborador) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE tb_colaborador_periodo_alocacao DROP FOREIGN KEY fk_tb_colaborador_periodo_alocacao_tb_colaborador1;
ALTER TABLE tb_colaborador_periodo_alocacao ADD CONSTRAINT fk_tb_colaborador_periodo_alocacao_tb_colaborador1 FOREIGN KEY (codigo_interno_colaborador) REFERENCES tb_colaborador(codigo_interno_colaborador) ON DELETE CASCADE ON UPDATE CASCADE;

delete from tb_curriculo_colaborador tcc where not exists (select 1 from tb_colaborador tc  where tc.codigo_interno_colaborador = tcc.codigo_interno_colaborador);
ALTER TABLE tb_curriculo_colaborador DROP FOREIGN KEY fk_tb_curriculo_colaborador_tb_colaborador1;
ALTER TABLE tb_curriculo_colaborador ADD CONSTRAINT fk_tb_curriculo_colaborador_tb_colaborador1 FOREIGN KEY (codigo_interno_colaborador) REFERENCES tb_colaborador(codigo_interno_colaborador) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE tb_pessoa_juridica DROP FOREIGN KEY fk_tb_pessoa_juridica_tb_colaborador1;
ALTER TABLE tb_pessoa_juridica ADD CONSTRAINT fk_tb_pessoa_juridica_tb_colaborador1 FOREIGN KEY (codigo_interno_colaborador) REFERENCES tb_colaborador(codigo_interno_colaborador) ON DELETE CASCADE ON UPDATE CASCADE;


ALTER TABLE tb_colaborador DROP FOREIGN KEY fk_tb_colaborador_tb_colaborador_saude1;
update tb_colaborador set colaborador_saude_id = null where colaborador_saude_id not in (select id from tb_colaborador_saude);
ALTER TABLE tb_colaborador ADD CONSTRAINT fk_tb_colaborador_tb_colaborador_saude1 FOREIGN KEY (colaborador_saude_id) REFERENCES tb_colaborador_saude(id) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE tb_colaborador_apontamento DROP FOREIGN KEY tb_colaborador_apontamento_ibfk_1;
ALTER TABLE tb_colaborador_apontamento ADD CONSTRAINT tb_colaborador_apontamento_ibfk_1 FOREIGN KEY (codigo_interno_colaborador) REFERENCES tb_colaborador(codigo_interno_colaborador) ON DELETE CASCADE ON UPDATE CASCADE;



ALTER TABLE tb_notificacao DROP FOREIGN KEY tb_notificacao_ibfk_3;
ALTER TABLE tb_notificacao ADD CONSTRAINT tb_notificacao_ibfk_3 FOREIGN KEY (codigo_interno_colaborador) REFERENCES tb_colaborador(codigo_interno_colaborador) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE tb_parametro_configuracao DROP FOREIGN KEY tb_parametro_configuracao_ibfk_3;
ALTER TABLE tb_parametro_configuracao ADD CONSTRAINT tb_parametro_configuracao_ibfk_3 FOREIGN KEY (codigo_interno_colaborador) REFERENCES tb_colaborador(codigo_interno_colaborador) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE tb_tbd_alocado DROP FOREIGN KEY tb_tbd_alocado_ibfk_2;
ALTER TABLE tb_tbd_alocado ADD CONSTRAINT tb_tbd_alocado_ibfk_2 FOREIGN KEY (codigo_interno_colaborador) REFERENCES tb_colaborador(codigo_interno_colaborador) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE tb_colaborador_sobre DROP FOREIGN KEY fk_colaborador_cpf1;
ALTER TABLE tb_colaborador_sobre ADD CONSTRAINT fk_colaborador_cpf1 FOREIGN KEY (codigo_interno_colaborador) REFERENCES tb_colaborador(codigo_interno_colaborador) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE tb_candidato DROP FOREIGN KEY fk_tb_candidato_tb_colaborador1;
delete  from tb_candidato tca where tca.codigo_interno_colaborador not in (select tc.codigo_interno_colaborador from tb_colaborador tc);
ALTER TABLE tb_candidato ADD CONSTRAINT fk_tb_candidato_tb_colaborador1 FOREIGN KEY (codigo_interno_colaborador) REFERENCES tb_colaborador(codigo_interno_colaborador) ON DELETE CASCADE ON UPDATE CASCADE;



ALTER TABLE tb_colaborador_apontamento ADD CONSTRAINT fk_tb_colaborador_apontamento_tb_colaborador1 FOREIGN KEY (codigo_interno_colaborador_criacao) REFERENCES tb_colaborador(codigo_interno_colaborador) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE tb_colaborador_apontamento ADD CONSTRAINT fk_tb_colaborador_apontamento_tb_colaborador2 FOREIGN KEY (codigo_interno_colaborador_alteracao) REFERENCES tb_colaborador(codigo_interno_colaborador) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE tb_colaborador_apontamento ADD CONSTRAINT fk_tb_colaborador_apontamento_tb_colaborador3 FOREIGN KEY (codigo_interno_colaborador_justificativa) REFERENCES tb_colaborador(codigo_interno_colaborador) ON DELETE CASCADE ON UPDATE CASCADE;



SET FOREIGN_KEY_CHECKS=1;

update tb_colaborador set codigo_interno_colaborador = uuid();

commit;


-- buscacolaborador source




