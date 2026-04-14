ALTER TABLE tb_cnab_remessa
ADD COLUMN competencia VARCHAR(7) NULL AFTER codigo_diretoria;

INSERT INTO tb_cnab_org
(id, tb_org_id, diretoria, codigo_banco, agencia, agencia_dv, conta, conta_dv, codigo_convenio, cnpj_empresa, forma_pagamento, data_criacao, data_atualizacao, nome_empresa, nome_banco, endereco_empresa, numero_local, complemento_endereco, cidade, cep, estado)
VALUES('4e77d64a-ebf5-11f0-8c05-12248fda2329', 2, NULL, '341', '4807', NULL, '03004', '6', NULL, '03808125000130', 'PIX', '2026-01-07 18:18:46', '2026-01-07 18:18:46', 'FOURSYS PROJETOS E SISTEMAS EM', 'BANCO ITAU S/A', 'AV COPACABANA', '00190', NULL, 'BARUERI', '00006472', 'SP');