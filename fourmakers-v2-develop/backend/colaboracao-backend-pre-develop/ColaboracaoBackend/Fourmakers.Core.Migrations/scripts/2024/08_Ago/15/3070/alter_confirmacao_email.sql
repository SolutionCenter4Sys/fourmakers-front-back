ALTER TABLE tb_confirmacao_email
RENAME COLUMN cpf to codigo_interno_colaborador;
ALTER TABLE tb_confirmacao_email
MODIFY COLUMN codigo_interno_colaborador varchar(36);