-- [DataForge] Reembolso — Fourmakers v2
-- Dialeto: SQLite 3.x

BEGIN TRANSACTION;

-- R-01 · [Positivo] Visualização dos painéis de resumo
INSERT INTO tb_organizacao (id, cnpj, razao_social, nome_fantasia, subdominio, email_contato, telefone, cep, cidade, uf, ativo)
VALUES (1, '84051927000140', 'Foursys Tecnologia e Informática Ltda', 'Foursys', 'foursys', 'contato@foursys.com.br', '6132049781', '70040010', 'Brasília', 'DF', 1);

INSERT INTO tb_cargo (cod, nome, nivel, descricao, salario_min, salario_max) VALUES
('DEV-PL', 'Desenvolvedora Full Stack', 'Pleno', 'Desenvolvimento de aplicações web e mobile', 7000.00, 12000.00),
('GES-PR', 'Gerente de Projetos', 'Gestor', 'Gestão de portfólio de projetos e equipes', 16000.00, 24000.00),
('ANA-FI', 'Analista Financeira', 'Senior', 'Análise financeira e controle de despesas', 11000.00, 17000.00);

INSERT INTO tb_departamento (cod, nome, org_id, centro_custo, email_depto, responsavel_cod) VALUES
('ENG', 'Engenharia de Software', 1, 'CC-300', 'engenharia@foursys.com.br', NULL),
('FIN', 'Financeiro', 1, 'CC-200', 'financeiro@foursys.com.br', NULL);

INSERT INTO tb_usuario (id, org_id, cpf, email, nome, telefone, data_nascimento, perfil, ativo) VALUES
(1, 1, '40387152997', 'camila.pereira@foursys.com.br', 'Camila Rodrigues Pereira', '11984073516', '1994-09-03', 'Colaborador', 1),
(2, 1, '61902345770', 'bruno.yamamoto@foursys.com.br', 'Bruno Takashi Yamamoto', '61981237046', '1983-12-17', 'Gestor', 1),
(3, 1, '52840731690', 'adriana.souza@foursys.com.br', 'Adriana Melo de Souza', '71974058293', '1988-06-25', 'Financeiro', 1),
(4, 1, '38510476217', 'lucas.nascimento@foursys.com.br', 'Lucas Oliveira Nascimento', '19983041752', '1999-02-11', 'Colaborador', 1);

INSERT INTO tb_colaborador (cod_profissional, usuario_id, cargo_cod, depto_cod, gestor_cod, nome, celular, data_admissao, modelo_contratacao, salario, banco, agencia, conta_corrente, tipo_pix, chave_pix, sou_gestor, sou_aprovador, ativo) VALUES
('EMP-002', 2, 'GES-PR', 'ENG', NULL, 'Bruno Takashi Yamamoto', '61981237046', '2018-04-02', 'CLT', 20500.00, 'Itaú', '0915', '41073-6', 'Email', 'bruno.yamamoto@foursys.com.br', 1, 0, 1),
('EMP-003', 3, 'ANA-FI', 'FIN', NULL, 'Adriana Melo de Souza', '71974058293', '2019-08-19', 'CLT', 13800.00, 'Banco do Brasil', '3071', '58204-1', 'CPF', '52840731690', 0, 1, 1),
('EMP-001', 1, 'DEV-PL', 'ENG', 'EMP-002', 'Camila Rodrigues Pereira', '11984073516', '2022-01-10', 'CLT', 9200.00, 'Nubank', '0001', '6140387-5', 'CPF', '40387152997', 0, 0, 1),
('EMP-004', 4, 'DEV-PL', 'ENG', 'EMP-002', 'Lucas Oliveira Nascimento', '19983041752', '2024-05-13', 'CLT', 7800.00, 'C6 Bank', '0001', '8270514-3', 'Telefone', '19983041752', 0, 0, 1);

