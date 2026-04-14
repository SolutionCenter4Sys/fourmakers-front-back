alter table tb_nivel add column prioridade_unificacao int;

-- Atualização para tb_item_perfil_id = 1
UPDATE tb_nivel
SET prioridade_unificacao = CASE
    WHEN descricao = 'Sênior' THEN 1
    WHEN descricao = 'Pleno' THEN 2
    WHEN descricao = 'Júnior' THEN 3
    WHEN descricao = 'Trainee' THEN 4
    WHEN descricao = 'A definir' THEN 5
    ELSE 6
END
WHERE tb_item_perfil_id = 1;

-- Atualização para tb_item_perfil_id = 2
UPDATE tb_nivel
SET prioridade_unificacao = CASE
    WHEN descricao = 'Doutorado' THEN 1
    WHEN descricao = 'Pós Graduação' THEN 2
    WHEN descricao = 'MBA' THEN 3
    WHEN descricao = 'Graduação' THEN 4
    WHEN descricao = 'Ensino Médio' THEN 5
    WHEN descricao = 'A definir' THEN 6
    ELSE 7
END
WHERE tb_item_perfil_id = 2;

-- Atualização para tb_item_perfil_id = 3
UPDATE tb_nivel
SET prioridade_unificacao = CASE
    WHEN descricao = 'Alto' THEN 1
    WHEN descricao = 'Médio' THEN 2
    WHEN descricao = 'Baixo' THEN 3
    WHEN descricao = 'Certificado' THEN 4
    WHEN descricao = 'A definir' THEN 5
    ELSE 6
END
WHERE tb_item_perfil_id = 3;

-- Atualização para tb_item_perfil_id = 4
UPDATE tb_nivel
SET prioridade_unificacao = CASE
    WHEN descricao = 'Alto' THEN 1
    WHEN descricao = 'Médio' THEN 2
    WHEN descricao = 'Baixo' THEN 3
    WHEN descricao = 'A definir' THEN 4
    ELSE 5
END
WHERE tb_item_perfil_id = 4;

-- Atualização para tb_item_perfil_id = 5
UPDATE tb_nivel
SET prioridade_unificacao = CASE
    WHEN descricao = 'Alto' THEN 1
    WHEN descricao = 'Médio' THEN 2
    WHEN descricao = 'Baixo' THEN 3
    WHEN descricao = 'Certificado' THEN 4
    ELSE 5
END
WHERE tb_item_perfil_id = 5;

-- Atualização para tb_item_perfil_id = 8
UPDATE tb_nivel
SET prioridade_unificacao = CASE
    WHEN descricao = 'Desenvolvido' THEN 1
    WHEN descricao = 'Em desenvolvimento' THEN 2
    WHEN descricao = 'A desenvolver' THEN 3
    WHEN descricao = 'A definir' THEN 4
    ELSE 5
END
WHERE tb_item_perfil_id = 8;

-- Atualização para tb_item_perfil_id = 9
UPDATE tb_nivel
SET prioridade_unificacao = CASE
    WHEN descricao = 'Nativo' THEN 1
    WHEN descricao = 'Fluente' THEN 2
    WHEN descricao = 'Avançado' THEN 3
    WHEN descricao = 'Intermediário' THEN 4
    WHEN descricao = 'Básico' THEN 5
    WHEN descricao = 'A definir' THEN 6
    ELSE 7
END
WHERE tb_item_perfil_id = 9;
