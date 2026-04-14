begin;
DELETE from `gcolb_prd`.`tb_like_cargo`;
DELETE from `gcolb_prd`.`tb_like_competencia`;
DELETE from `gcolb_prd`.`tb_like_dominionegocio`;
DELETE from `gcolb_prd`.`tb_like_formacao`;
DELETE from `gcolb_prd`.`tb_like_hobbies`;
DELETE from `gcolb_prd`.`tb_like_interesse`;
DELETE from `gcolb_prd`.`tb_like_metodologia`;
DELETE from `gcolb_prd`.`tb_like_modeloreferencia`;
DELETE from `gcolb_prd`.`tb_endosso_competencia`;
DELETE from `gcolb_prd`.`tb_endosso_dominionegocio`;
DELETE from `gcolb_prd`.`tb_endosso_formacao`;
DELETE from `gcolb_prd`.`tb_endosso_metodologia`;
DELETE from `gcolb_prd`.`tb_endosso_modeloreferencia`;
DELETE from `gcolb_prd`.`tb_contato_colaborador`;
DELETE from `gcolb_prd`.`tb_colaborador_status`;
DELETE from gcolb_prd.tb_colaborador_modeloreferencia;
DELETE from gcolb_prd.tb_colaborador_metodologia;
DELETE from gcolb_prd.tb_colaborador_interesse;
DELETE from gcolb_prd.tb_colaborador_hobbies;
DELETE from gcolb_prd.tb_colaborador_graugraduacao;
DELETE from gcolb_prd.tb_colaborador_formacao;
DELETE from gcolb_prd.tb_colaborador_cargo;
DELETE from gcolb_prd.tb_colaborador_comentario;
DELETE from gcolb_prd.tb_colaborador_competencia;
DELETE from gcolb_prd.tb_colaborador_competencia_certificado;
DELETE from gcolb_prd.tb_colaborador_dominionegocio;
DELETE from gcolb_prd.tb_certificado;
DELETE from gcolb_prd.tb_usuario_tokenacesso;
DELETE from gcolb_prd.tb_usuario_grupo_acesso;
DELETE from gcolb_prd.tb_usuario_acs;
INSERT IGNORE INTO gcolb_prd.tb_colaborador
    (cpf, nome_completo, data_nascimento, rg, matricula, admissao, diretoria_id, candidato)
VALUES
    ('00000000000', 'Colaborador Fixo Sistema', '0001-01-01T00:00:00', '000000', null, '0001-01-01T00:00:00', 2, 0);
INSERT IGNORE INTO gcolb_prd.tb_usuario
    (id, cpf, email, password, primeiroAcesso)
VALUES
    (-1, '00000000000', 'appcolaborador@foursys.com.br', '', 0);
UPDATE gcolb_prd.tb_competencia set usuario_criacao_id = -1;
UPDATE gcolb_prd.tb_formacao set usuario_criacao_id = -1;
UPDATE gcolb_prd.tb_dominionegocio set usuario_criacao_id = -1;
UPDATE gcolb_prd.tb_modeloreferencia set usuario_criacao_id = -1;
UPDATE gcolb_prd.tb_metodologia set usuario_criacao_id = -1;
UPDATE gcolb_prd.tb_hobbies set usuario_criacao_id = -1;
UPDATE gcolb_prd.tb_interesse set usuario_criacao_id = -1;
UPDATE gcolb_prd.tb_graugraduacao set usuario_criacao_id = -1;
DELETE from gcolb_prd.tb_usuario where cpf <> '00000000000';
DELETE from gcolb_prd.tb_token_acesso;
DELETE from gcolb_prd.tb_candidato;
DELETE from gcolb_prd.tb_colaborador where cpf <> '00000000000';
commit;