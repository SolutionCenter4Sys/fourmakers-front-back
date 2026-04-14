CREATE TABLE tb_destinatarios_email_candidatos (
  id bigint NOT NULL AUTO_INCREMENT,
  email VARCHAR(150) NOT NULL,
  assunto VARCHAR(255) DEFAULT NULL,
  area VARCHAR(50) DEFAULT NULL,
  tb_org_id int NOT NULL,
  anexo tinyint NOT NULL,
  ativo tinyint NOT NULL,
  data_criacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  data_alteracao DATETIME NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (id)
);

