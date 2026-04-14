INSERT INTO tb_status_app (id, descricao) VALUES
    (0, 'Pendente'),
    (1, 'Aceito'),
    (2, 'Recusado')
ON DUPLICATE KEY UPDATE descricao = VALUES(descricao);

