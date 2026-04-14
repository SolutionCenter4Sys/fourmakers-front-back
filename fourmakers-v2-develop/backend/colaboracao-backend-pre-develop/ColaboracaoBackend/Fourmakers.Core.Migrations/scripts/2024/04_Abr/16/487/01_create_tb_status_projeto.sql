
        
CREATE TABLE tb_status_projeto(
        id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
		descricao_status VARCHAR(255),
		cod_status INTEGER,
		tb_org_id INT,
		ativo BOOLEAN,
        FOREIGN KEY (tb_org_id) REFERENCES tb_org(id)
        );
