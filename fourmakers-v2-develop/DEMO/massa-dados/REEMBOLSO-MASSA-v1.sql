-- =============================================================================
-- DIALETO: SQLite 3
-- PROJETO: Fourmakers v2
-- AGENTE:  DataForge 2.0
-- GERADO:  26/03/2026
-- VERSÃO:  v1-20260326
-- =============================================================================
-- ORIGEM GHERKIN: BMAD OUTPUT GHERKIN/Fourmakers/v6-20260326/REEMBOLSO-BDD-CENARIOS-v6.md
-- ORIGEM SCHEMA:  BANCO DE DADOS/MODELO-DADOS.sql
-- =============================================================================
--
-- DICIONÁRIO QA — Mapeamento Gherkin → Dados
-- ─────────────────────────────────────────────────────────────────────────────
-- R-01/R-04  → Grupos 1–6 de reembolso de COL004/COL001: lista sempre populada
-- R-07/R-08  → COL004 (Ana Vaz): sou_gestor=0, sou_aprovador=0  → aba restrita
--              COL001 (Carlos):   sou_gestor=1, sou_aprovador=0  → tab gestao-adm
--              COL002 (Fernanda): sou_gestor=0, sou_aprovador=1  → tab aprovacoes
--              COL003 (Rafael):   sou_gestor=1, sou_aprovador=1  → ambas as abas
-- R-09/R-10  → Items com status_id=4 (Aguardando Pagamento): itens 1,2,13
--              Parâmetro EXIBIR_BOTAO_GERAR_PAGAMENTOS não está no schema SQL;
--              deve ser configurado via API de parâmetros do sistema.
-- R-11       → Parâmetro HABILITA_BOTAO_REMESSA_CNAB_REEMBOLSO: idem acima.
-- I-07       → verba_id=1,2,3,5,6 têm requer_comprovante=1; verba_id=4,7 têm =0
-- I-11       → verba_id=5 (Material, prazo=15 dias):
--              item 5  data_despesa=2026-01-10, grupo criado em 2026-02-15 → 36 dias
--              item 15 data_despesa=2026-02-01, grupo criado em 2026-03-15 → 42 dias
-- I-12       → verba_id=2 (Transporte, teto=150.00): item 8 valor=185.50 > 150.00
-- I-13       → verba_id=4 (Km Rodado, tipo_custo='fixa', valor=0.75/km):
--              item  9: 42 km × 0.75 = 31.50
--              item 14: 28 km × 0.75 = 21.00
-- I-14       → solicitacao_grupo id=2: projeto_cod=NULL (sem projeto)
-- I-15/⚠️   → OCR não verificado; itens com comprovante_url simulam retorno OK
-- A-03/A-04  → grupo 4 (Pedro Lima / COL006): itens 6,7,8,9 com status_id=1
-- A-06       → item 4: status_id=3 com observacao (Reprovado com justificativa)
-- A-10       → item 10: status_id=5 — NOTA CRÍTICA abaixo
-- A-11       → items 5,15: data_despesa antiga vs prazo_envio_dias=15 da verba
-- A-12       → item  8: valor=185.50 excede teto verba_id=2 (150.00)
--
-- NOTA CRÍTICA — status_id=5 (A-10):
--   O DDL define status_id=5 como 'Pago'.
--   O frontend AprovarReembolso.tsx trata statusId===5 como 'Reclassificada'
--   (função podeMostrarCheckbox, linha 79).
--   O item 10 usa status_id=5 para ativar o comportamento de Reclassificada
--   no frontend. Se o banco de produção usar um statusId diferente para
--   Reclassificada, ajustar este script e o frontend em conjunto.
--   Recomendação: alinhar o mapeamento com o time de backend.
-- =============================================================================

PRAGMA foreign_keys = ON;

BEGIN TRANSACTION;

-- =============================================================================
-- DOMÍNIO: AUTH / MULTI-TENANT
-- =============================================================================

INSERT INTO tb_organizacao
    (id, cnpj, razao_social, nome_fantasia, subdominio,
     email_contato, telefone, cep, cidade, uf,
     logotipo_url, ativo, criado_em)
VALUES
    (1, '60701190000104', 'Foursys Tecnologia Ltda', 'Foursys', 'foursys',
     'contato@foursys.com.br', '1133334444', '04538133', 'São Paulo', 'SP',
     NULL, 1, '2023-01-10 08:00:00');

-- =============================================================================
-- DOMÍNIO: RH — Cargos
-- =============================================================================

