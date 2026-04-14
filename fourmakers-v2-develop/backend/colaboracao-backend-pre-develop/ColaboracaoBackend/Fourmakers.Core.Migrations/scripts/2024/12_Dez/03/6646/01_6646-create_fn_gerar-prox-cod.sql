DELIMITER //
CREATE DEFINER=`admin`@`%` FUNCTION `fn_gestor_externo_gerar_prox_codigo`(org_id INT) RETURNS varchar(20) CHARSET utf8mb3
    DETERMINISTIC
BEGIN
    DECLARE ano_atual VARCHAR(2);
    DECLARE org_id_formatado VARCHAR(3);
    DECLARE proximo_codigo VARCHAR(20);

    -- Obter o ano atual com dois dígitos
    SET ano_atual = DATE_FORMAT(CURDATE(), '%y');

    -- Formatar o org_id para ter 3 dígitos
    SET org_id_formatado = LPAD(org_id, 3, '0');

    -- Gerar o próximo código
    SET proximo_codigo = CONCAT(
        'F',
        ano_atual,
        org_id_formatado,
        LPAD(
            COALESCE(
                (
                    SELECT 
                        MAX(CAST(SUBSTRING(cod_gestor_externo, 8) AS UNSIGNED))
                    FROM 
                        tb_gestor_externo
                    WHERE 
                        cod_gestor_externo LIKE CONCAT('F', ano_atual, org_id_formatado, '%')
                ), 
                0
            ) + 1,
            5,
            '0'
        )
    );

    RETURN proximo_codigo;
END//
DELIMITER ;