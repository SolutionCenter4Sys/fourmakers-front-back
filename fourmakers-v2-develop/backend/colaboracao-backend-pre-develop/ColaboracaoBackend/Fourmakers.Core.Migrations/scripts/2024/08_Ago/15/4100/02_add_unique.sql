ALTER TABLE tb_atividade
ADD UNIQUE KEY unique_descricao_org (descricao, tb_org_id);