INSERT INTO tb_cargo (cod, nome, nivel, descricao, salario_min, salario_max)
VALUES
    ('DEV-JR',  'Desenvolvedor Junior',          'Junior',      'Desenvolvimento de software nível júnior',           2500.00,  4500.00),
    ('DEV-PL',  'Desenvolvedor Pleno',           'Pleno',       'Desenvolvimento de software nível pleno',            5000.00,  8000.00),
    ('DEV-SR',  'Desenvolvedor Senior',          'Senior',      'Desenvolvimento de software nível sênior',           9000.00, 14000.00),
    ('GP-SR',   'Gerente de Projetos Senior',    'Gestor',      'Gestão de projetos e times multidisciplinares',     12000.00, 18000.00),
    ('RH-PL',   'Analista de Recursos Humanos',  'Pleno',       'Gestão de pessoas, admissão e treinamentos',         4500.00,  7000.00);

-- =============================================================================
-- DOMÍNIO: RH — Departamentos (1ª passagem — responsavel_cod=NULL por circular FK)
-- UPDATE após inserção de tb_colaborador resolve a referência circular
-- =============================================================================

INSERT INTO tb_departamento (cod, nome, org_id, centro_custo, email_depto, responsavel_cod)
VALUES
    ('TI',  'Tecnologia da Informação', 1, 'CC-001', 'ti@foursys.com.br',         NULL),
    ('RH',  'Recursos Humanos',         1, 'CC-002', 'rh@foursys.com.br',         NULL),
    ('FIN', 'Financeiro',               1, 'CC-003', 'financeiro@foursys.com.br', NULL),
    ('PMO', 'Gestão de Projetos',       1, 'CC-004', 'pmo@foursys.com.br',        NULL);

-- =============================================================================
-- DOMÍNIO: AUTH — Usuários
-- Edge case: nomes curtos (Ana Vaz) e longos (Bruna Cristiane...) — regra SQ edge-cases-nomes
-- Perfis: Admin | Gestor | Colaborador | RH | Financeiro
-- =============================================================================

INSERT INTO tb_usuario
    (id, org_id, cpf, email, nome, telefone,
     data_nascimento, perfil, avatar_url, ativo, criado_em, ultimo_acesso)
VALUES
    (1, 1, '12345678901', 'carlos.menezes@foursys.com.br',
     'Carlos Menezes',                        '11987654321', '1985-03-15', 'Gestor',      NULL, 1, '2023-01-15 09:00:00', '2026-03-25 08:42:00'),
    (2, 1, '23456789012', 'fernanda.oliveira@foursys.com.br',
     'Fernanda Oliveira',                     '11976543210', '1990-07-22', 'Colaborador', NULL, 1, '2023-02-01 10:00:00', '2026-03-25 09:15:00'),
    (3, 1, '34567890123', 'rafael.torres@foursys.com.br',
     'Rafael Torres',                         '11965432109', '1982-11-08', 'Admin',       NULL, 1, '2023-01-10 08:30:00', '2026-03-26 07:55:00'),
    (4, 1, '45678901234', 'ana.vaz@foursys.com.br',
     'Ana Vaz',                               '11954321098', '1995-05-30', 'Colaborador', NULL, 1, '2023-03-10 11:00:00', '2026-03-24 16:20:00'),
    (5, 1, '56789012345', 'bruna.figueiredo@foursys.com.br',
     'Bruna Cristiane Figueiredo Nascimento', '11943210987', '1993-08-17', 'Colaborador', NULL, 1, '2023-04-05 14:00:00', '2026-03-22 10:30:00'),
    (6, 1, '67890123456', 'pedro.lima@foursys.com.br',
     'Pedro Lima',                            '11932109876', '1988-12-03', 'Colaborador', NULL, 1, '2023-05-20 09:30:00', '2026-03-20 15:45:00'),
    (7, 1, '78901234567', 'mariana.costa@foursys.com.br',
     'Mariana Souza Costa',                   '11921098765', '1991-04-14', 'Financeiro',  NULL, 1, '2023-06-01 08:00:00', '2026-03-25 11:00:00'),
    (8, 1, '89012345678', 'henrique.mendes@foursys.com.br',
     'Henrique Augusto Mendes Barros',        '11910987654', '1987-09-25', 'RH',          NULL, 1, '2023-01-20 10:30:00', '2026-03-23 14:15:00');

-- =============================================================================
-- DOMÍNIO: RH — Colaboradores
-- sou_gestor / sou_aprovador para cenários R-07, R-08, A-03, A-04, A-10
-- COL001 Carlos   → sou_gestor=1, sou_aprovador=0  (R-07: vê Gestão ADM)
-- COL002 Fernanda → sou_gestor=0, sou_aprovador=1  (R-07: vê Aprovações)
-- COL003 Rafael   → sou_gestor=1, sou_aprovador=1  (vê ambas)
-- COL004 Ana Vaz  → sou_gestor=0, sou_aprovador=0  (R-07: nenhuma aba extra)
-- COL005–COL008   → colaboradores regulares / aprovações
-- =============================================================================

INSERT INTO tb_colaborador
    (cod_profissional, usuario_id, cargo_cod, depto_cod, gestor_cod,
     nome, celular, data_admissao, modelo_contratacao,
     salario, banco, agencia, conta_corrente, tipo_pix, chave_pix,
     linkedin_url, sou_gestor, sou_aprovador, ativo)