INSERT INTO tb_cliente (codigo_cliente, cnpj, razao_social, nome_fantasia, email_contato, telefone, responsavel_nome, cep, cidade, uf, ativo) VALUES
('CLI-001', '53702814000134', 'Innovatech Desenvolvimento de Software Ltda', 'Innovatech', 'projetos@innovatech.com.br', '1138470215', 'Rafael Henrique Costa', '04538132', 'São Paulo', 'SP', 1),
('CLI-002', '70935162000105', 'Rede Pharma Distribuição e Logística S.A.', 'Rede Pharma', 'ti@redepharma.com.br', '7135018472', 'Patrícia Mendes Farias', '40020020', 'Salvador', 'BA', 1);

INSERT INTO tb_projeto (codigo_projeto, codigo_cliente, responsavel_cod, nome, descricao, status, orcamento, data_inicio, data_fim) VALUES
('PRJ-001', 'CLI-001', 'EMP-001', 'Migração Cloud AWS', 'Migração de infraestrutura on-premise para AWS', 'Em andamento', 620000.00, '2025-10-01', '2026-12-31'),
('PRJ-002', 'CLI-002', NULL, 'App Logística Mobile', 'Aplicativo de rastreamento de entregas em tempo real', 'Em andamento', 290000.00, '2026-02-01', '2026-11-30');

INSERT INTO tb_verba (verba_id, categoria, descricao, valor, tipo_custo, requer_comprovante, prazo_envio_dias, ativo) VALUES
(1, 'Alimentação', 'Refeições durante viagens ou reuniões externas', 150.00, 'debito', 1, 30, 1),
(2, 'Transporte', 'Passagens aéreas, ônibus, táxi e aplicativos de mobilidade', 80.00, 'debito', 1, 30, 1),
(3, 'Hospedagem', 'Diárias de hotel durante viagens a trabalho', 500.00, 'debito', 1, 30, 1),
(4, 'Quilometragem', 'Deslocamento com veículo próprio — valor por quilômetro rodado', 1.20, 'variavel', 0, 45, 1),
(5, 'Material de Escritório', 'Materiais de escritório e papelaria para uso profissional', 200.00, 'debito', 1, 30, 1);

INSERT INTO tb_solicitacao_grupo (id, cod_profissional, projeto_cod, objetivo, total_declarado, data_solicitacao)
VALUES (1, 'EMP-001', 'PRJ-001', 'Treinamento AWS re:Invent 2026', 715.70, '2026-03-20');

INSERT INTO tb_solicitacao_item (id, solicitacao_grupo_id, verba_id, status_id, aprovador_cod, categoria, descricao, data_despesa, valor, valor_aprovado, comprovante_url, data_aprovacao, data_pagamento, observacao) VALUES
(1, 1, 3, 2, 'EMP-003', 'Hospedagem', 'Três diárias no Hotel Mercure Paulista para evento AWS', '2026-03-18', 420.00, 420.00, 'https://storage.foursys.com/comprovantes/2026/03/nf-mercure-5821.pdf', '2026-03-22', NULL, NULL),
(2, 1, 1, 5, 'EMP-003', 'Alimentação', 'Jantar de networking com parceiros AWS no Restaurante Figueira', '2026-03-19', 128.50, 128.50, 'https://storage.foursys.com/comprovantes/2026/03/nf-figueira-3094.pdf', '2026-03-22', '2026-03-28', NULL),
(3, 1, 2, 4, 'EMP-003', 'Transporte', 'Táxi aeroporto Congonhas–hotel e retorno após evento', '2026-03-19', 67.20, 67.20, 'https://storage.foursys.com/comprovantes/2026/03/recibo-99taxi-0319.pdf', '2026-03-22', NULL, NULL),
(4, 1, 1, 1, NULL, 'Alimentação', 'Almoço rápido no centro de convenções durante palestras', '2026-03-20', 100.00, NULL, NULL, NULL, NULL, NULL);

-- R-04 · [Positivo] Filtro de solicitações por período
INSERT INTO tb_solicitacao_grupo (id, cod_profissional, projeto_cod, objetivo, total_declarado, data_solicitacao)
VALUES (2, 'EMP-001', 'PRJ-002', 'Visita cliente Rede Pharma — Salvador', 234.80, '2026-01-28');

