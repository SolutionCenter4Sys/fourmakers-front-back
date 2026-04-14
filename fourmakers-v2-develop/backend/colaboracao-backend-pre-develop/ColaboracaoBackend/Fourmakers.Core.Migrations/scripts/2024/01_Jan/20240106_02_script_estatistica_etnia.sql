CREATE OR REPLACE VIEW estatisticas_etnia AS
SELECT 
    COUNT((CASE WHEN (`tb_colaborador`.`etnia` = 'preta') THEN 1 END)) AS `preta`,
    COUNT((CASE WHEN (`tb_colaborador`.`etnia` = 'branca') THEN 1 END)) AS `branca`,
    COUNT((CASE WHEN (`tb_colaborador`.`etnia` = 'amarela') THEN 1 END)) AS `amarela`,
    COUNT((CASE WHEN (`tb_colaborador`.`etnia` = 'parda') THEN 1 END)) AS `parda`,
    COUNT((CASE WHEN (`tb_colaborador`.`etnia` = 'prefiro não responder') THEN 1 END)) AS `prefiro_nao_responder`,
    COUNT((CASE WHEN (`tb_colaborador`.`etnia` = 'indígena') THEN 1 END)) AS `indigena`,
    COUNT((CASE WHEN (`tb_colaborador`.`etnia` is null) THEN 1 END)) AS `sem_resposta`
FROM `tb_colaborador`;