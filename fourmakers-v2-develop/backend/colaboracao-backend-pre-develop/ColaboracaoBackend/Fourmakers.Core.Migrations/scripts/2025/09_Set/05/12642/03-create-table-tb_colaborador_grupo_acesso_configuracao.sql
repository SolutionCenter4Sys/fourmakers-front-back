
CREATE TABLE tb_colaborador_grupo_acesso_configuracao (
    id INT AUTO_INCREMENT PRIMARY KEY,
    tabela VARCHAR(100) NOT NULL,       -- ex: 'tb_colaborador_org'
    coluna VARCHAR(100) NOT NULL,       -- ex: 'modelo_contratacao'
    condicao VARCHAR(200) NOT NULL,     -- ex: 'tb_org_id,codigo_interno_colaborador'
    chave VARCHAR(200) NOT NULL,        -- ex: 'Parceiro (PJ)'
    tb_grupo_acesso_id INT NOT NULL,
    tb_org_id INT NOT NULL,
    data_criacao DATETIME DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT fk_colab_grupoacesso_config_grupo 
        FOREIGN KEY (tb_grupo_acesso_id) REFERENCES tb_grupo_acesso(id),
        
    CONSTRAINT fk_colab_grupoacesso_config_org 
        FOREIGN KEY (tb_org_id) REFERENCES tb_org(id)
);

INSERT INTO tb_colaborador_grupo_acesso_configuracao (
    tabela,
    coluna,
    condicao,
    chave,
    tb_grupo_acesso_id,
    tb_org_id
) VALUES (
    'tb_colaborador_org',
    'modelo_contratacao',
    'tb_org_id,codigo_interno_colaborador',
    'Parceiro (PJ)',
    (select id from tb_grupo_acesso where descricao = 'PRESTADOR' and tb_org_id = 9),
    9
);

INSERT INTO tb_colaborador_grupo_acesso_configuracao (
    tabela,
    coluna,
    condicao,
    chave,
    tb_grupo_acesso_id,
    tb_org_id
) VALUES (
    'tb_colaborador_org',
    'modelo_contratacao',
    'tb_org_id,codigo_interno_colaborador',
    'Parceiro Terceiro (PJ)',
    (select id from tb_grupo_acesso where descricao = 'PRESTADOR' and tb_org_id = 9),
    9
);
