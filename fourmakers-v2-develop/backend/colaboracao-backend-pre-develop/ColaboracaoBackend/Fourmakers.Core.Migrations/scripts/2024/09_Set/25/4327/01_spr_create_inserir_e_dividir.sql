DELIMITER //
CREATE PROCEDURE `spr_fn_inserir_string_com_split`(
    IN p_lista VARCHAR(1000),
    IN p_nome_tabela VARCHAR(255),
    IN p_nome_coluna VARCHAR(255),
    IN p_caracter_para_dividir VARCHAR(1)
)
BEGIN
    DECLARE current_item VARCHAR(255);
    DECLARE split_index INT DEFAULT 1;
    
    -- Loop para dividir a lista e inserir na tabela
    WHILE CHAR_LENGTH(p_lista) > 0 DO
        SET split_index = LOCATE(p_caracter_para_dividir, p_lista);
        
        IF split_index = 0 THEN
            SET current_item = p_lista;
            SET p_lista = '';
        ELSE
            SET current_item = SUBSTRING(p_lista, 1, split_index - 1);
            SET p_lista = SUBSTRING(p_lista, split_index + 1);
        END IF;
        
        -- Construir e executar a query dinâmica para inserir na tabela temporária
        SET @insert_query = CONCAT('INSERT INTO ', p_nome_tabela, ' (', p_nome_coluna, ') VALUES (', QUOTE(current_item), ')');
        PREPARE stmt FROM @insert_query;
        EXECUTE stmt;
        DEALLOCATE PREPARE stmt;
    END WHILE;
    
END
//
DELIMITER ;