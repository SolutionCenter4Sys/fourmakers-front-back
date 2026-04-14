UPDATE
    tb_usuario u
JOIN
    tb_colaborador_org c
ON
    u.codigo_interno_colaborador = c.codigo_interno_colaborador
SET
    u.tb_org_id = c.tb_org_id
WHERE
    c.tb_org_id <> 1
    AND u.codigo_interno_colaborador NOT IN (
        SELECT 
            codigo_interno_colaborador
        FROM 
            tb_colaborador_org
        WHERE 
            tb_org_id <> 1
        GROUP BY 
            codigo_interno_colaborador
        HAVING 
            COUNT(*) > 1
    );
	
-- executei assim em dev e hml pra limpar
-- delete from tb_colaborador_org where tb_org_id = 1;

-- em prd utilizei esse script, para preservar os fourmakers raiz
DELETE FROM tb_colaborador_org
WHERE tb_org_id = 1
AND codigo_interno_colaborador IN (
    SELECT codigo_interno_colaborador FROM (
        SELECT codigo_interno_colaborador
        FROM tb_colaborador_org
        GROUP BY codigo_interno_colaborador
        HAVING COUNT(DISTINCT tb_org_id) > 1
    ) AS subquery
);