INSERT INTO tb_solicitacao_item (id, solicitacao_grupo_id, verba_id, status_id, aprovador_cod, categoria, descricao, data_despesa, valor, valor_aprovado, comprovante_url, data_aprovacao, data_pagamento, observacao) VALUES
(5, 2, 1, 5, 'EMP-003', 'Alimentação', 'Almoço com equipe de TI do cliente Rede Pharma no Restaurante Amado', '2026-01-28', 78.80, 78.80, 'https://storage.foursys.com/comprovantes/2026/01/nf-amado-7103.pdf', '2026-01-30', '2026-02-10', NULL),
(6, 2, 4, 5, 'EMP-003', 'Quilometragem', 'Deslocamento de carro próprio aeroporto–escritório cliente–hotel (130 km)', '2026-01-28', 156.00, 156.00, NULL, '2026-01-30', '2026-02-10', NULL);

-- R-05 · [Positivo] Busca textual na lista de reembolsos
INSERT INTO tb_solicitacao_grupo (id, cod_profissional, projeto_cod, objetivo, total_declarado, data_solicitacao)
VALUES (3, 'EMP-001', 'PRJ-001', 'Sprint Review — Retrospectiva Q1', 189.40, '2026-04-07');

INSERT INTO tb_solicitacao_item (id, solicitacao_grupo_id, verba_id, status_id, aprovador_cod, categoria, descricao, data_despesa, valor, valor_aprovado, comprovante_url, data_aprovacao, observacao) VALUES
(7, 3, 1, 2, 'EMP-003', 'Alimentação', 'Coffee break e almoço para 12 participantes da retrospectiva', '2026-04-07', 112.40, 112.40, 'https://storage.foursys.com/comprovantes/2026/04/nf-coffeebreak-2047.pdf', '2026-04-08', NULL),
(8, 3, 5, 2, 'EMP-003', 'Material de Escritório', 'Quadro kanban magnético e post-its para mural da sprint', '2026-04-07', 77.00, 77.00, 'https://storage.foursys.com/comprovantes/2026/04/nf-kalunga-8310.pdf', '2026-04-08', NULL);

-- I-10 · [Negativo] Data do comprovante fora da validade
INSERT INTO tb_solicitacao_grupo (id, cod_profissional, projeto_cod, objetivo, total_declarado, data_solicitacao)
VALUES (4, 'EMP-001', 'PRJ-001', 'Nota fiscal antiga — Jantar pré-projeto', 143.60, '2026-04-08');

INSERT INTO tb_solicitacao_item (id, solicitacao_grupo_id, verba_id, status_id, categoria, descricao, data_despesa, valor, comprovante_url, observacao)
VALUES (9, 4, 1, 1, 'Alimentação', 'Jantar de alinhamento com tech lead antes do kickoff do projeto', '2026-02-01', 143.60, 'https://storage.foursys.com/comprovantes/2026/02/nf-madero-4520.pdf', NULL);

-- I-17 · [Regressivo] Solicitação sem projeto quando permitido
INSERT INTO tb_solicitacao_grupo (id, cod_profissional, projeto_cod, objetivo, total_declarado, data_solicitacao)
VALUES (5, 'EMP-001', NULL, 'Happy hour da equipe — encerramento de milestone', 275.00, '2026-02-14');

INSERT INTO tb_solicitacao_item (id, solicitacao_grupo_id, verba_id, status_id, aprovador_cod, categoria, descricao, data_despesa, valor, observacao)
VALUES (10, 5, 1, 3, 'EMP-003', 'Alimentação', 'Happy hour para 15 pessoas no Bar Astor — celebração de entrega', '2026-02-13', 275.00, 'Valor excede o teto da categoria Alimentação (R$ 150,00). Solicitar aprovação prévia do gestor para eventos com mais de 10 participantes.');

COMMIT;
