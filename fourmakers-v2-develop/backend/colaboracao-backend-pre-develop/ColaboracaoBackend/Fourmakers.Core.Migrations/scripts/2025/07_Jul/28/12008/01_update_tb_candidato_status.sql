UPDATE tb_candidato_status
SET descricao = 'Inscrição registrada'
WHERE id = 1 AND origem = 'Fourmakers';

UPDATE tb_candidato_status
SET descricao = 'Entrevista Inicial'
WHERE id = 2 AND origem = 'Fourmakers';

UPDATE tb_candidato_status
SET descricao = 'Analise do Gestor'
WHERE id = 3 AND origem = 'Fourmakers';

UPDATE tb_candidato_status
SET descricao = 'Testes Comportamentais'
WHERE id = 4 AND origem = 'Fourmakers';

UPDATE tb_candidato_status
SET descricao = 'Testes Técnicos'
WHERE id = 5 AND origem = 'Fourmakers';

UPDATE tb_candidato_status
SET descricao = 'Entrevista com Cliente'
WHERE id = 6 AND origem = 'Fourmakers';


INSERT INTO tb_candidato_status (id, descricao, origem) VALUES
(7, 'Carta-Oferta', 'Fourmakers'),
(8, 'Aprovado', 'Fourmakers'),
(9, 'Reprovado', 'Fourmakers');