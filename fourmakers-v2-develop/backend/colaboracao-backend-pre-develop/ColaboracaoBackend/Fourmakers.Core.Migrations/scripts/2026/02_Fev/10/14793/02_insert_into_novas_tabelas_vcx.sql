INSERT INTO tb_vcx_impactos 
(id, descricao)
VALUES 
(UUID(), "Impacto Baixo"),
(UUID(), "Impacto Médio"),
(UUID(), "Impacto Alto");

INSERT INTO tb_vcx_urgencias 
(id, descricao)
VALUES 
(UUID(), "Urgência Baixa"),
(UUID(), "Urgência Média"),
(UUID(), "Urgência Alta");

INSERT INTO tb_vcx_status 
(id, descricao)
VALUES 
(UUID(), "Em Planejamento"),
(UUID(), "Ativa"),
(UUID(), "Pausada"),
(UUID(), "Concluída");