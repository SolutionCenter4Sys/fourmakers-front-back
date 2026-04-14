ALTER TABLE tb_colaborador_grupo_acesso_configuracao 
ADD acao VARCHAR(50) NOT NULL AFTER chave;

TRUNCATE TABLE tb_colaborador_grupo_acesso_configuracao;

UPDATE tb_colaborador_org
SET codigo_modelo_contratacao = 'Parceiro (PJ)'
WHERE codigo_modelo_contratacao IN ('Prestador (PJ)', 'PJ');

DELETE FROM tb_modelo_contratacao_org
WHERE codigo_modelo_contratacao IN ('Prestador (PJ)', 'PJ');




INSERT INTO tb_colaborador_grupo_acesso_configuracao
(tabela, coluna, condicao, chave, tb_grupo_acesso_id, acao, tb_org_id)
VALUES('tb_colaborador_org', 'modelo_contratacao', 'tb_org_id,codigo_interno_colaborador',
       'Parceiro (PJ)', (SELECT id FROM tb_grupo_acesso WHERE descricao = 'PRESTADOR' AND tb_org_id = 9), 'adicionar', 9);

INSERT INTO tb_colaborador_grupo_acesso_configuracao
(tabela, coluna, condicao, chave, tb_grupo_acesso_id, acao, tb_org_id)
VALUES('tb_colaborador_org', 'modelo_contratacao', 'tb_org_id,codigo_interno_colaborador',
       'Parceiro Terceiro (PJ)', (SELECT id FROM tb_grupo_acesso WHERE descricao = 'PRESTADOR' AND tb_org_id = 9), 'adicionar', 9);

INSERT INTO tb_colaborador_grupo_acesso_configuracao
(tabela, coluna, condicao, chave, tb_grupo_acesso_id, acao, tb_org_id)
VALUES('tb_colaborador_org', 'modelo_contratacao', 'tb_org_id,codigo_interno_colaborador',
       'CLT', (SELECT id FROM tb_grupo_acesso WHERE descricao = 'PRESTADOR' AND tb_org_id = 9), 'remover', 9);

INSERT INTO tb_colaborador_grupo_acesso_configuracao
(tabela, coluna, condicao, chave, tb_grupo_acesso_id, acao, tb_org_id)
VALUES('tb_colaborador_org', 'modelo_contratacao', 'tb_org_id,codigo_interno_colaborador',
       'Cooperado', (SELECT id FROM tb_grupo_acesso WHERE descricao = 'PRESTADOR' AND tb_org_id = 9), 'remover', 9);

INSERT INTO tb_colaborador_grupo_acesso_configuracao
(tabela, coluna, condicao, chave, tb_grupo_acesso_id, acao, tb_org_id)
VALUES('tb_colaborador_org', 'modelo_contratacao', 'tb_org_id,codigo_interno_colaborador',
       'Estagiário', (SELECT id FROM tb_grupo_acesso WHERE descricao = 'PRESTADOR' AND tb_org_id = 9), 'remover', 9);

INSERT INTO tb_colaborador_grupo_acesso_configuracao
(tabela, coluna, condicao, chave, tb_grupo_acesso_id, acao, tb_org_id)
VALUES('tb_colaborador_org', 'modelo_contratacao', 'tb_org_id,codigo_interno_colaborador',
       'Jovem aprendiz', (SELECT id FROM tb_grupo_acesso WHERE descricao = 'PRESTADOR' AND tb_org_id = 9), 'remover', 9);

INSERT INTO tb_colaborador_grupo_acesso_configuracao
(tabela, coluna, condicao, chave, tb_grupo_acesso_id, acao, tb_org_id)
VALUES('tb_colaborador_org', 'modelo_contratacao', 'tb_org_id,codigo_interno_colaborador',
       '-', (SELECT id FROM tb_grupo_acesso WHERE descricao = 'PRESTADOR' AND tb_org_id = 9), 'remover', 9);

