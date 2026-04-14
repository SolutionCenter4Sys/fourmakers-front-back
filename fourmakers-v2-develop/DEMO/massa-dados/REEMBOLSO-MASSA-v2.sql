-- [DataForge] Reembolso — Fourmakers v2
-- Dialeto: SQLite 3.x

BEGIN TRANSACTION;

-- R-01 · [Positivo] Visualização dos painéis de resumo
INSERT INTO tb_organizacao (id, cnpj, razao_social, nome_fantasia, subdominio, email_contato, telefone, cep, cidade, uf, ativo)
VALUES (1, '62415037000138', 'Foursys Tecnologia e Informática Ltda', 'Foursys', 'foursys', 'contato@foursys.com.br', '1134827619', '04538132', 'São Paulo', 'SP', 1);

INSERT INTO tb_cargo (cod, nome, nivel, descricao, salario_min, salario_max) VALUES
('ANA-SI', 'Analista de Sistemas', 'Pleno', 'Análise e desenvolvimento de sistemas corporativos', 6500.00, 11000.00),
('GER-TI', 'Gerente de TI', 'Gestor', 'Gestão da área de tecnologia e inovação', 15000.00, 22000.00),
('COO-FI', 'Coordenadora Financeira', 'Senior', 'Coordenação de contas a pagar e reembolsos', 12000.00, 18000.00);

INSERT INTO tb_departamento (cod, nome, org_id, centro_custo, email_depto, responsavel_cod) VALUES
('TI', 'Tecnologia da Informação', 1, 'CC-100', 'ti@foursys.com.br', NULL),
('FIN', 'Financeiro', 1, 'CC-200', 'financeiro@foursys.com.br', NULL);

INSERT INTO tb_usuario (id, org_id, cpf, email, nome, telefone, data_nascimento, perfil, ativo) VALUES
(1, 1, '47832510617', 'renata.santos@foursys.com.br', 'Renata Vieira dos Santos', '11984732651', '1992-07-14', 'Colaborador', 1),
(2, 1, '15870392497', 'carlos.mendonca@foursys.com.br', 'Carlos Eduardo Mendonça', '31991027438', '1985-03-22', 'Gestor', 1),
(3, 1, '28347106517', 'juliana.lima@foursys.com.br', 'Juliana Ferreira Lima', '21978320914', '1990-11-08', 'Financeiro', 1),
(4, 1, '36185074290', 'pedro.almeida@foursys.com.br', 'Pedro Henrique Almeida', '11983047162', '1997-05-30', 'Colaborador', 1);

INSERT INTO tb_colaborador (cod_profissional, usuario_id, cargo_cod, depto_cod, gestor_cod, nome, celular, data_admissao, modelo_contratacao, salario, banco, agencia, conta_corrente, tipo_pix, chave_pix, sou_gestor, sou_aprovador, ativo) VALUES
('EMP-002', 2, 'GER-TI', 'TI', NULL, 'Carlos Eduardo Mendonça', '31991027438', '2019-06-10', 'CLT', 18500.00, 'Itaú', '0347', '28150-3', 'Email', 'carlos.mendonca@foursys.com.br', 1, 0, 1),
('EMP-003', 3, 'COO-FI', 'FIN', NULL, 'Juliana Ferreira Lima', '21978320914', '2020-01-08', 'CLT', 14200.00, 'Bradesco', '1582', '43701-8', 'CPF', '28347106517', 0, 1, 1),
('EMP-001', 1, 'ANA-SI', 'TI', 'EMP-002', 'Renata Vieira dos Santos', '11984732651', '2021-03-15', 'CLT', 8500.00, 'Nubank', '0001', '7834219-0', 'CPF', '47832510617', 0, 0, 1),
('EMP-004', 4, 'ANA-SI', 'TI', 'EMP-002', 'Pedro Henrique Almeida', '11983047162', '2023-08-07', 'CLT', 7200.00, 'Inter', '0001', '9150473-2', 'Telefone', '11983047162', 0, 0, 1);

