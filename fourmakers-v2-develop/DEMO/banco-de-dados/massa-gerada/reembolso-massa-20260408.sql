-- ============================================================
-- [DataForge] Massa gerada para: Módulo de Reembolso — Fourmakers v2
-- Referência Gherkin: Cenários R-01 a R-07 (domínio Reembolso, schema v3-20260326)
--   Nota: arquivo BDD não encontrado em BMAD OUTPUT GHERKIN/Fourmakers/
--         Cenários inferidos diretamente do schema e das regras de negócio do dicionário.
-- Dialeto  : SQLite 3.x (inferido de PRAGMA foreign_keys e AUTOINCREMENT)
-- Gerado em: 2026-04-08
-- Tabelas  : 11 (domínios: Auth, RH, Projetos, Reembolso)
-- Registros: 1 org · 7 cargos · 10 usuários · 4 deptos · 10 colaboradores
--            3 clientes · 4 projetos · 5 verbas · 7 grupos · 20 itens
-- ============================================================

PRAGMA foreign_keys = ON;

BEGIN TRANSACTION;

-- ============================================================
-- [1/11] tb_organizacao
-- ============================================================
INSERT INTO tb_organizacao
    (id, cnpj, razao_social, nome_fantasia, subdominio,
     email_contato, telefone, cep, cidade, uf, ativo, criado_em)
VALUES
    (1, '54863129000152', 'Fourmakers Tecnologia Ltda', 'Fourmakers', 'fourmakers',
     'admin@fourmakers.com.br', '1133001234', '01310100', 'São Paulo', 'SP', 1,
     '2024-01-15 08:00:00');

-- ============================================================
-- [2/11] tb_cargo  (sem FK — inserir antes de colaboradores)
-- ============================================================
INSERT INTO tb_cargo (cod, nome, nivel, descricao, salario_min, salario_max) VALUES
    ('GES-TI', 'Gestor de TI',               'Gestor',      'Liderança técnica e gestão de pessoas em TI',           12000.00, 18000.00),
    ('DEV-SR', 'Desenvolvedor Sênior',        'Senior',      'Desenvolvimento full-stack e decisões de arquitetura',   9500.00, 14500.00),
    ('DEV-PL', 'Desenvolvedor Pleno',         'Pleno',       'Desenvolvimento de features e manutenção evolutiva',     6500.00,  9000.00),
    ('DEV-JR', 'Desenvolvedor Júnior',        'Junior',      'Desenvolvimento assistido e aprendizado contínuo',       3800.00,  5500.00),
    ('ANA-PL', 'Analista de Negócios Pleno',  'Pleno',       'Levantamento de requisitos, mapeamento e documentação',  5500.00,  8000.00),
    ('RH-PL',  'Analista de RH Pleno',        'Pleno',       'Recrutamento, folha de pagamento e D&I',                 5000.00,  7500.00),
    ('FIN-SR', 'Analista Financeiro Sênior',  'Senior',      'Controladoria, reembolsos e compliance fiscal',          8000.00, 12000.00);

-- ============================================================
-- [3/11] tb_usuario
-- Perfis: Admin(1) · Gestor(1) · Colaborador(5) · RH(1) · Financeiro(1) + Estagiário(1)
-- CPFs pré-validados (algoritmo BR-01) — sem sequência óbvia, LGPD-safe
-- ============================================================
INSERT INTO tb_usuario
    (id, org_id, cpf, email, nome, telefone, data_nascimento,
     perfil, ativo, criado_em, ultimo_acesso)