INSERT INTO tb_colaborador_grupo_acesso_configuracao
(tabela, coluna, condicao, chave, tb_grupo_acesso_id, acao, tb_org_id)
VALUES('tb_colaborador_org', 'modelo_contratacao', 'tb_org_id,codigo_interno_colaborador',
       '', (SELECT id FROM tb_grupo_acesso WHERE descricao = 'PRESTADOR' AND tb_org_id = 9), 'remover', 9);



INSERT INTO tb_colaborador_grupo_acesso_configuracao
(tabela, coluna, condicao, chave, tb_grupo_acesso_id, acao, tb_org_id)
VALUES('tb_colaborador_org', 'modelo_contratacao', 'tb_org_id,codigo_interno_colaborador',
       'Parceiro (PJ)', (SELECT id FROM tb_grupo_acesso WHERE descricao = 'COLABORADOR CLT' AND tb_org_id = 9), 'remover', 9);

INSERT INTO tb_colaborador_grupo_acesso_configuracao
(tabela, coluna, condicao, chave, tb_grupo_acesso_id, acao, tb_org_id)
VALUES('tb_colaborador_org', 'modelo_contratacao', 'tb_org_id,codigo_interno_colaborador',
       'Parceiro Terceiro (PJ)', (SELECT id FROM tb_grupo_acesso WHERE descricao = 'COLABORADOR CLT' AND tb_org_id = 9), 'remover', 9);

INSERT INTO tb_colaborador_grupo_acesso_configuracao
(tabela, coluna, condicao, chave, tb_grupo_acesso_id, acao, tb_org_id)
VALUES('tb_colaborador_org', 'modelo_contratacao', 'tb_org_id,codigo_interno_colaborador',
       'CLT', (SELECT id FROM tb_grupo_acesso WHERE descricao = 'COLABORADOR CLT' AND tb_org_id = 9), 'adicionar', 9);

INSERT INTO tb_colaborador_grupo_acesso_configuracao
(tabela, coluna, condicao, chave, tb_grupo_acesso_id, acao, tb_org_id)
VALUES('tb_colaborador_org', 'modelo_contratacao', 'tb_org_id,codigo_interno_colaborador',
       'Cooperado', (SELECT id FROM tb_grupo_acesso WHERE descricao = 'COLABORADOR CLT' AND tb_org_id = 9), 'adicionar', 9);

INSERT INTO tb_colaborador_grupo_acesso_configuracao
(tabela, coluna, condicao, chave, tb_grupo_acesso_id, acao, tb_org_id)
VALUES('tb_colaborador_org', 'modelo_contratacao', 'tb_org_id,codigo_interno_colaborador',
       'Estagiário', (SELECT id FROM tb_grupo_acesso WHERE descricao = 'COLABORADOR CLT' AND tb_org_id = 9), 'adicionar', 9);

INSERT INTO tb_colaborador_grupo_acesso_configuracao
(tabela, coluna, condicao, chave, tb_grupo_acesso_id, acao, tb_org_id)
VALUES('tb_colaborador_org', 'modelo_contratacao', 'tb_org_id,codigo_interno_colaborador',
       'Jovem aprendiz', (SELECT id FROM tb_grupo_acesso WHERE descricao = 'COLABORADOR CLT' AND tb_org_id = 9), 'adicionar', 9);

INSERT INTO tb_colaborador_grupo_acesso_configuracao
(tabela, coluna, condicao, chave, tb_grupo_acesso_id, acao, tb_org_id)
VALUES('tb_colaborador_org', 'modelo_contratacao', 'tb_org_id,codigo_interno_colaborador',
       '-', (SELECT id FROM tb_grupo_acesso WHERE descricao = 'COLABORADOR CLT' AND tb_org_id = 9), 'remover', 9);

INSERT INTO tb_colaborador_grupo_acesso_configuracao
(tabela, coluna, condicao, chave, tb_grupo_acesso_id, acao, tb_org_id)
VALUES('tb_colaborador_org', 'modelo_contratacao', 'tb_org_id,codigo_interno_colaborador',
       '', (SELECT id FROM tb_grupo_acesso WHERE descricao = 'COLABORADOR CLT' AND tb_org_id = 9), 'remover', 9);