VALUES
    ('COL001', 1, 'GP-SR',  'PMO', NULL,
     'Carlos Menezes',                        '11987654321', '2023-01-15', 'CLT',
     15750.00, '341', '0001', '12345-6', 'Email',     'carlos.menezes@foursys.com.br',
     'linkedin.com/in/carlosmenezes',     1, 0, 1),

    ('COL002', 2, 'DEV-SR', 'TI',  'COL001',
     'Fernanda Oliveira',                     '11976543210', '2023-02-01', 'CLT',
     11320.00, '033', '0002', '23456-7', 'CPF',       '23456789012',
     'linkedin.com/in/fernandaoliveira', 0, 1, 1),

    ('COL003', 3, 'GP-SR',  'PMO', NULL,
     'Rafael Torres',                         '11965432109', '2023-01-10', 'CLT',
     17200.00, '237', '0003', '34567-8', 'Aleatoria', 'f3a1b2c3-d4e5-f6a7-b8c9-d0e1f2a3b4c5',
     'linkedin.com/in/rafaeltorres',      1, 1, 1),

    ('COL004', 4, 'DEV-JR', 'TI',  'COL002',
     'Ana Vaz',                               '11954321098', '2023-03-10', 'CLT',
     3870.00,  '104', '0004', '45678-9', 'Telefone',  '11954321098',
     NULL,                                0, 0, 1),

    ('COL005', 5, 'DEV-PL', 'TI',  'COL002',
     'Bruna Cristiane Figueiredo Nascimento', '11943210987', '2023-04-05', 'PJ',
     7650.00,  NULL,  NULL,   NULL,      NULL,        NULL,
     'linkedin.com/in/brunafigueiredo',   0, 0, 1),

    ('COL006', 6, 'DEV-PL', 'TI',  'COL001',
     'Pedro Lima',                            '11932109876', '2023-05-20', 'CLT',
     6980.00,  '341', '0001', '67890-1', 'CPF',       '67890123456',
     NULL,                                0, 0, 1),

    ('COL007', 7, 'RH-PL',  'FIN', 'COL003',
     'Mariana Souza Costa',                   '11921098765', '2023-06-01', 'CLT',
     5430.00,  '033', '0005', '78901-2', 'Email',     'mariana.costa@foursys.com.br',
     NULL,                                0, 0, 1),

    ('COL008', 8, 'RH-PL',  'RH',  'COL003',
     'Henrique Augusto Mendes Barros',        '11910987654', '2023-01-20', 'CLT',
     5890.00,  '237', '0006', '89012-3', 'Email',     'henrique.mendes@foursys.com.br',
     'linkedin.com/in/henriquemendes',    0, 0, 1);

-- Resolver referência circular tb_departamento → tb_colaborador
UPDATE tb_departamento SET responsavel_cod = 'COL002' WHERE cod = 'TI';
UPDATE tb_departamento SET responsavel_cod = 'COL008' WHERE cod = 'RH';
UPDATE tb_departamento SET responsavel_cod = 'COL007' WHERE cod = 'FIN';
UPDATE tb_departamento SET responsavel_cod = 'COL001' WHERE cod = 'PMO';

-- =============================================================================
-- DOMÍNIO: PROJETOS — Clientes
-- Edge case CEP/UF/cidade: coerência geográfica (rule localidade-geografica)
-- CEP 04538133 → São Paulo / SP   CEP 20031003 → Rio de Janeiro / RJ
-- CEP 30130010 → Belo Horizonte / MG
-- =============================================================================

INSERT INTO tb_cliente
    (codigo_cliente, cnpj, razao_social, nome_fantasia,
     email_contato, telefone, responsavel_nome,
     cep, cidade, uf, ativo)
VALUES
    ('CLI001', '60872504000123', 'Royal Servicos de TI S.A.',           'Royal',
     'projetos@royal.com.br',       '1133334444', 'Gustavo Almeida',
     '04538133', 'São Paulo',       'SP', 1),

    ('CLI002', '45678912000156', 'TechCorp Solucoes Digitais Ltda',      'TechCorp',
     'contato@techcorp.com.br',     '2133334444', 'Patricia Sousa',
     '20031003', 'Rio de Janeiro',  'RJ', 1),

    ('CLI003', '78901234000189', 'Innovate Consultoria e Sistemas',      'Innovate',
     'inovacao@innovate.com.br',    '3133334444', 'Rodrigo Fernandes',
     '30130010', 'Belo Horizonte',  'MG', 1);

-- =============================================================================
-- DOMÍNIO: PROJETOS — Projetos
-- PROJ-001/PROJ-002/PROJ-004: Em andamento → itens de reembolso vinculados
-- PROJ-003: Encerrado → sem novos reembolsos
-- =============================================================================

