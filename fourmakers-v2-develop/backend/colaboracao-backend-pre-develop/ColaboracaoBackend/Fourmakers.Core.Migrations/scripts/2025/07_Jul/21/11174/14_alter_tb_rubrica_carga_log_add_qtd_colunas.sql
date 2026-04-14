-- Renomear coluna quantidade_registros_processados para qtd_registros_processados_sucesso
ALTER TABLE tb_rubrica_carga_log 
CHANGE COLUMN quantidade_registros_processados qtd_registros_processados_sucesso INT DEFAULT 0 COMMENT 'Quantidade de registros processados com sucesso';

-- Adicionar nova coluna qtd_registros_retornados
ALTER TABLE tb_rubrica_carga_log 
ADD COLUMN qtd_registros_retornados INT DEFAULT 0 COMMENT 'Quantidade total de registros retornados da API HuggingFace';