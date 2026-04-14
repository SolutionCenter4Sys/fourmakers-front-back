CREATE TABLE IF NOT EXISTS tb_colaborador_ia (
    id varchar(36) KEY,
    codigo_interno_colaborador VARCHAR(255) NOT NULL,
    email VARCHAR(255),
    phone VARCHAR(50),
    first_name VARCHAR(255),
    last_name VARCHAR(255),
    headline TEXT,
    summary TEXT,
    location_name VARCHAR(255),
    industry_name VARCHAR(255),
    url_linkedin VARCHAR(500),
    hard_skills TEXT,
    soft_skills TEXT,
    dominio_negocios TEXT,
    metodologias TEXT,
    data_criacao DATETIME NOT NULL,
    data_alteracao DATETIME NOT NULL
);