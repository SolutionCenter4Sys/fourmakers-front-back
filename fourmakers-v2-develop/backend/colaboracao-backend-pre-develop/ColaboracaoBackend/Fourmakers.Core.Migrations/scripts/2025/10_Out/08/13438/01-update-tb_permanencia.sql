UPDATE tb_permanencia
SET descricao = REPLACE(descricao, 'At?', 'Até')
WHERE descricao LIKE '%At?%';

