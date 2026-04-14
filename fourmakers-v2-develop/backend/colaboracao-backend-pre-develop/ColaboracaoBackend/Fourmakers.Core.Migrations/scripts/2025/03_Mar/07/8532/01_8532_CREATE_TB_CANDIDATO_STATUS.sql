CREATE TABLE tb_candidato_status (
    id INTEGER PRIMARY KEY,
    descricao VARCHAR(255) NOT NULL, 
    origem VARCHAR(50) NOT NULL 
);

INSERT INTO tb_candidato_status (id, descricao, origem) VALUES
(1, 'Standby', 'SRS'),
(100, 'Reprovação Técnica', 'SRS'),
(200, 'Reprovação Comportamental', 'SRS'),
(300, 'Reprovação Teste Técnico', 'SRS'),
(400, 'Encaminhado', 'SRS'),
(500, 'Entrevista RH', 'SRS'),
(525, 'Entrevista Técnica', 'SRS'),
(550, 'Entrevista Cliente', 'SRS'),
(600, 'Reprovação QI', 'SRS'),
(700, 'Sem interesse', 'SRS'),
(800, 'Declinou', 'SRS'),
(900, 'Em contato', 'SRS'),
(1000, 'Contratado', 'SRS'),
(1100, 'Pretensão Acima', 'SRS'),
(1200, 'Recusou Local Atuação', 'SRS'),
(1300, 'Aprovado', 'SRS'),
(1400, 'Reprovação do Cliente', 'SRS'),
(1500, 'Candidato sem interesse pelo cliente', 'SRS'),
(1600, 'Falta de sucesso no contato', 'SRS'),
(1700, 'Modelo de contratação', 'SRS'),
(1800, 'Pesquisa interna', 'SRS'),
(1900, 'Removido do Pipeline (uso SRS)', 'SRS'),
(2000, 'Consulta de Referências', 'SRS'),
(2100, 'Aprovada vaga duplicada', 'SRS'),
(2200, 'Aprovado RH', 'SRS'),
(2300, 'Reprovado RH', 'SRS'),
(2400, 'Aprovado grupo alocado', 'SRS');