INSERT INTO tb_admissao_remuneracao_clt (
    id, 
    tb_admissao_cargo_id, 
    tb_cbo_id, 
    faixa1_inicio, faixa1_final, 
    faixa2_inicio, faixa2_final, 
    faixa3_inicio, faixa3_final, 
    faixa4_inicio, faixa4_final, 
    piso, 
    ativo
)
SELECT 
    UUID(),          -- Gera um ID novo para a remuneração
    c.id,            -- Pega o ID que já existe na tb_admissao_cargo
    c.tb_cbo_id,     -- Pega o CBO vinculado a esse cargo
    0.00, 0.00,      -- Faixa 1 zerada
    0.00, 0.00,      -- Faixa 2 zerada
    0.00, 0.00,      -- Faixa 3 zerada
    0.00, 0.00,      -- Faixa 4 zerada
    0.00,            -- Piso zerado
    1                -- Ativo
FROM tb_admissao_cargo c
WHERE NOT EXISTS (
    SELECT 1 FROM tb_admissao_remuneracao_clt r 
    WHERE r.tb_admissao_cargo_id = c.id
) 
AND c.tb_cbo_id IS NOT NULL;