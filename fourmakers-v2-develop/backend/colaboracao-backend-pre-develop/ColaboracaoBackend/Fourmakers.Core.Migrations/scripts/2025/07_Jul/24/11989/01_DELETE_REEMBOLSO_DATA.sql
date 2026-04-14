DELETE FROM tb_extrato_pagamento WHERE tb_org_id IN (2, 5, 8);
DELETE FROM tb_solicitacao_pagamento WHERE tb_org_id IN (2, 5, 8);
DELETE FROM tb_solicitacao_reembolso WHERE tb_org_id IN (2, 5, 8);
DELETE FROM tb_verba_personalizada WHERE tb_org_id IN (2, 5, 8);
DELETE FROM tb_verba WHERE tb_org_id IN (2, 5, 8);
DELETE FROM tb_parametro_reembolso WHERE tb_org_id IN (2, 5, 8);
DELETE FROM tb_verba_tipo WHERE tb_org_id IN (2, 5, 8);
DELETE FROM tb_reembolso_saldo_colaborador WHERE tb_org_id IN (2, 5, 8);
