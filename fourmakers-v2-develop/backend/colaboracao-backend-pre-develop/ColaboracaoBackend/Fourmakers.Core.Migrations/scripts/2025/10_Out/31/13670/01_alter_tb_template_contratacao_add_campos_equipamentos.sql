ALTER TABLE tb_template_contratacao
ADD COLUMN celular TINYINT NOT NULL DEFAULT 0 COMMENT 'Indica se terá celular',
ADD COLUMN plano_dados TINYINT NOT NULL DEFAULT 0 COMMENT 'Indica se terá plano de dados',
ADD COLUMN quantidade_minutos_plano_dados DOUBLE NOT NULL DEFAULT 0 COMMENT 'Quantidade de minutos do plano de dados',
ADD COLUMN cartao_visitas TINYINT NOT NULL DEFAULT 0 COMMENT 'Indica se terá cartão de visitas',
ADD COLUMN quantidade_cartao_visitas TINYINT NOT NULL DEFAULT 0 COMMENT 'Quantidade de cartões de visitas',
ADD COLUMN outros_equipamentos TEXT NULL COMMENT 'Descrição de outros equipamentos';