INSERT INTO tb_projeto
    (codigo_projeto, codigo_cliente, responsavel_cod,
     nome, descricao, status, orcamento, data_inicio, data_fim)
VALUES
    ('PROJ-001', 'CLI001', 'COL001',
     'Plataforma Digital Royal',
     'Desenvolvimento de plataforma digital integrada ao ERP do cliente Royal',
     'Em andamento', 850000.00, '2024-01-15', NULL),

    ('PROJ-002', 'CLI001', 'COL003',
     'Integração ERP Royal',
     'Integração de módulos SAP com sistemas legados do cliente Royal',
     'Em andamento', 320000.00, '2024-06-01', NULL),

    ('PROJ-003', 'CLI002', 'COL001',
     'App Mobile TechCorp',
     'Desenvolvimento de aplicativo mobile iOS e Android para TechCorp',
     'Encerrado',   175000.00, '2023-08-01', '2024-02-28'),

    ('PROJ-004', 'CLI003', 'COL003',
     'Consultoria Ágil Innovate',
     'Implantação de metodologias ágeis e OKR na Innovate Consultoria',
     'Em andamento', 540000.00, '2025-01-10', NULL);

-- =============================================================================
-- DOMÍNIO: RECRUTAMENTO — Vagas
-- Distribuição: 2 Abertas, 1 Em andamento, 1 Encerrada (70/30 rule)
-- =============================================================================

INSERT INTO tb_vaga
    (id, org_id, cargo_cod, depto_cod, responsavel_cod,
     titulo, descricao, nivel, modalidade,
     salario_min, salario_max, status, numero_vagas, data_criacao, data_fim)
VALUES
    (1, 1, 'DEV-SR', 'TI', 'COL002',
     'Desenvolvedor Backend Senior Java',
     'Vaga para dev backend com experiência em Java 17, Spring Boot 3 e microsserviços',
     'Senior', 'Hibrido', 9500.00, 14000.00, 'Aberta', 2,
     '2026-01-10 09:00:00', NULL),

    (2, 1, 'DEV-PL', 'TI', 'COL001',
     'Desenvolvedor Frontend React Pleno',
     'Vaga para dev frontend especializado em React 18, TypeScript e testes automatizados',
     'Pleno', 'Remoto', 6000.00, 8500.00, 'Em andamento', 1,
     '2026-01-20 10:00:00', NULL),

    (3, 1, 'RH-PL', 'RH', 'COL008',
     'Analista de Recursos Humanos',
     'Analista de RH para apoio em recrutamento, treinamento e comunicação interna',
     'Pleno', 'Presencial', 4500.00, 6500.00, 'Encerrada', 1,
     '2025-10-05 14:00:00', '2025-12-31'),

    (4, 1, 'DEV-JR', 'TI', 'COL002',
     'Desenvolvedor Junior Full Stack',
     'Oportunidade para dev júnior com conhecimento básico em React e Node.js',
     'Junior', 'Hibrido', 2500.00, 4000.00, 'Aberta', 3,
     '2026-02-15 11:00:00', NULL);

-- =============================================================================
-- DOMÍNIO: RECRUTAMENTO — Candidaturas
-- Edge cases: nome curto (Sol Kim, Ju Pires), nome longo (Marcos Vinicius...)
-- Distribuição: 2 Aprovado, 2 Reprovado, 3 Em analise, 1 Inscrito
-- =============================================================================

INSERT INTO tb_candidatura
    (id, vaga_id, nome_candidato, cpf_candidato, email_candidato,
     telefone_candidato, linkedin_url, curriculo_url,
     etapa, status, observacao, data_inscricao)
