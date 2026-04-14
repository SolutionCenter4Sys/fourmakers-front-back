ALTER TABLE tb_indicacao_premiada_parcial
ADD COLUMN codigo_colaborador_interno_analista VARCHAR(36),
ADD COLUMN cod_diretoria VARCHAR(255),
ADD COLUMN cv_valido TINYINT(1) DEFAULT 0,
ADD COLUMN retorno_ao_profissional TINYINT(1) DEFAULT 0,
ADD COLUMN perfil_avaliado VARCHAR(1000),
ADD COLUMN status VARCHAR(1000);