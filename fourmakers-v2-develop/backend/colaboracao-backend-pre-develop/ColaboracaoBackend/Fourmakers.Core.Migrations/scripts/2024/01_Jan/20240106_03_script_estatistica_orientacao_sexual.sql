CREATE OR REPLACE VIEW estatisticas_orientacao_sexual AS
SELECT
    COUNT((CASE WHEN (`tb_colaborador`.`orientacao_sexual` = 'Assexual') THEN 1 END)) AS `assexual`,
    COUNT((CASE WHEN (`tb_colaborador`.`orientacao_sexual` = 'Bissexual') THEN 1 END)) AS `bissexual`,
    COUNT((CASE WHEN (`tb_colaborador`.`orientacao_sexual` = 'Heterossexual') THEN 1 END)) AS `heterossexual`,
    COUNT((CASE WHEN (`tb_colaborador`.`orientacao_sexual` = 'Homossexual') THEN 1 END)) AS `homossexual`,
    COUNT((CASE WHEN (`tb_colaborador`.`orientacao_sexual` = 'Outras') THEN 1 END)) AS `outras`,
    COUNT((CASE WHEN (`tb_colaborador`.`orientacao_sexual` = 'Prefiro não responder' OR `tb_colaborador`.`orientacao_sexual` IS NULL) THEN 1 END)) AS `sem_resposta`
FROM `tb_colaborador`;