INSERT INTO tb_cliente (codigo_cliente, cnpj, razao_social, nome_fantasia, email_contato, telefone, responsavel_nome, cep, cidade, uf, ativo) VALUES
('CLI-001', '45872013000199', 'TechVision Soluções Digitais Ltda', 'TechVision', 'comercial@techvision.com.br', '1132847015', 'Marcos Antônio Ribeiro', '01310100', 'São Paulo', 'SP', 1),
('CLI-002', '31708526000154', 'Nexus Consultoria Empresarial S.A.', 'Nexus', 'atendimento@nexus.com.br', '2125903847', 'Fernanda Costa e Silva', '20040020', 'Rio de Janeiro', 'RJ', 1);

INSERT INTO tb_projeto (codigo_projeto, codigo_cliente, responsavel_cod, nome, descricao, status, orcamento, data_inicio, data_fim) VALUES
('PRJ-001', 'CLI-001', 'EMP-001', 'Sistema ERP Cloud', 'Implantação do ERP em nuvem para gestão integrada', 'Em andamento', 450000.00, '2025-08-01', '2026-12-31'),
('PRJ-002', 'CLI-002', NULL, 'Portal do Colaborador', 'Desenvolvimento do portal de autoatendimento RH', 'Em andamento', 180000.00, '2026-01-10', '2026-09-30');

INSERT INTO tb_verba (verba_id, categoria, descricao, valor, tipo_custo, requer_comprovante, prazo_envio_dias, ativo) VALUES
(1, 'Alimentação', 'Refeições durante viagens ou reuniões externas', 150.00, 'debito', 1, 30, 1),
(2, 'Transporte', 'Passagens aéreas, ônibus, táxi e aplicativos de mobilidade', 80.00, 'debito', 1, 30, 1),
(3, 'Hospedagem', 'Diárias de hotel durante viagens a trabalho', 500.00, 'debito', 1, 30, 1),
(4, 'Quilometragem', 'Deslocamento com veículo próprio — valor por quilômetro rodado', 1.20, 'variavel', 0, 45, 1),
(5, 'Material de Escritório', 'Materiais de escritório e papelaria para uso profissional', 200.00, 'debito', 1, 30, 1);

INSERT INTO tb_solicitacao_grupo (id, cod_profissional, projeto_cod, objetivo, total_declarado, data_solicitacao)
VALUES (1, 'EMP-001', 'PRJ-001', 'Viagem SP - TechConf 2026', 512.50, '2026-03-15');

INSERT INTO tb_solicitacao_item (id, solicitacao_grupo_id, verba_id, status_id, aprovador_cod, categoria, descricao, data_despesa, valor, valor_aprovado, comprovante_url, data_aprovacao, data_pagamento, observacao) VALUES
(1, 1, 1, 2, 'EMP-003', 'Alimentação', 'Almoço de negócios no restaurante Fasano durante TechConf', '2026-03-14', 87.50, 87.50, 'https://storage.foursys.com/comprovantes/2026/03/nf-8723.pdf', '2026-03-18', NULL, NULL),
(2, 1, 2, 5, 'EMP-003', 'Transporte', 'Uber do hotel para o centro de convenções — ida e volta', '2026-03-14', 45.00, 45.00, 'https://storage.foursys.com/comprovantes/2026/03/recibo-uber-0314.pdf', '2026-03-18', '2026-03-25', NULL),
(3, 1, 3, 4, 'EMP-003', 'Hospedagem', 'Duas diárias no Hotel Ibis Paulista para TechConf', '2026-03-13', 380.00, 380.00, 'https://storage.foursys.com/comprovantes/2026/03/nf-hotel-ibis-9041.pdf', '2026-03-18', NULL, NULL);

