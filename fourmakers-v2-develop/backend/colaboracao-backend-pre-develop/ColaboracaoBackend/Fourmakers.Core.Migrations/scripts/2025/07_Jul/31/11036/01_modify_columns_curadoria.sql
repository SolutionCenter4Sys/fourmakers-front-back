alter table tb_competencia
    modify column descricao VARCHAR(150) not null;
alter table tb_dominionegocio
    modify column descricao VARCHAR(150) not null;
alter table tb_metodologia
    modify column descricao VARCHAR(150) not null;
alter table tb_idioma
    modify column descricao VARCHAR(150) not null;
alter table tb_softskill
    modify column descricao VARCHAR(150) not null;

UPDATE tb_competencia
SET descricao = CONCAT(descricao, '_X')
WHERE ativo = 0;
UPDATE tb_idioma
SET descricao = CONCAT(descricao, '_X')
WHERE ativo = 0;
UPDATE tb_metodologia
SET descricao = CONCAT(descricao, '_X')
WHERE ativo = 0;
UPDATE tb_dominionegocio
SET descricao = CONCAT(descricao, '_X')
WHERE ativo = 0;
UPDATE tb_softskill
SET descricao = CONCAT(descricao, '_X')
WHERE ativo = 0;

DELETE FROM tb_colaborador_competencia tcc
WHERE NOT EXISTS (
    SELECT 1
    FROM tb_colaborador tc
    WHERE tc.codigo_interno_colaborador = tcc.codigo_interno_colaborador
);
DELETE FROM tb_colaborador_idioma tcc
WHERE NOT EXISTS (
    SELECT 1
    FROM tb_colaborador tc
    WHERE tc.codigo_interno_colaborador = tcc.codigo_interno_colaborador
);
DELETE FROM tb_colaborador_metodologia tcc
WHERE NOT EXISTS (
    SELECT 1
    FROM tb_colaborador tc
    WHERE tc.codigo_interno_colaborador = tcc.codigo_interno_colaborador
);
DELETE FROM tb_colaborador_dominionegocio tcc
WHERE NOT EXISTS (
    SELECT 1
    FROM tb_colaborador tc
    WHERE tc.codigo_interno_colaborador = tcc.codigo_interno_colaborador
);
DELETE FROM tb_colaborador_softskill tcc
WHERE NOT EXISTS (
    SELECT 1
    FROM tb_colaborador tc
    WHERE tc.codigo_interno_colaborador = tcc.codigo_interno_colaborador
);