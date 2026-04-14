UPDATE tb_colaborador_org c
    JOIN tb_modelo_contratacao_org m
ON c.modelo_contratacao = m.codigo_modelo_contratacao
    AND c.tb_org_id = m.tb_org_id
    SET c.codigo_modelo_contratacao = m.codigo_modelo_contratacao;