VALUES
    (1, 1, 'Lucas Andrade Ferreira',               '11122233344',
     'lucas.andrade@gmail.com',      '11998765432',
     'linkedin.com/in/lucasandrade', NULL,
     'Entrevista Tecnica', 'Em analise',
     'Boa performance na triagem; aguardando avaliação técnica prática',
     '2026-01-15 10:00:00'),

    (2, 1, 'Camila Rodrigues de Souza Monteiro',   '22233344455',
     'camila.rodrigues@hotmail.com', '11987654321',
     NULL, 'drive.google.com/curriculo/camila',
     'Proposta', 'Aprovado',
     'Salário negociado aprovado; proposta enviada em 20/01/2026',
     '2026-01-18 14:30:00'),

    (3, 1, 'Bia Nunes',                            '33344455566',
     'bia.nunes@outlook.com',        NULL,
     'linkedin.com/in/bianunes', NULL,
     'Triagem', 'Reprovado',
     'Perfil não aderente: experiência mínima de 5 anos não atingida',
     '2026-02-01 09:00:00'),

    (4, 2, 'Diego Martins Cavalcanti',             '44455566677',
     'diego.martins@gmail.com',      '21976543210',
     'linkedin.com/in/diegomartins', NULL,
     'Entrevista RH', 'Em analise',
     NULL,
     '2026-01-25 11:00:00'),

    (5, 2, 'Sol Kim',                              '55566677788',
     'sol.kim@gmail.com',            '11965432109',
     NULL, NULL,
     'Triagem', 'Inscrito',
     NULL,
     '2026-02-10 16:00:00'),

    (6, 4, 'Thiago Henrique Albuquerque da Costa', '66677788899',
     'thiago.albuquerque@gmail.com', '31954321098',
     NULL, NULL,
     'Triagem', 'Em analise',
     NULL,
     '2026-02-20 09:30:00'),

    (7, 4, 'Ju Pires',                             '77788899900',
     'ju.pires@gmail.com',           '11943210987',
     'linkedin.com/in/jupires', 'docs.google.com/curriculo/ju',
     'Entrevista RH', 'Aprovado',
     'Aprovada como estagiária; aguardando documentação admissional',
     '2026-02-18 10:00:00'),

    (8, 4, 'Marcos Vinicius Pereira dos Santos Filho', NULL,
     'marcos.pereira@gmail.com',     NULL,
     NULL, NULL,
     'Triagem', 'Reprovado',
     'Currículo incompleto: ausência de formação e experiências anteriores',
     '2026-03-01 08:00:00');

-- =============================================================================
-- DOMÍNIO: REEMBOLSO — Verbas (categorias de despesa)
-- verba_id=1..3,5,6 → requer_comprovante=1 (I-07: obrigatório)
-- verba_id=4,7      → requer_comprovante=0 (I-07: não exige comprovante)
-- verba_id=5        → prazo_envio_dias=15  (I-11: expiração rápida)
-- verba_id=2        → valor=150.00 teto    (I-12: fácil de exceder)
-- verba_id=4        → tipo_custo='fixa', valor=0.75/un (I-13: tipoCodigo=2 / km rodado)
-- Valores propositalmente não redondos conforme constraint da ferramenta
-- =============================================================================

INSERT INTO tb_verba
    (verba_id, categoria, descricao, valor, tipo_custo,
     requer_comprovante, prazo_envio_dias, ativo)
VALUES
    (1, 'Alimentação',
     'Refeições em viagens, reuniões e eventos de trabalho com cliente',
     85.50, 'debito', 1, 30, 1),

    (2, 'Transporte',
     'Uber, táxi, combustível e pedágios em deslocamentos a serviço',
     150.00, 'debito', 1, 30, 1),

    (3, 'Hospedagem',
     'Hotéis e pousadas em viagens corporativas previamente aprovadas',
     420.00, 'debito', 1, 60, 1),

    (4, 'Km Rodado',
     'Reembolso por quilômetro rodado em veículo próprio a serviço',
     0.75, 'fixa', 0, 30, 1),

    (5, 'Material de Escritório',
     'Insumos, papelaria e materiais de TI adquiridos para o trabalho',
     200.00, 'debito', 1, 15, 1),

    (6, 'Cursos e Capacitação',
     'Treinamentos, cursos técnicos, certificações e conferências profissionais',
     1500.00, 'variavel', 1, 45, 1),

    (7, 'Estacionamento',
     'Estacionamentos em reuniões externas e visitas a clientes',
     45.00, 'debito', 0, 30, 1);

-- =============================================================================
-- DOMÍNIO: REEMBOLSO — Status adicionais
-- tb_status_solicitacao 1–5 já inseridos no DDL — não regenerados
-- A-10: frontend interpreta statusId===5 como 'Reclassificada'; DDL define
--       status_id=5 como 'Pago'. Item 10 usa status_id=5 para ativar o cenário.
--       Nenhum status extra adicionado para não criar ambiguidade.
-- =============================================================================

-- =============================================================================
-- DOMÍNIO: REEMBOLSO — Grupos de Solicitação
-- total_declarado = soma dos itens de cada grupo (valores declarados)
-- Grupo 2 → projeto_cod=NULL: cenário I-14 (sem projeto obrigatório)
-- =============================================================================

INSERT INTO tb_solicitacao_grupo
    (id, cod_profissional, projeto_cod, objetivo, total_declarado, data_solicitacao)
