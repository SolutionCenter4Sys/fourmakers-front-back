CREATE OR REPLACE VIEW estatisticas_genero AS
SELECT 
    COUNT((CASE WHEN (`tb_colaborador`.`genero` = 'Feminino') THEN 1 END)) AS `feminino`,
    COUNT((CASE WHEN (`tb_colaborador`.`genero` = 'Masculino') THEN 1 END)) AS `masculino`,
    COUNT((CASE WHEN (`tb_colaborador`.`genero` = 'Não binário') THEN 1 END)) AS `nao_binario`,
    COUNT((CASE WHEN (`tb_colaborador`.`genero` = 'Homem cisgênero') THEN 1 END)) AS `homem_cisgenero`,
    COUNT((CASE WHEN (`tb_colaborador`.`genero` = 'Agênero') THEN 1 END)) AS `Agenero`,
    COUNT((CASE WHEN (`tb_colaborador`.`genero` = 'Transgênero') THEN 1 END)) AS `transgenero`,
    COUNT((CASE WHEN (`tb_colaborador`.`genero` = 'Mulher cisgênero') THEN 1 END)) AS `mulher_cisgenero`,
    COUNT((CASE WHEN (`tb_colaborador`.`genero` = 'Prefiro não responder') THEN 1 END)) AS `prefiro_nao_responder`,
    COUNT((CASE WHEN (`tb_colaborador`.`genero` is null ) THEN 1 END)) AS `sem_resposta`
FROM `tb_colaborador`;