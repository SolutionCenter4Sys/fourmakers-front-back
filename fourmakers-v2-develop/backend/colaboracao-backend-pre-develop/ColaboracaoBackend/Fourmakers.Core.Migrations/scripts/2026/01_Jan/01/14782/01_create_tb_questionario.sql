CREATE TABLE tb_questionario (
     id INT AUTO_INCREMENT PRIMARY KEY,
     codigo_questionario VARCHAR(50) NOT NULL,
     tb_org_id INT NOT NULL,
     titulo VARCHAR(150) NOT NULL,
     data_criacao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
     data_alteracao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,

     UNIQUE KEY uk_questionario_codigo_org (codigo_questionario, tb_org_id)
)