VALUES
    -- Grupo 1 — Ana Vaz, PROJ-001 | R-01/R-02/R-04 (lista do colaborador)
    -- Itens 1+2 = 78.90+143.70 = 222.60 → status_id=4 Aguardando Pagamento (R-09/R-10)
    (1, 'COL004', 'PROJ-001',
     'Visita técnica ao cliente Royal - São Paulo',
     222.60, '2026-01-20 09:00:00'),

    -- Grupo 2 — Ana Vaz, sem projeto | I-14 (REEMBOLSO_PERMITIR_SOLICITACAO_SEM_PROJETO)
    -- Item 3 = 349.90 → status_id=2 Aprovado
    (2, 'COL004', NULL,
     'Participação em treinamento interno React',
     349.90, '2026-02-10 10:00:00'),

    -- Grupo 3 — Bruna Figueiredo (nome longo, edge case layout), PROJ-002
    -- Item 4 = 897.50 Reprovado | Item 5 = 198.30 Aprovado (data antiga — I-11)
    (3, 'COL005', 'PROJ-002',
     'Workshop de integração ERP em Campinas',
     1095.80, '2026-02-15 11:00:00'),

    -- Grupo 4 — Pedro Lima, PROJ-001 | Aprovação (A-03/A-04/A-05/A-07)
    -- Itens 6–9 todos status_id=1 Pendentes
    -- Item 8: valor excede teto verba_id=2 (I-12/A-12)
    -- Item 9: Km Rodado tipoCodigo=2 (I-13)
    (4, 'COL006', 'PROJ-001',
     'Viagem de levantamento de requisitos Royal - São Paulo',
     1145.90, '2026-03-01 08:30:00'),

    -- Grupo 5 — Fernanda Oliveira (sou_aprovador=1), PROJ-004
    -- Item 10: status_id=5 → cenário A-10 (Reclassificada no frontend)
    -- Item 11: status_id=1 Pendente
    -- Item 12: status_id=1 sem comprovante (verba requer_comprovante=0 — estacionamento)
    (5, 'COL002', 'PROJ-004',
     'Conferência de Tecnologia - Belo Horizonte',
     1463.30, '2026-03-10 14:00:00'),

    -- Grupo 6 — Carlos Menezes (sou_gestor=1), PROJ-001
    -- Item 13: status_id=4 Aguardando Pagamento (R-09/R-10)
    -- Item 14: Km Rodado tipoCodigo=2 status_id=2
    -- Item 15: Material, data antiga → I-11 (prazo_envio_dias=15)
    (6, 'COL001', 'PROJ-001',
     'Reunião de kick-off presencial com stakeholders Royal',
     376.10, '2026-03-15 16:00:00');

-- =============================================================================
-- DOMÍNIO: REEMBOLSO — Itens de Solicitação
-- Constraints verificadas:
--   status_id=3 → observacao IS NOT NULL (item 4)
--   status_id=2 → aprovador_cod IS NOT NULL (itens 3,5,14)
--   data_aprovacao >= data_despesa (todos os itens com data_aprovacao)
--   valor > 0 (todos)
-- =============================================================================

INSERT INTO tb_solicitacao_item
    (id, solicitacao_grupo_id, verba_id, status_id, aprovador_cod,
     categoria, descricao, data_despesa, valor, valor_aprovado,
     comprovante_url, data_aprovacao, data_pagamento, observacao)
