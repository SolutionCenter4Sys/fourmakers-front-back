CREATE TABLE tb_verba (
      id INT AUTO_INCREMENT PRIMARY KEY,
      categoria VARCHAR(100) NOT NULL,
      tipo_custo INT NOT NULL,
      unidade VARCHAR(100),
      valor DECIMAL(10,2) NOT NULL,
      custo_cliente TINYINT(1) NOT NULL,
      tb_org_id INT NOT NULL,
      ativo TINYINT(1) NOT NULL,
      data_criacao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
      data_alteracao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
      codigo_interno_colaborador_criacao VARCHAR(36) NOT NULL,
      codigo_interno_colaborador_alteracao VARCHAR(36) NOT NULL,
      FOREIGN KEY (tb_org_id) REFERENCES tb_org(id),
      FOREIGN KEY (codigo_interno_colaborador_criacao) REFERENCES tb_colaborador(codigo_interno_colaborador),
      FOREIGN KEY (codigo_interno_colaborador_alteracao) REFERENCES tb_colaborador(codigo_interno_colaborador)
);