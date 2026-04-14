SELECT CONCAT(
    'ALTER TABLE ', TABLE_NAME,
    ' DROP FOREIGN KEY ', CONSTRAINT_NAME, '; ',
    'ALTER TABLE ', TABLE_NAME,
    ' ADD CONSTRAINT ', CONSTRAINT_NAME, 
    ' FOREIGN KEY (', COLUMN_NAME, ') ',
    'REFERENCES tb_colaborador(codigo_interno_colaborador);'
) AS comando
FROM
    information_schema.KEY_COLUMN_USAGE
WHERE
    REFERENCED_TABLE_NAME = 'tb_usuario'
    AND REFERENCED_COLUMN_NAME = 'codigo_interno_colaborador'
    AND TABLE_SCHEMA = 'gcolb_hml ou gcolb_prd';
    
ALTER TABLE tb_usuario DROP FOREIGN KEY fk_usuario_colaborador;
ALTER TABLE tb_usuario DROP INDEX cpf_UNIQUE;
		
SELECT
    CONSTRAINT_NAME, TABLE_NAME, COLUMN_NAME, REFERENCED_TABLE_NAME, REFERENCED_COLUMN_NAME
FROM
    information_schema.KEY_COLUMN_USAGE
WHERE
    REFERENCED_TABLE_NAME = 'tb_usuario'
    AND TABLE_SCHEMA = 'gcolb_hml ou gcolb_prd';

   
   
SELECT  DISTINCT
            codigo_interno_colaborador, GROUP_CONCAT(tb_org_id, ',') as tb_orgs_id
        FROM 
            tb_colaborador_org
        WHERE 
            tb_org_id <> 1
        GROUP BY 
            codigo_interno_colaborador
        HAVING 
            COUNT(*) > 1
   
-- aqui foi feito manualmente a duplicação dos usuário em dev e hml, pois são poucos casos (apenas 2)
-- em PRD não temos casos de duplicação
-- select * from tb_usuario tu where codigo_interno_colaborador = '2cbe5478-538c-11ef-9eb3-0e1e12942759' -- 5,6
-- select * from tb_usuario tu where codigo_interno_colaborador = '306b3cb4-538c-11ef-9eb3-0e1e12942759' -- 5,6

ALTER TABLE tb_usuario
ADD CONSTRAINT fk_tb_usuario_colaborador
FOREIGN KEY (codigo_interno_colaborador)
REFERENCES tb_colaborador(codigo_interno_colaborador)
ON DELETE CASCADE
ON UPDATE CASCADE;


CREATE UNIQUE INDEX idx_codigo_interno_colaborador_org_id ON tb_usuario (codigo_interno_colaborador,tb_org_id);