VALUES

    -- ─── Grupo 1 | Ana Vaz | Visita Royal SP ───────────────────────────────
    -- status_id=4 (Aguardando Pagamento) → cobre R-09/R-10 (Gerar Relatório)
    (1, 1, 1, 4, 'COL003',
     'Alimentação',
     'Almoço de trabalho com equipe Royal durante visita técnica — restaurante Figueira Rubaiyat',
     '2026-01-19', 78.90, 78.90,
     'https://storage.foursys.com/docs/nota-fiscal-001.pdf',
     '2026-01-22 10:00:00', NULL, NULL),

    (2, 1, 2, 4, 'COL003',
     'Transporte',
     'Uber aeroporto Congonhas → cliente Royal (ida e volta) — 2 corridas',
     '2026-01-19', 143.70, 143.70,
     'https://storage.foursys.com/docs/recibo-uber-002.pdf',
     '2026-01-22 10:00:00', NULL, NULL),

    -- ─── Grupo 2 | Ana Vaz | Treinamento React ──────────────────────────────
    -- status_id=2 (Aprovado) — sem projeto (I-14)
    (3, 2, 6, 2, 'COL002',
     'Cursos e Capacitação',
     'Assinatura Udemy Business — React Advanced e Testing Library — 1 mês',
     '2026-02-08', 349.90, 349.90,
     'https://storage.foursys.com/docs/nota-udemy-003.pdf',
     '2026-02-14 09:00:00', NULL, NULL),

    -- ─── Grupo 3 | Bruna Figueiredo | Workshop Campinas ─────────────────────
    -- status_id=3 (Reprovado) — observacao OBRIGATÓRIA por constraint
    (4, 3, 3, 3, 'COL001',
     'Hospedagem',
     'Hotel ibis Campinas 3 noites — workshop de integração ERP (SAP/TOTVS)',
     '2026-02-13', 897.50, NULL,
     'https://storage.foursys.com/docs/nota-hotel-004.pdf',
     '2026-02-18 11:00:00', NULL,
     'Valor (R$ 897,50) supera o teto de R$ 420,00/noite para Campinas; solicitar nova cotação e aprovar previamente'),

    -- status_id=2 (Aprovado) — data_despesa=2026-01-10 com verba prazo=15 dias
    -- Diferença: 2026-01-10 → grupo criado 2026-02-15 = 36 dias > 15 → ALERTA I-11
    (5, 3, 5, 2, 'COL001',
     'Material de Escritório',
     'Cartucho toner Brother TN-1060 para impressora departamento TI',
     '2026-01-10', 198.30, 198.30,
     'https://storage.foursys.com/docs/nota-papelaria-005.pdf',
     '2026-02-16 14:00:00', NULL, NULL),

    -- ─── Grupo 4 | Pedro Lima | Viagem Levantamento Requisitos ─────────────
    -- Todos status_id=1 (Pendente) → fluxo de aprovação A-03/A-04/A-05
    (6, 4, 1, 1, NULL,
     'Alimentação',
     'Refeições durante 3 dias de visita técnica em São Paulo — almoço e jantar',
     '2026-02-28', 241.50, NULL,
     'https://storage.foursys.com/docs/nota-refeicoes-006.pdf',
     NULL, NULL, NULL),

    (7, 4, 2, 1, NULL,
     'Transporte',
     'Passagem aérea BH→SP→BH econômica — Azul voo AD4820/AD4821',
     '2026-02-27', 687.40, NULL,
     'https://storage.foursys.com/docs/passagem-aerea-007.pdf',
     NULL, NULL, NULL),

    -- valor=185.50 > teto verba_id=2 (150.00) → ALERTA I-12 / A-12
    (8, 4, 2, 1, NULL,
     'Transporte',
     'Táxi aeroporto Congonhas → hotel e retorno no dia seguinte — 2 corridas',
     '2026-02-28', 185.50, NULL,
     'https://storage.foursys.com/docs/recibo-taxi-008.pdf',
     NULL, NULL, NULL),

    -- verba_id=4 tipo_custo='fixa' (I-13: tipoCodigo=2) — 42 km × R$ 0,75 = R$ 31,50
    (9, 4, 4, 1, NULL,
     'Km Rodado',
     'Deslocamento veículo próprio — residência BH até aeroporto Confins (42 km)',
     '2026-02-27', 31.50, NULL,
     NULL,
     NULL, NULL, NULL),

    -- ─── Grupo 5 | Fernanda Oliveira | Conferência BH ───────────────────────
    -- status_id=5 → frontend interpreta como 'Reclassificada' (A-10)
    -- Sem aprovador obrigatório (constraint aplica apenas a status_id=2)
    (10, 5, 3, 5, 'COL003',
     'Hospedagem',
     'Hotel Mercure BH 2 noites — Conferência Agile Brasil 2026',
     '2026-03-09', 534.80, 440.00,
     'https://storage.foursys.com/docs/nota-hotel-010.pdf',
     '2026-03-14 10:00:00', NULL, NULL),

    -- status_id=1 Pendente — para comparação A-10 (item normal aprovável)
    (11, 5, 6, 1, NULL,
     'Cursos e Capacitação',
     'Inscrição Conferência Agile Brasil 2026 — ingresso Executor 2 dias',
     '2026-03-08', 890.00, NULL,
     'https://storage.foursys.com/docs/nota-conferencia-011.pdf',
     NULL, NULL, NULL),

    -- verba_id=7 requer_comprovante=0 → sem comprovante_url é válido (I-07 contra-exemplo)
    (12, 5, 7, 1, NULL,
     'Estacionamento',
     'Estacionamento Shopping Vila Olímpia 6h — reunião pré-conferência com equipe cliente',
     '2026-03-08', 38.50, NULL,
     NULL,
     NULL, NULL, NULL),

    -- ─── Grupo 6 | Carlos Menezes | Kick-off Royal ──────────────────────────
    -- status_id=4 (Aguardando Pagamento) → R-09/R-10 (Gerar Relatório)
    (13, 6, 1, 4, 'COL002',
     'Alimentação',
     'Almoço de kick-off com equipe interna e stakeholders Royal — restaurante D.O.M.',
     '2026-03-14', 312.80, 312.80,
     'https://storage.foursys.com/docs/nota-almoco-013.pdf',
     '2026-03-18 09:00:00', NULL, NULL),

    -- verba_id=4 tipo_custo='fixa' status_id=2 (I-13 + A-13) — 28 km × R$ 0,75 = R$ 21,00
    (14, 6, 4, 2, 'COL002',
     'Km Rodado',
     'Deslocamento veículo próprio — escritório Foursys até sede Royal Av. Faria Lima (28 km)',
     '2026-03-14', 21.00, 21.00,
     NULL,
     '2026-03-18 09:00:00', NULL, NULL),

    -- verba_id=5 prazo=15 dias; data_despesa=2026-02-01; grupo criado em 2026-03-15
    -- Diferença: 2026-02-01 → 2026-03-15 = 42 dias > 15 → ALERTA I-11 / A-11
    (15, 6, 5, 1, NULL,
     'Material de Escritório',
     'Post-its, canetas coloridas e flipchart para dinâmica de kick-off com cliente',
     '2026-02-01', 42.30, NULL,
     'https://storage.foursys.com/docs/nota-materiais-015.pdf',
     NULL, NULL, NULL);

