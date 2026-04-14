ALTER TABLE tb_colaborador_org
ADD COLUMN modelo_contratacao VARCHAR(255),
ADD COLUMN empresa_relacionada VARCHAR(255),
ADD COLUMN modelo_trabalho VARCHAR(255),
ADD COLUMN dias_por_semana INT,
ADD COLUMN valor_hora DECIMAL(10,2),
ADD COLUMN custo_hora DECIMAL(10,2),
ADD COLUMN base_hora_mes INT;

ALTER TABLE tb_colaborador
ADD COLUMN contato_principal_ddi VARCHAR(4) AFTER data_alteracao;