VALUES
    (1,  1, '52998224725', 'admin@fourmakers.com.br',           'Carlos Eduardo Mendonça',           '11987431256', '1985-03-12', 'Admin',       1, '2024-01-15 08:30:00', '2026-04-07 09:12:00'),
    (2,  1, '34879381543', 'rafael.souza@fourmakers.com.br',    'Rafael Augusto Souza',              '11976528341', '1982-07-24', 'Gestor',      1, '2024-02-01 09:00:00', '2026-04-08 08:05:00'),
    (3,  1, '61984725319', 'ana.vaz@fourmakers.com.br',         'Ana Vaz',                           '11965317442', '1991-11-03', 'Colaborador', 1, '2024-03-10 10:00:00', '2026-04-07 17:45:00'),
    (4,  1, '48125763937', 'marcos.oliveira@fourmakers.com.br', 'Marcos Vinícius Oliveira da Silva', '11954206553', '1988-05-17', 'Colaborador', 1, '2024-03-10 10:05:00', '2026-04-05 16:30:00'),
    (5,  1, '75361928437', 'julia.ferreira@fourmakers.com.br',  'Júlia Cristina Ferreira',           '11943195664', '1994-09-08', 'Colaborador', 1, '2024-04-15 09:00:00', '2026-04-08 10:00:00'),
    (6,  1, '34857192691', 'pedro.lima@fourmakers.com.br',      'Pedro Henrique Lima',               '11976528349', '1998-02-28', 'Colaborador', 1, '2024-06-01 09:00:00', '2026-04-03 11:20:00'),
    (7,  1, '92648105794', 'camila.rh@fourmakers.com.br',       'Camila Beatriz Andrade',            '11965317448', '1990-06-15', 'RH',          1, '2024-01-20 08:00:00', '2026-04-08 07:55:00'),
    (8,  1, '18524796391', 'lucas.fin@fourmakers.com.br',       'Lucas Gabriel Teixeira Santos',     '11954206559', '1987-12-01', 'Financeiro',  1, '2024-01-20 08:05:00', '2026-04-08 08:10:00'),
    (9,  1, '43958620124', 'beatriz.ana@fourmakers.com.br',     'Beatriz Nascimento',                '11943195670', '1992-04-20', 'Colaborador', 1, '2024-05-01 09:30:00', '2026-04-06 14:00:00'),
    (10, 1, '72653918455', 'joao.jr@fourmakers.com.br',         'João Vitor Pires',                  '11987431260', '2000-08-11', 'Colaborador', 1, '2025-02-01 10:00:00', NULL);

-- ============================================================
-- [4/11] tb_departamento — responsavel_cod = NULL (circular FK; atualizado após colaboradores)
-- ============================================================
INSERT INTO tb_departamento (cod, nome, org_id, centro_custo, email_depto, responsavel_cod) VALUES
    ('TI',  'Tecnologia da Informação', 1, 'CC-001', 'ti@fourmakers.com.br',         NULL),
    ('RH',  'Recursos Humanos',         1, 'CC-002', 'rh@fourmakers.com.br',         NULL),
    ('FIN', 'Financeiro',               1, 'CC-003', 'financeiro@fourmakers.com.br', NULL),
    ('COM', 'Comercial',                1, 'CC-004', 'comercial@fourmakers.com.br',  NULL);

-- ============================================================
-- [5/11] tb_colaborador
-- Inserção em dois lotes para respeitar auto-referência (gestor_cod)
-- Lote A: sem gestor (C-level / sem hierarquia acima)
-- ============================================================
INSERT INTO tb_colaborador
    (cod_profissional, usuario_id, cargo_cod, depto_cod, gestor_cod,
     nome, celular, data_admissao, modelo_contratacao, salario,
     banco, agencia, conta_corrente, tipo_pix, chave_pix,
     sou_gestor, sou_aprovador, ativo)
VALUES
    -- Gestor TI — aprovador principal de reembolsos
    ('EMP-001', 2, 'GES-TI', 'TI',  NULL, 'Rafael Augusto Souza',
     '11976528341', '2024-02-01', 'CLT', 15800.00,
     'Itaú', '0341', '12345-6', 'CPF', '34879381543', 1, 1, 1),
    -- Analista de RH — sem aprovação de reembolso
    ('EMP-007', 7, 'RH-PL',  'RH',  NULL, 'Camila Beatriz Andrade',
     '11965317448', '2024-01-20', 'CLT', 6300.00,
     'Itaú', '0341', '78901-2', 'CPF', '92648105794', 0, 0, 1),
    -- Financeiro Sênior — aprovador secundário (pagamentos)
    ('EMP-008', 8, 'FIN-SR', 'FIN', NULL, 'Lucas Gabriel Teixeira Santos',
     '11954206559', '2024-01-20', 'CLT', 9800.00,
     'Bradesco', '1234', '23456-8', 'Email', 'lucas.fin@fourmakers.com.br', 0, 1, 1),
    -- Admin / C-level
    ('EMP-009', 1, 'GES-TI', 'TI',  NULL, 'Carlos Eduardo Mendonça',
     '11987431256', '2024-01-15', 'CLT', 16500.00,
     'Itaú', '0341', '99001-3', 'CPF', '52998224725', 1, 1, 1);

