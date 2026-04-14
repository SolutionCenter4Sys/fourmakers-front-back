CREATE TABLE tb_candidato_template_email (
  id        		int NOT NULL AUTO_INCREMENT,
  tb_org_id 		int NOT NULL,
  emails_copia      varchar(1000) NOT NULL,
  descricao 		text COLLATE utf8mb4_unicode_ci NOT NULL,
  data_criacao 		datetime DEFAULT CURRENT_TIMESTAMP, 
  data_alteracao 	datetime DEFAULT CURRENT_TIMESTAMP, 
  PRIMARY KEY (id),
  KEY idx_tb_template_email_candidato_orgid (tb_org_id), 
  CONSTRAINT fk_tb_template_email_candidato_tb_org FOREIGN KEY (tb_org_id) REFERENCES tb_org (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;