-- R-04 · [Positivo] Filtro de solicitações por período
INSERT INTO tb_solicitacao_grupo (id, cod_profissional, projeto_cod, objetivo, total_declarado, data_solicitacao)
VALUES (2, 'EMP-001', 'PRJ-002', 'Reunião cliente Nexus — Campinas', 158.30, '2026-02-10');

INSERT INTO tb_solicitacao_item (id, solicitacao_grupo_id, verba_id, status_id, aprovador_cod, categoria, descricao, data_despesa, valor, valor_aprovado, comprovante_url, data_aprovacao, data_pagamento, observacao) VALUES
(4, 2, 1, 5, 'EMP-003', 'Alimentação', 'Almoço com equipe do cliente Nexus no Bistrô da Lapa', '2026-02-10', 62.30, 62.30, 'https://storage.foursys.com/comprovantes/2026/02/nf-bistro-4210.pdf', '2026-02-12', '2026-02-20', NULL),
(5, 2, 4, 5, 'EMP-003', 'Quilometragem', 'Deslocamento de carro próprio SP–Campinas para visita técnica (80 km)', '2026-02-10', 96.00, 96.00, NULL, '2026-02-12', '2026-02-20', NULL);

-- R-05 · [Positivo] Busca textual na lista de reembolsos
INSERT INTO tb_solicitacao_grupo (id, cod_profissional, projeto_cod, objetivo, total_declarado, data_solicitacao)
VALUES (3, 'EMP-001', 'PRJ-001', 'Workshop Design Thinking — Equipe de Produto', 243.30, '2026-04-01');

INSERT INTO tb_solicitacao_item (id, solicitacao_grupo_id, verba_id, status_id, aprovador_cod, categoria, descricao, data_despesa, valor, valor_aprovado, comprovante_url, data_aprovacao, observacao) VALUES
(6, 3, 1, 1, NULL, 'Alimentação', 'Coffee break e almoço para equipe de 8 participantes', '2026-04-01', 95.40, NULL, NULL, NULL, NULL),
(7, 3, 5, 2, 'EMP-003', 'Material de Escritório', 'Post-its, canetas coloridas e flipchart para dinâmica de grupo', '2026-04-01', 147.90, 147.90, 'https://storage.foursys.com/comprovantes/2026/04/nf-papelaria-1830.pdf', '2026-04-03', NULL);

-- I-10 · [Negativo] Data do comprovante fora da validade
INSERT INTO tb_solicitacao_grupo (id, cod_profissional, projeto_cod, objetivo, total_declarado, data_solicitacao)
VALUES (4, 'EMP-001', 'PRJ-001', 'Despesa retroativa — Almoço equipe desenvolvimento', 134.70, '2026-04-05');

INSERT INTO tb_solicitacao_item (id, solicitacao_grupo_id, verba_id, status_id, categoria, descricao, data_despesa, valor, comprovante_url, observacao)
VALUES (8, 4, 1, 1, 'Alimentação', 'Almoço de integração com novos membros da equipe', '2026-02-15', 134.70, 'https://storage.foursys.com/comprovantes/2026/02/nf-restaurante-7215.pdf', NULL);

-- I-17 · [Regressivo] Solicitação sem projeto quando permitido
INSERT INTO tb_solicitacao_grupo (id, cod_profissional, projeto_cod, objetivo, total_declarado, data_solicitacao)
VALUES (5, 'EMP-001', NULL, 'Confraternização equipe TI — fim de sprint', 210.00, '2026-01-20');

INSERT INTO tb_solicitacao_item (id, solicitacao_grupo_id, verba_id, status_id, aprovador_cod, categoria, descricao, data_despesa, valor, observacao)
VALUES (9, 5, 1, 3, 'EMP-003', 'Alimentação', 'Jantar para 12 pessoas no restaurante Outback Morumbi', '2026-01-18', 210.00, 'Valor ultrapassa o teto da categoria Alimentação (R$ 150,00). Necessário aprovação prévia do gestor para exceções.');

COMMIT;
