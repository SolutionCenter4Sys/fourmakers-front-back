PRAGMA foreign_keys = ON;

-- ──────────────────────────────────────────────────────────────
-- DOMÍNIO: AUTH / MULTI-TENANT
-- ──────────────────────────────────────────────────────────────

CREATE TABLE tb_organizacao (
    id             INTEGER PRIMARY KEY AUTOINCREMENT,
    cnpj           TEXT    UNIQUE,
    razao_social   TEXT    NOT NULL,
    nome_fantasia  TEXT,
    subdominio     TEXT    UNIQUE,
    email_contato  TEXT,
    telefone       TEXT,
    cep            TEXT,
    cidade         TEXT,
    uf             TEXT,
    logotipo_url   TEXT,
    ativo          INTEGER NOT NULL DEFAULT 1,
    criado_em      TEXT    NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CHECK (length(cnpj) = 14 OR cnpj IS NULL),
    CHECK (length(uf) = 2 OR uf IS NULL)
);

CREATE TABLE tb_usuario (
    id              INTEGER PRIMARY KEY AUTOINCREMENT,
    org_id          INTEGER NOT NULL REFERENCES tb_organizacao(id),
    cpf             TEXT,
    email           TEXT    NOT NULL,
    nome            TEXT    NOT NULL,
    telefone        TEXT,
    data_nascimento TEXT,
    perfil          TEXT    NOT NULL DEFAULT 'Colaborador',
    avatar_url      TEXT,
    ativo           INTEGER NOT NULL DEFAULT 1,
    criado_em       TEXT    NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ultimo_acesso   TEXT,
    UNIQUE (org_id, email),
    UNIQUE (org_id, cpf),
    CHECK (perfil IN ('Admin','Gestor','Colaborador','RH','Financeiro')),
    CHECK (length(cpf) = 11 OR cpf IS NULL)
);

-- ──────────────────────────────────────────────────────────────
-- DOMÍNIO: RH
-- ──────────────────────────────────────────────────────────────

CREATE TABLE tb_cargo (
    cod         TEXT PRIMARY KEY,
    nome        TEXT NOT NULL,
    nivel       TEXT NOT NULL DEFAULT 'Pleno',
    descricao   TEXT,
    salario_min REAL,
    salario_max REAL,
    CHECK (nivel IN ('Junior','Pleno','Senior','Especialista','Gestor')),
    CHECK (salario_min IS NULL OR salario_max IS NULL OR salario_min <= salario_max)
);

CREATE TABLE tb_departamento (
    cod              TEXT    PRIMARY KEY,
    nome             TEXT    NOT NULL,
    org_id           INTEGER NOT NULL REFERENCES tb_organizacao(id),
    centro_custo     TEXT,
    email_depto      TEXT,
    responsavel_cod  TEXT    REFERENCES tb_colaborador(cod_profissional)
);

CREATE TABLE tb_colaborador (
    cod_profissional   TEXT    PRIMARY KEY,
    usuario_id         INTEGER NOT NULL REFERENCES tb_usuario(id),
    cargo_cod          TEXT    NOT NULL REFERENCES tb_cargo(cod),
    depto_cod          TEXT    NOT NULL REFERENCES tb_departamento(cod),
    gestor_cod         TEXT    REFERENCES tb_colaborador(cod_profissional),
    nome               TEXT    NOT NULL,
    celular            TEXT,
    data_admissao      TEXT    NOT NULL,
    data_demissao      TEXT,
    modelo_contratacao TEXT    NOT NULL DEFAULT 'CLT',
    salario            REAL,
    banco              TEXT,
    agencia            TEXT,
    conta_corrente     TEXT,
    tipo_pix           TEXT,
    chave_pix          TEXT,
    linkedin_url       TEXT,
    sou_gestor         INTEGER NOT NULL DEFAULT 0,
    sou_aprovador      INTEGER NOT NULL DEFAULT 0,
    ativo              INTEGER NOT NULL DEFAULT 1,
    CHECK (modelo_contratacao IN ('CLT','PJ','Estagio','Terceiro')),
    CHECK (tipo_pix IN ('CPF','CNPJ','Email','Telefone','Aleatoria') OR tipo_pix IS NULL),
    CHECK (data_demissao IS NULL OR data_demissao >= data_admissao)
);

-- ──────────────────────────────────────────────────────────────
-- DOMÍNIO: PROJETOS
-- ──────────────────────────────────────────────────────────────

