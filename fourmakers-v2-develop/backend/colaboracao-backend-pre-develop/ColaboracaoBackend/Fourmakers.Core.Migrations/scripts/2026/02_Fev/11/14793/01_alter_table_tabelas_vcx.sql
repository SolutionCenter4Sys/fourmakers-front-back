-- 1 Alterar a coluna
ALTER TABLE tb_vcx_iniciativas_log
MODIFY COLUMN id CHAR(36) NOT NULL;

-- 2 Remover a FK
ALTER TABLE tb_vcx_iniciativas_log
DROP FOREIGN KEY fk_tb_vcx_iniciativas_log_tb_organograma_posicao;

-- 3 Alterar a coluna
ALTER TABLE tb_vcx_iniciativas_log
MODIFY COLUMN tb_organograma_posicao_id CHAR(36) NOT NULL;

-- 4 Recriar a FK
ALTER TABLE tb_vcx_iniciativas_log
ADD CONSTRAINT fk_tb_vcx_iniciativas_log_tb_organograma_posicao
FOREIGN KEY (tb_organograma_posicao_id)
REFERENCES tb_organograma_posicao (id)
ON DELETE CASCADE
ON UPDATE CASCADE;