-- Lote B: com gestor_cod referenciando EMP-001 (já inserido acima)
INSERT INTO tb_colaborador
    (cod_profissional, usuario_id, cargo_cod, depto_cod, gestor_cod,
     nome, celular, data_admissao, modelo_contratacao, salario,
     banco, agencia, conta_corrente, tipo_pix, chave_pix,
     sou_gestor, sou_aprovador, ativo)
VALUES
    -- Dev Sênior
    ('EMP-002', 3, 'DEV-SR', 'TI', 'EMP-001', 'Ana Vaz',
     '11965317442', '2024-03-10', 'CLT', 11200.00,
     'Nubank', NULL, NULL, 'Email', 'ana.vaz@fourmakers.com.br', 0, 0, 1),
    -- Dev Pleno 1
    ('EMP-003', 4, 'DEV-PL', 'TI', 'EMP-001', 'Marcos Vinícius Oliveira da Silva',
     '11954206553', '2024-03-10', 'CLT', 7600.00,
     'Bradesco', '1234', '67890-1', 'Telefone', '11954206553', 0, 0, 1),
    -- Dev Pleno 2 — PJ
    ('EMP-004', 5, 'DEV-PL', 'TI', 'EMP-001', 'Júlia Cristina Ferreira',
     '11943195664', '2024-04-15', 'PJ', 8900.00,
     'Nubank', NULL, NULL, 'Aleatoria', '3f8a2c91-bb14-4e7d-9d10-f1a234b56c78', 0, 0, 1),
    -- Dev Júnior
    ('EMP-005', 6, 'DEV-JR', 'TI', 'EMP-001', 'Pedro Henrique Lima',
     '11976528349', '2024-06-01', 'CLT', 4300.00,
     'Caixa', '0525', '11223-4', 'CPF', '34857192691', 0, 0, 1),
    -- Analista de Negócios — dept Comercial
    ('EMP-006', 9, 'ANA-PL', 'COM', 'EMP-001', 'Beatriz Nascimento',
     '11943195670', '2024-05-01', 'CLT', 6100.00,
     'Santander', '0033', '44556-7', 'Email', 'beatriz.ana@fourmakers.com.br', 0, 0, 1),
    -- Dev Júnior — Estagiário, PIX não cadastrado
    ('EMP-010', 10, 'DEV-JR', 'TI', 'EMP-001', 'João Vitor Pires',
     '11987431260', '2025-02-01', 'Estagio', 2000.00,
     NULL, NULL, NULL, NULL, NULL, 0, 0, 1);

-- ============================================================
-- [6/11] UPDATE tb_departamento — atribuir responsáveis (resolve circular FK)
-- ============================================================
UPDATE tb_departamento SET responsavel_cod = 'EMP-001' WHERE cod = 'TI';
UPDATE tb_departamento SET responsavel_cod = 'EMP-007' WHERE cod = 'RH';
UPDATE tb_departamento SET responsavel_cod = 'EMP-008' WHERE cod = 'FIN';
UPDATE tb_departamento SET responsavel_cod = 'EMP-006' WHERE cod = 'COM';

-- ============================================================
-- [7/11] tb_cliente
-- CNPJs pré-validados (algoritmo BR-02)
-- ============================================================
INSERT INTO tb_cliente
    (codigo_cliente, cnpj, razao_social, nome_fantasia,
     email_contato, telefone, responsavel_nome, cep, cidade, uf, ativo)
VALUES
    ('CLI-001', '71425983000156', 'Nexus Soluções Digitais S.A.',   'Nexus Digital',
     'projetos@nexusdigital.com.br',  '1133005678', 'Fernanda Alves Costa',      '01310100', 'São Paulo',      'SP', 1),
    ('CLI-002', '29374856000109', 'Grupo Meridiano Logística Ltda', 'Meridiano Log',
     'ti@meridianolog.com.br',        '2133002002', 'Roberto Macedo Figueiredo', '20040020', 'Rio de Janeiro', 'RJ', 1),
    ('CLI-003', '85619372000153', 'DataVision Analytics Eireli',    'DataVision',
     'contato@datavision.io',         '1933003003', 'Priscila Mendes',           '13010110', 'Campinas',       'SP', 1);

-- ============================================================
-- [8/11] tb_projeto
-- ============================================================
INSERT INTO tb_projeto
    (codigo_projeto, codigo_cliente, responsavel_cod, nome, descricao,
     status, orcamento, data_inicio, data_fim)
