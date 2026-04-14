UPDATE tb_permanencia
SET descricao = CASE 
                   WHEN descricao = '1 a 3 anos' THEN 'Até 2 anos'
                   WHEN descricao = 'Mais de 5 anos' THEN 'Até 3 anos'
                   ELSE descricao
                END
WHERE descricao IN ('1 a 3 anos', 'Mais de 5 anos');

