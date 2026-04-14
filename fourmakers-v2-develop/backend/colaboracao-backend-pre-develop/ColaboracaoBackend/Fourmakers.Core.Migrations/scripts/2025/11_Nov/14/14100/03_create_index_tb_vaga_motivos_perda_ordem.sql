-- Índice para melhorar performance em consultas ordenadas por ordem
CREATE INDEX idx_tb_vaga_motivos_perda_ordem ON tb_vaga_motivos_perda(ordem);