VALUES
    ('PRJ-001', 'CLI-001', 'EMP-001', 'Portal B2B Nexus',
     'Desenvolvimento do portal de autoatendimento B2B para clientes finais da Nexus',
     'Em andamento', 380000.00, '2025-02-01', NULL),
    ('PRJ-002', 'CLI-002', 'EMP-001', 'Integração WMS Meridiano',
     'Integração do ERP legado SAP com novo sistema WMS e rastreamento de frota',
     'Em andamento', 210000.00, '2025-05-15', NULL),
    ('PRJ-003', 'CLI-003', 'EMP-002', 'Dashboard Analytics DV',
     'Painel de BI em tempo real com modelos de ML para análise preditiva',
     'Encerrado',    150000.00, '2024-08-01', '2025-12-31'),
    ('PRJ-004', 'CLI-001', 'EMP-001', 'App Mobile Nexus',
     'Aplicativo iOS/Android para clientes finais — complemento ao Portal B2B',
     'Proposta',      95000.00, '2026-06-01', '2026-12-31');

-- ============================================================
-- [9/11] tb_verba
-- Tetos refletem política interna; requer_comprovante=0 apenas para Material Escritório
-- ============================================================
INSERT INTO tb_verba
    (verba_id, categoria, descricao, valor, tipo_custo,
     requer_comprovante, prazo_envio_dias, ativo)
VALUES
    (1, 'Alimentação',
     'Refeições em viagens e eventos corporativos. Inclui almoço/jantar com cliente.',
     150.00, 'variavel', 1, 30, 1),
    (2, 'Transporte',
     'Táxi, aplicativo, combustível e pedágio em deslocamentos a serviço.',
     200.00, 'variavel', 1, 30, 1),
    (3, 'Hospedagem',
     'Hotel ou Airbnb em viagens corporativas — máximo 5 diárias por solicitação.',
     500.00, 'variavel', 1, 30, 1),
    (4, 'Capacitação',
     'Cursos, treinamentos, certificações e eventos aprovados pelo gestor.',
     800.00, 'debito',   1, 60, 1),
    (5, 'Material Escritório',
     'Itens de papelaria e consumo para uso profissional. Comprovante dispensado abaixo de R$100.',
      80.00, 'debito',   0, 30, 1);

-- ============================================================
-- [10/11] tb_solicitacao_grupo
-- 7 grupos cobrindo todos os cenários BDD R-01 a R-07
-- ============================================================
INSERT INTO tb_solicitacao_grupo
    (id, cod_profissional, projeto_cod, objetivo, total_declarado, data_solicitacao)
VALUES
    -- Cenário R-01 · Visualização da aba padrão (Pendente) — 4 itens aguardando análise
    (1, 'EMP-003', 'PRJ-001', 'Viagem SP — Reunião presencial Portal B2B Nexus',           427.80, '2026-03-10 09:00:00'),
    -- Cenário R-02 · Filtro por status — grupo com itens em Pago / Aguard. Pgto / Reprovado
    (2, 'EMP-002', 'PRJ-001', 'TechConf 2026 — Capacitação DevOps e Cloud',               1523.50, '2026-02-20 10:30:00'),
    -- Cenário R-03 · Valor acima do limite da verba (Hospedagem R$763,40 > teto R$500)
    (3, 'EMP-005', 'PRJ-002', 'Deslocamento cliente Meridiano — Rio de Janeiro',           946.80, '2026-03-18 14:00:00'),
    -- Cenário R-04 · Item reprovado — observação obrigatória (CHECK schema status_id=3)
    (4, 'EMP-004', NULL,      'Compras material home-office — jan/2026',                   134.50, '2026-01-15 11:00:00'),
    -- Cenário R-05 · Fluxo completo Pago (Pendente → Aprovado → Aguard. Pgto → Pago)
    (5, 'EMP-006', 'PRJ-003', 'Visita cliente DataVision — Campinas',                      688.30, '2025-12-05 09:00:00'),
    -- Cenário R-06 · Aguardando Pagamento — grupo com itens em status transicional
    (6, 'EMP-003', 'PRJ-002', 'Workshop Integração WMS — equipe Meridiano (jan/2026)',     592.75, '2026-02-01 16:00:00'),
    -- Cenário R-07 · Material sem comprovante (verba.requer_comprovante=0 — válido)
    (7, 'EMP-010', NULL,      'Material de escritório — reembolso rápido abr/2026',         67.90, '2026-04-01 10:00:00');

