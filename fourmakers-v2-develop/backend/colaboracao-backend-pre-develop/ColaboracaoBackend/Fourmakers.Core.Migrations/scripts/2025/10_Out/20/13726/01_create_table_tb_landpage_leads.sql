CREATE TABLE tb_landpage_lead (
      id BIGINT PRIMARY KEY AUTO_INCREMENT,
      nome VARCHAR(255) NOT NULL,
      email VARCHAR(255) NOT NULL,
      telefone VARCHAR(50) NOT NULL,
      nome_empresa VARCHAR(255) NOT NULL,
      opcao_colaborador_enum INT NOT NULL,
      data_criacao TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
      data_alteracao TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);