CREATE TABLE tb_cliente (
    codigo_cliente   TEXT    PRIMARY KEY,
    cnpj             TEXT    UNIQUE,
    razao_social     TEXT    NOT NULL,
    nome_fantasia    TEXT,
    email_contato    TEXT,
    telefone         TEXT,
    responsavel_nome TEXT,
    cep              TEXT,
    cidade           TEXT,
    uf               TEXT,
    ativo            INTEGER NOT NULL DEFAULT 1,
    CHECK (length(cnpj) = 14 OR cnpj IS NULL),
    CHECK (length(uf) = 2 OR uf IS NULL)
);

CREATE TABLE tb_projeto (
    codigo_projeto   TEXT    PRIMARY KEY,
    codigo_cliente   TEXT    NOT NULL REFERENCES tb_cliente(codigo_cliente),
    responsavel_cod  TEXT    REFERENCES tb_colaborador(cod_profissional),
    nome             TEXT    NOT NULL,
    descricao        TEXT,
    status           TEXT    NOT NULL DEFAULT 'Em andamento',
    orcamento        REAL,
    data_inicio      TEXT    NOT NULL,
    data_fim         TEXT,
    CHECK (status IN ('Em andamento','Encerrado','Suspenso','Proposta')),
    CHECK (data_fim IS NULL OR data_fim >= data_inicio)
);

-- ──────────────────────────────────────────────────────────────
-- DOMÍNIO: REEMBOLSO
-- ──────────────────────────────────────────────────────────────

CREATE TABLE tb_verba (
    verba_id            INTEGER PRIMARY KEY AUTOINCREMENT,
    categoria           TEXT    NOT NULL UNIQUE,
    descricao           TEXT,
    valor               REAL    NOT NULL,
    tipo_custo          TEXT    NOT NULL DEFAULT 'debito',
    requer_comprovante  INTEGER NOT NULL DEFAULT 1,
    prazo_envio_dias    INTEGER NOT NULL DEFAULT 30,
    ativo               INTEGER NOT NULL DEFAULT 1,
    CHECK (tipo_custo IN ('debito','credito','fixa','variavel')),
    CHECK (prazo_envio_dias > 0)
);

CREATE TABLE tb_status_solicitacao (
    status_id INTEGER PRIMARY KEY,
    descricao TEXT    NOT NULL
);

INSERT INTO tb_status_solicitacao (status_id, descricao) VALUES
    (1, 'Pendente'),
    (2, 'Aprovado'),
    (3, 'Reprovado'),
    (4, 'Aguardando Pagamento'),
    (5, 'Pago');

CREATE TABLE tb_solicitacao_grupo (
    id               INTEGER PRIMARY KEY AUTOINCREMENT,
    cod_profissional TEXT    NOT NULL REFERENCES tb_colaborador(cod_profissional),
    projeto_cod      TEXT    REFERENCES tb_projeto(codigo_projeto),
    objetivo         TEXT    NOT NULL,
    total_declarado  REAL    NOT NULL DEFAULT 0,
    data_solicitacao TEXT    NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE tb_solicitacao_item (
    id                   INTEGER PRIMARY KEY AUTOINCREMENT,
    solicitacao_grupo_id INTEGER NOT NULL REFERENCES tb_solicitacao_grupo(id),
    verba_id             INTEGER NOT NULL REFERENCES tb_verba(verba_id),
    status_id            INTEGER NOT NULL DEFAULT 1 REFERENCES tb_status_solicitacao(status_id),
    aprovador_cod        TEXT    REFERENCES tb_colaborador(cod_profissional),
    categoria            TEXT    NOT NULL,
    descricao            TEXT    NOT NULL,
    data_despesa         TEXT    NOT NULL,
    valor                REAL    NOT NULL CHECK (valor > 0),
    valor_aprovado       REAL,
    comprovante_url      TEXT,
    data_aprovacao       TEXT,
    data_pagamento       TEXT,
    observacao           TEXT,
    CHECK (status_id != 3 OR observacao IS NOT NULL),
    CHECK (status_id != 2 OR aprovador_cod IS NOT NULL),
    CHECK (data_aprovacao IS NULL OR data_aprovacao >= data_despesa),
    CHECK (data_pagamento IS NULL OR data_pagamento >= data_aprovacao OR data_aprovacao IS NULL)
);
