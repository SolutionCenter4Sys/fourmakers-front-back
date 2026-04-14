CREATE OR REPLACE VIEW estatisticas_escolaridade AS
SELECT 
    COUNT(CASE WHEN `tb_colaborador`.`escolaridade` = 'Mestrado ou Doutorado cursando' OR `tb_colaborador`.`escolaridade` = 'Mestrado ou Doutorado incompleto' THEN 1 END) AS `mestrado_doutorado_incompleto_ou_cursando`,
    COUNT(CASE WHEN `tb_colaborador`.`escolaridade` = 'Mestrado ou Doutorado completo' THEN 1 END) AS `mestrado_doutorado_completo` ,
    COUNT(CASE WHEN `tb_colaborador`.`escolaridade` = 'Ensino fundamental completo ou menos' THEN 1 END) AS `ensino_fundamental_completo_menos`,
    COUNT(CASE WHEN `tb_colaborador`.`escolaridade` = 'Ensino médio completo ou menos' THEN 1 END) AS `ensino_medio_completo_menos`,
    COUNT(CASE WHEN `tb_colaborador`.`escolaridade` = 'Ensino superior incompleto' OR `tb_colaborador`.`escolaridade` = 'Ensino superior cursando' THEN 1 END) AS `ensino_superior_incompleto_ou_cursando`,
    COUNT(CASE WHEN `tb_colaborador`.`escolaridade` = 'Ensino superior completo' THEN 1 END) AS `ensino_superior_completo`,
    COUNT(CASE WHEN `tb_colaborador`.`escolaridade` = 'Pós-graduação incompleto' OR `tb_colaborador`.`escolaridade` = 'Pós-graduação cursando' THEN 1 END) AS `pos_graduacao_incompleto_ou_cursando`,
    COUNT(CASE WHEN `tb_colaborador`.`escolaridade` = 'Pós-graduação completo' THEN 1 END) AS `pos_graduacao_completo`,
    COUNT((CASE WHEN (`tb_colaborador`.`escolaridade` = '' OR `tb_colaborador`.`escolaridade` IS NULL) THEN 1 END)) AS `sem_resposta`,
    COUNT(CASE WHEN `tb_colaborador`.`escolaridade` = "Nenhum" THEN 1 END) AS 'outros'
FROM 
    `tb_colaborador`;