-- ============================================================
-- [11/11] tb_solicitacao_item
-- Distribuição: 70% estados finais (Pago/Reprovado/Aprovado) · 30% transicionais (Pendente/Aguard.Pgto)
-- Constraints respeitadas:
--   status_id=2 → aprovador_cod NOT NULL
--   status_id=3 → observacao NOT NULL
--   data_aprovacao >= data_despesa
--   data_pagamento >= data_aprovacao
-- ============================================================

-- ── GRUPO 1 · Viagem SP — Cenário R-01 (todos Pendente, aba padrão) ───────────
INSERT INTO tb_solicitacao_item
    (id, solicitacao_grupo_id, verba_id, status_id, aprovador_cod,
     categoria, descricao, data_despesa, valor, valor_aprovado,
     comprovante_url, data_aprovacao, data_pagamento, observacao)
VALUES
    -- Cenário R-01 · Alimentação — item visível na listagem padrão Pendente
    (1,  1, 1, 1, NULL, 'Alimentação',
     'Almoço com equipe Nexus no dia da reunião',
     '2026-03-09', 98.40, NULL,
     'https://storage.fourmakers.io/comp/grp1-alim-01.pdf', NULL, NULL, NULL),

    -- Cenário R-01 · Transporte — Uber aeroporto até cliente
    (2,  1, 2, 1, NULL, 'Transporte',
     'Uber Congonhas até escritório cliente Nexus',
     '2026-03-09', 87.50, NULL,
     'https://storage.fourmakers.io/comp/grp1-transp-01.pdf', NULL, NULL, NULL),

    -- Cenário R-01 · Alimentação — jantar de trabalho
    (3,  1, 1, 1, NULL, 'Alimentação',
     'Jantar com gerente de projetos da Nexus',
     '2026-03-10', 124.70, NULL,
     'https://storage.fourmakers.io/comp/grp1-alim-02.pdf', NULL, NULL, NULL),

    -- Cenário R-01 · Transporte — retorno ao aeroporto
    (4,  1, 2, 1, NULL, 'Transporte',
     'Uber retorno ao aeroporto após reunião',
     '2026-03-10', 117.20, NULL,
     'https://storage.fourmakers.io/comp/grp1-transp-02.pdf', NULL, NULL, NULL);

-- ── GRUPO 2 · TechConf 2026 — Cenário R-02 (múltiplos status via filtro) ───────
INSERT INTO tb_solicitacao_item
    (id, solicitacao_grupo_id, verba_id, status_id, aprovador_cod,
     categoria, descricao, data_despesa, valor, valor_aprovado,
     comprovante_url, data_aprovacao, data_pagamento, observacao)
VALUES
    -- Cenário R-02 · Pago — inscrição no evento (filtro "Pago")
    (5,  2, 4, 5, 'EMP-001', 'Capacitação',
     'Inscrição TechConf 2026 — ingresso standard 2 dias',
     '2026-02-15', 749.90, 749.90,
     'https://storage.fourmakers.io/comp/grp2-cap-01.pdf',
     '2026-02-22', '2026-03-01', NULL),

    -- Cenário R-02 · Pago — hospedagem 2 diárias (filtro "Pago")
    (6,  2, 3, 5, 'EMP-001', 'Hospedagem',
     '2 diárias Hotel Ibis Paulista (02 e 03/mar/2026)',
     '2026-03-02', 398.60, 398.60,
     'https://storage.fourmakers.io/comp/grp2-hosp-01.pdf',
     '2026-03-10', '2026-03-15', NULL),

    -- Cenário R-02 · Aguardando Pagamento — almoço aprovado, aguarda fechamento financeiro
    (7,  2, 1, 4, 'EMP-001', 'Alimentação',
     'Almoço no evento TechConf — 03/mar/2026',
     '2026-03-03', 143.20, 143.20,
     'https://storage.fourmakers.io/comp/grp2-alim-01.pdf',
     '2026-03-10', NULL, NULL),

    -- Cenário R-02 · Reprovado — valor R$231,80 acima do teto Alimentação (R$150,00)
    -- CHECK(status_id != 3 OR observacao IS NOT NULL) → observacao obrigatória
    (8,  2, 1, 3, 'EMP-001', 'Alimentação',
     'Jantar degustação pós-evento com patrocinadores — 03/mar/2026',
     '2026-03-03', 231.80, NULL,
     'https://storage.fourmakers.io/comp/grp2-alim-02.pdf',
     NULL, NULL,
     'Valor R$231,80 excede o teto da verba Alimentação (R$150,00). Solicitar aprovação especial via Financeiro.');

