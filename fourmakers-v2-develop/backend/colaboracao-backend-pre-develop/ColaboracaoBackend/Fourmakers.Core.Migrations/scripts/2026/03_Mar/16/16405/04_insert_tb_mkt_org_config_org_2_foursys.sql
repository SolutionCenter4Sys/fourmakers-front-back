-- ------------------------------------------------------------------------------
-- Marketing / Comunicacao - Config org 2: nome autor alternativo Foursys
-- Data: 2026-03-16
-- Pasta: scripts/2026/03_Mar/16/16405
-- ------------------------------------------------------------------------------

INSERT INTO tb_mkt_org_config (tb_org_id, nome_autor_alternativo)
VALUES (2, 'Foursys')
ON DUPLICATE KEY UPDATE nome_autor_alternativo = 'Foursys';
