CREATE TABLE tb_verba_logs (
   id INT AUTO_INCREMENT PRIMARY KEY,
   regra VARCHAR(100) NOT NULL,
   acao VARCHAR(40) NOT NULL,
   valor_anterior VARCHAR(100) NOT NULL,
   novo_valor VARCHAR(100) NOT NULL,
   codigo_interno_colaborador VARCHAR(36) NOT NULL,
   tb_org_id INT NOT NULL,
   data_criacao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
   FOREIGN KEY (tb_org_id) REFERENCES tb_org(id),
   FOREIGN KEY (codigo_interno_colaborador) REFERENCES tb_colaborador(codigo_interno_colaborador)
);