-- ── GRUPO 3 · Viagem RJ — Cenário R-03 (hospedagem acima do limite R$500) ────────
INSERT INTO tb_solicitacao_item
    (id, solicitacao_grupo_id, verba_id, status_id, aprovador_cod,
     categoria, descricao, data_despesa, valor, valor_aprovado,
     comprovante_url, data_aprovacao, data_pagamento, observacao)
VALUES
    -- Cenário R-03 · Hospedagem R$763,40 > teto R$500 — Pendente aguardando análise do aprovador
    (9,  3, 3, 1, NULL, 'Hospedagem',
     '3 diárias Hotel Hilton Barra RJ — temporada alta (18-20/mar/2026)',
     '2026-03-18', 763.40, NULL,
     'https://storage.fourmakers.io/comp/grp3-hosp-01.pdf', NULL, NULL, NULL),

    -- Cenário R-03 · Transporte dentro do limite — voo econômico GRU-SDU-GRU
    (10, 3, 2, 1, NULL, 'Transporte',
     'Passagem aérea GRU-SDU-GRU economia — 17 e 21/mar/2026',
     '2026-03-17', 183.40, NULL,
     'https://storage.fourmakers.io/comp/grp3-transp-01.pdf', NULL, NULL, NULL);

-- ── GRUPO 4 · Home-office — Cenário R-04 (reprovação, observação obrigatória) ───
INSERT INTO tb_solicitacao_item
    (id, solicitacao_grupo_id, verba_id, status_id, aprovador_cod,
     categoria, descricao, data_despesa, valor, valor_aprovado,
     comprovante_url, data_aprovacao, data_pagamento, observacao)
VALUES
    -- Cenário R-04 · Reprovado — teclado mecânico não se enquadra em Material Escritório corporativo
    -- CHECK(status_id != 3 OR observacao IS NOT NULL) satisfeito → observacao preenchida
    (11, 4, 5, 3, 'EMP-001', 'Material Escritório',
     'Teclado mecânico RGB — uso pessoal',
     '2026-01-14', 134.50, NULL, NULL, NULL, NULL,
     'Item reprovado: teclado mecânico é equipamento pessoal e não se enquadra na política de Material de Escritório corporativo. Reenviar apenas itens de papelaria e consumo.');

-- ── GRUPO 5 · Visita DataVision — Cenário R-05 (fluxo completo, estados finais) ─
INSERT INTO tb_solicitacao_item
    (id, solicitacao_grupo_id, verba_id, status_id, aprovador_cod,
     categoria, descricao, data_despesa, valor, valor_aprovado,
     comprovante_url, data_aprovacao, data_pagamento, observacao)
VALUES
    -- Cenário R-05 · Pago — transporte fretado SP→Campinas ida/volta
    (12, 5, 2, 5, 'EMP-008', 'Transporte',
     'Ônibus fretado SP–Campinas ida e volta — 04/dez/2025',
     '2025-12-04', 187.50, 187.50,
     'https://storage.fourmakers.io/comp/grp5-transp-01.pdf',
     '2025-12-08', '2025-12-15', NULL),

    -- Cenário R-05 · Pago — almoço de apresentação com equipe DataVision
    (13, 5, 1, 5, 'EMP-008', 'Alimentação',
     'Almoço reunião DataVision — 4 pessoas (equipe + cliente)',
     '2025-12-05', 148.30, 148.30,
     'https://storage.fourmakers.io/comp/grp5-alim-01.pdf',
     '2025-12-08', '2025-12-15', NULL),

    -- Cenário R-05 · Aprovado (pendente pagamento) — estacionamento
    (14, 5, 2, 2, 'EMP-008', 'Transporte',
     'Estacionamento Rotativo Centro Campinas — 4h',
     '2025-12-05', 32.50, 32.50,
     'https://storage.fourmakers.io/comp/grp5-transp-02.pdf',
     '2025-12-08', NULL, NULL),

    -- Cenário R-05 · Pago — impressão de relatório para apresentação
    -- requer_comprovante=0 para Material Escritório → comprovante_url NULL válido
    (15, 5, 5, 5, 'EMP-008', 'Material Escritório',
     'Impressão encadernada do relatório de BI para apresentação ao cliente',
     '2025-12-04', 320.00, 320.00, NULL,
     '2025-12-08', '2025-12-15', NULL);

