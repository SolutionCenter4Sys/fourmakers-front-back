CREATE TABLE tb_usuario_permissao_log (
    id INT AUTO_INCREMENT PRIMARY KEY,
    tb_org_id INT NOT NULL,
    data_alteracao datetime DEFAULT CURRENT_TIMESTAMP,
    tb_colaborador_cpf_criacao VARCHAR(20) NOT NULL,
    operacao VARCHAR(50) NOT NULL,
    tb_usuario_id INT,
    tb_grupo_acesso_id INT,
    tb_funcionalidade_sistema_id INT
);