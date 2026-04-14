DROP FUNCTION IF EXISTS `fn_get_dia_semana`;
DELIMITER $$
CREATE FUNCTION fn_get_dia_semana(data DATE) RETURNS VARCHAR(3)
DETERMINISTIC
READS SQL DATA
BEGIN
    DECLARE dia_abreviado VARCHAR(3);

    SET dia_abreviado = CASE DAYOFWEEK(data)
        WHEN 1 THEN 'Dom'
        WHEN 2 THEN 'Seg'
        WHEN 3 THEN 'Ter'
        WHEN 4 THEN 'Qua'
        WHEN 5 THEN 'Qui'
        WHEN 6 THEN 'Sex'
        WHEN 7 THEN 'Sáb'
    END;

    RETURN dia_abreviado;
END$$

DELIMITER ;