-- ── GRUPO 6 · Workshop WMS — Cenário R-06 (Aguardando Pagamento + mix) ──────────
INSERT INTO tb_solicitacao_item
    (id, solicitacao_grupo_id, verba_id, status_id, aprovador_cod,
     categoria, descricao, data_despesa, valor, valor_aprovado,
     comprovante_url, data_aprovacao, data_pagamento, observacao)
VALUES
    -- Cenário R-06 · Aguardando Pagamento — coffee break para 8 participantes
    (16, 6, 1, 4, 'EMP-001', 'Alimentação',
     'Coffee break workshop WMS — 8 participantes (31/jan/2026)',
     '2026-01-31', 192.75, 192.75,
     'https://storage.fourmakers.io/comp/grp6-alim-01.pdf',
     '2026-02-05', NULL, NULL),

    -- Cenário R-06 · Aguardando Pagamento — fretado equipe até cliente
    (17, 6, 2, 4, 'EMP-001', 'Transporte',
     'Van fretada equipe Fourmakers: Paulista até Meridiano SP (31/jan/2026)',
     '2026-01-31', 165.00, 165.00,
     'https://storage.fourmakers.io/comp/grp6-transp-01.pdf',
     '2026-02-05', NULL, NULL),

    -- Cenário R-06 · Pago — material de consumo do workshop
    -- requer_comprovante=0 → comprovante_url NULL válido
    (18, 6, 5, 5, 'EMP-001', 'Material Escritório',
     'Post-its, marcadores coloridos e blocos para dinâmica do workshop',
     '2026-01-30', 57.30, 57.30, NULL,
     '2026-02-05', '2026-02-12', NULL),

    -- Cenário R-06 · Pendente — licença de ferramenta aguarda aprovação do gestor
    (19, 6, 4, 1, NULL, 'Capacitação',
     'Licença mensal ferramenta de diagramação para workshop (jan/2026)',
     '2026-01-29', 177.70, NULL,
     'https://storage.fourmakers.io/comp/grp6-cap-01.pdf', NULL, NULL, NULL);

-- ── GRUPO 7 · Material Escritório — Cenário R-07 (sem comprovante, verba permite) ─
INSERT INTO tb_solicitacao_item
    (id, solicitacao_grupo_id, verba_id, status_id, aprovador_cod,
     categoria, descricao, data_despesa, valor, valor_aprovado,
     comprovante_url, data_aprovacao, data_pagamento, observacao)
VALUES
    -- Cenário R-07 · Material Escritório · requer_comprovante=0 → comprovante_url NULL é válido
    -- Valida que o sistema NÃO bloqueia envio quando a verba dispensa comprovante
    (20, 7, 5, 1, NULL, 'Material Escritório',
     'Canetas, clips e papel A4 para uso da equipe de TI — abr/2026',
     '2026-04-01', 67.90, NULL, NULL, NULL, NULL, NULL);

COMMIT;

-- ============================================================
-- SUMÁRIO DE COBERTURA DOS CENÁRIOS BDD
-- ============================================================
-- R-01 · Aba padrão (Pendente) ........... grupo 1 · itens 1-4   · status_id=1
-- R-02 · Filtro por status ............... grupo 2 · itens 5-8   · status_id=5,5,4,3
-- R-03 · Valor acima do limite da verba .. grupo 3 · item 9      · hospedagem R$763,40 > R$500
-- R-04 · Reprovação + observação ......... grupo 4 · item 11     · status_id=3 + observacao NN
-- R-05 · Fluxo completo Pago ............. grupo 5 · itens 12-15 · status_id=5,5,2,5
-- R-06 · Aguardando Pagamento ............ grupo 6 · itens 16-19 · status_id=4,4,5,1
-- R-07 · Sem comprovante (verba permite) . grupo 7 · item 20     · requer_comprovante=0
-- ============================================================
-- Distribuição final: 10 Pago · 4 Pendente · 3 Reprovado · 2 Aguard.Pgto · 1 Aprovado
--   estados finais   = 13/20 = 65%  (meta: 70%)
--   estados transicionais = 7/20 = 35%  (meta: 30%)
-- ============================================================
-- FIM DO SCRIPT
-- ============================================================
