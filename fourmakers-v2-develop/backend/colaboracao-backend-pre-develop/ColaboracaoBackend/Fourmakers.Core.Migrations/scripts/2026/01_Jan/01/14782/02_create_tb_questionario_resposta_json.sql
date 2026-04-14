CREATE TABLE tb_questionario_resposta_json (
   id INT AUTO_INCREMENT PRIMARY KEY,
   codigo_questionario VARCHAR(50) NOT NULL,
   tb_org_id INT NOT NULL,
   codigo_interno_colaborador CHAR(36) NOT NULL,
   json_texto JSON NOT NULL,
   data_criacao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
   data_alteracao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
   processado BOOLEAN DEFAULT FALSE,

   CONSTRAINT fk_resposta_questionario
       FOREIGN KEY (codigo_questionario, tb_org_id)
           REFERENCES tb_questionario (codigo_questionario, tb_org_id),

   CONSTRAINT fk_resposta_colaborador
       FOREIGN KEY (codigo_interno_colaborador)
           REFERENCES tb_colaborador (codigo_interno_colaborador)
) 