COMMIT;

-- =============================================================================
-- RESUMO DE COBERTURA — Cenários × Dados
-- =============================================================================
--
-- DASHBOARD (Reembolso.tsx)
-- R-01  Lista padrão ............. grupos 1–6 sempre populados para COL001/COL004
-- R-02  Filtro período ........... grupos com datas distintas (jan/fev/mar 2026)
-- R-03  Limpar filtro ............ idem R-02
-- R-04  Busca textual ............. grupos com objetivos e clientes variados
-- R-05  Modal detalhes ........... itens 1–15 com campos completos de visualização
-- R-06  Modal documentos ......... itens 1–3,5–7,10–11,13–15 com comprovante_url
-- R-07  Abas por perfil .......... COL004 (sem extra), COL001 (gestor), COL002 (aprov.), COL003 (ambas)
-- R-08  URL restrita → redirect .. idem R-07 (controle no frontend após carregamento)
-- R-09  Botão Gerar Relatório .... itens 1,2,13 com status_id=4
-- R-10  ⚠️ Download arquivo ...... idem R-09 (backend não verificado)
-- R-11  Botão Remessa CNAB ....... parâmetro HABILITA_BOTAO_REMESSA_CNAB_REEMBOLSO → via API
--
-- INSERIR REEMBOLSO (InserirReembolso.tsx)
-- I-01  Campos preenchidos ........ verbas 1–7 disponíveis; projetos CLI001–CLI003
-- I-02  Múltiplos itens .......... grupos 1,4,6 têm 2+ itens
-- I-03  Editar item carrinho ..... todos os grupos
-- I-04  Remover item carrinho .... todos os grupos
-- I-05  Modal sucesso + redirect . inserção de novo grupo + itens (runtime)
-- I-06  Campos obrigatórios ...... verbas todas populadas; validação no frontend
-- I-07  Comprovante obrigatório .. verba_id=1,2,3,5,6 requer_comprovante=1
-- I-08  Formato inválido ......... validação no frontend (runtime)
-- I-09  Carrinho vazio ........... validação no frontend (runtime)
-- I-10  Erros da API ............. validação no frontend (runtime)
-- I-11  Data vencida ............. itens 5,15 (prazo=15 dias; 36 e 42 dias excedidos)
-- I-12  Valor excede teto ........ item 8: R$ 185,50 > teto R$ 150,00 verba_id=2
-- I-13  tipoCodigo=2 / km rodado . itens 9,14 (verba_id=4, tipo_custo='fixa')
-- I-14  Sem projeto opcional ..... grupo 2: projeto_cod=NULL
-- I-15  ⚠️ OCR comprovante ...... itens com comprovante_url (backend não verificado)
--
-- APROVAR REEMBOLSO (AprovarReembolso.tsx)
-- A-01  Visualizar agrupado ....... grupos 4,5 de COL006/COL002 (pendentes para aprovador)
-- A-02  Filtro por cliente ........ CLI001 (PROJ-001), CLI003 (PROJ-004)
-- A-03  Selecionar grupo .......... grupo 4: 4 itens pendentes (itens 6–9)
-- A-04  Aprovar com confirmação ... grupo 4: itens 6–9 status_id=1 → aprovação
-- A-05  Documentos no modal ........ itens 6,7,8,11,13 com comprovante_url
-- A-06  Botões desabilitados ....... nenhuma seleção (runtime)
-- A-07  Reprovar sem justificativa . validação no frontend (runtime)
-- A-08  Projeto sem cliente ........ validação no frontend (runtime)
-- A-09  Lista vazia ................ filtro por cliente sem correspondência (runtime)
-- A-10  Reclassificada (status=5) .. item 10: status_id=5 → COL003 com PARAMETRIZACAO_REEMBOLSO
-- A-11  Data vencida na aprovação .. itens 5,15 (mesmos do I-11)
-- A-12  Valor acima do teto ........ item 8 (mesmo do I-12)
-- A-13  Ordenação por data/pendentes grupos 4,5 com datas e pendentes distintos
--
-- NOTA: Parâmetros de sistema (EXIBIR_BOTAO_GERAR_PAGAMENTOS,
--       HABILITA_BOTAO_REMESSA_CNAB_REEMBOLSO, REEMBOLSO_PERMITIR_SOLICITACAO_SEM_PROJETO,
--       PARAMETRIZACAO_REEMBOLSO) não possuem tabela no schema atual.
--       Devem ser configurados via painel de administração / API de parâmetros.
-- =============================================================================
